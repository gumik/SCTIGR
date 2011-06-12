using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace SCTIGR
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ProfileControl : Gtk.Bin
	{
		public ProfileControl ()
		{
			this.Build ();
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
			Console.WriteLine(string.Format("{0} at {1}", sequence, begin));
			if (begin < 0)
			{
				InsertEmptyToAll(0, -begin);
				begin = 0;
			}
			
			var text = sequence.Clone() as string;
			
			foreach (var widget in vbox)
			{
				var sequenceControl = widget as SequenceControl;
				if (sequenceControl == null) continue;
				
				if (sequenceControl.Text.Length < begin)
				{
					sequenceControl.Text += string.Concat(Enumerable.Repeat(' ', begin - sequenceControl.Text.Length));
					sequenceControl.Text += text;
					return;
				}
				else if (sequence.Length + begin < sequenceControl.Text.Length - 2 
				         && IsSpace(sequenceControl.Text, begin, sequence.Length))
				{
					var sb = new StringBuilder();
					
					sb.Append(sequenceControl.Text.Substring(0, begin));
					sb.Append(sequence);
					sb.Append(sequenceControl.Text.Substring(begin + sequence.Length));
					
					sequenceControl.Text = sb.ToString();
					return;
				}
			}
			
			if (begin > 0)
			{
				text = text.Insert(0, string.Concat(Enumerable.Repeat(' ', begin)));
			}
			var sc = new SequenceControl() { Text = text, Visible = true };
			vbox.Add(sc);
		}
		
		private void EmptyInserted(int position)
		{
			InsertEmptyToAll(position, 1);
		}
		
		private void InsertEmptyToAll(int position, int length)
		{
			var textLine = string.Concat(Enumerable.Repeat('-', length));
			var textEmpty = string.Concat(Enumerable.Repeat(' ', length));
			
			foreach (var widget in vbox.Children)
			{
				var sequence = widget as SequenceControl;
				if (sequence == null) continue;
				if (sequence.Text.Length < position) continue;
				
				if (position > 0 && sequence.Text[position - 1] != ' ')
				{
					sequence.Text = sequence.Text.Insert(position, textLine);
				}
				else
				{
					sequence.Text = sequence.Text.Insert(position, textEmpty);
				}
			}
		}
		
		private bool IsSpace(string sequence, int begin, int length)
		{
			for (int i = begin; i < begin + length; ++i)
			{
				if (sequence[i] != ' ')
				{
					return false;
				}
			}
			
			if (begin > 0 && sequence[begin - 1] != ' ')
			{
				return false;
			}
			
			if (sequence[begin + length] != ' ')
			{
				return false;
			}
			
			return true;
		}
		
		private Profile profile;
	}
}

