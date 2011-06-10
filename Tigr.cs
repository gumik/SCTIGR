using System;
using System.Collections.Generic;

namespace SCTIGR
{
	public class Tigr
	{
		#region public methods
		
		public Tigr(int merLength, string[] sequences)
		{
//			this.sequences = new LinkedList<string>(sequences);
			this.sequences = sequences;
			this.merLength = merLength;
		}
		
//		public Tigr (int merLength)
//		{
//			sequences = new LinkedList<string>();
//			this.merLength = merLength;
//		}
		
//		public void AddSequence(string sequence)
//		{
//			sequences.AddLast(sequence);
//		}
		
		#endregion
		
		#region public properties
		
		#region public events
		
		
		
		#endregion
		
		#endregion
		
		#region private methods
		
		private void PairwaiseComparision()
		{
			mers = new Dictionary<string, HashSet<int>>();
			probablyOverlap = new Dictionary<int, int>[sequences.Length];
			
			int i = 0;
			foreach (var sequence in sequences)
			{
				probablyOverlap[i] = new Dictionary<int, int>();
				
				int begin = 0;				
				while (begin + merLength <= sequence.Length)
				{
					var subseq = sequence.Substring(begin, merLength);
					HashSet<int> hashset;
					if (!mers.TryGetValue(subseq, out hashset))
					{
						hashset = new HashSet<int>();
						mers[subseq] = hashset;
					}
					
					hashset.Add(i);
					
					//probablyOverlap[i] = new Dictionary<int, int>();
					
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
		
		#endregion
		
		#region private fields
		
		private int merLength;
		private string[] sequences;
		private Dictionary<string, HashSet<int>> mers;
		private Dictionary<int, int>[] probablyOverlap;
		
		#endregion
		
		#region temp members
		
//		public HashSet<string> GetMers ()
//		{ 
//			PairwaiseComparision();
//			return mers; 
//		}
		
		public Dictionary<int, int>[] GetProbablyOverlap()
		{
			PairwaiseComparision();
			return probablyOverlap;
		}
		
		#endregion
	}
}

