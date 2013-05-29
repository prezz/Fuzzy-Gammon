using System;
using System.IO;
using GammonAgent;
using GammonGame;

namespace TDModule
{
	class TDDM : AgentDecisionModule
	{
		private NeuralNetwork[]		m_NeuralNetworks;

		private const string		SAVE_FILE = ".\\tdnn.wt";
		private int					m_GamesTrained;
		private int					m_LastGamesTrainedSave;


		public TDDM()
		{
			m_GamesTrained = 0;
			m_LastGamesTrainedSave = 0;

			m_NeuralNetworks = new NeuralNetwork[2];
			m_NeuralNetworks[0] = new NeuralNetwork( 196, 80, 5 );
			m_NeuralNetworks[1] = new NeuralNetwork( 196, 80, 5 );
			m_NeuralNetworks[0].PutWeights( ContactWeights.GetWeights() );
			m_NeuralNetworks[1].PutWeights( RaceWeights.GetWeights() );

			Load( SAVE_FILE );
		}


		public override string NameId()
		{
			return "TD-NN";
		}


		public override void GradeBoards( MoveRepresentationList list, BoardRepresentation initialBoard )
		{
			for ( int i = 0; i < list.Count(); i++ )
			{
				float[] output = Grade( list.GetMoveRepresentation( i ) );
				double s = output[0] + (2 * output[1]) + (3 * output[2]) - (2 * output[3]) - (3 * output[4]);
				list.GetMoveRepresentation( i ).AddScore( s );
			}
		}


