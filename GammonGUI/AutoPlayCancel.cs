/*
using System;
using System.Windows.Forms;


namespace GammonGUI
{
	public class AutoPlayCancelForm : Form
	{
		public MainForm		m_Owner;
		public Label		m_Text;
		public Button		m_Stop;

		public AutoPlayCancelForm( MainForm owner )
		{
			m_Owner = owner;

			this.Text = "Stop auto play";
			this.Width = 165;
			this.Height = 100;
			this.MaximizeBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.ControlBox = false;

			m_Text = new Label();
			m_Text.Text = "Press button to end auto play";
			m_Text.Left = 5;
			m_Text.Top = 5;
			m_Text.AutoSize = true;
			m_Text.Show();
			this.Controls.Add( m_Text );

			m_Stop = new Button();
			m_Stop.Text = "Stop";
			m_Stop.Left = 40;
			m_Stop.Top = 30;
			m_Stop.Show();
			m_Stop.Click += new EventHandler( m_Stop_Click );
			this.Controls.Add( m_Stop );
		}

		private void m_Stop_Click(object sender, EventArgs e)
		{
			m_Owner.m_AutoPlayMode = false;
			this.Close();
		}
	}
}
*/