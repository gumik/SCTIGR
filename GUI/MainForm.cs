using System;
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
	}
}

