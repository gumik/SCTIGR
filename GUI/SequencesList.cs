using System;
using System.Collections.Generic;

namespace SCTIGR
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SequencesList : Gtk.Bin
	{
		public SequencesList ()
		{
			this.Build ();
		}
		
		public void SetSequences(string[] sequences)
		{
			var children = vbox.Children.Clone() as ICollection<Gtk.Widget>;
			foreach (var control in children)
			{
				control.Destroy();
			}
			
			foreach (var sequence in sequences)
			{
				var sequenceControl = new SequenceControl() { Text = sequence, Visible = true };
				vbox.Add(sequenceControl);
			}
		}
		
		public void Remove(string sequence)
		{
			SequenceControl toRemove = null;
			foreach (var control in vbox.Children)
			{
				var sequenceControl = control as SequenceControl;
				if (sequenceControl.Text == sequence)
				{
					toRemove = sequenceControl;
					break;
				}
			}
			
			if (toRemove != null)
			{
				toRemove.Destroy();
			}
		}
	}
}

