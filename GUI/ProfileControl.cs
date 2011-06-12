using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace SCTIGR
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ProfileControl : Gtk.Bin
	{
		public ProfileControl ()
		{
			this.Build ();
			sequences = new LinkedList<SequenceControl>();
		}
		
		public Profile Profile
		{
			set 
			{ 
				this.profile = value;
				profile.SequenceAdded += SequenceAdded;
				profile.EmptyInserted += EmptyInserted;
			}
		}
		
		private void SequenceAdded(string sequence, int begin)
		{
			//Monitor.Enter(_lock);
			
			if (begin < 0)
			{
				InsertEmpty(0, -begin);
				begin = 0;
			}
			
			var text = sequence.Clone() as string;
			
			foreach (var widget in vbox)
			//foreach (var sequenceControl in sequences)
			{
				var sequenceControl = widget as SequenceControl;
				if (sequenceControl == null) continue;
				
				if (sequenceControl.Text.Length < begin)
				{
					sequenceControl.Text = InsertEmpty(sequenceControl.Text.Length, begin - sequenceControl.Text.Length, sequenceControl.Text);
					sequenceControl.Text += text;
					//Monitor.Exit(_lock);
					return;
				}
			}
			
			text = InsertEmpty(0, begin, text);
			//var children = vbox.Children.Length;
			var sc = new SequenceControl() { Text = text, Visible = true };
			vbox.Add(sc);
			//sequences.AddLast(sc);
			
			//Monitor.Exit(_lock);
		}
		
		private void EmptyInserted(int position)
		{
			InsertEmpty(position, 1);
		}
		
		private void InsertEmpty(int position, int length)
		{
			var sb = new StringBuilder(length);
			for (int i = 0; i < length; ++i)
			{
				sb.Append(' ');
			}
			var text = sb.ToString();
			
			foreach (var widget in vbox.Children)
			{
				var sequence = widget as SequenceControl;
				if (sequence == null) continue;
				
				sequence.Text = sequence.Text.Insert(position, text);
			}
		}
		
		private string InsertEmpty(int position, int length, string str)
		{
			var sb = new StringBuilder(length);
			for (int i = 0; i < length; ++i)
			{
				sb.Append(' ');
			}
			var text = sb.ToString();
			return str.Insert(position, text);
		}
		
		private Profile profile;
		private LinkedList<SequenceControl> sequences;
		//private bool added;
		//private object _lock = new object();
	}
}

