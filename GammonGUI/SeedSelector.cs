using System;
using System.Windows.Forms;

namespace GammonGUI
{
	public class SeedSelector : Form
	{
		private CheckBox		m_Random;
		private NumericUpDown	m_Seed;


		public SeedSelector()
		{
			this.Width = 160;
			this.Height = 190;
			this.Text = "Set dice seed";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.StartPosition = FormStartPosition.CenterParent;
			this.ControlBox = false;

			m_Random = new CheckBox();
			m_Random.Text = "Random";
			m_Random.Checked = true;
			m_Random.Top = 20;
			m_Random.Left = 20;
			m_Random.CheckedChanged += new EventHandler(m_UseSeed_CheckedChanged);
			this.Controls.Add( m_Random );

			Label seedLabel = new Label();
			seedLabel.Text = "Seed value";
			seedLabel.AutoSize = true;
			seedLabel.Top = 60;
			seedLabel.Left = 18;
			this.Controls.Add( seedLabel );

			m_Seed = new NumericUpDown();
			m_Seed.Maximum = 2147483647;
			m_Seed.Minimum = 0;
			m_Seed.Value = new Random().Next( 0, 2147483647 );
			m_Seed.Enabled = false;
			m_Seed.Top = 78;
			m_Seed.Left = 18;
			m_Seed.Leave += new EventHandler(m_Seed_Leave);
			m_Seed.KeyPress += new KeyPressEventHandler(m_Seed_KeyPress);
			this.Controls.Add( m_Seed );

			Button ok = new Button();
			ok.Text = "OK";
			ok.Left = 38;
			ok.Top = 118;
			ok.Click += new EventHandler(ok_Click);
			ok.Show();
			this.Controls.Add( ok );
		}


		public bool RandomSeed
		{
			get{ return m_Random.Checked; }
		}


		public int Seed
		{
			get
			{
				return (int)m_Seed.Value;
			}
		}


		private void ok_Click(object sender, EventArgs e)
		{
			this.Close();
		}


		protected override void OnClosing( System.ComponentModel.CancelEventArgs e )
		{
			base.OnClosing( e );
			this.Hide();
			e.Cancel = true;
		}


		private void m_UseSeed_CheckedChanged(object sender, EventArgs e)
		{
			m_Seed.Enabled = !m_Random.Checked;
		}


		private void m_Seed_Leave(object sender, EventArgs e)
		{
			if ( m_Seed.Value > m_Seed.Maximum )
				m_Seed.Value = m_Seed.Maximum;
			if ( m_Seed.Value < m_Seed.Minimum )
				m_Seed.Value = m_Seed.Minimum;

		}

		private void m_Seed_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ( e.KeyChar == ',' || e.KeyChar == '-' )
				e.Handled = true;
		}
	}

}
