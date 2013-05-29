using System;


namespace GammonGame
{
	public class GammonInterface
	{
		private BgBoard			m_Board;
		private int[]			m_Moves;
		private int				m_LastUsedMoveId;
		private BgDiceCup		m_BgDiceCup;
		private Cube			m_Cube;
		private UndoStack		m_UndoStack;
		private GameHistory		m_History;
		private int				m_MaxMovesPossible;

		private bool			m_SwitchedBoard;
		private BgPlayer		m_CurrentPlayer;
		private BgPlayer		m_Winner;

		private bool			m_MaxFivePiecesOnAPoint;


		public GammonInterface( bool nackgammon, bool maxFivePiecesOnPoint )
		{
			m_Board = new BgBoard( nackgammon );
			m_BgDiceCup = new BgDiceCup();
			m_Moves = new int[4];
			m_Cube = new Cube();
			m_UndoStack = new UndoStack();
			m_History = new GameHistory();

			NewGame( nackgammon, maxFivePiecesOnPoint );
		}


		public BgPlayer Winner()
		{
			return m_Winner;
		}


		public void SetWinner( BgPlayer w )
		{
			m_Winner = w;
		}


		public int VictoryType()
		{
			if ( m_Winner == BgPlayer.None )
				return 0;

			if ( m_Winner != BgPlayer.None && m_Board.GetOffBoardSquare( CurrentPlayer ).Count != 15 && m_Board.GetOffBoardSquare( CurrentOpponentPlayer ).Count != 15 )
				return 1;

			if ( m_Board.GetOffBoardSquare( CurrentOpponentPlayer ).Count == 0 )
			{			
				for ( int i = 0; i < 7; i++ )
					if ( m_Board.GetSquare( i, m_SwitchedBoard ).Owner == CurrentOpponentPlayer )
						return 3;

				return 2;
			}
			return 1;
		}


		public bool MakeMove( int from, int to )
		{
			if ( ValidateMove( from, to ) )
				return MakeUnvalidatedMove( from, to );

			return false;
		}


		private bool MakeUnvalidatedMove( int from, int to )
		{
			if ( to == 26 )
			{
				m_Board.GetSquare( from, m_SwitchedBoard ).RemovePiece();
				m_Board.GetOffBoardSquare( m_CurrentPlayer ).PutPiece( m_CurrentPlayer );
				
				if ( !UseMove( from ) )
					m_UndoStack.PushMove( from, to, UseHighestMove(), false, m_LastUsedMoveId );
				else
					m_UndoStack.PushMove( from, to, from, false, m_LastUsedMoveId );

				return true;
			}
			else if ( to < 26 )
			{
				bool opponentHit = false;
				if ( m_Board.GetSquare( to, m_SwitchedBoard ).Count == 1 && m_Board.GetSquare( to, m_SwitchedBoard ).Owner != m_CurrentPlayer )
				{
					m_Board.GetSquare( to, m_SwitchedBoard ).RemovePiece();
					m_Board.GetSquare( 0, m_SwitchedBoard ).PutPiece( CurrentOpponentPlayer );
					opponentHit = true;
				}

				m_Board.GetSquare( from, m_SwitchedBoard ).RemovePiece();
				m_Board.GetSquare( to, m_SwitchedBoard ).PutPiece( m_CurrentPlayer );
				UseMove( from - to );
				m_UndoStack.PushMove( from, to, from - to, opponentHit, m_LastUsedMoveId );
				return true;
			}
			return false;
		}


		public void Undo()
		{
			if ( !m_UndoStack.Empty() )
			{
				UndoMove move = m_UndoStack.PopMove();

				if ( move.To > 25 )
					m_Board.GetOffBoardSquare( m_CurrentPlayer ).RemovePiece();
				else
					m_Board.GetSquare( move.To, m_SwitchedBoard ).RemovePiece();

				m_Board.GetSquare( move.From, m_SwitchedBoard ).PutPiece( m_CurrentPlayer );

				if ( move.HitOpponent )
				{
					m_Board.GetSquare( 0, m_SwitchedBoard ).RemovePiece();
					m_Board.GetSquare( move.To, m_SwitchedBoard ).PutPiece( CurrentOpponentPlayer );
				}

				m_Moves[move.MoveId] = move.Value;
			}
		}


