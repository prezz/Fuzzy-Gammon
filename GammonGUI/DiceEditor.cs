using System;
using System.Windows.Forms;
using GammonGame;

namespace GammonGUI
{
	public class DiceEditor : Form
	{
		public static DiceEditor	m_Singelton = null;

		public GroupBox				m_GroupBox1;
		public GroupBox				m_GroupBox2;

		public RadioButton			m_Dice1_Val1;
		public RadioButton			m_Dice1_Val2;
		public RadioButton			m_Dice1_Val3;
		public RadioButton			m_Dice1_Val4;
		public RadioButton			m_Dice1_Val5;
		public RadioButton			m_Dice1_Val6;

		public RadioButton			m_Dice2_Val1;
		public RadioButton			m_Dice2_Val2;
		public RadioButton			m_Dice2_Val3;
		public RadioButton			m_Dice2_Val4;
		public RadioButton			m_Dice2_Val5;
		public RadioButton			m_Dice2_Val6;

		public Button				m_SetValue;

		public bool					m_SetNextMode;

		public static MainForm		m_OwnerForm;


		private DiceEditor()
		{
			this.Text = "Set Dice Roll";
			this.Width = 234;
			this.Height = 250;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimizeBox = false;
			this.MaximizeBox = false;


			m_GroupBox1 = new GroupBox();
			m_GroupBox1.Text = "Dice 1 value";
			m_GroupBox1.Width = 90;
			m_GroupBox1.Height = 160;
			m_GroupBox1.Left = 20;
			m_GroupBox1.Top = 10;
			m_GroupBox1.Show();
			this.Controls.Add( m_GroupBox1 );

			m_GroupBox2 = new GroupBox();
			m_GroupBox2.Text = "Dice 2 value";
			m_GroupBox2.Width = 90;
			m_GroupBox2.Height = 160;
			m_GroupBox2.Left = 120;
			m_GroupBox2.Top = 10;
			m_GroupBox2.Show();
			this.Controls.Add( m_GroupBox2 );

			m_SetValue = new Button();
			m_SetValue.Text = "Roll";
			m_SetValue.Left = 76;
			m_SetValue.Top = 182;
			m_SetValue.Click += new EventHandler( m_SetValue_Click );
			this.Controls.Add( m_SetValue );


			m_Dice1_Val1 = new RadioButton();
			m_Dice1_Val1.Text = "1";
			m_Dice1_Val1.Width = 50;
			m_Dice1_Val1.Left = 20;
			m_Dice1_Val1.Top = 20;
			m_GroupBox1.Controls.Add( m_Dice1_Val1 );

			m_Dice1_Val2 = new RadioButton();
			m_Dice1_Val2.Text = "2";
			m_Dice1_Val2.Width = 50;
			m_Dice1_Val2.Left = 20;
			m_Dice1_Val2.Top = 40;
			m_GroupBox1.Controls.Add( m_Dice1_Val2 );

			m_Dice1_Val3 = new RadioButton();
			m_Dice1_Val3.Text = "3";
			m_Dice1_Val3.Width = 50;
			m_Dice1_Val3.Left = 20;
			m_Dice1_Val3.Top = 60;
			m_GroupBox1.Controls.Add( m_Dice1_Val3 );

			m_Dice1_Val4 = new RadioButton();
			m_Dice1_Val4.Text = "4";
			m_Dice1_Val4.Width = 50;
			m_Dice1_Val4.Left = 20;
			m_Dice1_Val4.Top = 80;
			m_GroupBox1.Controls.Add( m_Dice1_Val4 );

			m_Dice1_Val5 = new RadioButton();
			m_Dice1_Val5.Text = "5";
			m_Dice1_Val5.Width = 50;
			m_Dice1_Val5.Left = 20;
			m_Dice1_Val5.Top = 100;
			m_GroupBox1.Controls.Add( m_Dice1_Val5 );

			m_Dice1_Val6 = new RadioButton();
			m_Dice1_Val6.Text = "6";
			m_Dice1_Val6.Width = 50;
			m_Dice1_Val6.Left = 20;
			m_Dice1_Val6.Top = 120;
			m_Dice1_Val6.Checked = true;
			m_GroupBox1.Controls.Add( m_Dice1_Val6 );


			m_Dice2_Val1 = new RadioButton();
			m_Dice2_Val1.Text = "1";
			m_Dice2_Val1.Width = 50;
			m_Dice2_Val1.Left = 20;
			m_Dice2_Val1.Top = 20;
			m_GroupBox2.Controls.Add( m_Dice2_Val1 );

			m_Dice2_Val2 = new RadioButton();
			m_Dice2_Val2.Text = "2";
			m_Dice2_Val2.Width = 50;
			m_Dice2_Val2.Left = 20;
			m_Dice2_Val2.Top = 40;
			m_GroupBox2.Controls.Add( m_Dice2_Val2 );

			m_Dice2_Val3 = new RadioButton();
			m_Dice2_Val3.Text = "3";
			m_Dice2_Val3.Width = 50;
			m_Dice2_Val3.Left = 20;
			m_Dice2_Val3.Top = 60;
			m_GroupBox2.Controls.Add( m_Dice2_Val3 );

			m_Dice2_Val4 = new RadioButton();
			m_Dice2_Val4.Text = "4";
			m_Dice2_Val4.Width = 50;
			m_Dice2_Val4.Left = 20;
			m_Dice2_Val4.Top = 80;
			m_GroupBox2.Controls.Add( m_Dice2_Val4 );

			m_Dice2_Val5 = new RadioButton();
			m_Dice2_Val5.Text = "5";
			m_Dice2_Val5.Width = 50;
			m_Dice2_Val5.Left = 20;
			m_Dice2_Val5.Top = 100;
			m_GroupBox2.Controls.Add( m_Dice2_Val5 );

			m_Dice2_Val6 = new RadioButton();
			m_Dice2_Val6.Text = "6";
			m_Dice2_Val6.Width = 50;
			m_Dice2_Val6.Left = 20;
			m_Dice2_Val6.Top = 120;
			m_Dice2_Val6.Checked = true;
			m_GroupBox2.Controls.Add( m_Dice2_Val6 );
		}


