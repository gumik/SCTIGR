using System;
namespace SCTIGR
{
	public class Test
	{
		public static void Go()
		{
			var sequence = Utils.RandomSequence(2500);
			Console.WriteLine(sequence);
			
			var shots = Utils.Shotgun(sequence, 25000, 100, 16);
			Console.WriteLine(string.Format("Shots ({0}):", shots.Length));
//			foreach (var shot in shots)
//			{
//				Console.WriteLine(shot);
//			}
			
//			var shots = new [] 
//			{
//				"ATACTGACATCAG",
//				"GGAGAAATAC",
//				
//			};
			
			var tigr = new Tigr(11, shots);
			var mers = tigr.GetProbablyOverlap();
//			
//			Console.WriteLine(string.Format("Mers ({0}):", mers.Count));
//			foreach (var mer in mers)
//			{
//				Console.WriteLine(mer);
//			}
			
			for (int i = 0; i < mers.Length; ++i)
			{
				//Console.WriteLine(string.Format("{0}: ", shots[i]));
				Console.WriteLine(string.Format("{0}: {1} ", i, mers[i].Count));
				
//				foreach (var key in mers[i].Keys)
//				{
//					Console.WriteLine(string.Format("\t{0} ({1})", shots[key], mers[i][key]));
//				}
			}
		}
	}
}

