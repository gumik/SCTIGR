using System;
using System.Collections.Generic;

namespace SCTIGR
{
	public class Tigr
	{
		#region public methods
		
		public Tigr(int merLength, int maxOverhang, int minOverlap, float minSimilarity, string[] sequences)
		{
			this.sequences = sequences;
			this.merLength = merLength;
			this.maxOverhang = maxOverhang;
			this.minOverlap = minOverlap;
			this.minSimilarity = minSimilarity;
			profile = new Profile();
		}
		
		public void Calculate()
		{
			PairwaiseComparision();
			Assembly();
		}
		
		#endregion
		
		#region public properties
		
		public Profile Profile { get { return profile; } }
		
		#endregion
		
		#region public events
		
		public event Action PairwaiseComparisionBegin = delegate { };
		public event Action<string> PairwaiseComparisionSequence = delegate { };
		public event Action<int, int> PairwaiseComparisionSubSeq = delegate { };
		public event Action PairwaiseComparisionNewMer = delegate { };
		public event Action<HashSet<int>> PairwaiseComparisionExistingMer = delegate { };
		
		public event Action<string> AssemblyInit = delegate { };
		public event Action<string, int> AssemblyCandidate = delegate { };
		public event Action<int, int, float> AssemblyCandidateScore = delegate { };
		public event Action AssemblyGoodAlignment = delegate { };
		public event Action AssemblyEnd = delegate { };
		//public event Action<Profile> AssemblyGoodAlignmentAdded = delegate { };
		
		#endregion
		
		#region private methods
		
		private void PairwaiseComparision()
		{
			PairwaiseComparisionBegin();	
			mers = new Dictionary<string, HashSet<int>>();
			probablyOverlap = new Dictionary<int, int>[sequences.Length];
			
			int i = 0;
			foreach (var sequence in sequences)
			{
				PairwaiseComparisionSequence(sequence);
				probablyOverlap[i] = new Dictionary<int, int>();
				
				int begin = 0;				
				while (begin + merLength <= sequence.Length)
				{
					var subseq = sequence.Substring(begin, merLength);
					PairwaiseComparisionSubSeq(begin, merLength);
					
					HashSet<int> hashset;
					if (!mers.TryGetValue(subseq, out hashset))
					{
						hashset = new HashSet<int>();
						mers[subseq] = hashset;
						PairwaiseComparisionNewMer();
					}
					else
					{
						PairwaiseComparisionExistingMer(hashset);
					}
					
					hashset.Add(i);
					
					foreach (var seqIdx in hashset)
					{
						if (seqIdx == i) continue;
						
						AddOverlap(seqIdx, i);
						AddOverlap(i, seqIdx);
					}
										
					++begin;
				}
				
				++i;
			}
		}
		
		private void AddOverlap(int index, int key)
		{
			int val;
			if (!probablyOverlap[index].TryGetValue(key, out val))
			{
				probablyOverlap[index][key] = 1;
			}
			else
			{
				probablyOverlap[index][key] = val + 1;
			}
		}
		
