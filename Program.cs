using System;
using Gtk;

namespace SCTIGR
{
	public class Program
	{
		public static void Main()
		{
			//Test.Go();
			//Test.BigTest();
			//Test.ProfileTest();
			//Test.SM();
			
			//Test.Gui();
			//Test.Fasta();
			//Test.Shotgun();
			Application.Init();
			var form = new MainForm();
			form.Visible = true;
			Application.Run();
		}
	}
}

