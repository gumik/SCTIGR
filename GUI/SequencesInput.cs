using System;
namespace SCTIGR
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SequencesInput : Gtk.Bin
	{
		public SequencesInput ()
		{
			this.Build ();
		}
		
		public string SequencesText
		{
			get { return textview3.Buffer.Text; }
		}
		
		public event EventHandler OkButtonPressed
		{
			add  { okButton.Activated += value; }
			remove { okButton.Activated -= value; }
		}
		
		public event EventHandler CancelButtonPressed
		{
			add  { cancelButton.Activated += value; }
			remove { cancelButton.Activated -= value; }
		}
	}
}

