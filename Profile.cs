using System;
using System.Collections.Generic;
using System.Text;

namespace SCTIGR
{
	public class Profile
	{
		public Profile ()
		{
			profile = new List<int[]>();
		}
		
		public void AddSequence(string sequence, int begin)
		{
			SequenceAdded(sequence, begin);
			
			if (begin < 0) // at the beggining
			{
				profile.InsertRange(0, NewEmptyFragment(-begin));
				begin = 0;
			}
			
			if (begin + sequence.Length > profile.Count) // at the end
			{
				var count = begin + sequence.Length - profile.Count;
				profile.AddRange(NewEmptyFragment(count));
			}
			
			foreach (var c in sequence)
			{
				profile[begin][CharToKey(c)]++;
				++begin;
			}
			
		}
		
		public void InsertEmpty(int position)
		{
			profile.InsertRange(position, NewEmptyFragment(1));
			EmptyInserted(position);
		}
		
		public float Score(int index, char c, float matchScore, float mismatchScore)
		{
			var key = CharToKey(c);
			var score = 0f;
			
			foreach (var cc in profile[index])
			{
				score += (key == cc) ? matchScore : mismatchScore;
			}
			
			return score;
		}
		
		public char this[int i]
		{
			get
			{
				var best = 0;
				var bestCount = profile[i][0];
				
				for (int j = 1; j < 5; ++j)
				{
					if (profile[i][j] > bestCount)
					{
						best = j;
						bestCount = profile[i][j];
					}
				}
				
				return KeyToChar(best);
			}
		}
				
		public override string ToString ()
		{
			var sb = new StringBuilder();
			var l =  new [] { 'A', 'T', 'C', 'G', ' ' };
			
			for (int i = 0; i < 5; ++i)
			{
				sb.Append(l[i]);
				sb.Append(": ");
				
				foreach (var element in profile)
				{
					sb.Append(element[i]);
					sb.Append(' ');
				}
				sb.AppendLine();
			}
			
			return sb.ToString();
		}
		
		public int Length { get { return profile.Count; } }
		
		public event Action<string, int> SequenceAdded = delegate { };
		public event Action<int> EmptyInserted = delegate { };
		
		private int CharToKey(char c)
		{
			switch (c)
			{
			case 'A': return 0;
			case 'T': return 1;
			case 'C': return 2;
			case 'G': return 3;
			case ' ': return 4;
			default: throw new ArgumentOutOfRangeException(string.Format("{0} is not one of {{A, T, C, G, <space>}}", c));
			}
		}
		
		private char KeyToChar(int key)
		{
			switch (key)
			{
			case 0: return 'A';
			case 1: return 'T';
			case 2: return 'C';
			case 3: return 'G';
			case 4: return ' ';
			default: throw new ArgumentOutOfRangeException();
			}
		}
				
		private int[][] NewEmptyFragment(int length)
		{
			var tab = new int[length][];
			for (int i = 0; i < length; ++i)
			{
				tab[i] = new int[5];
			}
			
			return tab;
		}
		
		private List<int[]> profile;
	}
	
	
}

