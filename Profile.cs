using System;
using System.Collections.Generic;
using System.Text;

namespace SCTIGR
{
	public class Profile
	{
		public Profile ()
		{
			profile = new LinkedList<int[]>();
		}
		
		public void AddSequence(string sequence, int begin)
		{
			if (begin < 0) // at the beggining
			{
				for (int i = -begin - 1; i >= 0; --i)
				{
					profile.AddFirst(new int[5]);
				}
				
				begin = 0;
			}
			
			if (begin + sequence.Length > profile.Count) // at the end
			{
				var count = begin + sequence.Length - profile.Count;
				for (int i = 0; i < count; ++i)
				{
					profile.AddLast(new int[5]);
				}
			}
			
			var node = profile.First;
			for (int i = 0; i < begin; ++i)
			{
				node = node.Next;
			}
			
			foreach (var c in sequence)
			{
				node.Value[CharToKey(c)]++;
				node = node.Next;
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
		
		private LinkedList<int[]> profile;
	}
	
	
}

