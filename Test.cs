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
		
		public static void BigTest()
		{
			var time = DateTime.Now;
			var lastCount = 0;
			var count = 25000 * 25000 * 1;
			
			for (int i = 0; i < count; ++i)
			{
				var diff = (DateTime.Now - time).TotalSeconds;
				if (diff >= 2)
				{
					var perSec = (i - lastCount) / diff;
					var remaining = (count - lastCount) / perSec;
					Console.WriteLine(string.Format("{0} /s, {1} remaining", perSec, remaining));
					
					time = DateTime.Now;
					lastCount = i;
				}
			}
		}
		
		public static void ProfileTest()
		{
			var profile = new Profile();
			
			while (true)
			{
				var sequence = Console.ReadLine();
				var begin = int.Parse(Console.ReadLine());
				
				profile.AddSequence(sequence, begin);
				Console.WriteLine(profile);
			}
		}
		
		public static void SM()
		{
			var sequence = "AC TGGCAT";
			var profile = new Profile();
			profile.AddSequence("TATCACTG", 0);
			
			var sm = new SmithWaterman(profile, sequence);
			//sm.InsertionScore = 0;
			sm.Calculate();
			sm.PrintResult();
			var seq = sm.GetBest();
			
			foreach (var i in seq)
			{
				Console.Write(string.Format("({0},{1}) ", i.Item1, i.Item2));
			}
		}
	}
}