		public void NewGame( bool nackgammon, bool maxFivePiecesOnPoint, int seed )
		{
			m_BgDiceCup.SetSeed( seed );
			_NewGame( nackgammon, maxFivePiecesOnPoint );
		}

		public void NewGame( bool nackgammon, bool maxFivePiecesOnPoint )
		{
			m_BgDiceCup.SetRandomSeed();
			_NewGame( nackgammon, maxFivePiecesOnPoint );
		}


		private void _NewGame( bool nackgammon, bool maxFivePiecesOnPoint )
		{
			m_MaxFivePiecesOnAPoint = maxFivePiecesOnPoint;
			m_Board.SetStartLayout( nackgammon );
			m_Cube.Reset();
			m_UndoStack.Clear();
			m_Winner = BgPlayer.None;
			m_History.Clear();

			do
			{
				RollDices();
			}
			while ( m_BgDiceCup.GetDice( 0 ).Value == m_BgDiceCup.GetDice( 1 ).Value );

			if ( m_Moves[0] > m_Moves[1] )
				m_CurrentPlayer = BgPlayer.Dark;
			else
				m_CurrentPlayer = BgPlayer.Light;

			SetCorrectlySwitchedBoard();
			m_MaxMovesPossible = MaxMovesPossible( 0 );
		}


		public bool EndTurn()
		{
			FindWinner();
			if ( m_Winner != BgPlayer.None )
			{
				BoardRepresentation finalBoard = new BoardRepresentation( this );
				if ( m_History.Count() > 0 && !m_History.PeekLast().PatternMatches( finalBoard ) )
				{
					m_History.Add( finalBoard );
					m_History.Winner = m_Winner;
					m_History.WinType = VictoryType();
				}
				return false;
			}

			if ( !IsMovesUsed() )
				if ( DoesValidMoveExist() )
					return false;

			m_History.Add( new BoardRepresentation( this ) );
			m_UndoStack.Clear();
			m_CurrentPlayer = CurrentOpponentPlayer;
			SetCorrectlySwitchedBoard();
			RollDices();
			m_MaxMovesPossible = MaxMovesPossible( 0 );
			return true;
		}


		public bool IsTurnOver()
		{
			if ( !IsMovesUsed() )
				if ( DoesValidMoveExist() )
					return false;
		
			return true;
		}


		public bool ValidateMove( int from, int to )
		{
			if ( ValidateSingleMove( from, to ) )
			{
				int maxMovesBefore = m_MaxMovesPossible - m_UndoStack.Size();
			
				if ( to != 26 && m_Moves[0] != 0 && m_Moves[1] != 0 && m_Moves[0] != m_Moves[1] && maxMovesBefore == 1 && ( from - to ) != GetHighestMove() )
					if ( DoesValidRegularMoveExist( GetHighestMove() ) )
						return false;

				MakeUnvalidatedMove( from, to );
				int maxMovesAfter = MaxMovesPossible( 1 );
				Undo();
				if ( maxMovesBefore == maxMovesAfter + 1 )
					return true;
				else
					return false;
			}
			else
			{
				return false;
			}
		}


		private bool DoesValidRegularMoveExist( int move )
		{
			for ( int i = m_Board.SquareCount - 1; i > 0; i-- )
			{
				if ( m_Board.GetSquare( i, m_SwitchedBoard ).Owner == m_CurrentPlayer )
					if ( ValidateSingleMove( i, i - move ) )
						return true;
			}
			return false;
		}


