using System;

namespace GammonGame
{
	public class BoardRepresentation
	{
		protected int[]					m_BoardPattern;


		public BoardRepresentation( GammonInterface game )
		{
			if ( game != null )
				m_BoardPattern = BoardToNumberPattern( game );
		}


		public BoardRepresentation( int[] pattern )
		{
			m_BoardPattern = pattern;
		}


		public int SquareCount()
		{
			return m_BoardPattern.Length;
		}


		public int GetPiecesAt( int square )
		{
			return m_BoardPattern[square];
		}

		public int BearOffCountCurrent()
		{
			int piecesOnBoard = 0;
			for ( int i = 0; i < m_BoardPattern.Length; i++ )
			{
				if ( m_BoardPattern[i] > 0 )
					piecesOnBoard += m_BoardPattern[i];
			}
			return 15 - piecesOnBoard;
		}

		public int BearOffCountOpponent()
		{
			int piecesOnBoard = 0;
			for ( int i = 0; i < m_BoardPattern.Length; i++ )
			{
				if ( m_BoardPattern[i] < 0 )
					piecesOnBoard -= m_BoardPattern[i];
			}
			return -( 15 - piecesOnBoard );
		}

		public bool PatternMatches( BoardRepresentation repr )
		{
			int[] tmp = repr.m_BoardPattern;

			if ( tmp.Length != m_BoardPattern.Length )
				return false;

			for ( int i = 0; i < m_BoardPattern.Length; i++ )
				if ( m_BoardPattern[i] != tmp[i] )
					return false;

			return true;
		}


		protected int[] BoardToNumberPattern( GammonInterface game )
		{
			int[] result = new int[26];

			for ( int i = 0; i < game.BoardSquareCount; i++ )
			{
				if ( game.SquareOwner( i ) == game.CurrentPlayer )
					result[i] = game.SquareCheckerCount( i );
				else if ( game.SquareOwner( i ) == game.CurrentOpponentPlayer )
					result[i] = -( game.SquareCheckerCount( i ) );
				else
					result[i] = 0;
			}
			return result;		
		}
	}
}
