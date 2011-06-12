using System;
using System.Threading;
using Gtk;

namespace SCTIGR
{
	public partial class MainForm : Gtk.Window
	{
		public MainForm () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
		
		protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			var sequence = Utils.RandomSequence(80);
			Console.WriteLine(sequence);
			
			var shots = Utils.Shotgun(sequence, 20, 30, 5);
			Console.WriteLine(string.Format("Shots ({0}):", shots.Length));
			
			var tigr = new Tigr(4, 1, 3, 0.9f, shots);
			ProfileControl.Profile = tigr.Profile;
			
			tigr.AssemblyCandidate += tigr_AssemblyCandidate;
			tigr.AssemblyCandidateScore += tigr_AssemblyCandidateScore;
			tigr.AssemblyGoodAlignment += tigr_AssemblyGoodAlignment;
			;
			//Monitor.Enter(mutex);
			
			//tigr.Calculate();
			new Thread(new ThreadStart(tigr.Calculate)).Start();
		}
		
//		protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
//		{
//			var shots = new []
//			{
//				"ATTTTTTT",
//				"TTGTTTAGGCA"
//			};
//			
//			//var tigr = new Tigr(4, 1, 3, 0.8f, shots);
//			var profile = new Profile();
//			ProfileControl.Profile = profile;
//			
//			profile.AddSequence("ATGCCATG", 0);
//			profile.AddSequence("GCCATG", 2);
//			profile.AddSequence("CCACTGATGC", -6);
//			profile.InsertEmpty(9);
//			
//			//tigr.AssemblyGoodAlignmentAdded += test;
//			
//			//tigr.Calculate();
//		}
		
		private object mutex = new object();
		
		private void tigr_AssemblyCandidate(string sequence, int begin)
		{
			Gtk.Application.Invoke(delegate { AssemblyCandidate(sequence, begin); });
		}
		
		private void AssemblyCandidate(string sequence, int begin)
		{
			if (begin < 0)
			{
				profilecontrol1.EmptySpace = -begin;
				sequencecontrol2.Text = sequence;
			}
			else
			{
				profilecontrol1.EmptySpace = 0;
				sequencecontrol2.Text = Utils.RepeatChar(' ', begin) + sequence;
			}
		}
		
		private void tigr_AssemblyCandidateScore(int overlap, int overhang, float similarity)
		{
			Gtk.Application.Invoke(delegate { AssemblyCandidateScore(overlap, overhang, similarity); });
			Lock();
		}
			
		private void AssemblyCandidateScore(int overlap, int overhang, float similarity)
		{
			overlapLabel.LabelProp = overlap.ToString();
			overhangLabel.LabelProp = overhang.ToString();
			similarityLabel.LabelProp = similarity.ToString();
		}
		
		private void tigr_AssemblyGoodAlignment()
		{
			Gtk.Application.Invoke(delegate { AssemblyGoodAlignment(); });
			Lock();
		}
		
		private void AssemblyGoodAlignment()
		{
			profilecontrol1.EmptySpace = 0;
			sequencecontrol2.Text = "";
			overlapLabel.LabelProp = "";
			overhangLabel.LabelProp = "";
			similarityLabel.LabelProp = "";
		}
		
		private void Lock()
		{
			lock (mutex)
			{
				Monitor.Wait(mutex);
			}
		}
		
		protected virtual void OnButton1Clicked (object sender, System.EventArgs e)
		{
			try
			{
				lock (mutex)
				{
					Monitor.PulseAll(mutex);
				}
			} 
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
		
		
		
		public ProfileControl ProfileControl
		{
			get { return profilecontrol1; }
		}
	}
}