		private int MaxMovesPossible( int depth )
		{
			int result = 0;

			if ( m_BgDiceCup.GetDice( 0 ).Value != m_BgDiceCup.GetDice( 1 ).Value && depth == 2 )
				return result;
			if ( m_BgDiceCup.GetDice( 0 ).Value == m_BgDiceCup.GetDice( 1 ).Value && depth == 4 )
				return result;

			for ( int i = m_Board.SquareCount - 1; i > 0; i-- )
			{
				if ( m_Board.GetSquare( i, m_SwitchedBoard ).Owner == m_CurrentPlayer )
				{
					for ( int j = 0; j < m_Moves.Length; j++ )
					{
						if ( ValidateSingleMove( i, i - m_Moves[j] ) )
						{
							MakeUnvalidatedMove( i, i - m_Moves[j] );
							int r = 1 + MaxMovesPossible( depth + 1 );
							Undo();
							if ( r > result )
								result = r;
							if ( m_BgDiceCup.GetDice( 0 ).Value != m_BgDiceCup.GetDice( 1 ).Value && result == ( 2 - depth ) )
								return result;
							if ( m_BgDiceCup.GetDice( 0 ).Value == m_BgDiceCup.GetDice( 1 ).Value && result == ( 4 - depth ) )
								return result;
						}
					}
					if ( i <= 6 && ValidateSingleMove( i, 26 ) )
					{
						MakeUnvalidatedMove( i, 26 );
						int r = 1 + MaxMovesPossible( depth + 1 );
						Undo();
						if ( r > result )
							result = r;
						if ( m_BgDiceCup.GetDice( 0 ).Value != m_BgDiceCup.GetDice( 1 ).Value && result == ( 2 - depth ) )
							return result;
						if ( m_BgDiceCup.GetDice( 0 ).Value == m_BgDiceCup.GetDice( 1 ).Value && result == ( 4 - depth ) )
							return result;
					}
				}
			}
			return result;
		}


		private bool ValidateSingleMove( int from, int to )
		{
			if ( m_Winner != BgPlayer.None )
				return false;

			//trying to take piece from invalid squares
			if ( from > 25 )
				return false;

			//trying to take piece from invalid squares
			if ( from < 1 )
				return false;

			//trying to place piece on invalid squares
			if ( to > 26 || to == 25)
				return false;

			//trying to place piece on invalid squares
			if ( to < 1 )
				return false;

			//No more moves Avalible
			if ( m_Moves[0] == 0 && m_Moves[1] == 0 && m_Moves[2] == 0 && m_Moves[3] == 0 )
				return false;

			//Having pieces on bar, and bar square is not the 'from' square
			if ( m_Board.GetSquare( 25, m_SwitchedBoard ).Owner == m_CurrentPlayer && from != 25 )
				return false;

			//'From' square does not hold a pice owned by current player
			if ( m_Board.GetSquare( from, m_SwitchedBoard ).Owner != m_CurrentPlayer )
				return false;			
			
			if ( to < 26 )	//not removing pieces from the board
			{
				//Move does not exist
				if ( !ExistsMove( from - to ) )
					return false;

				//'To' square is blocked by opponent
				if ( m_Board.GetSquare( to, m_SwitchedBoard ).Owner != m_CurrentPlayer && m_Board.GetSquare( to, m_SwitchedBoard ).Count > 1 )
					return false;

				//Max five pieces is allowed on a location
				if ( m_MaxFivePiecesOnAPoint && m_Board.GetSquare( to, m_SwitchedBoard ).Owner == m_CurrentPlayer && m_Board.GetSquare( to, m_SwitchedBoard ).Count >= 5 )
					return false;

				return true;
			}
			else			//removing pieces from the board
			{
				//All pieces is not home
				for ( int i = 7; i < m_Board.SquareCount; i++ )
				{
					if ( m_Board.GetSquare( i, m_SwitchedBoard ).Owner == CurrentPlayer )
						return false;
				}
				
				//Not using exact dice value and pices exist behind 'from' square
				if ( CurrentPlayerHavingHomePiecesBehind( from ) && !ExistsMove( from ) )
					return false;

				//High enough value does not exist for move to be valid
				if ( from > GetHighestMove() )
					return false;

				return true;
			}
		}


