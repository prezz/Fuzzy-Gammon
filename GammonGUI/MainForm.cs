using System;
using System.Drawing;
using System.Windows.Forms;
using GammonGame;
using GammonAgent;


namespace GammonGUI
{
	public class MainForm : Form
	{
		public NotifyIcon					m_NotifyIcon;

		public MenuItem						m_GameMenu;
		public MenuItem						m_NewGameMenuItem;
		public MenuItem						m_AboutMenuItem;
		public MenuItem						m_ExitMenuItem;
		public MenuItem						m_EditMenu;
		public MenuItem						m_UndoMenuItem;
		public MenuItem						m_ManualDiceMenuItem;
		public MenuItem						m_BoardEditMenuItem;
		public MenuItem						m_SetGameSeedMenuItem;
		public MenuItem						m_UseCubeMenuItem;
		public MenuItem						m_NackgammonMenuItem;
		public MenuItem						m_MaxFivePiecesOnPointMenuItem;
		public MenuItem						m_AgentMenu;
		public MenuItem						m_ShowAgent1MenuItem;
		public MenuItem						m_ShowAgent2MenuItem;
		public MenuItem						m_AutoPlay;
		public MenuItem						m_AutoPlayFast;
		public MenuItem						m_AutoPlayThousand;
		public MenuItem						m_AutoPlayStop;

		public StatusBar					m_StatusBar;
		public StatusBarPanel				m_StatusPips;
		public StatusBarPanel				m_StatusMovesAvalible;
		public StatusBarPanel				m_StatusMovesTaken;

		public ListView						m_StatusListView;
		public ContextMenu					m_StatusContextMenu;
		public MenuItem						m_ClearWinStats;

		public SeedSelector					m_SeedSelectorForm;

		public PictureBox					m_BoardPictureBox;
		public Image						m_Board;
		public Image						m_CheckerDark;
		public Image						m_CheckerLight;
		public Image						m_CheckerCount;
		public Image[]						m_DiceDark;
		public Image[]						m_DiceLight;
		public Image[]						m_Cube;
		public Graphics						m_BoardPainter;
		public Font							m_CheckerCountFont;
		public SolidBrush					m_CheckerCountFontBrush;
		public Font							m_CubeFont;
		public SolidBrush					m_CubeFontBrush;
		public Font							m_SquareNumberFont;
		public SolidBrush					m_SquareNumberFontBrush;
		public Pen							m_SquareFramePenOne;
		public Pen							m_SquareFramePenTwo;

		public GammonInterface				m_GammonGame;
		public int							m_HoldingPieceFrom;
		public bool							m_GameOver;
		public bool							m_AutoPlayMode;
		public int[]						m_LightWins;
		public int[]						m_DarkWins;
		public int							m_LightDiceSum;
		public int							m_DarkDiceSum;
		public int							m_LightDoubles;
		public int							m_DarkDoubles;
		public bool							m_UsingCube;

		public Agent						m_Agent1;
		public Agent						m_Agent2;
		public BgMove						m_LastMoveA1;
		public BgMove						m_LastMoveA2;


		public MainForm()
		{
			this.Width = 582;
			this.Height = 574;
			this.MaximizeBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Icon = new Icon( GetType(), "App.ico" );
			this.Text = "Fuzzy-Gammon";
			
			m_NotifyIcon = new NotifyIcon();
			m_NotifyIcon.Text = this.Text;
			m_NotifyIcon.Icon = this.Icon;
			m_NotifyIcon.Visible = false;
			m_NotifyIcon.Click += new EventHandler( m_NotifyIcon_Click );
			this.Resize += new EventHandler( MainForm_Resize );

			m_SeedSelectorForm = new SeedSelector();

			SetupMenu();
			SetupGraphic();
			SetupPanel();

			m_GammonGame = new GammonInterface( m_NackgammonMenuItem.Checked, m_MaxFivePiecesOnPointMenuItem.Checked );
			m_Agent1 = new Agent( m_GammonGame );
			m_Agent2 = new Agent( m_GammonGame );

			m_AutoPlayMode = false;

			m_DarkWins = new int[4];
			for ( int i = 0; i < m_DarkWins.Length; i++ )
				m_DarkWins[i] = 0;

			m_LightWins = new int[4];
			for ( int i = 0; i < m_LightWins.Length; i++ )
				m_LightWins[i] = 0;

			NewGame();
		}


		public void NewGame()
		{
			if ( m_ManualDiceMenuItem.Checked )
				DiceEditor.ShowDiceEditor( this, true );
			
			if ( m_SeedSelectorForm.RandomSeed )
				m_GammonGame.NewGame( m_NackgammonMenuItem.Checked, m_MaxFivePiecesOnPointMenuItem.Checked );
			else
				m_GammonGame.NewGame( m_NackgammonMenuItem.Checked, m_MaxFivePiecesOnPointMenuItem.Checked, m_SeedSelectorForm.Seed );

			m_HoldingPieceFrom = -1;
			m_GameOver = false;
			m_LightDiceSum = 0;
			m_DarkDiceSum = 0;
			m_LightDoubles = 0;
			m_DarkDoubles = 0;
			m_UsingCube = m_UseCubeMenuItem.Checked;

			UpdateDiceStats();

			m_Agent1.ResetGeneretedBoards();
			m_Agent2.ResetGeneretedBoards();
			m_Agent1.ViewBoard();
			m_Agent2.ViewBoard();
			m_LastMoveA1 = m_Agent1.CommitMove();
			m_LastMoveA2 = m_Agent2.CommitMove();

			DrawBoard();
			UpdatePanel();
		}


		public void UpdateDiceStats()
		{
			if ( m_GammonGame.CurrentPlayer == BgPlayer.Light )
			{
				if ( m_GammonGame.GetDiceValue( 0 ) == m_GammonGame.GetDiceValue( 1 ) )
				{
					m_LightDiceSum += ( m_GammonGame.GetDiceValue( 0 ) * 4 );
					m_LightDoubles++;;
				}
				else
					m_LightDiceSum += ( m_GammonGame.GetDiceValue( 0 ) + m_GammonGame.GetDiceValue( 1 ) );
			}

			if ( m_GammonGame.CurrentPlayer == BgPlayer.Dark )
			{
				if ( m_GammonGame.GetDiceValue( 0 ) == m_GammonGame.GetDiceValue( 1 ) )
				{
					m_DarkDiceSum += ( m_GammonGame.GetDiceValue( 0 ) * 4 );
					m_DarkDoubles++;
				}
				else
					m_DarkDiceSum += ( m_GammonGame.GetDiceValue( 0 ) + m_GammonGame.GetDiceValue( 1 ) );
			}		
		}


