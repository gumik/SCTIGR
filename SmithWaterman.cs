using System;
using System.Collections.Generic;

namespace SCTIGR
{
	public class SmithWaterman
	{
		#region public methods
		
		public SmithWaterman (Profile profile, string sequence)
		{
			MatchScore = 2;
			MismatchScore = -1;
			InsertionScore = -2;
			DeletionScore = -2;
			
			this.profile = profile;
			this.sequence = sequence;
		}
		
		public void Calculate()
		{
			int n = profile.Length;
			int m = sequence.Length;
			score = new int[n + 1, m + 1];
			prev = new Tuple<int, int>[n + 1, m + 1];
			
			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < m; ++j)
				{
					var mm = score[i, j] + Score(profile[i], sequence[j]);
					var del = score[i, j+1] + Score(profile[i], '-');
					var ins = score[i+1, j] + Score('-', sequence[j]);
					
					score[i+1, j+1] = 0;
					
					if (mm > 0)
					{
						score[i+1, j+1] = mm;
						prev[i+1, j+1] = new Tuple<int, int>(i, j);
					}
					
					if (del > mm && del > 0)
					{
						score[i+1, j+1] = del;
						prev[i+1, j+1] = new Tuple<int, int>(i, j+1);
					}
					
					if (ins > del && ins > mm && ins > 0)
					{
						score[i+1, j+1] = ins;
						prev[i+1, j+1] = new Tuple<int, int>(i+1, j);
					}
				}
			}
		}
		
		public LinkedList<Tuple<int,int>> GetBest()
		{
			var n = profile.Length;
			var m = sequence.Length;
			var best = new Tuple<int,int>(0,0);
			var bestScore = 0;
			
			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < m; ++j)
				{
					if (score[i+1, j+1] > bestScore)
					{
						bestScore = score[i+1, j+1];
						best = prev[i+1, j+1];
					}
				}
			}
			
			var seq = new LinkedList<Tuple<int, int>>();
			
			var act = best;
			var p = null as Tuple<int, int>;
			while (act != null)
			{
				Tuple<int, int> toAdd;
				
				if (p != null && p.Item1 == act.Item1)
				{
					toAdd = new Tuple<int, int>(-1, act.Item2);
				}
				else if (p != null && p.Item2 == act.Item2)
				{
					toAdd = new Tuple<int, int>(act.Item1, -1);
				}
				else
				{
					toAdd = new Tuple<int, int>(act.Item1, act.Item2);
				}
				
				seq.AddFirst(toAdd);
				p = act;
				act = prev[act.Item1, act.Item2];
			}
			
			return seq;
		}
		
		#endregion
		
		#region public properties
		
		public int MatchScore { get; set; }
		public int MismatchScore { get; set; }
		public int InsertionScore { get; set; }
		public int DeletionScore { get; set; }
		
		#endregion
		
		#region private methods
		
		private int Score(char a, char b)
		{
			if (a == b) return MatchScore;
			if (a == '-') return InsertionScore;
			if (b == '-') return DeletionScore;
			return MismatchScore;
		}
		
		#endregion
		
		#region private fields
		
		private Profile profile;
		private string sequence;
		private int[,] score;
		private Tuple<int, int>[,] prev;
		
		#endregion
		
		#region temp members
		
		public void PrintResult()
		{			
			var n = profile.Length;
			var m = sequence.Length;
			
			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < m; ++j)
				{
					Console.Write ((int)score[i+1,j+1]);
					Console.Write(' ');
				}
				Console.WriteLine();
			}
			
			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < m; ++j)
				{
					char c = 'x';
					if (prev[i+1,j+1] != null)
					{
						var x = prev[i+1,j+1].Item1;
						var y = prev[i+1,j+1].Item2;
						
						if (x < i + 1 && y < j + 1) c = '\\';
						if (x < i + 1 && y == j + 1) c = '|';
						if (x == i + 1 && y < j + 1) c = '-';
					}

					Console.Write (c);
					Console.Write (' ');
				}
				Console.WriteLine();
			}
		}
		
		#endregion
	}
}

