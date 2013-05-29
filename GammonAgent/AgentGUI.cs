using System;
using System.Windows.Forms;
using GammonGame;


namespace GammonAgent
{
	class AgentGUI : Form
	{
		public int					m_Id;
		public static int			m_IdCounter = 0;
		public static int			m_PlayerLightOwner = 0;
		public static int			m_PlayerDarkOwner = 0;
		public static int			m_PlayerBothOwner = 0;
		public static int			m_PlayerLearnOwner = 0;

		public Label				m_PossibilitiesLabel;
		public ListBox				m_PossibilitiesList;
		public Label				m_NumberPossibilitiesLabel;

		public GroupBox				m_GroupBoxPlayer;
		public RadioButton			m_PlayerLight;
		public RadioButton			m_PlayerDark;
		public RadioButton			m_PlayerNone;
		public RadioButton			m_PlayerBoth;

		public Panel				m_LearnPanel;
		public CheckBox				m_Learn;
		
		public Label				m_EvaluatorLabel;
		public ComboBox				m_PluginSelector;


		public delegate void RadioGroupUpdater();
		public static RadioGroupUpdater[] m_UpdaterList = null;


		public AgentGUI( string caption )
		{
			m_Id = ++m_IdCounter;
			RadioGroupUpdater[] tmpOld = m_UpdaterList;
			m_UpdaterList = new RadioGroupUpdater[m_Id];
			if ( tmpOld != null )
				for ( int i = 0; i < tmpOld.Length; i++ )
					m_UpdaterList[i] = tmpOld[i];
			m_UpdaterList[m_Id-1] = new RadioGroupUpdater( SyncPlayerSelectors );


			this.Text = caption;
			this.Width = 430;
			this.Height = 340;
			this.MaximizeBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;


			m_GroupBoxPlayer = new GroupBox();
			m_GroupBoxPlayer.Text = "Play as";
			m_GroupBoxPlayer.Width = 100;
			m_GroupBoxPlayer.Height = 150;
			m_GroupBoxPlayer.Left = 10;
			m_GroupBoxPlayer.Top = 25;
			m_GroupBoxPlayer.Show();
			this.Controls.Add( m_GroupBoxPlayer );

			m_PlayerLight = new RadioButton();
			m_PlayerLight.Text = "Light";
			m_PlayerLight.Width = 50;
			m_PlayerLight.Left = 25;
			m_PlayerLight.Top = 50;
			m_PlayerLight.Checked = true;
			m_PlayerLight.CheckedChanged += new EventHandler( m_PlayerSelect_Click );
			m_PlayerLight.Show();
			m_GroupBoxPlayer.Controls.Add( m_PlayerLight );

			m_PlayerDark = new RadioButton();
			m_PlayerDark.Text = "Dark";
			m_PlayerDark.Width = 50;
			m_PlayerDark.Left = 25;
			m_PlayerDark.Top = 80;
			m_PlayerDark.CheckedChanged += new EventHandler( m_PlayerSelect_Click );
			m_PlayerDark.Show();
			m_GroupBoxPlayer.Controls.Add( m_PlayerDark );

			m_PlayerNone = new RadioButton();
			m_PlayerNone.Text = "None";
			m_PlayerNone.Width = 50;
			m_PlayerNone.Left = 25;
			m_PlayerNone.Top = 20;
			m_PlayerNone.CheckedChanged += new EventHandler( m_PlayerSelect_Click );
			m_PlayerNone.Show();
			m_GroupBoxPlayer.Controls.Add( m_PlayerNone );
			
			m_PlayerBoth = new RadioButton();
			m_PlayerBoth.Text = "Both";
			m_PlayerBoth.Width = 50;
			m_PlayerBoth.Left = 25;
			m_PlayerBoth.Top = 110;
			m_PlayerBoth.CheckedChanged += new EventHandler( m_PlayerSelect_Click );
			m_PlayerBoth.Show();
			m_GroupBoxPlayer.Controls.Add( m_PlayerBoth );


			m_EvaluatorLabel = new Label();
			m_EvaluatorLabel.Text = "Evaluator";
			m_EvaluatorLabel.Left = 10;
			m_EvaluatorLabel.Top = 190;
			m_EvaluatorLabel.AutoSize = true;
			m_EvaluatorLabel.Show();
			this.Controls.Add( m_EvaluatorLabel );

			m_PluginSelector = new ComboBox();
			m_PluginSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			m_PluginSelector.Left = 10;
			m_PluginSelector.Top = 210;
			m_PluginSelector.Width = 100;
			m_PluginSelector.Items.Add( "None" );
			m_PluginSelector.SelectedIndex = 0;
			m_PluginSelector.Show();
			this.Controls.Add( m_PluginSelector );


			m_LearnPanel = new Panel();
			m_LearnPanel.BorderStyle = BorderStyle.None;
			m_LearnPanel.Left = 5;
			m_LearnPanel.Top = 240;
			m_LearnPanel.Width = 100;
			m_LearnPanel.Height = 25;
			m_LearnPanel.Show();
			this.Controls.Add( m_LearnPanel );

			m_Learn = new CheckBox();
			m_Learn.Text = "Learn";
			m_Learn.Left = 5;
			m_Learn.Top = 0;
			m_Learn.Checked = false;
			m_Learn.CheckedChanged += new EventHandler( m_Learn_CheckedChanged );
			m_Learn.Show();
			m_LearnPanel.Controls.Add( m_Learn );
			if (m_Id == 1)
				m_Learn.Checked = true;


			m_PossibilitiesLabel = new Label();
			m_PossibilitiesLabel.Text = "Possible moves and score";
			m_PossibilitiesLabel.Left = 130;
			m_PossibilitiesLabel.Top = 10;
			m_PossibilitiesLabel.AutoSize = true;
			m_PossibilitiesLabel.Show();
			this.Controls.Add( m_PossibilitiesLabel );

			m_PossibilitiesList = new ListBox();
			m_PossibilitiesList.Left = 130;
			m_PossibilitiesList.Top = 30;
			m_PossibilitiesList.Width = 280;
			m_PossibilitiesList.Height = 250;
			m_PossibilitiesList.Show();
			this.Controls.Add( m_PossibilitiesList );

			m_NumberPossibilitiesLabel = new Label();
			m_NumberPossibilitiesLabel.Text = "Possibilities: 0";
			m_NumberPossibilitiesLabel.Left = 130;
			m_NumberPossibilitiesLabel.Top = 280;
			m_NumberPossibilitiesLabel.AutoSize = true;
			m_NumberPossibilitiesLabel.Show();
			this.Controls.Add( m_NumberPossibilitiesLabel );


			SyncAllSelectors();
			SyncAllSelectors();
		}


