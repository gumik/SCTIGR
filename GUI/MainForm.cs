using System;
using System.Threading;
using Gtk;

namespace SCTIGR
{
	public partial class MainForm : Gtk.Window
	{
		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			Application.Quit();
		}
		
		
		public MainForm () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
		
		protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			var sequence = Utils.RandomSequence(80);
			Console.WriteLine(sequence);
			
			var shots = Utils.Shotgun(sequence, 160, 10, 2);
			Console.WriteLine(string.Format("Shots ({0}):", shots.Length));
			
			var tigr = new Tigr(4, 1, 3, 0.9f, shots);
			ProfileControl.Profile = tigr.Profile;
			
			tigr.AssemblyGoodAlignmentAdded += test;
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
		private void test(Profile profile)
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