		public int SquareClicked( int x, int y )
		{
			int result = -1;

			if ( x < 54 )
			{
				result = -1;
			}
			else if ( x > 54 + ( 36 * 13 ) )
			{
				result = 26;
			}
			else if ( y > 9 && y < ( 9 + ( 36 * 5 ) ) )
			{
				result = ( ( x - 54 ) / 36 ) + 13;
				if ( result == 19 )
					result = 25;
				else if ( result > 19 && result <= 25 )
					result--;
			}
			else if ( y < ( 450 - 9 ) && y > ( ( 450 - 9 ) - ( 36 * 5 ) ) )
			{
				result = 12 - ( ( x - 54 ) / 36 );
				if ( result == 6 )
					result = 0;
				else if ( result < 6 && result >= 0 )
					result++;
			}

			return ( m_GammonGame.CurrentPlayer == BgPlayer.Light && result != -1 && result != 26 )? 25 - result : result;
		}


		public bool DiceClicked( int x, int y )
		{
			bool result = false;

			if ( x >= 362 && x < ( 432-1 ) && y >= 207 && y < ( 237 - 1 ) )
				result = true;

			return result;
		}


		public bool CubeClicked( int x, int y )
		{
			bool result = false;

			int top = 209;
			if ( m_GammonGame.GetCubeOwner() == BgPlayer.Light )
				top = 10;
			else if ( m_GammonGame.GetCubeOwner() == BgPlayer.Dark )
				top = 408;

			if ( x > 12 && x < 44 && y > top && y < top + 32 )
				result = true;

			return result;
		}


		public void SetupGraphic()
		{
			m_BoardPictureBox = new PictureBox();
			m_BoardPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
			m_BoardPictureBox.Width = 576;
			m_BoardPictureBox.Height = 450;
			m_BoardPictureBox.Left = 0;
			m_BoardPictureBox.Top = 0;
			m_BoardPictureBox.Show();
			m_BoardPictureBox.MouseDown += new MouseEventHandler( m_BoardPictureBox_MouseDown );
			this.Controls.Add( m_BoardPictureBox );

			m_CheckerCountFont = new Font( "Arial", 16, FontStyle.Regular );
			m_CheckerCountFontBrush = new SolidBrush( Color.Black );
			m_CubeFont = new Font( "Arial", 13, FontStyle.Bold );;
			m_CubeFontBrush = new SolidBrush( Color.White );
			m_SquareNumberFont = new Font( "Arial", 11, FontStyle.Regular );
			m_SquareNumberFontBrush = new SolidBrush( Color.DarkBlue );
			m_SquareFramePenOne = new Pen( Color.Red );
			m_SquareFramePenTwo = new Pen( Color.Yellow );

			System.Reflection.Assembly graphicAssembly = System.Reflection.Assembly.LoadFrom( ".\\GammonGraphic.dll" );
			m_Board = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.board_texture.jpg" ) );
			m_CheckerDark = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.checker_dark.gif" ) );
			m_CheckerLight = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.checker_light.gif" ) );
			m_CheckerCount = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.checker_count.gif" ) );
			m_DiceDark = new Image[6];
			m_DiceLight = new Image[6];
			m_DiceDark[0] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_dark_1.gif" ) );
			m_DiceDark[1] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_dark_2.gif" ) );
			m_DiceDark[2] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_dark_3.gif" ) );
			m_DiceDark[3] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_dark_4.gif" ) );
			m_DiceDark[4] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_dark_5.gif" ) );
			m_DiceDark[5] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_dark_6.gif" ) );
			m_DiceLight[0] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_light_1.gif" ) );
			m_DiceLight[1] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_light_2.gif" ) );
			m_DiceLight[2] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_light_3.gif" ) );
			m_DiceLight[3] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_light_4.gif" ) );
			m_DiceLight[4] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_light_5.gif" ) );
			m_DiceLight[5] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.dice_light_6.gif" ) );
			m_Cube = new Image[8];
			m_Cube[0] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_1.gif" ) );
			m_Cube[1] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_2.gif" ) );
			m_Cube[2] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_4.gif" ) );
			m_Cube[3] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_8.gif" ) );
			m_Cube[4] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_16.gif" ) );
			m_Cube[5] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_32.gif" ) );
			m_Cube[6] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_64.gif" ) );
			m_Cube[7] = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.cube_nill.gif" ) );

			m_BoardPictureBox.Image = Image.FromStream( graphicAssembly.GetManifestResourceStream( "GammonGraphic.board_texture.jpg" ) );
			m_BoardPainter = Graphics.FromImage( m_BoardPictureBox.Image );
		}


		public void DrawBoard()
		{
			m_BoardPainter.DrawImage( m_Board, 0, 0, 576, 450 );

			for ( int i = 0; i < m_GammonGame.BoardSquareCount; i++ )
				DrawSquare( i, m_GammonGame.SquareOwner( i ), m_GammonGame.SquareCheckerCount( i ) );

			DrawSquareNumbers();
			DrawDices();
			DrawCube();
			DrawRemovedPieces();
			DrawSquareFrame( m_HoldingPieceFrom, true );
			DrawPossibleMoveToSquares();
			m_BoardPictureBox.Refresh();
		}


		public void DrawPossibleMoveToSquares()
		{
			for ( int i = m_HoldingPieceFrom; i > 0; i-- )
			{
				if ( m_GammonGame.ValidateMove( m_HoldingPieceFrom, i ) )
					DrawSquareFrame( i, false );
				if ( i <= 6 && m_GammonGame.ValidateMove( m_HoldingPieceFrom, 26 ) )
					DrawSquareFrame( 26, false );
			}
		}


