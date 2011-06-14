using System;
using System.IO;
using Gtk;

namespace SCTIGR
{
	public partial class SequenceLoadForm : Gtk.Window
	{
		protected virtual void OnButton1Activated (object sender, System.EventArgs e)
		{
			Console.WriteLine("dupa");
			try 
			{
				var reader = new StringReader(textview1.Buffer.Text);
				var Sequences = Utils.ReadFasta(reader);
				reader.Close();
				Dispose();
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
		
		public string[] Sequences { get; private set; }
		
		public SequenceLoadForm () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			button1.Activated += (a, b) => Console.WriteLine("dasdf");
		}
	}
}

