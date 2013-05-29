using System;
using System.Windows.Forms;

namespace GammonGUI
{
	public class AboutGUI : Form
	{
		public AboutGUI()
		{
			this.Width = 415;
			this.Height = 345;
			this.Text = "About";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.StartPosition = FormStartPosition.CenterParent;
			this.ControlBox = false;

			Panel frame = new Panel();
			frame.BorderStyle = BorderStyle.FixedSingle;
			frame.Left = 15;
			frame.Width = 380;
			frame.Top = 15;
			frame.Height = 258;
			frame.Show();
			this.Controls.Add( frame );

			Label about = new Label();
			//about.BackColor = System.Drawing.Color.Green;
			about.Left = 2;
			about.Width = frame.Width - 4;
			about.Top = 2;
			about.Height = frame.Height - 4;
			about.Show();
			frame.Controls.Add( about );

			string a = "";

			a += "This backgammon program is freeware and has been part of a Master thesis on intelligent game agents. The original program and its associated paper were composed by Mikael Heinze at Aalborg University Esbjerg in Denmark in the period 2nd February to 21st December 2004.\n\n";

			a += "The programs purpose was to work as a problem/test domain for an adaptive and intelligent computer player named Fuzzeval based on fuzzy logic and fuzzy control. For evaluation purpose other computer players based on other principles has also been implemented. To configure an opponent (agent) one can select what decision module that is to be used for move and cube evaluation.\n\n";

			a += "This is however a greatly improved version of this original backgammon program completed at the 26th of April 2005. This version includes a large range of new features, improvements and bug fixes. Examples are a stronger computer opponent named TD-NN 2 and implementation of the cube. It is however only the two TD-NN evaluators that in reality supports cube handling.\n\n";

			a += "Happy playing.";

			about.Text = a;

			Button ok = new Button();
			ok.Text = "OK";
			ok.Left = 320;
			ok.Top = 280;
			ok.Click += new EventHandler(ok_Click);
			ok.Show();
			this.Controls.Add( ok );
		}

		private void ok_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}

}