		public void DrawRemovedPieces()
		{
			int leftCoord = GetSquaresLeftCoordinate( 26 );

			for ( int i = 1; i <= m_GammonGame.PiecesRemoved( BgPlayer.Dark ) && i <= 5; i++ )
				m_BoardPainter.DrawImage( m_CheckerDark, leftCoord, GetSquaresFreeTopCoordinate( 0, i ), 36, 36 );

			if ( m_GammonGame.PiecesRemoved( BgPlayer.Dark ) > 5 )
			{
				m_BoardPainter.DrawImage( m_CheckerCount, leftCoord + 6, GetSquaresFreeTopCoordinate( 0, 5 ) + 6, 24, 24);
				int leftAdjust = ( m_GammonGame.PiecesRemoved( BgPlayer.Dark ) < 10 )? 10 : 5;
				m_BoardPainter.DrawString( m_GammonGame.PiecesRemoved( BgPlayer.Dark ).ToString(), m_CheckerCountFont, m_CheckerCountFontBrush, leftCoord + leftAdjust, GetSquaresFreeTopCoordinate( 0, 5 ) + 9 );
			}

			for ( int i = 1; i <= m_GammonGame.PiecesRemoved( BgPlayer.Light ) && i <= 5; i++ )
				m_BoardPainter.DrawImage( m_CheckerLight, leftCoord, GetSquaresFreeTopCoordinate( 25, i ), 36, 36 );

			if ( m_GammonGame.PiecesRemoved( BgPlayer.Light ) > 5 )
			{
				m_BoardPainter.DrawImage( m_CheckerCount, leftCoord + 6, GetSquaresFreeTopCoordinate( 25, 5 ) + 6, 24, 24);
				int leftAdjust = ( m_GammonGame.PiecesRemoved( BgPlayer.Light ) < 10 )? 10 : 5;
				m_BoardPainter.DrawString( m_GammonGame.PiecesRemoved( BgPlayer.Light ).ToString(), m_CheckerCountFont, m_CheckerCountFontBrush, leftCoord + leftAdjust, GetSquaresFreeTopCoordinate( 25, 5 ) + 9 );
			}
		}


		public void DrawSquareFrame( int bgSquare, bool primaryColor )
		{
			Pen p = ( primaryColor )? m_SquareFramePenOne : m_SquareFramePenTwo;

			int square = ( m_GammonGame.CurrentPlayer == BgPlayer.Light && bgSquare != 26 && bgSquare != -1 )? 25 - bgSquare : bgSquare;

			if ( square == 26 && m_GammonGame.CurrentPlayer == BgPlayer.Dark )
				m_BoardPainter.DrawRectangle( p, 54 + ( 36 * 13 ) + 9, 261, 35, 36 * 5 );
			else if ( square == 26 && m_GammonGame.CurrentPlayer == BgPlayer.Light )
				m_BoardPainter.DrawRectangle( p, 54 + ( 36 * 13 ) + 9, 8, 35, 36 * 5 );
			else if ( square >= 13 && square <= 25 )
				m_BoardPainter.DrawRectangle( p, GetSquaresLeftCoordinate( square ), 8, 35, 36 * 5 );
			else if ( square <= 12 && square >= 0 )
				m_BoardPainter.DrawRectangle( p, GetSquaresLeftCoordinate( square ), GetSquaresFreeTopCoordinate( square, 5 ), 35, 36 * 5 );
		}


		public void DrawDices()
		{
			Image diceImg1 = null;
			Image diceImg2 = null;
			if ( m_GammonGame.CurrentPlayer == BgPlayer.Dark )
			{
				diceImg1 = m_DiceDark[m_GammonGame.GetDiceValue( 0 ) - 1];
				diceImg2 = m_DiceDark[m_GammonGame.GetDiceValue( 1 ) - 1];
			}
			else if ( m_GammonGame.CurrentPlayer == BgPlayer.Light )
			{
				diceImg1 = m_DiceLight[m_GammonGame.GetDiceValue( 0 ) - 1];
				diceImg2 = m_DiceLight[m_GammonGame.GetDiceValue( 1 ) - 1];
			}

			m_BoardPainter.DrawImage( diceImg1, 362, 207, 30, 30 );
			m_BoardPainter.DrawImage( diceImg2, 402, 207, 30, 30 );
		}


		public void DrawCube()
		{
			int top = 209;
			if ( m_GammonGame.GetCubeOwner() == BgPlayer.Light )
				top = 10;
			else if ( m_GammonGame.GetCubeOwner() == BgPlayer.Dark )
				top = 408;

			Image cubeImg = m_Cube[0];
			if ( m_GammonGame.GetCubeValue() == 2 )
				cubeImg = m_Cube[1];
			else if ( m_GammonGame.GetCubeValue() == 4 )
				cubeImg = m_Cube[2];
			else if ( m_GammonGame.GetCubeValue() == 8 )
				cubeImg = m_Cube[3];
			else if ( m_GammonGame.GetCubeValue() == 16 )
				cubeImg = m_Cube[4];
			else if ( m_GammonGame.GetCubeValue() == 32 )
				cubeImg = m_Cube[5];
			else if ( m_GammonGame.GetCubeValue() == 64 )
				cubeImg = m_Cube[6];
			else if ( m_GammonGame.GetCubeValue() > 64 )
				cubeImg = m_Cube[7];

			m_BoardPainter.DrawImage( cubeImg, 11, top, 32, 32 );

			if ( m_GammonGame.GetCubeValue() > 64 )
			{
				int l = ( m_GammonGame.GetCubeValue() < 1000 )? 14 : 11;
				string s = m_GammonGame.GetCubeValue().ToString();
				m_BoardPainter.DrawString( s, m_CubeFont, m_CubeFontBrush, l, top+8 );
			}
		}


