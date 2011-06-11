using System;
namespace SCTIGR
{
	public class Test
	{
		public static void Go()
		{
			var sequence = Utils.RandomSequence(80);
			Console.WriteLine(sequence);
			
			var shots = Utils.Shotgun(sequence, 160, 10, 2);
			Console.WriteLine(string.Format("Shots ({0}):", shots.Length));
			
			var tigr = new Tigr(4, 1, 3, 0.9f, shots);
			tigr.AssemblyInit += AssemblyInit;
			tigr.AssemblyCandidate += AssemblyCandidate;
			tigr.AssemblyGoodAlignment += AssemblyGoodAlignment;
			tigr.AssemblyGoodAlignmentAdded += AssemblyGoodAlignmentAdded;
			tigr.Calculate();
		}
		
		public static void AssemblyInit(string seq)
		{
			Console.WriteLine(string.Format("AssemblyInit: {0}", seq));
			//Console.ReadKey();
		}
		
		public static void AssemblyCandidate(string seq)
		{
			Console.WriteLine(string.Format("Candidate: {0}", seq));
			//Console.ReadKey();
		}
		
		public static void AssemblyGoodAlignment(int begin)
		{
			Console.WriteLine(string.Format("AssemblyGoodAlignment: {0}", begin));
			//Console.ReadKey();
		}
		
		public static void AssemblyGoodAlignmentAdded(Profile profile)
		{
			Console.WriteLine(string.Format("AssemblyGoodAlignmentAdded: \n{0}", profile));
			for (var c = 0; c < profile.Length; ++c)
			{
				Console.Write(profile[c]);
			}
			Console.WriteLine();
			//Console.ReadKey();
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
			var sequence = "AC TGTTTT";
			var profile = new Profile();
			profile.AddSequence("GGGGACTG", 0);
			
			var sm = new SmithWaterman(profile, sequence);
			sm.InsertionScore = -2;
			sm.DeletionScore = -2;
			sm.MismatchScore = -1;
			sm.MatchScore = 2;
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