		public GameHistory GetHistory()
		{
			return m_History;
		}

		
		public void DoubleCube( BgPlayer player )
		{
			if ( !IsMovesUsed() )
				if ( DoesValidMoveExist() )
					return;

			if ( m_Cube.Owner == BgPlayer.None || player == m_Cube.Owner )
			{
				m_Cube.Double( player );
				m_Cube.Owner = m_CurrentPlayer;
			}
		}


		private bool IsMovesUsed()
		{
			for ( int i = 0; i < m_Moves.Length; i++ )
			{
				if ( m_Moves[i] != 0 )
					return false;
			}
			return true;
		}


		private bool DoesValidMoveExist()
		{
			for ( int i = m_Board.SquareCount - 1; i > 0; i-- )
			{
				if ( m_Board.GetSquare( i, m_SwitchedBoard ).Owner == m_CurrentPlayer )
				{
					for ( int j = 0; j < m_Moves.Length; j++ )
					{
						if ( ValidateSingleMove( i, i - m_Moves[j] ) )
							return true;
					}
					if ( i <= 6 && ValidateSingleMove( i, 26 ) )
						return true;
				}
			}
			return false;
		}


		private bool CurrentPlayerHavingHomePiecesBehind( int square )
		{
			for ( int i = square + 1; i <= 6; i++ )
			{
				if ( m_Board.GetSquare( i, m_SwitchedBoard ).Owner == CurrentPlayer )
					return true;
			}
			return false;
		}


		public BgPlayer CurrentPlayer
		{
			get
			{
				return m_CurrentPlayer;
			}
		}
		

		public BgPlayer CurrentOpponentPlayer
		{
			get
			{
				if ( m_CurrentPlayer == BgPlayer.Light )
					return BgPlayer.Dark;
				else if ( m_CurrentPlayer == BgPlayer.Dark )
					return BgPlayer.Light;
				else
					return BgPlayer.None;
			}
		}


		private void FindWinner()
		{
			if ( m_Board.GetOffBoardSquare( BgPlayer.Dark ).Count == 15 )
				m_Winner = BgPlayer.Dark;
			else if ( m_Board.GetOffBoardSquare( BgPlayer.Light ).Count == 15 )
				m_Winner = BgPlayer.Light;
		}


		private void SetCorrectlySwitchedBoard()
		{
			if ( m_CurrentPlayer == BgPlayer.Light )
				m_SwitchedBoard = true;
			else
				m_SwitchedBoard = false;
		}


		public int BoardSquareCount
		{
			get
			{
				return m_Board.SquareCount;
			}
		}


		public BgPlayer SquareOwner( int square )
		{
			return m_Board.GetSquare( square, m_SwitchedBoard ).Owner;
		}


		public int SquareCheckerCount( int square )
		{
			return m_Board.GetSquare( square, m_SwitchedBoard ).Count;
		}


		public int PiecesRemoved( BgPlayer player )
		{
			return m_Board.GetOffBoardSquare( player ).Count;
		}


		private void RollDices()
		{
			m_BgDiceCup.Roll();
			DiceRollsToMoves();
		}


		public void SetDiceValues( int val1, int val2 )
		{
			if ( m_UndoStack.Empty() )
			{
				m_BgDiceCup.GetDice( 0 ).Value = val1;
				m_BgDiceCup.GetDice( 1 ).Value = val2;
				DiceRollsToMoves();
				m_MaxMovesPossible = MaxMovesPossible( 0 );
			}
		}


		public void SetNextDiceValues( int val1, int val2 )
		{
			m_BgDiceCup.GetDice( 0 ).SetNextRoll( val1 );
			m_BgDiceCup.GetDice( 1 ).SetNextRoll( val2 );
		}