		public void DrawSquare( int bgSquare, BgPlayer owner, int numPieces )
		{
			if ( bgSquare < 0 || bgSquare > 25 || owner == BgPlayer.None || numPieces <= 0 )
				return;

			int square = ( m_GammonGame.CurrentPlayer == BgPlayer.Light )? 25 - bgSquare : bgSquare;

			Image img = null;
			if ( owner == BgPlayer.Dark )
				img = m_CheckerDark;
			else if ( owner == BgPlayer.Light )
				img = m_CheckerLight;

			for ( int i = 1; i <= numPieces && i <= 5; i++ )
				m_BoardPainter.DrawImage( img, GetSquaresLeftCoordinate( square ), GetSquaresFreeTopCoordinate( square, i ), 36, 36 );

			if ( numPieces > 5 )
			{
				m_BoardPainter.DrawImage( m_CheckerCount, GetSquaresLeftCoordinate( square ) + 6, GetSquaresFreeTopCoordinate( square, 5 ) + 6, 24, 24);
				int leftAdjust = ( numPieces < 10 )? 10 : 5;
				m_BoardPainter.DrawString( numPieces.ToString(), m_CheckerCountFont, m_CheckerCountFontBrush, GetSquaresLeftCoordinate( square ) + leftAdjust, GetSquaresFreeTopCoordinate( square, 5 ) + 9 );
			}
		}


		public int GetSquaresLeftCoordinate( int square )
		{
			int result = 0;

			if ( square >= 13 && square <= 24 )
			{
				result = 54 + ( ( square - 13 ) * 36 );
				if ( square >= 19 )
					result += 36;
			}
			else if ( square <= 12 && square >= 1 )
			{
				result = ( 576 - 54 - 36 ) - ( square * 36 );
				if ( square <= 6 )
					result += 36;
			}
			else if ( square == 0 || square == 25 )
			{
				result = 54 + ( 6 * 36 );
			}
			else if ( square == 26 )
			{
				result = 54 + ( 36 * 13 ) + 9;
			}
			return result;
		}


		public int GetSquaresFreeTopCoordinate( int square, int numPiece )
		{
			int result = 0;

			if ( square >= 13 && square <= 25 && numPiece > 0 )
			{
				result = 9 + ( ( numPiece - 1 ) * 36 );
			}
			else if ( square <= 12 && square >= 0 && numPiece > 0 )
			{
				result = ( 450 - 9 ) - ( 36 * numPiece );
			}
			return result;
		}


		public void DrawSquareNumbers()
		{
			for ( int i = 1; i < 25; i++ )
			{
				if ( m_GammonGame.CurrentPlayer == BgPlayer.Light )
				{
					int leftAdjust = ( (25 - i) < 10 )? 13 : 11;
					m_BoardPainter.DrawString( (25 - i).ToString(), m_SquareNumberFont, m_SquareNumberFontBrush, GetSquaresLeftCoordinate( i ) + leftAdjust, GetSquaresFreeTopCoordinate( i, 6 ) + 7 );
				}
				if ( m_GammonGame.CurrentPlayer == BgPlayer.Dark )
				{
					int leftAdjust = ( i < 10 )? 13 : 11;
					m_BoardPainter.DrawString( i.ToString(), m_SquareNumberFont, m_SquareNumberFontBrush, GetSquaresLeftCoordinate( i ) + leftAdjust, GetSquaresFreeTopCoordinate( i, 6 ) + 7 );
				}

			}
		}


		public void m_BoardPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			int squareClick = SquareClicked( e.X, e.Y );

