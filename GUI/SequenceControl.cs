using System;
using System.Text;
using Pango;

namespace SCTIGR
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SequenceControl : Gtk.Bin
	{
		public SequenceControl ()
		{
			this.Build ();
			label.ModifyFont(FontDescription.FromString("Monospace 10"));
			label.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(255, 0, 0));
		}
		
		private string text;
		public string Text
		{
			get { return text; }
			set 
			{
				text = value; 
				label.LabelProp = ParseText(value);
			}
		}
		
		private string ParseText(string text)
		{
			var sb = new StringBuilder();
			var format = "<span bgcolor=\"#{0}\">{1}</span>";
			
			foreach (var c in text)
			{
				switch (c)
				{
				case 'A':
					sb.Append(string.Format(format, "88FF88", c));
					break;
					
				case 'T':
					sb.Append(string.Format(format, "FF8888", c));
					break;
					
				case 'G':
					sb.Append(string.Format(format, "8888FF", c));
					break;
					
				case 'C':
					sb.Append(string.Format(format, "FFFF88", c));
					break;
					
				case '-':
					sb.Append('-');
					break;
					
				case ' ':
					sb.Append(' ');
					break;
					
				default:
					sb.Append('x');
					break;
				}
			}
			
			return sb.ToString();
		}
	}
}

