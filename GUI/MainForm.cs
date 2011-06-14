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
		
		protected virtual void OnButton1Clicked (object sender, System.EventArgs e)
		{
			if (isStarted)
			{
				try
				{
					lock (mutex)
					{
						Monitor.PulseAll(mutex);
					}
				}
				catch { }
			}
			else 
			{
				isStarted = true;
				button1.Label = "Dalej";
				
				var tigr = new Tigr
					(
					 (int)merSpin.Value, 
					 (int)overhangSpin.Value, 
					 (int)overlapSpin.Value, 
					 (float)similaritySpin.Value, 
					 sequences
					);
				profilecontrol1.Profile = tigr.Profile;
				
				tigr.AssemblyCandidate += tigr_AssemblyCandidate;
				tigr.AssemblyCandidateScore += tigr_AssemblyCandidateScore;
				tigr.AssemblyGoodAlignment += tigr_AssemblyGoodAlignment;
				tigr.AssemblyEnd += tigr_AssemblyEnd;
	
				tigrThread = new Thread(new ThreadStart(tigr.Calculate));
				tigrThread.Start();
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
				
				var fileReader = new StreamReader(new FileStream(filename, FileMode.Open));
				ReadSequences(fileReader);
				fileReader.Close();
			}
		}
		
		protected virtual void OnWprowadActionActivated (object sender, System.EventArgs e)
		{
			vbox7.Visible = true;
			hpaned1.Visible = false;
		}
		
		protected virtual void OnButton4Activated (object sender, System.EventArgs e)
		{
			vbox7.Visible = false;
			hpaned1.Visible = false;
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
			
			this.sequence = sequence;
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
			table1.Visible = true;
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
			table1.Visible = false;
			sequencesList.Remove(sequence);
		}
		
		private void tigr_AssemblyEnd() 
		{
			Gtk.Application.Invoke(delegate { AssemblyEnd(); });
		}
		
		private void AssemblyEnd()
		{
			var mb = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Algorytm zakończony");
			mb.Run();
			mb.Destroy();
			isStarted = false;
			button1.Label = "Start";
			button1.Sensitive = false;
		}
		
		#endregion
		
		private void Lock()
		{
			lock (mutex)
			{
				Monitor.Wait(mutex);
			}
		}
		
		private bool ReadSequences(TextReader reader) 
		{
			try 
			{
				sequences = Utils.ReadFasta(reader);
				sequencesList.SetSequences(sequences);
				button1.Sensitive = true;
				button1.Label = "Start";
				profilecontrol1.Clear();
				table1.Visible = false;
				sequencecontrol2.Text = "";
				return true;
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
				return false;
			}
		}
		
		
		
		private Thread tigrThread;
		private object mutex = new object();
		private string lastFolder;
		private SequenceLoadForm slf;
		private bool isStarted;
		private string[] sequences;
		private string sequence;
		//private SequencesInput sequencesInput;
	}
}