		public bool PlayersTurn( BgPlayer player )
		{
			if ( m_PlayerBoth.Checked && player != BgPlayer.None )
				return true;
			else if ( m_PlayerLight.Checked && player == BgPlayer.Light )
				return true;
			else if ( m_PlayerDark.Checked && player == BgPlayer.Dark )
				return true;

			return false;
		}


		public void UpdatePossibleMovesList( MoveRepresentationList knowledgeList )
		{
			m_PossibilitiesList.Items.Clear();

			for ( int i = 0; i < knowledgeList.Count(); i++ )
			{
				string stringToAdd = "";

				BgMove m = knowledgeList.GetMoveRepresentation( i ).GetMoves();
				while ( m != null )
				{
					stringToAdd += ( ( m.From == 25 )? "bar" : m.From.ToString() ) + "/" + ( ( m.To == 26 )? "off" : m.To.ToString() ) + "\t";
					m = m.NextMove;
				}
				stringToAdd += Math.Round( knowledgeList.GetMoveRepresentation( i ).GetScore(), 5 );
				m_PossibilitiesList.Items.Add( stringToAdd );
			}
			
			m_NumberPossibilitiesLabel.Text = "Possibilities: " + m_PossibilitiesList.Items.Count;
		}


		public void AddLoadedPlugin( string[] plugins )
		{
			m_PluginSelector.Items.AddRange( plugins );
		}


