using System;
using System.IO;
using GammonAgent;
using GammonGame;

namespace TD2Module
{
	class TD2DM : AgentDecisionModule
	{
		private	const float			STEP_SIZE = 0.05f;
		private const float			LEARNING_RATE = 0.1f;
		private const float			MAX_SQUARE_ERROR = 0.000001f;

		private const int			CONTACT_INPUT_COUNT = 248;
		private const int			CONTACT_HIDDEN_COUNT = 128;
		private const int			RACE_INPUT_COUNT = 224;
		private const int			RACE_HIDDEN_COUNT = 116;
		private const string		SAVE_FILE = ".\\td2nn.wt";

		private int[]				m_OwnBoardView;
		private int[]				m_OppBoardView;
		private float[]				m_Output;
		private NeuralNetwork		m_ContactNetwork;
		private float[]				m_ContactInput;
		private NeuralNetwork		m_RaceNetwork;
		private float[]				m_RaceInput;
		private int					m_GamesTrained;
		private int					m_LastGamesTrainedSave;


		public TD2DM()
		{
			m_OwnBoardView = new int[26];
            m_OppBoardView = new int[26];

			m_ContactNetwork = new NeuralNetwork( CONTACT_INPUT_COUNT, CONTACT_HIDDEN_COUNT, 5 );
			m_RaceNetwork = new NeuralNetwork( RACE_INPUT_COUNT, RACE_HIDDEN_COUNT, 5 );
			m_GamesTrained = 0;
			m_LastGamesTrainedSave = 0;

			m_Output = new float[5];
			m_ContactInput = new float[CONTACT_INPUT_COUNT];
			m_RaceInput = new float[RACE_INPUT_COUNT];

			m_ContactNetwork.PutWeights( ContactWeights.GetWeights() );
			m_RaceNetwork.PutWeights( RaceWeights.GetWeights() );

			Load( SAVE_FILE );
		}


		public override string NameId()
		{
			return "TD-NN 2";
		}


		public override void GradeBoards( MoveRepresentationList list, BoardRepresentation initialBoard )
		{
			bool race = Race( initialBoard );
			for ( int i = 0; i < list.Count(); i++ )
			{
				Grade( list.GetMoveRepresentation( i ), race, m_Output );
				float regularLoseProberbilety = 1.0f - ( m_Output[0] + m_Output[1] + m_Output[2] + m_Output[3] + m_Output[4] );
				double equity = m_Output[0] + (2 * m_Output[1]) + (3 * m_Output[2]) - regularLoseProberbilety - (2 * m_Output[3]) - (3 * m_Output[4]);
				list.GetMoveRepresentation( i ).AddScore( equity );
			}
		}