		private void DiceRollsToMoves()
		{
			if ( m_BgDiceCup.GetDice( 0 ).Value == m_BgDiceCup.GetDice( 1 ).Value )
			{
				for ( int i = 0; i < m_Moves.Length; i++ )
					m_Moves[i] = m_BgDiceCup.GetDice( 0 ).Value;
			}
			else
			{
				m_Moves[0] = m_BgDiceCup.GetDice( 0 ).Value;
				m_Moves[1] = m_BgDiceCup.GetDice( 1 ).Value;
				m_Moves[2] = 0;
				m_Moves[3] = 0;
			}		
		}
		
		public int GetDiceValue( int idx )
		{
			return m_BgDiceCup.GetDice( idx ).Value;
		}


		private bool ExistsMove( int val )
		{
			if ( val < 1 || val > 6 )
				return false;

			for ( int i = 0; i < m_Moves.Length; i++ )
			{
				if ( m_Moves[i] == val )
					return true;
			}
			return false;
		}


		private bool UseMove( int val )
		{
			for ( int i = 0; i < m_Moves.Length; i++ )
			{
				if ( m_Moves[i] == val )
				{
					m_Moves[i] = 0;
					m_LastUsedMoveId = i;
					return true;
				}
			}
			return false;
		}


		private int GetHighestMove()
		{
			int highestVal = 0;

			for ( int i = 0; i < m_Moves.Length; i++ )
			{
				if ( m_Moves[i] > highestVal )
					highestVal = m_Moves[i];
			}
			return highestVal;
		}


		private int UseHighestMove()
		{
			int result = GetHighestMove();
			UseMove( result );
			return result;
		}


		public BgPlayer GetCubeOwner()
		{
			return m_Cube.Owner;
		}


		public int GetCubeValue()
		{
			return m_Cube.Value;
		}


		public int GetMove( int idx )
		{
			return m_Moves[idx];
		}


		public int GetPips( BgPlayer player )
		{
			int result = 0;
			bool boardSwitch = ( player == m_CurrentPlayer )? m_SwitchedBoard : !m_SwitchedBoard;

			for ( int i = 0; i < m_Board.SquareCount; i++ )
			{
				if ( m_Board.GetSquare( i, boardSwitch ).Owner == player )
					result += i * m_Board.GetSquare( i, boardSwitch ).Count;
			}
			return result;
		}


		public bool PlayWithMaxFiveCheckersRule( bool playWithRule )
		{
			if ( playWithRule == false )
			{
				m_MaxFivePiecesOnAPoint = false;
				return false;
			}
			else
			{
				for ( int i = 1; i < m_Board.SquareCount - 1; i++ )
					if ( m_Board.GetSquare( i, m_SwitchedBoard ).Count > 5 )
						return false;

				m_MaxFivePiecesOnAPoint = true;
				return true;
			}
		}


		public int MovesUsed()
		{
			return m_UndoStack.Size();
		}


		public void EditBoard()
		{
			m_Cube.Reset();
			m_UndoStack.Clear();
			m_Winner = BgPlayer.None;
			m_History.Clear();
			m_Board.ClearBoard();
		}


		public void StopEditBoard()
		{
			m_MaxMovesPossible = MaxMovesPossible( 0 );
		}


		public void PutPiece( int square, BgPlayer player )
		{
			if ( square < 0 || square > m_Board.SquareCount - 1 )
				return;

			if ( player == CurrentPlayer && square == 0 )
				return;
			if ( player == CurrentOpponentPlayer && square == 25 )
				return;

			if ( ( m_Board.GetSquare( square, m_SwitchedBoard ).Owner == BgPlayer.None || m_Board.GetSquare( square, m_SwitchedBoard ).Owner == player ) && m_Board.GetOffBoardSquare( player ).Count > 0 )
			{
				if ( m_MaxFivePiecesOnAPoint && m_Board.GetSquare( square, m_SwitchedBoard ).Count >= 5 )
					return;

				m_Board.GetOffBoardSquare( player ).RemovePiece();
				m_Board.GetSquare( square, m_SwitchedBoard ).PutPiece( player );
			}
		}


		public void SwitchPlayer()
		{
			m_CurrentPlayer = CurrentOpponentPlayer;
			SetCorrectlySwitchedBoard();
		}
	}
}