		public void SelectEvaluator( int id )
		{
			m_PluginSelector.SelectedIndex = id + 1;
		}


		public int SelectedEvaluator()
		{
			return m_PluginSelector.SelectedIndex - 1;
		}


		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged( e );
			SyncAllSelectors();
			SyncAllSelectors();
		}


		protected override void OnClosing( System.ComponentModel.CancelEventArgs e )
		{
			base.OnClosing( e );
			this.Hide();
			e.Cancel = true;
		}


		public void EnableGUI( bool enable )
		{
			if ( enable )
			{
				m_GroupBoxPlayer.Enabled = true;
				m_PluginSelector.Enabled = true;
				m_LearnPanel.Enabled = true;
			}
			else
			{
				m_GroupBoxPlayer.Enabled = false;
				m_PluginSelector.Enabled = false;
				m_LearnPanel.Enabled = false;
			}

		}


		private static void SyncAllSelectors()
		{
			for ( int i = 0; i < m_UpdaterList.Length; i++ )
				m_UpdaterList[i]();
		}


		private void SyncPlayerSelectors()
		{
			m_PlayerLight.Enabled = true;
			m_PlayerDark.Enabled = true;
			m_PlayerNone.Enabled = true;
			m_PlayerBoth.Enabled = true;

			if ( m_PlayerLightOwner != 0 && m_PlayerLightOwner != m_Id )
			{
				if ( m_PlayerLight.Checked )
					m_PlayerNone.Checked = true;
				m_PlayerLight.Enabled = false;
				m_PlayerBoth.Enabled = false;
			}

			if ( m_PlayerDarkOwner != 0 && m_PlayerDarkOwner != m_Id )
			{
				if ( m_PlayerDark.Checked )
					m_PlayerNone.Checked = true;
				m_PlayerDark.Enabled = false;
				m_PlayerBoth.Enabled = false;
			}

			if ( m_PlayerBothOwner != 0 && m_PlayerBothOwner != m_Id )
			{
				m_PlayerNone.Checked = true;
				m_PlayerLight.Enabled = false;
				m_PlayerDark.Enabled = false;
				m_PlayerBoth.Enabled = false;
			}

			if ( m_PlayerLight.Checked )
			{
				m_PlayerLightOwner = m_Id;
				m_PlayerDarkOwner = 0;
				m_PlayerBothOwner = 0;
			}

			if ( m_PlayerDark.Checked )
			{
				m_PlayerLightOwner = 0;
				m_PlayerDarkOwner = m_Id;
				m_PlayerBothOwner = 0;
			}

			if ( m_PlayerNone.Checked )
			{
				m_PlayerLightOwner = 0;
				m_PlayerDarkOwner = 0;
				m_PlayerBothOwner = 0;
			}

			if ( m_PlayerBoth.Checked )
			{
				m_PlayerLightOwner = 0;
				m_PlayerDarkOwner = 0;
				m_PlayerBothOwner = m_Id;
			}


			m_Learn.Enabled = true;

			if ( m_Learn.Checked )
			{
				m_PlayerLearnOwner = m_Id;
			}
			else if ( !m_Learn.Checked && m_PlayerLearnOwner == m_Id  )
			{
				m_PlayerLearnOwner = 0;
			}

			if ( m_PlayerLearnOwner != 0 && m_PlayerLearnOwner != m_Id )
			{
				m_Learn.Checked = false;
				m_Learn.Enabled = false;
			}
		}


		private void m_PlayerSelect_Click(object sender, EventArgs e)
		{
			SyncAllSelectors();
			SyncAllSelectors();
		}


		private void m_Learn_CheckedChanged(object sender, EventArgs e)
		{
			SyncAllSelectors();
			SyncAllSelectors();
		}
	}		   
}