		public static void ShowDiceEditor( MainForm owner, bool setNextMode )
		{
			if ( m_Singelton == null )
				m_Singelton = new DiceEditor();

			m_Singelton.m_SetNextMode = setNextMode;

			m_OwnerForm = owner;
			m_Singelton.ShowDialog( owner );
		}


		protected override void OnClosing( System.ComponentModel.CancelEventArgs e )
		{
			base.OnClosing( e );
			m_Singelton.Hide();
			e.Cancel = true;
		}


		public void m_SetValue_Click( object sender, EventArgs e )
		{
			int val1 = m_OwnerForm.m_GammonGame.GetDiceValue( 0 );
			int val2 = m_OwnerForm.m_GammonGame.GetDiceValue( 1 );
			
			if ( m_Dice1_Val1.Checked )
				val1 = 1;
			else if ( m_Dice1_Val2.Checked )
				val1 = 2;
			else if ( m_Dice1_Val3.Checked )
				val1 = 3;
			else if ( m_Dice1_Val4.Checked )
				val1 = 4;
			else if ( m_Dice1_Val5.Checked )
				val1 = 5;
			else if ( m_Dice1_Val6.Checked )
				val1 = 6;

			if ( m_Dice2_Val1.Checked )
				val2 = 1;
			else if ( m_Dice2_Val2.Checked )
				val2 = 2;
			else if ( m_Dice2_Val3.Checked )
				val2 = 3;
			else if ( m_Dice2_Val4.Checked )
				val2 = 4;
			else if ( m_Dice2_Val5.Checked )
				val2 = 5;
			else if ( m_Dice2_Val6.Checked )
				val2 = 6;

			if ( m_SetNextMode )
				m_OwnerForm.m_GammonGame.SetNextDiceValues( val1, val2 );
			else
				m_OwnerForm.m_GammonGame.SetDiceValues( val1, val2 );

			this.Close();
		}
	}
}
