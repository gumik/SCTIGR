using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Pango;

namespace SCTIGR
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ProfileControl : Gtk.Bin
	{
		public ProfileControl ()
		{
			this.Build ();
			mutex = new object();
			spaceLabel1.ModifyFont(FontDescription.FromString("Monospace 10"));
			spaceLabel2.ModifyFont(FontDescription.FromString("Monospace 10"));
		}
		
		public Profile Profile
		{
			set 
			{ 
				this.profile = value;
				
				profile.SequenceAdded += (s, b) => Gtk.Application.Invoke(delegate 
				{
					lock (mutex)
					{
						SequenceAdded(s, b); 
					}
				});
				
				profile.EmptyInserted += (p) => Gtk.Application.Invoke(delegate 
				{
					lock (mutex)
					{
						EmptyInserted(p); 
					}
				});
			}
		}
		
		public int EmptySpace
		{
			set 
			{
				spaceLabel1.LabelProp = string.Concat(Enumerable.Repeat(' ', value));
				spaceLabel2.LabelProp = string.Concat(Enumerable.Repeat(' ', value));
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
					UpdateSequence();
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
					UpdateSequence();
					return;
				}
			}
			
			if (begin > 0)
			{
				text = text.Insert(0, string.Concat(Enumerable.Repeat(' ', begin)));
			}
			var sc = new SequenceControl() { Text = text, Visible = true };
			vbox.Add(sc);
			
			UpdateSequence();
		}
		
		private void EmptyInserted(int position)
		{
			InsertEmptyToAll(position, 1);
			UpdateSequence();
		}
		
		private void InsertEmptyToAll(int position, int length)
		{
			var textLine = string.Concat(Enumerable.Repeat('-', length));
			var textEmpty = string.Concat(Enumerable.Repeat(' ', length));
			
			foreach (var widget in vbox.Children)
			{
				var sequence = widget as SequenceControl;
				if (sequence == null) continue;
				if (sequence.Text.Length <= position) continue;
				
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
		
		private void UpdateSequence()
		{
			var sb = new StringBuilder();
			
			for (int i = 0; i < profile.Length; ++i)
			{
				sb.Append(profile[i]);
			}
			
			sequencecontrol1.Text = sb.ToString();
		}
		
		private Profile profile;
		private object mutex;
	}
}