		public override void Learn( GameHistory history )
		{
			BoardRepresentation[] boards = new BoardRepresentation[history.Count()];
			for ( int i = boards.Length-1; i >= 0; i-- )
				boards[i] = history.Peek( i );

			int idx = boards.Length-1;
			int firstRaceBoardIdx = idx;
			for ( int i = 0; i < boards.Length; i++ )
			{
				if ( Race( boards[i] ) )
				{
					firstRaceBoardIdx = i;
					break;
				}
			}

			float[] wTarget = new float[5];
			float[] lTarget = new float[5];
			Grade( boards[idx], idx > firstRaceBoardIdx, wTarget );
			Grade( boards[idx-1], (idx-1) > firstRaceBoardIdx, lTarget );

			if ( history.WinType == 1 )
			{
				wTarget[0] = wTarget[0] + ( STEP_SIZE * ( 1.0f - wTarget[0] ) );
				wTarget[1] = wTarget[1] + ( STEP_SIZE * ( 0.0f - wTarget[1] ) );
				wTarget[2] = wTarget[2] + ( STEP_SIZE * ( 0.0f - wTarget[2] ) );
				wTarget[3] = wTarget[3] + ( STEP_SIZE * ( 0.0f - wTarget[3] ) );
				wTarget[4] = wTarget[4] + ( STEP_SIZE * ( 0.0f - wTarget[4] ) );

				lTarget[0] = lTarget[0] + ( STEP_SIZE * ( 0.0f - lTarget[0] ) );
				lTarget[1] = lTarget[1] + ( STEP_SIZE * ( 0.0f - lTarget[1] ) );
				lTarget[2] = lTarget[2] + ( STEP_SIZE * ( 0.0f - lTarget[2] ) );
				lTarget[3] = lTarget[3] + ( STEP_SIZE * ( 0.0f - lTarget[3] ) );
				lTarget[4] = lTarget[4] + ( STEP_SIZE * ( 0.0f - lTarget[4] ) );
			}
			else if ( history.WinType == 2 )
			{
				wTarget[0] = wTarget[0] + ( STEP_SIZE * ( 0.0f - wTarget[0] ) );
				wTarget[1] = wTarget[1] + ( STEP_SIZE * ( 1.0f - wTarget[1] ) );
				wTarget[2] = wTarget[2] + ( STEP_SIZE * ( 0.0f - wTarget[2] ) );
				wTarget[3] = wTarget[3] + ( STEP_SIZE * ( 0.0f - wTarget[3] ) );
				wTarget[4] = wTarget[4] + ( STEP_SIZE * ( 0.0f - wTarget[4] ) );

				lTarget[0] = lTarget[0] + ( STEP_SIZE * ( 0.0f - lTarget[0] ) );
				lTarget[1] = lTarget[1] + ( STEP_SIZE * ( 0.0f - lTarget[1] ) );
				lTarget[2] = lTarget[2] + ( STEP_SIZE * ( 0.0f - lTarget[2] ) );
				lTarget[3] = lTarget[3] + ( STEP_SIZE * ( 1.0f - lTarget[3] ) );
				lTarget[4] = lTarget[4] + ( STEP_SIZE * ( 0.0f - lTarget[4] ) );
			}
			else if ( history.WinType == 3 )
			{
				wTarget[0] = wTarget[0] + ( STEP_SIZE * ( 0.0f - wTarget[0] ) );
				wTarget[1] = wTarget[1] + ( STEP_SIZE * ( 0.0f - wTarget[1] ) );
				wTarget[2] = wTarget[2] + ( STEP_SIZE * ( 1.0f - wTarget[2] ) );
				wTarget[3] = wTarget[3] + ( STEP_SIZE * ( 0.0f - wTarget[3] ) );
				wTarget[4] = wTarget[4] + ( STEP_SIZE * ( 0.0f - wTarget[4] ) );

				lTarget[0] = lTarget[0] + ( STEP_SIZE * ( 0.0f - lTarget[0] ) );
				lTarget[1] = lTarget[1] + ( STEP_SIZE * ( 0.0f - lTarget[1] ) );
				lTarget[2] = lTarget[2] + ( STEP_SIZE * ( 0.0f - lTarget[2] ) );
				lTarget[3] = lTarget[3] + ( STEP_SIZE * ( 0.0f - lTarget[3] ) );
				lTarget[4] = lTarget[4] + ( STEP_SIZE * ( 1.0f - lTarget[4] ) );
			}

			bool isCurrentBoardWinner = true;
			float[] prevWState = new float[5];
			float[] prevLState = new float[5];
			while ( idx >= 0 )
			{
				if ( isCurrentBoardWinner )
				{
					Train( boards[idx], idx > firstRaceBoardIdx, wTarget );
					idx--;

					if ( idx > 0 )
					{
						Grade( boards[idx-1], idx-1 > firstRaceBoardIdx, prevWState );
						wTarget[0] = prevWState[0] + ( STEP_SIZE * ( wTarget[0] - prevWState[0] ) );
						wTarget[1] = prevWState[1] + ( STEP_SIZE * ( wTarget[1] - prevWState[1] ) );
						wTarget[2] = prevWState[2] + ( STEP_SIZE * ( wTarget[2] - prevWState[2] ) );
						wTarget[3] = prevWState[3] + ( STEP_SIZE * ( wTarget[3] - prevWState[3] ) );
						wTarget[4] = prevWState[4] + ( STEP_SIZE * ( wTarget[4] - prevWState[4] ) );
					}
				}
				else
				{
					Train( boards[idx], idx > firstRaceBoardIdx, lTarget );
					idx--;

					if ( idx > 0 )
					{
						Grade( boards[idx-1], idx-1 > firstRaceBoardIdx, prevLState );
						lTarget[0] = prevLState[0] + ( STEP_SIZE * ( lTarget[0] - prevLState[0] ) );
						lTarget[1] = prevLState[1] + ( STEP_SIZE * ( lTarget[1] - prevLState[1] ) );
						lTarget[2] = prevLState[2] + ( STEP_SIZE * ( lTarget[2] - prevLState[2] ) );
						lTarget[3] = prevLState[3] + ( STEP_SIZE * ( lTarget[3] - prevLState[3] ) );
						lTarget[4] = prevLState[4] + ( STEP_SIZE * ( lTarget[4] - prevLState[4] ) );
					}
				}
				isCurrentBoardWinner = !isCurrentBoardWinner;
			}
			m_GamesTrained++;

			if ( m_GamesTrained % 100 == 0 )
				Save( SAVE_FILE );
		}


		public override bool Double( BoardRepresentation flippedBoard )
		{
			bool race = Race( flippedBoard );
			Grade( flippedBoard, race, m_Output );

			float totalWinSum = m_Output[0] + m_Output[1] + m_Output[2];
			float gammonWinSum = m_Output[1] + m_Output[2];

			if ( totalWinSum >= 0.66f && gammonWinSum < 0.5f )
				return true;

			return false;
		}


		public override bool AcceptDouble( BoardRepresentation board )
		{
			bool race = Race( board );
			Grade( board, race, m_Output );

			float totalWinSum = m_Output[0] + m_Output[1] + m_Output[2];

			if ( totalWinSum < 0.25f )
				return false;

			return true;
		}
		
		
		public override void ShutDown()
		{
			Save( SAVE_FILE );
		}