		private void Assembly()
		{
			profile.AddSequence(sequences[0], 0);
			
			var lastSegment = 0;
			var firstSegment = 0;
			
			AssemblyInit(sequences[0]);
			
			int change = 0; // 0 - there was align, 1 - there wasn't align on last, 2 - there wasn't align at all
			while (true)
			{
				int seq;
				
				if (change == 0)
				{
					seq = lastSegment;
				}
				else if (change == 1)
				{
					seq = firstSegment;
				}
				else 
				{
					break;
				}
				
				var list = GetProbablyBest(seq);
				if (list.Count == 0)
				{
					if (change == 0)
					{
						seq = firstSegment;
						list = GetProbablyBest(seq);
						if (list.Count == 0)
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
				
				var wasAlign = false;
				foreach (var probSeq in list)
				{
					var sm = new SmithWaterman(profile, sequences[probSeq]);
					sm.Calculate();
					var alignment = sm.GetBest();
					AssemblyCandidate(sequences[probSeq], alignment.First.Value.Item1 - alignment.First.Value.Item2);
					
					if (IsGoodAlignment(sequences[probSeq], alignment))
					{						
						var side = AddSequence(sequences[probSeq], alignment);
						
						if ((side & 1) != 0)
						{
							firstSegment = probSeq;
						}
						else if ((side & 2) != 0)
						{
							lastSegment = probSeq;
							change = 0;
						}
						
						RemoveSeq(probSeq);
						//AssemblyGoodAlignmentAdded(profile);
						AssemblyGoodAlignment();
						wasAlign = true;
						break;
					}
				}
				
				if (!wasAlign)
				{
					change++;
				}
			}
			
			AssemblyEnd();
		}
		
		private IList<int> GetProbablyBest(int baseSeq)
		{
			var dict = probablyOverlap[baseSeq];			
			var sortedList = new SortedList<float, int>();
			var random = new Random();
			
			foreach (var element in dict)
			{
				if (element.Value > 0)
				{
					var ok = false;
					while (!ok)
					{
						try
						{
							// Cóż za paskudna rzecz.
							sortedList.Add(-element.Value - (float)(random.NextDouble() * 0.99), element.Key);
							ok = true;
						}
						catch { }
					}
				}
			}
			
			return sortedList.Values;
		}
		
		private bool IsGoodAlignment(string sequence, LinkedList<Tuple<int,int>> alignment)
		{
			var first = alignment.First.Value;
			var last = alignment.Last.Value;
			
			var overhang = Math.Max(
			                       Math.Min(first.Item1, first.Item2),
			                       Math.Min(profile.Length - last.Item1 - 1, sequence.Length - last.Item2 - 1)
			                       );
			
			var overlap = alignment.Count;
			
			var sameBits = 0;
			foreach (var b in alignment)
			{
				if (b.Item1 == -1 || b.Item2 == -1) continue;				
				if (profile[b.Item1] == sequence[b.Item2]) ++ sameBits;
			}
			
			var similarity = (float)sameBits / alignment.Count;
			
			AssemblyCandidateScore(overlap, overhang, similarity);
			
			return (similarity >= minSimilarity)
				&& (overlap >= minOverlap)
				&& (overhang <= maxOverhang);
		}
		
		/// <summary>
		/// return 1/2 (bitmask) if profile was extended on left/right side
		/// 0 otherwise
		/// </summary>
		private int AddSequence(string sequence, LinkedList<Tuple<int,int>> alignment)
		{
			var profileI = alignment.First.Value.Item1;
			var seqI = alignment.First.Value.Item2;
			
			foreach (var align in alignment)
			{
				if (align.Item1 == -1)
				{
					++profileI;
					profile.InsertEmpty(profileI + 1);
				}
				
				if (align.Item2 == -1)
				{
					++seqI;
					sequence.Insert(seqI, "-");
				}
			}
			
			var prevLength = profile.Length;
			var p = alignment.First.Value.Item1;
			var s = alignment.First.Value.Item2;
			var begin = p - s;
			profile.AddSequence(sequence, begin);
			
			var mask = 0;
			if (begin < 0) mask |= 1;
			if (profile.Length != prevLength) mask |= 2;	
			return mask;
		}
		
		private void RemoveSeq(int seq)
		{
			var dict = probablyOverlap[seq];
			foreach (var key in dict.Keys)
			{
				probablyOverlap[key][seq] = 0;
			}
		}
		
		#endregion
		
		#region private fields
		
		private int merLength;
		private int minOverlap;
		private float minSimilarity;
		private int maxOverhang;
		//private int maxLocalErrors;
		private string[] sequences;
		private Dictionary<string, HashSet<int>> mers;
		private Dictionary<int, int>[] probablyOverlap;
		private Profile profile;
		
		#endregion
		
		#region temp members
		
//		public HashSet<string> GetMers ()
//		{ 
//			PairwaiseComparision();
//			return mers; 
//		}
		
		public Dictionary<int, int>[] GetProbablyOverlap()
		{
			//PairwaiseComparision();
			return probablyOverlap;
		}
		
		#endregion
	}
}