		public override void Learn( GameHistory history )
		{
			BoardRepresentation[] looserBoards = new BoardRepresentation[history.Count() / 2];
			int looserIdx = looserBoards.Length;
			for ( int i = history.Count()-2; i >= 0; i -= 2 )
				looserBoards[--looserIdx] = history.Peek( i );

			BoardRepresentation[] winnerBoards = new BoardRepresentation[history.Count() - looserBoards.Length];
			int winnerIdx = winnerBoards.Length;
			for ( int i = history.Count()-1; i >= 0; i -= 2 )
				winnerBoards[--winnerIdx] = history.Peek( i );

			int wIdx = winnerBoards.Length - 1;
			int lIdx = looserBoards.Length - 1;

			float[] wTarget = Grade( winnerBoards[wIdx] );;
			float[] lTarget = Grade( looserBoards[lIdx] );
			if ( history.WinType == 1 )
			{
				wTarget[0] = wTarget[0] + ( 0.1f * ( 1.0f - wTarget[0] ) );
				wTarget[1] = wTarget[1] + ( 0.1f * ( 0.0f - wTarget[1] ) );
				wTarget[2] = wTarget[2] + ( 0.1f * ( 0.0f - wTarget[2] ) );
				wTarget[3] = wTarget[3] + ( 0.1f * ( 0.0f - wTarget[3] ) );
				wTarget[4] = wTarget[4] + ( 0.1f * ( 0.0f - wTarget[4] ) );

				lTarget[0] = lTarget[0] + ( 0.1f * ( 0.0f - lTarget[0] ) );
				lTarget[1] = lTarget[1] + ( 0.1f * ( 0.0f - lTarget[1] ) );
				lTarget[2] = lTarget[2] + ( 0.1f * ( 0.0f - lTarget[2] ) );
				lTarget[3] = lTarget[3] + ( 0.1f * ( 0.0f - lTarget[3] ) );
				lTarget[4] = lTarget[4] + ( 0.1f * ( 0.0f - lTarget[4] ) );
			}
			else if ( history.WinType == 2 )
			{
				wTarget[0] = wTarget[0] + ( 0.1f * ( 0.0f - wTarget[0] ) );
				wTarget[1] = wTarget[1] + ( 0.1f * ( 1.0f - wTarget[1] ) );
				wTarget[2] = wTarget[2] + ( 0.1f * ( 0.0f - wTarget[2] ) );
				wTarget[3] = wTarget[3] + ( 0.1f * ( 0.0f - wTarget[3] ) );
				wTarget[4] = wTarget[4] + ( 0.1f * ( 0.0f - wTarget[4] ) );

				lTarget[0] = lTarget[0] + ( 0.1f * ( 0.0f - lTarget[0] ) );
				lTarget[1] = lTarget[1] + ( 0.1f * ( 0.0f - lTarget[1] ) );
				lTarget[2] = lTarget[2] + ( 0.1f * ( 0.0f - lTarget[2] ) );
				lTarget[3] = lTarget[3] + ( 0.1f * ( 1.0f - lTarget[3] ) );
				lTarget[4] = lTarget[4] + ( 0.1f * ( 0.0f - lTarget[4] ) );
			}
			else if ( history.WinType == 3 )
			{
				wTarget[0] = wTarget[0] + ( 0.1f * ( 0.0f - wTarget[0] ) );
				wTarget[1] = wTarget[1] + ( 0.1f * ( 0.0f - wTarget[1] ) );
				wTarget[2] = wTarget[2] + ( 0.1f * ( 1.0f - wTarget[2] ) );
				wTarget[3] = wTarget[3] + ( 0.1f * ( 0.0f - wTarget[3] ) );
				wTarget[4] = wTarget[4] + ( 0.1f * ( 0.0f - wTarget[4] ) );

				lTarget[0] = lTarget[0] + ( 0.1f * ( 0.0f - lTarget[0] ) );
				lTarget[1] = lTarget[1] + ( 0.1f * ( 0.0f - lTarget[1] ) );
				lTarget[2] = lTarget[2] + ( 0.1f * ( 0.0f - lTarget[2] ) );
				lTarget[3] = lTarget[3] + ( 0.1f * ( 0.0f - lTarget[3] ) );
				lTarget[4] = lTarget[4] + ( 0.1f * ( 1.0f - lTarget[4] ) );
			}

			while ( lIdx >= 0 || wIdx >= 0 )
			{
				if ( wIdx >= 0 )
				{
					Train( winnerBoards[wIdx], wTarget );

					if ( wIdx > 0 )
					{
						float[] prevWState = Grade( winnerBoards[wIdx-1] );
						wTarget[0] = prevWState[0] + ( 0.1f * ( wTarget[0] - prevWState[0] ) );
						wTarget[1] = prevWState[1] + ( 0.1f * ( wTarget[1] - prevWState[1] ) );
						wTarget[2] = prevWState[2] + ( 0.1f * ( wTarget[2] - prevWState[2] ) );
						wTarget[3] = prevWState[3] + ( 0.1f * ( wTarget[3] - prevWState[3] ) );
						wTarget[4] = prevWState[4] + ( 0.1f * ( wTarget[4] - prevWState[4] ) );
					}
					wIdx--;
				}

				if ( lIdx >= 0 )
				{
					Train( looserBoards[lIdx], lTarget );
					
					if ( lIdx > 0 )
					{
						float[] prevLState = Grade( looserBoards[lIdx-1] );				
						lTarget[0] = prevLState[0] + ( 0.1f * ( lTarget[0] - prevLState[0] ) );
						lTarget[1] = prevLState[1] + ( 0.1f * ( lTarget[1] - prevLState[1] ) );
						lTarget[2] = prevLState[2] + ( 0.1f * ( lTarget[2] - prevLState[2] ) );
						lTarget[3] = prevLState[3] + ( 0.1f * ( lTarget[3] - prevLState[3] ) );
						lTarget[4] = prevLState[4] + ( 0.1f * ( lTarget[4] - prevLState[4] ) );
					}
					lIdx--;
				}
			}
			m_GamesTrained++;

			if ( m_GamesTrained % 100 == 0 )
				Save( SAVE_FILE );
		}


		public override bool Double( BoardRepresentation flippedBoard )
		{
			float[] output = Grade( flippedBoard );

			float totalWinSum = output[0] + output[1] + output[2];
			float gammonWinSum = output[1] + output[2];

			if ( totalWinSum >= 0.66f && gammonWinSum < 0.5f )
				return true;

			return false;
		}


		public override bool AcceptDouble( BoardRepresentation board )
		{
			float[] output = Grade( board );
			float totalWinSum = output[0] + output[1] + output[2];
			if ( totalWinSum < 0.25f )
				return false;

			return true;
		}


		public override void ShutDown()
		{
			Save( SAVE_FILE );
		}


