using System;
using System.Threading;
using Gtk;
using System.IO;

namespace SCTIGR
{
	public partial class MainForm : Gtk.Window
	{
		public MainForm () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
		
		#region gui events
		
		protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			var sequence = Utils.RandomSequence(80);
			Console.WriteLine(sequence);
			
			var shots = Utils.Shotgun(sequence, 20, 30, 5);
			Console.WriteLine(string.Format("Shots ({0}):", shots.Length));
			
			var tigr = new Tigr(4, 1, 3, 0.9f, shots);
			profilecontrol1.Profile = tigr.Profile;
			
			tigr.AssemblyCandidate += tigr_AssemblyCandidate;
			tigr.AssemblyCandidateScore += tigr_AssemblyCandidateScore;
			tigr.AssemblyGoodAlignment += tigr_AssemblyGoodAlignment;

			tigrThread = new Thread(new ThreadStart(tigr.Calculate));
			tigrThread.Start();
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
		
		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			Application.Quit();
			try 
			{
				tigrThread.Abort();
			}
			catch { }
		}
		
		protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
		{
			var fileOpenDialog = new FileChooserDialog
				(
				 "Wybierz plik...", 
				 this, 
				 FileChooserAction.Open, 
				 "Otwórz", ResponseType.Accept,
				 "Anuluj", ResponseType.Cancel
				);
			
			if (lastFolder != null)
			{
				fileOpenDialog.SetCurrentFolder(lastFolder);
			}
			
			var responseType = fileOpenDialog.Run();
			var filename = fileOpenDialog.Filename;
			lastFolder = fileOpenDialog.CurrentFolder;
			fileOpenDialog.Destroy();
			if (responseType == (int)ResponseType.Accept)
			{
				try 
				{
					var fileReader = new StreamReader(new FileStream(filename, FileMode.Open));
					var sequences = Utils.ReadFasta(fileReader);
					fileReader.Close();
					sequencesList.SetSequences(sequences);
				}
				catch
				{
					var md = new MessageDialog
						(
						 this, 
                         DialogFlags.DestroyWithParent,
	                     MessageType.Error, 
                         ButtonsType.Close, 
						 "Błędny format pliku."
						);
					md.Title = "Błąd";
					md.Run();
					md.Destroy();
				}
			}
		}
		
		protected virtual void OnWprowadActionActivated (object sender, System.EventArgs e)
		{
			slf = new SequenceLoadForm();
			//slf.Modal = true;
			//slf.Show();
			slf.Visible = true;
		}
		
		#endregion
		
		#region tigr event handlers
		
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
		
		#endregion
		
		private void Lock()
		{
			lock (mutex)
			{
				Monitor.Wait(mutex);
			}
		}
		
		
		
		private Thread tigrThread;
		private object mutex = new object();
		private string lastFolder;
		private SequenceLoadForm slf;
	}
}