			if ( m_BoardEditMenuItem.Checked )
			{
				if ( squareClick == 26 )
					m_GammonGame.EditBoard();

				if ( DiceClicked( e.X, e.Y ) && e.Button == MouseButtons.Left )
					DiceEditor.ShowDiceEditor( this, false );
				else if ( DiceClicked( e.X, e.Y ) && e.Button == MouseButtons.Right )
					m_GammonGame.SwitchPlayer();

				//TODO: enable cube editing

				if ( e.Button == MouseButtons.Left )
					m_GammonGame.PutPiece( squareClick, BgPlayer.Dark );
				else if ( e.Button == MouseButtons.Right )
					m_GammonGame.PutPiece( squareClick, BgPlayer.Light );
			}
			else if ( !m_GameOver )
			{
				if ( DiceClicked( e.X, e.Y ) && m_GammonGame.IsTurnOver() )
				{
					if ( m_UsingCube )
					{
						if ( m_Agent1.Double() || m_Agent2.Double() )
						{
							if ( !( m_Agent1.IsAgentPlaying( m_GammonGame.CurrentPlayer ) || m_Agent2.IsAgentPlaying( m_GammonGame.CurrentPlayer ) ) )
							{
								DialogResult r = MessageBox.Show( "Opponent doubles, do you accept?", "Opponent doubles", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
								if ( r == DialogResult.Yes )
									m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
								else if ( r == DialogResult.No )
									m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
							}
							else
							{
								if ( m_Agent1.IsAgentPlaying( m_GammonGame.CurrentPlayer ) )
								{
									if ( m_Agent1.AcceptDouble() )
										m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
									else
										m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
								}

								if ( m_Agent2.IsAgentPlaying( m_GammonGame.CurrentPlayer ) )
								{
									if ( m_Agent2.AcceptDouble() )
										m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
									else
										m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
								}							
							}
						}
					}

					if ( m_GammonGame.EndTurn() )
					{
						if ( m_ManualDiceMenuItem.Checked )
							DiceEditor.ShowDiceEditor( this, false );
						
						m_Agent1.ViewBoard();
						m_Agent2.ViewBoard();
						m_LastMoveA1 = m_Agent1.CommitMove();
						m_LastMoveA2 = m_Agent2.CommitMove();

						if ( !m_GameOver )
							UpdateDiceStats();
					}
				}
			
				if ( CubeClicked( e.X, e.Y ) && m_GammonGame.IsTurnOver() )
				{
					if ( m_UsingCube )
					{
						if ( ( m_GammonGame.GetCubeOwner() == m_GammonGame.CurrentOpponentPlayer || m_GammonGame.GetCubeOwner() == BgPlayer.None ) && !( m_Agent1.IsAgentPlaying( m_GammonGame.CurrentOpponentPlayer ) || m_Agent2.IsAgentPlaying( m_GammonGame.CurrentOpponentPlayer ) ) )
						{
							if ( m_Agent1.IsAgentPlaying( m_GammonGame.CurrentPlayer ) )
							{
								if ( m_Agent1.AcceptDouble() )
									m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
								else
									m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
							}
							else if ( m_Agent2.IsAgentPlaying( m_GammonGame.CurrentPlayer ) )
							{
								if ( m_Agent2.AcceptDouble() )
									m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
								else
									m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
							}
							else if ( !( m_Agent1.IsAgentPlaying( m_GammonGame.CurrentPlayer ) || m_Agent2.IsAgentPlaying( m_GammonGame.CurrentPlayer ) ) )
							{
								DialogResult r = MessageBox.Show( "Opponent doubles, do you accept?", "Opponent doubles", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
								if ( r == DialogResult.Yes )
									m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
								else if ( r == DialogResult.No )
									m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
							}
						}
					}
				}
				
				if ( m_GammonGame.Winner() != BgPlayer.None && !m_GameOver )
				{
					m_Agent1.Learn();
					m_Agent2.Learn();

					string winner = "";
					if ( m_GammonGame.Winner() == BgPlayer.Dark )
					{
						winner = "Dark";
						m_DarkWins[m_GammonGame.VictoryType() - 1]++;
						m_DarkWins[3] += m_GammonGame.VictoryType() * m_GammonGame.GetCubeValue();
					}
					if ( m_GammonGame.Winner() == BgPlayer.Light )
					{
						winner = "Light";
						m_LightWins[m_GammonGame.VictoryType() - 1]++;
						m_LightWins[3] += m_GammonGame.VictoryType() * m_GammonGame.GetCubeValue();
					}

					string winType = "";
					if ( m_GammonGame.VictoryType() == 3 )
						winType = "backgammon";
					if ( m_GammonGame.VictoryType() == 2 )
						winType = "gammon";
					if ( m_GammonGame.VictoryType() == 1 )
						winType = "normal";
						
					MessageBox.Show( this, winner + " wins a " + winType + " victory\n\nDark sum: " + m_DarkDiceSum + ", Doubles: " + m_DarkDoubles + "\nLight sum: " + m_LightDiceSum + ", Doubles: " + m_LightDoubles, "Game over" );
					m_GameOver = true;
				}
				
				if ( m_HoldingPieceFrom == -1 && squareClick >= 0 && squareClick <= 25 && m_GammonGame.SquareOwner( squareClick ) == m_GammonGame.CurrentPlayer )
				{
					m_HoldingPieceFrom = squareClick;
				}
				else if ( m_HoldingPieceFrom >= 0 && squareClick >= 0 && squareClick <= 26 )
				{
					m_GammonGame.MakeMove( m_HoldingPieceFrom, squareClick );
					m_HoldingPieceFrom = -1;
				}
			}

			DrawBoard();
			UpdatePanel();
		}
		

		public void SetupMenu()
		{
			this.Menu = new MainMenu();


			m_GameMenu = new MenuItem( "Game" );
			this.Menu.MenuItems.Add( m_GameMenu );

			m_NewGameMenuItem = new MenuItem( "New Game" );
			m_NewGameMenuItem.Click += new EventHandler( m_NewGameMenuItem_Click );
			m_GameMenu.MenuItems.Add( m_NewGameMenuItem );

			m_AboutMenuItem = new MenuItem( "About" );
			m_AboutMenuItem.Click += new EventHandler(m_AboutMenuItem_Click);
			m_GameMenu.MenuItems.Add( m_AboutMenuItem );

			m_ExitMenuItem = new MenuItem( "Exit" );
			m_ExitMenuItem.Click += new EventHandler( m_ExitMenuItem_Click );
			m_GameMenu.MenuItems.Add( m_ExitMenuItem );


			m_EditMenu = new MenuItem( "Edit" );
			this.Menu.MenuItems.Add( m_EditMenu );

			m_UndoMenuItem = new MenuItem( "Undo" );
			m_UndoMenuItem.Click += new EventHandler( m_UndoMenuItem_Click );
			m_EditMenu.MenuItems.Add( m_UndoMenuItem );

			m_ManualDiceMenuItem = new MenuItem( "Manual Dices" );
			m_ManualDiceMenuItem.Click += new EventHandler( m_ManualDice_Click );
			m_EditMenu.MenuItems.Add( m_ManualDiceMenuItem );

			m_BoardEditMenuItem = new MenuItem( "Edit Board" );
			m_BoardEditMenuItem.Click += new EventHandler( m_BoardEditMenuItem_Click );
			m_EditMenu.MenuItems.Add( m_BoardEditMenuItem );

			m_SetGameSeedMenuItem = new MenuItem( "Set Game Seed" );
			m_SetGameSeedMenuItem.Click += new EventHandler(m_SetGameSeedMenuItem_Click);
			m_EditMenu.MenuItems.Add( m_SetGameSeedMenuItem );

			m_EditMenu.MenuItems.Add( new MenuItem( "-" ) );

			m_UseCubeMenuItem = new MenuItem( "Use Cube" );
			m_UseCubeMenuItem.Click += new EventHandler(m_UseCubeMenuItem_Click);
			m_UseCubeMenuItem.Checked = true;
			m_EditMenu.MenuItems.Add( m_UseCubeMenuItem );

			m_NackgammonMenuItem = new MenuItem( "Nackgammon layout" );
			m_NackgammonMenuItem.Click += new EventHandler( m_NackgammonMenuItem_Click );
			m_EditMenu.MenuItems.Add( m_NackgammonMenuItem );

			m_MaxFivePiecesOnPointMenuItem = new MenuItem( "Max five checkers on location" );
			m_MaxFivePiecesOnPointMenuItem.Click += new EventHandler( m_MaxFivePiecesOnPointMenuItem_Click );
			m_EditMenu.MenuItems.Add( m_MaxFivePiecesOnPointMenuItem );

			m_AgentMenu = new MenuItem( "Agents" );
			this.Menu.MenuItems.Add( m_AgentMenu );

			m_ShowAgent1MenuItem = new MenuItem( "Show Agent 1" );
			m_ShowAgent1MenuItem.Click += new EventHandler( m_ShowAgent1MenuItem_Click );
			m_AgentMenu.MenuItems.Add( m_ShowAgent1MenuItem );

			m_ShowAgent2MenuItem = new MenuItem( "Show Agent 2" );
			m_ShowAgent2MenuItem.Click += new EventHandler( m_ShowAgent2MenuItem_Click );
			m_AgentMenu.MenuItems.Add( m_ShowAgent2MenuItem );

			m_AutoPlayThousand = new MenuItem( "Play 500 (Fast)" );
			m_AutoPlayThousand.Click += new EventHandler( m_AutoPlayFiveHundred_Click );
			m_AgentMenu.MenuItems.Add( m_AutoPlayThousand );
			
			m_AutoPlay = new MenuItem( "Auto Play (Animated)" );
			m_AutoPlay.Click += new EventHandler( m_AutoPlay_Click );
			m_AgentMenu.MenuItems.Add( m_AutoPlay );

			m_AutoPlayFast = new MenuItem( "Auto Play (Fast)" );
			m_AutoPlayFast.Click += new EventHandler( m_AutoPlayFast_Click );
			m_AgentMenu.MenuItems.Add( m_AutoPlayFast );


			m_AutoPlayStop = new MenuItem( "Stop Auto Play" );
			m_AutoPlayStop.Enabled = false;
			m_AutoPlayStop.Click += new EventHandler( m_AutoPlayStop_Click );
			m_AgentMenu.MenuItems.Add( m_AutoPlayStop );
		
		}


		public void m_NewGameMenuItem_Click( object sender, EventArgs e )
		{
			NewGame();
		}


		private void m_AboutMenuItem_Click( object sender, EventArgs e )
		{
			new AboutGUI().ShowDialog( this );
		}


		public void m_ExitMenuItem_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}