		private int Race( BoardRepresentation currentBoard )
		{
			int race = 1;
			bool opponentFound = false;
			for ( int j = 0; j < currentBoard.SquareCount() && race == 1; j++ )
			{
				if ( !opponentFound && currentBoard.GetPiecesAt( j ) < 0 )
					opponentFound = true;

				if ( opponentFound && currentBoard.GetPiecesAt( j ) > 0 )
					race = 0;
			}
			return race;
		}


		private float[] Grade( BoardRepresentation currentBoard )
		{
			int race = Race( currentBoard );
			float[] input = BoardToNetworkInput( currentBoard );
			float[] output = new float[5];
			m_NeuralNetworks[race].Run( input, output );
			return output;
		}


		private void Train( BoardRepresentation currentBoard, float[] target )
		{
			int race = Race( currentBoard );
			float[] input = BoardToNetworkInput( currentBoard );
			m_NeuralNetworks[race].Train( input, target, 0.00001f, 0.2f );
		}


		private float[] BoardToNetworkInput( BoardRepresentation currentBoard )
		{
			int Idx = 0;
			float[] board = new float[196];

			for ( int i = 1; i < 25; i++ )
			{
				int pieces = currentBoard.GetPiecesAt( i );

				if ( pieces == 0 )
				{
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;

					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
				}
				else if ( pieces == 1 )
				{
					board[Idx++] = 1;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;

					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
				}
				else if ( pieces == 2 )
				{
					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 0;
					board[Idx++] = 0;

					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
				}
				else if ( pieces == 3 )
				{
					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 0;

					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
				}
				else if ( pieces > 3 )
				{
					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = ( pieces - 3 ) / 2.0f;

					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
				}
				else if ( pieces == -1 )
				{
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;

					board[Idx++] = 1;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
				}
				else if ( pieces == -2 )
				{
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;

					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 0;
					board[Idx++] = 0;
				}
				else if ( pieces == -3 )
				{
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;

					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 0;
				}
				else if ( pieces < -3 )
				{
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;
					board[Idx++] = 0;

					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = 1;
					board[Idx++] = ( -pieces - 3 ) / 2.0f;;
				}
			}

			board[Idx++] = currentBoard.GetPiecesAt( 25 ) / 2.0f;
			board[Idx++] = -currentBoard.GetPiecesAt( 0 ) / 2.0f;

			board[Idx++] = currentBoard.BearOffCountCurrent() / 15.0f;
			board[Idx++] = -currentBoard.BearOffCountOpponent() / 15.0f;

			return board;
		}


		private void Load( string file )
		{
			if ( File.Exists( file ) )
			{
				StreamReader reader = new StreamReader( file );
				string line = reader.ReadLine();
				m_GamesTrained = int.Parse( line );
				m_LastGamesTrainedSave = m_GamesTrained;

				float[] contactWeights = m_NeuralNetworks[0].GetWeights();
				for ( int i = 0; i < contactWeights.Length; i++ )
				{
					line = reader.ReadLine();
					contactWeights[i] = float.Parse( line );
				}
				m_NeuralNetworks[0].PutWeights( contactWeights );

				float[] raceWeights = m_NeuralNetworks[1].GetWeights();
				for ( int i = 0; i < raceWeights.Length; i++ )
				{
					line = reader.ReadLine();
					raceWeights[i] = float.Parse( line );
				}
				m_NeuralNetworks[1].PutWeights( raceWeights );

				reader.Close();
			}		
		}

		private void Save( string file )
		{
			try
			{
				if ( m_GamesTrained > m_LastGamesTrainedSave )
				{
					StreamWriter writer = new StreamWriter( file, false );

					m_LastGamesTrainedSave = m_GamesTrained;
					writer.WriteLine( m_GamesTrained.ToString() );

					float[] contactWeights = m_NeuralNetworks[0].GetWeights();
					for ( int i = 0; i < contactWeights.Length; i++ )
					{
						writer.WriteLine( contactWeights[i].ToString() );
					}

					float[] raceWeights = m_NeuralNetworks[1].GetWeights();
					for ( int i = 0; i < raceWeights.Length; i++ )
					{
						writer.WriteLine( raceWeights[i].ToString() );
					}
					writer.Close();
				}
			}
			catch ( Exception )
			{}
		
		}
	}
}