		private void Grade( BoardRepresentation board, bool race, float[] grade )
		{
			if ( race )
			{
				SetRaceInput( board, m_RaceInput );
				m_RaceNetwork.Run( m_RaceInput, grade );
			}
			else
			{
				SetContactInput( board, m_ContactInput );
				m_ContactNetwork.Run( m_ContactInput, grade );
			}
		}


		private void Train( BoardRepresentation board, bool race, float[] target )
		{
			if ( race )
			{
				SetRaceInput( board, m_RaceInput );
				m_RaceNetwork.Train( m_RaceInput, target, MAX_SQUARE_ERROR, LEARNING_RATE );
			}
			else
			{
				SetContactInput( board, m_ContactInput );
				m_ContactNetwork.Train( m_ContactInput, target, MAX_SQUARE_ERROR, LEARNING_RATE );
			}
		}


		private bool Race( BoardRepresentation currentBoard )
		{
			bool race = true;
			bool opponentFound = false;
			for ( int j = 0; j < currentBoard.SquareCount() && race; j++ )
			{
				if ( !opponentFound && currentBoard.GetPiecesAt( j ) < 0 )
					opponentFound = true;

				if ( opponentFound && currentBoard.GetPiecesAt( j ) > 0 )
					race = false;
			}
			return race;
		}