		public void m_UndoMenuItem_Click( object sender, EventArgs e )
		{
			if ( m_Agent1.IsAgentMoveing() || m_Agent2.IsAgentMoveing() )
				return;

			m_GammonGame.Undo();
			m_HoldingPieceFrom = -1;
			DrawBoard();
			UpdatePanel();
		}


		public void m_ManualDice_Click( object sender, EventArgs e )
		{
			m_ManualDiceMenuItem.Checked = !m_ManualDiceMenuItem.Checked;
		}


		private void m_BoardEditMenuItem_Click( object sender, EventArgs e )
		{
			m_BoardEditMenuItem.Checked = !m_BoardEditMenuItem.Checked;

			m_GameOver = false;

			if ( m_BoardEditMenuItem.Checked )
			{
				m_NewGameMenuItem.Enabled = false;
				m_GammonGame.EditBoard();
				DrawBoard();
				UpdatePanel();
			}
			else
			{
				m_NewGameMenuItem.Enabled = true;
				m_GammonGame.StopEditBoard();
				m_Agent1.ViewBoard();
				m_Agent2.ViewBoard();
				m_LastMoveA1 = m_Agent1.CommitMove();
				m_LastMoveA2 = m_Agent2.CommitMove();
				DrawBoard();
				UpdatePanel();
			}
		}


		private void m_SetGameSeedMenuItem_Click( object sender, EventArgs e )
		{
			m_SeedSelectorForm.ShowDialog( this );
		}


		private void m_UseCubeMenuItem_Click( object sender, EventArgs e )
		{
			m_UseCubeMenuItem.Checked = !m_UseCubeMenuItem.Checked;
		}


		private void m_NackgammonMenuItem_Click( object sender, EventArgs e )
		{
			m_NackgammonMenuItem.Checked = !m_NackgammonMenuItem.Checked;
		}


		private void m_MaxFivePiecesOnPointMenuItem_Click( object sender, EventArgs e )
		{
			m_MaxFivePiecesOnPointMenuItem.Checked = !m_MaxFivePiecesOnPointMenuItem.Checked;
		}


		private void m_ShowAgent1MenuItem_Click( object sender, EventArgs e )
		{
			m_Agent1.ShowAgentGUI();
		}


		private void m_ShowAgent2MenuItem_Click( object sender, EventArgs e )
		{
			m_Agent2.ShowAgentGUI();
		}


		private void m_AutoPlay_Click( object sender, EventArgs e )
		{
			AutoPlay( true, false );
		}


		private void m_AutoPlayFast_Click( object sender, EventArgs e )
		{
			AutoPlay( false, false );
		}


		private void m_AutoPlayFiveHundred_Click( object sender, EventArgs e )
		{
			AutoPlay( false, true );
		}


		private void m_AutoPlayStop_Click( object sender, EventArgs e )
		{
			m_AutoPlayMode = false;
		}


		protected override void OnClosing( System.ComponentModel.CancelEventArgs e )
		{
			if ( m_AutoPlayMode )
				m_AutoPlayMode = false;

			m_Agent1.ShutDown();
			m_Agent2.ShutDown();
			base.OnClosing( e );
		}


		private void MainForm_Resize( object sender, EventArgs e )
		{
			if ( m_AutoPlayMode )
			{
				if ( this.WindowState == FormWindowState.Minimized )
				{
					m_NotifyIcon.Visible = true;
					this.ShowInTaskbar = false;
					System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
				}
			
				if ( this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized )
				{
					System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
					this.ShowInTaskbar = true;
					m_NotifyIcon.Visible = false;
				}
			}
		}


		private void m_NotifyIcon_Click( object sender, EventArgs e )
		{
			this.WindowState = FormWindowState.Normal;
			this.Activate();
		}
		

