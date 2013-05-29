using System;
using System.IO;
using System.Collections;
using GammonGame;


namespace GammonAgent
{
	public class Agent
	{
		private static int								m_InstanceCounter = 0;

		private AgentGUI								m_GUI;
		private GammonInterface							m_BgGame;
		private	MoveRepresentationList					m_MoveRepresentationList;
		private static MoveRepresentationGenerator		m_MoveRepresentationGenerator = null;
		private static AgentDecisionModule[]			m_Plugins = null;
		private Random									m_Rand;

		public bool										OutputMovesAndScore;
		

		public Agent( GammonInterface game )
		{
			m_GUI = new AgentGUI( "Agent " + (++m_InstanceCounter) );
			m_Rand = new Random();
			m_BgGame = game;
			m_MoveRepresentationList = new MoveRepresentationList();
			
			if ( m_MoveRepresentationGenerator == null )
				m_MoveRepresentationGenerator = new MoveRepresentationGenerator( game );

			LoadPlugins();
			OutputMovesAndScore = true;
		}


		public bool IsAgentMoveing()
		{
			return m_GUI.PlayersTurn( m_BgGame.CurrentPlayer );
		}


		public bool IsAgentPlaying( BgPlayer player )
		{
			return m_GUI.PlayersTurn( player );
		}


		public void ResetGeneretedBoards()
		{
			m_MoveRepresentationGenerator.Reset();
		}


		private void GradePossibleMoves( MoveRepresentationList knowledgeList, BoardRepresentation initialBoard )
		{
			int evaluator = m_GUI.SelectedEvaluator();
			if ( evaluator >= 0 )
				m_Plugins[evaluator].GradeBoards( knowledgeList, initialBoard );
		}


		public void ViewBoard()
		{
			BoardRepresentation initialBoard = new BoardRepresentation( m_BgGame );

			m_MoveRepresentationGenerator.GeneratePossibleRepresentations( m_MoveRepresentationList );
			GradePossibleMoves( m_MoveRepresentationList, initialBoard );
			m_MoveRepresentationList.SortByScore();
			if ( OutputMovesAndScore )
				m_GUI.UpdatePossibleMovesList( m_MoveRepresentationList );
		}


		public BgMove CommitMove()
		{
			if ( IsAgentMoveing() && m_MoveRepresentationList.Count() > 0 )
			{
				int numBestMoves = 1;
				double bestMoveScore = m_MoveRepresentationList.GetMoveRepresentation( 0 ).GetScore();
				for ( int i = 1; i < m_MoveRepresentationList.Count(); i++ )
				{
					double currentCompareMoveScore = m_MoveRepresentationList.GetMoveRepresentation( i ).GetScore();
					if ( bestMoveScore == currentCompareMoveScore )
						numBestMoves++;
					else
						break;
				}

				int selectedMove = m_Rand.Next( numBestMoves );

				MoveRepresentation bestMove = m_MoveRepresentationList.GetMoveRepresentation( selectedMove );
				BgMove m = bestMove.GetMoves().FirstMove;
				while ( m != null )
				{
					m_BgGame.MakeMove( m.From, m.To );
					m = m.NextMove;
				}
				return bestMove.GetMoves();
			}
			else
			{
				return null;
			}
		}



		public bool Double()
		{
			if ( IsAgentPlaying( m_BgGame.CurrentOpponentPlayer ) && ( m_BgGame.CurrentOpponentPlayer == m_BgGame.GetCubeOwner() || m_BgGame.GetCubeOwner() == BgPlayer.None) )
			{
				BoardRepresentation currentBoard = new BoardRepresentation( m_BgGame );

				int[] flippedPattern = new int[currentBoard.SquareCount()];
				for ( int i = 0; i < currentBoard.SquareCount(); i++ )
					flippedPattern[25-i] = -currentBoard.GetPiecesAt( i );

				BoardRepresentation flippedBoard = new BoardRepresentation( flippedPattern );

				int evaluator = m_GUI.SelectedEvaluator();
				if ( evaluator >= 0 )
					return m_Plugins[evaluator].Double( flippedBoard );
			}
			return false;
		}



		public bool AcceptDouble()
		{
			if ( IsAgentPlaying( m_BgGame.CurrentPlayer ) )
			{
				BoardRepresentation board = new BoardRepresentation( m_BgGame );

				int evaluator = m_GUI.SelectedEvaluator();
				if ( evaluator >= 0 )
					return m_Plugins[evaluator].AcceptDouble( board );
			}
						
			return true;
		}


		public void Learn()
		{
			if ( m_GUI.m_Learn.Checked == true && m_BgGame.GetHistory().PeekLast().BearOffCountCurrent() == 15 )
			{
				int evaluator = m_GUI.SelectedEvaluator();
				if ( evaluator >= 0 )
					m_Plugins[evaluator].Learn( m_BgGame.GetHistory() );
			}
		}


		public void ShowAgentGUI()
		{
			m_GUI.Show();
		}


		public void EnableGUI( bool enable )
		{
			m_GUI.EnableGUI( enable );
		}


		private void LoadPlugins()
		{
			int defaultChecked = 0;
			
			if ( m_Plugins == null )
			{
				ArrayList pluginsKeeper = new ArrayList();

				StreamReader pluginReader = new StreamReader( "Agent.dm" );
				string line;
				char[] separator = {' ', '\t', '\n'};
				string[] tokens;

				while ( ( line = pluginReader.ReadLine() ) != null )
				{
					line = line.Trim();
					if ( line.Length > 0 && line[0] != ';' )
					{
						tokens = line.Split( separator );
						if ( tokens[0].Equals( "Agent1Default" ) )
						{
							defaultChecked = int.Parse( tokens[1] );
						}
						else
						{
							System.Runtime.Remoting.ObjectHandle oh = Activator.CreateInstanceFrom( tokens[0], tokens[1] );
							pluginsKeeper.Add( oh.Unwrap() );
						}
					}
				}
				pluginReader.Close();

				m_Plugins = new AgentDecisionModule[pluginsKeeper.Count];
				for ( int i = 0; i < pluginsKeeper.Count; i++ )
					m_Plugins[i] = ( AgentDecisionModule )pluginsKeeper[i];
			}

			string[] pluginNames = new string[m_Plugins.Length];
			for ( int i = 0; i < m_Plugins.Length; i++ )
				pluginNames[i] = m_Plugins[i].NameId();

			m_GUI.AddLoadedPlugin( pluginNames );

			if ( defaultChecked > 0 )
				m_GUI.SelectEvaluator( defaultChecked-1 );
		}


		public void ShutDown()
		{
			for ( int i = 0; i < m_Plugins.Length; i++ )
					m_Plugins[i].ShutDown();
		}

	}
}
