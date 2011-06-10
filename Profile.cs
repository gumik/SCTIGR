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