		private void AutoPlay( bool animate, bool fiveHundredLimit )
		{
			bool lightDefined = false;
			bool darkDefined = false;

			if ( m_Agent1.IsAgentPlaying( BgPlayer.Light ) || m_Agent2.IsAgentPlaying( BgPlayer.Light ) )
				lightDefined = true;

			if ( m_Agent1.IsAgentPlaying( BgPlayer.Dark ) || m_Agent2.IsAgentPlaying( BgPlayer.Dark ) )
				darkDefined = true;

			if ( !lightDefined || !darkDefined )
			{
				MessageBox.Show( "One or more of the playing sides is not assigned to an agent", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}

			if ( !m_SeedSelectorForm.RandomSeed )
			{
				DialogResult r = MessageBox.Show( "Are you sure you want to autoplay with a defined dice seed?", "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
				if ( r == DialogResult.No )
					return;
			}

            m_Agent1.EnableGUI( false );
			m_Agent2.EnableGUI( false );
			m_AutoPlayStop.Enabled = true;
			m_AutoPlay.Enabled = false;
			m_AutoPlayFast.Enabled = false;
			m_AutoPlayThousand.Enabled = false;
			m_EditMenu.Enabled = false;
			m_NewGameMenuItem.Enabled = false;
			m_BoardPictureBox.Enabled = false;

//			AutoPlayCancelForm cancelForm = new AutoPlayCancelForm( this );
//			cancelForm.Show();

			m_HoldingPieceFrom = -1;
			m_GameOver = false;
			m_UsingCube = m_UseCubeMenuItem.Checked;

			if ( !animate )
			{
				m_Agent1.OutputMovesAndScore = false;
				m_Agent2.OutputMovesAndScore = false;
			}

			if ( m_SeedSelectorForm.RandomSeed )
				m_GammonGame.NewGame( m_NackgammonMenuItem.Checked, m_MaxFivePiecesOnPointMenuItem.Checked );
			else
				m_GammonGame.NewGame( m_NackgammonMenuItem.Checked, m_MaxFivePiecesOnPointMenuItem.Checked, m_SeedSelectorForm.Seed );

			m_LightDiceSum = 0;
			m_DarkDiceSum = 0;
			m_LightDoubles = 0;
			m_DarkDoubles = 0;
			UpdateDiceStats();

			DrawBoard();
			UpdatePanel();	

			int count = 500;
			m_AutoPlayMode = true;
			while ( m_AutoPlayMode && count > 0 )
			{
				m_Agent1.ViewBoard();
				m_Agent2.ViewBoard();
				m_LastMoveA1 = m_Agent1.CommitMove();
				m_LastMoveA2 = m_Agent2.CommitMove();

				if ( m_UsingCube )
				{
					if ( m_Agent1.Double() || m_Agent2.Double() )
					{
						if ( m_Agent1.IsAgentPlaying( m_GammonGame.CurrentPlayer ) )
						{
							if ( m_Agent1.AcceptDouble() )
								m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
							else
								m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
						}

						if ( m_Agent2.IsAgentPlaying( m_GammonGame.CurrentPlayer ) )
						{
							if ( m_Agent2.AcceptDouble() )
								m_GammonGame.DoubleCube( m_GammonGame.CurrentOpponentPlayer );
							else
								m_GammonGame.SetWinner( m_GammonGame.CurrentOpponentPlayer );
						}							
					}
				}

				if ( animate )
				{
					DrawBoard();
					UpdatePanel();
					System.Threading.Thread.Sleep( 500 );
				}

				m_GammonGame.EndTurn();
				UpdateDiceStats();

				if ( m_GammonGame.Winner() != BgPlayer.None )
				{
					if ( fiveHundredLimit )
						count--;

					m_Agent1.Learn();
					m_Agent2.Learn();

					if ( m_GammonGame.Winner() == BgPlayer.Dark )
					{
						m_DarkWins[m_GammonGame.VictoryType() - 1]++;
						m_DarkWins[3] += m_GammonGame.VictoryType() * m_GammonGame.GetCubeValue();
					}
					
					if ( m_GammonGame.Winner() == BgPlayer.Light )
					{
						m_LightWins[m_GammonGame.VictoryType() - 1]++;
						m_LightWins[3] += m_GammonGame.VictoryType() * m_GammonGame.GetCubeValue();
					}

					if ( !animate )
						UpdatePanel();

					if ( m_SeedSelectorForm.RandomSeed )
						m_GammonGame.NewGame( m_NackgammonMenuItem.Checked, m_MaxFivePiecesOnPointMenuItem.Checked );
					else
						m_GammonGame.NewGame( m_NackgammonMenuItem.Checked, m_MaxFivePiecesOnPointMenuItem.Checked, m_SeedSelectorForm.Seed );

					m_LightDiceSum = 0;
					m_DarkDiceSum = 0;
					m_LightDoubles = 0;
					m_DarkDoubles = 0;
					UpdateDiceStats();

					if ( animate )
					{
						DrawBoard();
						UpdatePanel();
						System.Threading.Thread.Sleep( 500 );
					}
				}
				Application.DoEvents();
			}

			m_Agent1.OutputMovesAndScore = true;
			m_Agent2.OutputMovesAndScore = true;

			m_Agent1.ViewBoard();
			m_Agent2.ViewBoard();
			m_LastMoveA1 = m_Agent1.CommitMove();
			m_LastMoveA2 = m_Agent2.CommitMove();

			DrawBoard();
			UpdatePanel();

			m_BoardPictureBox.Enabled = true;
			m_EditMenu.Enabled = true;
			m_NewGameMenuItem.Enabled = true;
			m_AutoPlayStop.Enabled = false;
			m_AutoPlay.Enabled = true;
			m_AutoPlayFast.Enabled = true;
			m_AutoPlayThousand.Enabled = true;
			m_Agent1.EnableGUI( true );
			m_Agent2.EnableGUI( true );
		}


		private void m_ClearWinStats_Click(object sender, EventArgs e)
		{
			for ( int i = 0; i < m_DarkWins.Length; i++ )
				m_DarkWins[i] = 0;
			for ( int i = 0; i < m_LightWins.Length; i++ )
				m_LightWins[i] = 0;
			UpdatePanel();
		}


		public void SetupPanel()
		{
			m_StatusListView = new ListView();
			m_StatusListView.Top = 450;
			m_StatusListView.Left = 0;
			m_StatusListView.Width = 576;
			m_StatusListView.Height = 50;
			m_StatusListView.View = View.Details;
//			m_StatusListView.GridLines = true;
			m_StatusListView.AllowColumnReorder = false;
			m_StatusListView.Scrollable = false;
			m_StatusListView.Columns.Add( "Player", 60, HorizontalAlignment.Left );
			m_StatusListView.Columns.Add( "Won", 64, HorizontalAlignment.Center );
			m_StatusListView.Columns.Add( "Normal", 64, HorizontalAlignment.Center );
			m_StatusListView.Columns.Add( "Gammon", 64, HorizontalAlignment.Center );
			m_StatusListView.Columns.Add( "Bkgammon", 64, HorizontalAlignment.Center );
			m_StatusListView.Columns.Add( "Cube avg.", 64, HorizontalAlignment.Center );
			m_StatusListView.Columns.Add( "Score", 64, HorizontalAlignment.Center );
			m_StatusListView.Columns.Add( "Lead", 64, HorizontalAlignment.Center );
			m_StatusListView.Columns.Add( "Win %", 64, HorizontalAlignment.Center );
			string[] d = {"Dark:", "0", "0", "0", "0", "0", "0", "0", "0" };
			m_StatusListView.Items.Add( new ListViewItem( d ) );
			string[] l = {"Light:", "0", "0", "0", "0", "0", "0", "0", "0" };
			m_StatusListView.Items.Add( new ListViewItem( l ) );
			this.Controls.Add( m_StatusListView );			

			m_StatusContextMenu = new ContextMenu();
			m_ClearWinStats = new MenuItem( "Reset winning stats" );
			m_ClearWinStats.Click += new EventHandler( m_ClearWinStats_Click );
			m_StatusContextMenu.MenuItems.Add( m_ClearWinStats );
			m_StatusListView.ContextMenu = m_StatusContextMenu;

			m_StatusBar = new StatusBar();
			m_StatusBar.ShowPanels = true;
			m_StatusBar.SizingGrip = false;
			m_StatusBar.ContextMenu = m_StatusContextMenu;
			this.Controls.Add( m_StatusBar );

			m_StatusPips = new StatusBarPanel();
			m_StatusPips.AutoSize = StatusBarPanelAutoSize.Contents;
			m_StatusBar.Panels.Add( m_StatusPips );

			m_StatusMovesAvalible = new StatusBarPanel();
			m_StatusMovesAvalible.AutoSize = StatusBarPanelAutoSize.Contents;
			m_StatusBar.Panels.Add( m_StatusMovesAvalible );

			m_StatusMovesTaken = new StatusBarPanel();
			m_StatusMovesTaken.AutoSize = StatusBarPanelAutoSize.Contents;
			m_StatusBar.Panels.Add( m_StatusMovesTaken );
		}


		public void UpdatePanel()
		{
			m_StatusListView.Items[0].SubItems[1].Text = (m_DarkWins[0] + m_DarkWins[1] + m_DarkWins[2]).ToString();
			m_StatusListView.Items[1].SubItems[1].Text = (m_LightWins[0] + m_LightWins[1] + m_LightWins[2]).ToString();

			m_StatusListView.Items[0].SubItems[2].Text = m_DarkWins[0].ToString();
			m_StatusListView.Items[1].SubItems[2].Text = m_LightWins[0].ToString();

			m_StatusListView.Items[0].SubItems[3].Text = m_DarkWins[1].ToString();
			m_StatusListView.Items[1].SubItems[3].Text = m_LightWins[1].ToString();

			m_StatusListView.Items[0].SubItems[4].Text = m_DarkWins[2].ToString();
			m_StatusListView.Items[1].SubItems[4].Text = m_LightWins[2].ToString();

			int pureDarkScore = m_DarkWins[0] + (2*m_DarkWins[1]) + (3*m_DarkWins[2]);
			int pureLightScore = m_LightWins[0] + (2*m_LightWins[1]) + (3*m_LightWins[2]);
			double darkCubeFactor = ( pureDarkScore != 0 )? ((double)m_DarkWins[3]) / pureDarkScore : 0.0;
			double lightCubeFactor = ( pureLightScore != 0 )? ((double)m_LightWins[3]) / pureLightScore : 0.0;
			m_StatusListView.Items[0].SubItems[5].Text = Math.Round( darkCubeFactor, 2 ).ToString();
			m_StatusListView.Items[1].SubItems[5].Text = Math.Round( lightCubeFactor, 2 ).ToString();

			m_StatusListView.Items[0].SubItems[6].Text = m_DarkWins[3].ToString();
			m_StatusListView.Items[1].SubItems[6].Text = m_LightWins[3].ToString();

			int darkLead = m_DarkWins[3] - m_LightWins[3];
			m_StatusListView.Items[0].SubItems[7].Text = ( darkLead > 0 )? "+" + darkLead.ToString() : darkLead.ToString();
			int lightLead = m_LightWins[3] - m_DarkWins[3];
			m_StatusListView.Items[1].SubItems[7].Text = ( lightLead > 0 )? "+" + lightLead.ToString() : lightLead.ToString();

			double darkWins = 0.0f;
			double lightWins = 0.0f;
			int totalScore = m_DarkWins[3] + m_LightWins[3];
			if ( totalScore != 0 )
			{
				darkWins = Math.Round( ((double)m_DarkWins[3]) / totalScore, 3 ) * 100;
				lightWins = Math.Round( ((double)m_LightWins[3]) / totalScore, 3 ) * 100;
			}
			m_StatusListView.Items[0].SubItems[8].Text = darkWins.ToString();
			m_StatusListView.Items[1].SubItems[8].Text = lightWins.ToString();


			string pips = "Pips: ";
			pips += m_GammonGame.GetPips( BgPlayer.Dark ) + " / " + m_GammonGame.GetPips( BgPlayer.Light );
			m_StatusPips.Text = pips;

			string remaining = "Moves: ";
			for ( int i = 0; i < 4; i++ )
				remaining += m_GammonGame.GetMove( i ) + "; ";
			m_StatusMovesAvalible.Text = remaining;

			string agentMove = "";
			BgMove m = ( m_LastMoveA1 != null)? m_LastMoveA1 : m_LastMoveA2;
			while ( m != null )
			{
				agentMove += ( m.From == 25 )? "bar" : m.From.ToString();
				agentMove += "/";
				agentMove += ( m.To == 26 )? "off" : m.To.ToString();
				agentMove += " ";
				
				m = m.NextMove;
			}
			m_StatusMovesTaken.Text = "Moved: " + agentMove;
		}


		[STAThread]
		static void Main() 
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
			Application.Run( new MainForm() );
		}
	}
}