		private void Load( string file )
		{
			if ( File.Exists( file ) )
			{
				StreamReader reader = new StreamReader( file );
				string line = reader.ReadLine();
				m_GamesTrained = int.Parse( line );
				m_LastGamesTrainedSave = m_GamesTrained;

				float[] contactWeights = m_ContactNetwork.GetWeights();
				for ( int i = 0; i < contactWeights.Length; i++ )
				{
					line = reader.ReadLine();
					contactWeights[i] = float.Parse( line );
				}
				m_ContactNetwork.PutWeights( contactWeights );

				float[] raceWeights = m_RaceNetwork.GetWeights();
				for ( int i = 0; i < raceWeights.Length; i++ )
				{
					line = reader.ReadLine();
					raceWeights[i] = float.Parse( line );
				}
				m_RaceNetwork.PutWeights( raceWeights );

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

					float[] contactWeights = m_ContactNetwork.GetWeights();
					for ( int i = 0; i < contactWeights.Length; i++ )
					{
						writer.WriteLine( contactWeights[i].ToString() );
					}

					float[] raceWeights = m_RaceNetwork.GetWeights();
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

		private void SetContactInput( BoardRepresentation board, float[] input )
		{
			for ( int i = 0; i < input.Length; i++ )
				input[i] = 0.0f;

			for ( int i = 0; i < 26; i++ )
			{
				m_OwnBoardView[i] = board.GetPiecesAt( i );
				m_OppBoardView[25-i] = -board.GetPiecesAt( i );
			}

			int idx = 0;
			idx = SetBoard( m_OwnBoardView, input, idx );
			idx = SetContactBeardOff( m_OwnBoardView, input, idx );
			idx = SetBar( m_OwnBoardView, input, idx );
			idx = SetHalfContactInput( m_OwnBoardView, input, idx );
			idx = SetHalfContactInput( m_OppBoardView, input, idx );
		}


		private void SetRaceInput( BoardRepresentation board, float[] input )
		{
			for ( int i = 0; i < input.Length; i++ )
				input[i] = 0.0f;

			for ( int i = 0; i < 26; i++ )
			{
				m_OwnBoardView[i] = board.GetPiecesAt( i );
				m_OppBoardView[25-i] = -board.GetPiecesAt( i );
			}

			int idx = 0;
			idx = SetBoard( m_OwnBoardView, input, idx );
			idx = SetRaceBeardOff( m_OwnBoardView, input, idx );
			idx = SetHalfRaceInput( m_OwnBoardView, input, idx );
			idx = SetHalfRaceInput( m_OppBoardView, input, idx );		
		}


		private int SetHalfContactInput( int[] board, float[] input, int targetStartIndex )
		{
			int idx = targetStartIndex;
			idx = SetBreakContact( board, input, idx );
			idx = SetBackChecker( board, input, idx );
			idx = SetBackAnchor( board, input, idx );
			idx = SetFrontAnchor( board, input, idx );
			idx = SetFreePips( board, input, idx );
			idx = SetAveragePipLoss( board, input, idx );
			idx = SetHomeClosing( board, input, idx );
			idx = SetOnBarAvgPipLoss( board, input, idx );
			idx = SetOpponentBackCheckerEscapes( board, input, idx );
			idx = SetCurrentHardestBackContainment( board, input, idx );
			idx = SetHardestBackContainment( board, input, idx );
			idx = SetOpponentMobilety( board, input, idx );
			idx = SetMoment( board, input, idx );
			idx = SetTiming( board, input, idx );
			idx = SetBackbone( board, input, idx );
			idx = SetBackboardHandling( board, input, idx );
			idx = SetToBeHit( board, input, idx );
			idx = SetToBeDoubleHit( board, input, idx );
			return idx;
		}


		private int SetHalfRaceInput( int[] board, float[] input, int targetStartIndex )
		{
			int idx = targetStartIndex;
			idx = SetCrossOver( board, input, idx );
			return idx;
		}


		private int SetBoard( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			for ( int i = 1; i < 25; i++ )
			{
				int pieces = board[i];

				if ( pieces >= 1 )
				{
					target[idx+0] = 1;
					if ( pieces >= 2 )
					{
						target[idx+1] = 1;
						if ( pieces >= 3 )
						{
							target[idx+2] = 1;
							if ( pieces > 3 )
							{
								target[idx+3] = ( ((float)pieces) - 3.0f ) / 2.0f;
							}
						}
					}
				}

				if ( pieces <= -1 )
				{
					target[idx+4] = 1;
					if ( pieces <= -2 )
					{
						target[idx+5] = 1;
						if ( pieces <= -3 )
						{
							target[idx+6] = 1;
							if ( pieces < -3 )
							{
								target[idx+7] = ( ((float)-pieces) - 3.0f ) / 2.0f;
							}
						}
					}
				}

				idx += 8;
			}

			return idx;
		}


		private int SetContactBeardOff( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int ownBeardOff = 15;
			int oppBeardOff = 15;
			for ( int i = 0; i < 26; i++ )
			{
				if ( board[i] > 0 )
					ownBeardOff -= board[i];
				if ( board[i] < 0 )
					oppBeardOff += board[i];
			}
			
			if ( ownBeardOff > 10 )
			{
				target[idx++] = 1.0f;
				target[idx++] = 1.0f;
				target[idx++] = ( ((float)ownBeardOff) - 10.0f ) / 5.0f;
			}
			else if ( ownBeardOff > 5 )
			{
				target[idx++] = 1.0f;
				target[idx++] = ( ((float)ownBeardOff) - 5.0f) / 5.0f;
				target[idx++] = 0.0f;
			}
			else
			{
				target[idx++] = ((float)ownBeardOff) / 5.0f;
				target[idx++] = 0.0f;
				target[idx++] = 0.0f;
			}

			if ( oppBeardOff > 10 )
			{
				target[idx++] = 1.0f;
				target[idx++] = 1.0f;
				target[idx++] = ( ((float)oppBeardOff) - 10.0f ) / 5.0f;
			}
			else if ( oppBeardOff > 5 )
			{
				target[idx++] = 1.0f;
				target[idx++] = ( ((float)oppBeardOff) - 5.0f) / 5.0f;
				target[idx++] = 0.0f;
			}
			else
			{
				target[idx++] = ((float)oppBeardOff) / 5.0f;
				target[idx++] = 0.0f;
				target[idx++] = 0.0f;
			}

			return idx;
		}


		private int SetRaceBeardOff( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int ownBeardOff = 15;
			int oppBeardOff = 15;
			for ( int i = 0; i < 26; i++ )
			{
				if ( board[i] > 0 )
					ownBeardOff -= board[i];
				if ( board[i] < 0 )
					oppBeardOff += board[i];
			}

			for ( int i = 0; i < ownBeardOff; i++ )
				target[idx+i] = 1.0f;

			idx += 15;

			for ( int i = 0; i < oppBeardOff; i++ )
				target[idx+i] = 1.0f;

			return idx + 15;
		}


		private int SetCrossOver( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			float crossOver = 0.0f;
			for ( int i = 1; i < 4; i++ )
			{
				for( int j = (6*i)+1; j < ((6*i) + 7); j++ )
				{
					int n = board[j];
					if ( n > 0 )
						crossOver += n * i;
				}
			}
			target[idx++] = crossOver / 10.0f;

			return idx;
		}


		private int SetBar( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int ownOnBar = board[25];
			if ( ownOnBar >= 1 )
			{
				target[idx+0] = 1.0f;
				if ( ownOnBar >= 2 )
				{
					target[idx+1] = 1.0f;
					if ( ownOnBar >= 3 )
					{
						target[idx+2] = 1.0f;
						if ( ownOnBar > 3 )
						{
							target[idx+3] = ( ((float)ownOnBar) - 3.0f ) / 2.0f;
						}
					}
				}
			}

			int oppOnBar = -board[0];
			if ( oppOnBar >= 1 )
			{
				target[idx+4] = 1.0f;
				if ( oppOnBar >= 2 )
				{
					target[idx+5] = 1.0f;
					if ( oppOnBar >= 3 )
					{
						target[idx+6] = 1.0f;
						if ( oppOnBar > 3 )
						{
							target[idx+7] = ( ((float)oppOnBar) - 3.0f ) / 2.0f;
						}
					}
				}
			}

			return idx + 8;
		}


		private int SetBreakContact( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			bool oppFound = false;
			int oppBack = 0;
			float ownBreakDistance = 0.0f;
			for ( int i = 0; i < 26; i++ )
			{
				int n = board[i];
				if ( !oppFound && n < 0 )
				{
					oppFound = true;
					oppBack = ( i == 0 )? 1 : i; 
				}
				else if ( oppFound && n > 0 )
				{
					ownBreakDistance += ( i + 1 - oppBack ) * n;
				}
			}
			target[idx++] = ownBreakDistance / 167.0f;

			return idx;
		}


		private int SetBackChecker( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			float backLocation = 0.0f;
			for ( int i = 25; i > 0; i-- )
			{
				if ( board[i] > 0 )
				{
					backLocation = i;
					break;
				}
			}
			target[idx++] = backLocation / 25.0f;

			return idx;
		}


		private int SetBackAnchor( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			float backAnchor = 0.0f;
			for ( int i = 24; i > 0; i-- )
			{
				if ( board[i] > 1 )
				{
					backAnchor = i;
					break;
				}
			}
			target[idx++] = backAnchor / 24.0f;

			return idx;
		}


		private int SetFrontAnchor( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int frontAnchor = 0;
			for ( int i = 19; i < 25; i++ )
			{
				if ( board[i] > 1 )
				{
					frontAnchor = 25 - i;
					break;
				}
			}
			if ( frontAnchor == 0 )
			{
				for( int i = 18; i > 12 ; i-- )
				{
					if ( board[i] > 1 )
					{
						frontAnchor = 25 - i;
						break;
					}				
				}
			}
			target[idx++] = ( frontAnchor == 0 )? 2.0f : ((float)frontAnchor) / 6.0f;

			return idx;
		}


		private int SetFreePips( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			float freePips = 0.0f;
			for ( int i = 0; i < 26; i++ )
			{
				int n = board[i];

				if ( n > 0 )
					freePips += i * n;
				else if ( n < 0 )
					break;
			}
			target[idx++] = freePips / 100.0f;

			return idx;
		}


		private int SetAveragePipLoss( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int oppHomeAnchors = 0;
			int numBlots = 0;
			for ( int i = 1; i < 25; i++ )
			{
				if ( board[i] == 1 )
					numBlots++;
				if ( i > 18 && board[i] < -1 )
					oppHomeAnchors++;
			}

			if ( numBlots > 0 )
			{
				int index = 0;
				int[] location = new int[numBlots];
				int[] hits = new int[numBlots];

				bool[,] diceRollsHitting = new bool[6,6];
				for ( int i = 1; i < 25; i++ )
				{
					if ( board[i] == 1 )
					{
						for ( int x = 0; x < 6; x++ )
							for ( int y = 0; y < 6; y++ )
								diceRollsHitting[x,y] = false;

						for ( int j = 0; j < i; j++ )
						{
							if ( ( j < 19 && board[j] < 0  ) || ( j > 18 && board[j] < 0 && board[j] != -2 ) )
								FindHittingDices( i, j, board, diceRollsHitting );
						}
						for ( int x = 0; x < 6; x++ )
							for ( int y = 0; y < 6; y++ )
								if ( diceRollsHitting[x,y] )
									hits[index]++;
						location[index++] = i;
					}
				}
				float avgPiploss = 0.0f;
				for ( int i = 0; i < numBlots; i++ )
					if ( !( ( location[i] == 24 || location[i] == 23 ) && oppHomeAnchors < 3 ) )
						avgPiploss += ((float)(hits[i] * (25.0f-location[i]))) / 36.0f;

				target[idx++] = avgPiploss / 12.0f;
			}
			else
			{
				target[idx++] = 0.0f;
			}

			return idx;
		}


		private int SetHomeClosing( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			float closing = 0.0f;
			for ( int i = 1; i < 7; i++ )
				if ( board[i] > 1 )
					closing += 1.0f;

			target[idx++] = 1.0f - ( ( 1.0f - ( closing/6.0f ) ) * ( 1.0f - ( closing/6.0f ) ) );

			return idx;
		}


		private int SetOnBarAvgPipLoss( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			if( board[0] < 0 ) 
			{
				int loss = 0;
				bool two = board[0] < -1;
      
				for( int i = 1; i < 7; i++ ) 
				{
					if( board[i] > 1 ) 
					{
						loss += 4 * i; //any double loses

						for( int j = i+1; j < 7; j++ ) 
						{
							if( board[j] > 1 ) 
							{
								loss += 2 * ( i + j );
							} 
							else 
							{
								if( two ) 
								{
									loss += 2 * i;
								}
							}
						}
					} 
					else 
					{
						if( two ) 
						{
							for( int j = i+1; j < 7; j++ ) 
							{
								if( board[j] > 1 ) 
								{
									loss += 2 * j;
								}
							}
						}
					}
				}
      			target[idx++] = ((float)loss) / (36.0f * (49.0f/6.0f));
			} 
			else 
			{
				target[idx++] = 0.0f;
			}

			return idx;
		}


		private int SetOpponentBackCheckerEscapes( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int oppBackLocation = 25;
			for ( int i = 0; i < 25; i++ )
			{
				if ( board[i] < 0 )
				{
					oppBackLocation = i;
					break;
				}
			}

			float escapes = ((float)GetOppEscapeMoves( oppBackLocation, board ));
			target[idx++] = escapes / 36.0f;
			return idx;
		}


		private int SetCurrentHardestBackContainment( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int oppBackLocation = 25;
			for ( int i = 0; i < 10; i++ )
			{
				if ( board[i] < 0 )
				{
					oppBackLocation = i;
					break;
				}
			}

			float hardest = 36.0f;
			for ( int i = oppBackLocation; i < 10; i++ )
			{
				float escapes = (float)GetOppEscapeMoves( i, board );
				if ( escapes < hardest )
					hardest = escapes;
			}

			target[idx++] = ( 36.0f - hardest ) / 36.0f;
			target[idx++] = ( ( 36.0f - hardest ) / 36.0f ) * ( ( 36.0f - hardest ) / 36.0f );

			return idx;
		}


		private int SetHardestBackContainment( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			float hardest = 36.0f;
			for ( int i = 0; i < 10; i++ )
			{
				float escapes = (float)GetOppEscapeMoves( i, board );
				if ( escapes < hardest )
					hardest = escapes;
			}

			target[idx++] = ( 36.0f - hardest ) / 36.0f;
			target[idx++] = ( ( 36.0f - hardest ) / 36.0f ) * ( ( 36.0f - hardest ) / 36.0f );

			return idx;
		}


		private int SetOpponentMobilety( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			float mobilety = 0.0f;
			for ( int i = 0; i < 19; i++ )
			{
				if ( board[i] < 0 )
					mobilety += ( 19 - i ) * ( -board[i] ) * GetOppEscapeMoves( i, board );
			}

			target[idx++] = mobilety / 3600.0f;

			return idx;			
		}


		private int SetMoment( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int pips = 0; 
			int numCheckers = 0;
			for( int i = 1; i < 26; i++ ) 
			{
				if ( board[i] > 0 )
				{
					numCheckers += board[i];
					pips += i * board[i];
				}
			}

			int avg = 0;
			if( numCheckers > 0 ) 
				avg = pips / numCheckers;

			numCheckers = 0;
			float moment = 0.0f;
			for( int i = avg + 1; i < 26; i++ ) 
			{
				if( board[i] > 0 ) 
				{
					numCheckers += board[i];
					moment += board[i] * ( ( i - avg ) * ( i - avg ) );
				}
			}

			if( numCheckers > 0 ) 
				moment = moment / numCheckers;

			target[idx++] = moment / 400.0f;

			return idx;			
		}


		private int SetTiming( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int oppBackLocation = 25;
			for ( int i = 0; i < 25; i++ )
			{
				if ( board[i] < 0 )
				{
					oppBackLocation = i;
					break;
				}
			}

			float t = 25 * board[25];
			int no = board[25];
			int j = 24;
			for( ;  j > 12 && j > oppBackLocation; j-- ) 
			{
				if( board[j] > 0 && board[j] != 2 ) 
				{
					int n = ( board[j] > 2 )? ( board[j] - 2 ) : 1;
					no += n;
					t += j * n;
				}
			}

			for( ; j > 6; j-- ) 
			{
				if( board[j] > 0 ) 
				{
					int n = board[j];
					no += n;
					t += j * n;
				}
			}
    
			for( ;  j > 0; j-- ) 
			{
				if( board[j] > 2 ) 
				{
					t += j * (board[j] - 2);
					no += (board[j] - 2);
				} 
				else if( board[j] < 2 ) 
				{
					int n = ( board[j] == 1 )? 1 : 2;

					if( no >= n ) 
					{
						t -= j * n;
						no -= n;
					}
				}
			}

			if( t < 0.0f ) 
				t = 0.0f;

			target[idx++] = t / 100.0f;

			return idx;			
		}


		private int SetBackbone( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int backAnchor = -1;
			int w = 0;
			int tot = 0;
    		for( int i = 24; i > 0; i-- ) 
			{
				if( board[i] > 1 ) 
				{
					if( backAnchor == -1 ) 
					{
						backAnchor = i;
					}
					else
					{
						int dist = backAnchor - i;

						int c = 0;
						if( dist < 7 ) 
							c = 11;
						else if( dist < 12 ) 
							c = 13 - dist;

						w += c * board[backAnchor];
						tot += board[backAnchor];
					}
				}
			}

			if( tot != 0 ) 
				target[idx++] = 1.0f - ( ((float)w) / ( ((float)tot) * 11.0f ) );
			else 
				target[idx++] = 0.0f;

			return idx;			
		}


		private int SetBackboardHandling( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			int anchors = 0;
			for ( int i = 19; i < 25; i++ )
			{
				if ( board[i] > 1 )
					anchors++;
			}

			int total = 0;
			if ( anchors > 0 )
			{
				for ( int i = 19; i < 26; i++ )
					if ( board[i] > 0 )
						total += board[i];
			}

			if ( anchors > 1 )
				target[idx] = ( total - 3 ) / 4.0f;
			else
				target[idx+1] = total / 8.0f;

			idx += 2;

			return idx;
		}


		private int SetToBeHit( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;
			
			bool[,] diceRollsHitting = new bool[6,6];
			for ( int i = 0; i < 6; i++ )
				for ( int j = 0; j < 6; j++ )
					diceRollsHitting[i,j] = false;

			for ( int i = 1; i < 25; i++ )
			{
				if ( board[i] == 1 )
					for ( int j = 0; j < i; j++ )
						if ( board[j] < 0 )
							FindHittingDices( i, j, board, diceRollsHitting );
			}

			float noRollsHitting = 0.0f;
			for ( int i = 0; i < 6; i++ )
				for ( int j = 0; j < 6; j++ )
					if ( diceRollsHitting[i,j] )
						noRollsHitting += 1.0f;

			target[idx++] = noRollsHitting / 36.0f;

			return idx;
		}


		private int SetToBeDoubleHit( int[] board, float[] target, int targetStartIndex )
		{
			int idx = targetStartIndex;

			bool[,] hittingDices = new bool[6,6];
			for ( int i = 0; i < 6; i++ )
				for ( int j = 0; j < 6; j++ )
					hittingDices[i,j] = false;

			for ( int i = 0; i < 25; i++ )
				if ( board[i] < 0 )
					for ( int x = 1; x < 7; x++ )
						for ( int y = 1; y < 7; y++ )
							if ( SingleCheckerDoubleHits( x, y, i, board ) || SinglePosDoubleHits( x, y, i, board ) )
								hittingDices[x-1,y-1] = true;

			for ( int i = 1; i < 25; i++ )
				if ( board[i] == 1 )
					for ( int j = i+1; j < 25; j++ )
						if ( board[j] == 1 )
							for ( int x = 1; x < 7; x++ )
								for ( int y = 1; y < 7; y++ )
									if ( DifCheckersDifPosDoubleHits( x, y, i, j, board ) )
										hittingDices[x-1,y-1] = true;


			float noRollsHittingTwice = 0.0f;
			for ( int i = 0; i < 6; i++ )
				for ( int j = 0; j < 6; j++ )
					if ( hittingDices[i,j] )
						noRollsHittingTwice += 1.0f;

			target[idx++] = noRollsHittingTwice / 36.0f;
			
			return idx;
		}


		private int GetOppEscapeMoves( int from, int[] board )
		{
			int legalMoves = 0;
			for ( int i = 1; i <= 6; i++ )
				for ( int j = 1; j <= 6; j++ )
					if ( ( ( from + i ) > 24 || ( from + j ) > 24 || board[from + i] < 2 || board[from + j] < 2 ) && ( ( from + i + j ) > 24 || board[from + i + j] < 2 ) )
						legalMoves++;

			return legalMoves;
		}


		private void FindHittingDices( int position, int hitFrom, int[] board, bool[,] hittingDices )
		{
			int distance = position - hitFrom;

			for ( int i = 1; i <= 6; i++ )
			{
				for ( int j = 1; j <= 6; j++ )
				{
					if ( distance == i || distance == j )
					{
						if ( board[0] == 0 )
						{
							hittingDices[i-1,j-1] = true;
						}
						else if ( board[0] < 0 && hitFrom == 0 )
						{
							hittingDices[i-1,j-1] = true;
						}
						else if ( board[0] == -1 && hitFrom != 0 )
						{
							if ( distance == i && board[j] < 2 )
							{
								hittingDices[i-1,j-1] = true;
							}
							else if ( distance == j && board[i] < 2 )
							{
								hittingDices[i-1,j-1] = true;
							}
						}
						else if ( i == j && ( board[0] == -1 || board[0] == -2 || board[0] == -3 ) && board[i] < 2 )
						{
							hittingDices[i-1,j-1] = true;
						}
					}
					else if ( distance == i + j )
					{
						if ( board[0] == 0 )
						{
							if ( board[hitFrom + i] < 2 || board[hitFrom + j] < 2 )
								hittingDices[i-1,j-1] = true;
						}
						else if ( board[0] == -1 && hitFrom == 0 )
						{
							if ( board[hitFrom + i] < 2 || board[hitFrom + j] < 2 )
								hittingDices[i-1,j-1] = true;
						}
						else if ( i == j && ( board[0] == -1 || board[0] == -2 ) && board[i] < 2 )
						{
							if ( board[hitFrom + i] < 2 )
								hittingDices[i-1,j-1] = true;
						}
						else if ( i == j && board[0] == -3 && hitFrom == 0 && board[i] < 2 )
						{
							hittingDices[i-1,j-1] = true;
						}
					}
					else if ( i == j && ( distance == 3 * i || distance == 4 * i ) )
					{
						if ( board[0] == 0 )
						{
							if ( distance == 3 * i )
							{
								if ( board[hitFrom + i] < 2 && board[hitFrom + ( i * 2 )] < 2 )
									hittingDices[i-1,j-1] = true;
							}
							if ( distance == 4 * i )
							{
								if ( board[hitFrom + i] < 2 && board[hitFrom + ( i * 2 )] < 2 && board[hitFrom + ( i * 3 )] < 2 )
									hittingDices[i-1,j-1] = true;
							}
						}
						else if ( ( board[0] == -1 && hitFrom == 0 ) )
						{
							if ( distance == 3 * i )
							{
								if ( board[hitFrom + i] < 2 && board[hitFrom + ( i * 2 )] < 2 )
									hittingDices[i-1,j-1] = true;
							}
							if ( distance == 4 * i )
							{
								if ( board[hitFrom + i] < 2 && board[hitFrom + ( i * 2 )] < 2 && board[hitFrom + ( i * 3 )] < 2 )
									hittingDices[i-1,j-1] = true;
							}
						}
						else if ( ( board[0] == -2 && hitFrom == 0 ) )
						{
							if ( distance == 3 * i )
							{
								if ( board[hitFrom + i] < 2 && board[hitFrom + ( i * 2 )] < 2 )
									hittingDices[i-1,j-1] = true;
							}
						}
						else if ( board[0] == -1 && hitFrom != 0 && board[i] < 2 )
						{
							if ( distance == 3 * i )
							{
								if ( board[hitFrom + i] < 2 && board[hitFrom + ( i * 2 )] < 2 )
									hittingDices[i-1,j-1] = true;
							}
						}
					}
				}
			}
		}


		private bool DifCheckersDifPosDoubleHits(int d1, int d2, int blot1, int blot2, int[] board )
		{
			if ( d1 != d2 )
			{
				if ( blot1 - d1 >= 0 && blot2 - d2 >= 0 && blot1 - d1 != blot2 - d2 )
					if ( board[blot1 - d1] < 0 && board[blot2 - d2] < 0 )
						if ( board[0] == 0 )
							return true;
						else if ( board[0] == -1 && ( blot1 - d1 == 0 || blot2 - d2 == 0 ) )
							return true;

				if ( blot1 - d2 >= 0 && blot2 - d1 >= 0 && blot1 - d2 != blot2 - d1 )
					if ( board[blot1 - d2] < 0 && board[blot2 - d1] < 0 )
						if ( board[0] == 0 )
							return true;
						else if ( board[0] == -1 && ( blot1 - d2 == 0 || blot2 - d1 == 0 ) )
							return true;
			}
			else
			{
				for ( int i = 1; i <= 3; i++ )
				{
					if ( blot1 - (d1*i) >= 0 && board[blot1 - (d1*i)] < 0 )
					{
						for ( int j = 1; j <= 4-i; j++ )
						{
							if ( blot2 - (d2*i) >= 0 && board[blot2 - (d2*i)] < 0 )
							{
								if ( board[0] == 0 )
									return true;
								else if ( board[0] == -1 && ( blot1 - (d1*i) == 0 || blot2 - (d2*i) == 0 ) )
									return true;
							}
						}
					}
				}

				for ( int i = 1; i <= 3; i++ )
				{
					if ( blot1 - (d2*i) >= 0 && board[blot1 - (d2*i)] < 0 )
					{
						for ( int j = 1; j <= 4-i; j++ )
						{
							if ( blot2 - (d1*i) >= 0 && board[blot2 - (d1*i)] < 0 )
							{
								if ( board[0] == 0 )
									return true;
								else if ( board[0] == -1 && ( blot1 - (d2*i) == 0 || blot2 - (d1*i) == 0 ) )
									return true;
							}
						}
					}
				}
			}
		
			return false;
		}


		private bool SingleCheckerDoubleHits( int d1, int d2, int hitter, int[] board )
		{
			if ( d1 == d2 )
			{
				if ( board[0] == 0 || ( board[0] < 0 && board[d1] < 2 ) )
				{
					int loopMax = 4 + board[0];
					loopMax += ( hitter == 0 )? 1 : 0;

					int numHits = 0;
					for ( int i = 1; i <= loopMax; i++ )
					{
						if ( hitter + ( d1 * i ) > 24 || board[hitter + ( d1 * i )] == 2 )
							break;
						else if ( board[hitter + ( d1 * i )] == 1 )
							numHits++;
					}
					return numHits > 1;
				}
			}
			else if ( hitter + d1 < 25 && hitter + d2 < 25 && hitter + d1 + d2 < 25 )
			{
				if ( board[0] == 0 || ( board[0] == -1 && hitter == 0 ) )
					if ( ( board[hitter + d1] == 1 || board[hitter + d2] == 1 ) && board[hitter + d1 + d2] == 1 )
						return true;
			}
			return false;
		}


		private bool SinglePosDoubleHits( int d1, int d2, int hitters, int[] board )
		{
			if ( d1 != d2 && hitters + d1 < 25 && hitters + d2 < 25 )
				if ( board[0] == 0 || hitters == 0 )
					if ( board[hitters] < -1 && board[hitters + d1] == 1 && board[hitters + d2] == 1 )
						return true;

			return false;
		}
	}
}
