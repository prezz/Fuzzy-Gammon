using System;
using System.Collections;
using GammonGame;


namespace GammonAgent
{
	public class BgMove
	{
		public int		From;
		public int		To;
		public BgMove	FirstMove;
		public BgMove	NextMove;


		public BgMove( int from, int to, BgMove precedingMove )
		{
			From = from;
			To = to;
			NextMove = null;

			if ( precedingMove == null )
			{
				FirstMove = this;
			}
			else
			{
				precedingMove.NextMove = this;
				FirstMove = precedingMove.FirstMove;
			}
		}


		private BgMove()
		{
			From = 0;
			To = 0;
			FirstMove = null;
			NextMove = null;
		}


		public static BgMove CloneFromFirst( BgMove move )
		{
			if ( !BgMove.ReferenceEquals( move, move.FirstMove ) )
				return null;

			BgMove result = new BgMove();

			result.From = move.From;
			result.To = move.To;
			result.FirstMove = result;
			result.NextMove = CloneSubMove( result, move.NextMove );
		
			return result;
		}


		private static BgMove CloneSubMove( BgMove precedingMove, BgMove cloneFrom )
		{
			if ( cloneFrom == null )
				return null;

			BgMove result = new BgMove();

			result.From = cloneFrom.From;
			result.To = cloneFrom.To;
			result.FirstMove = precedingMove.FirstMove;
			result.NextMove = CloneSubMove( result, cloneFrom.NextMove );

			return result;
		}
	}



	class BgMoveList
	{
		private ArrayList	m_List;


		public BgMoveList()
		{
			m_List = new ArrayList();
		}


		public void AddBgMove( BgMove move )
		{
			m_List.Add( move );
		}
	

		public BgMove GetBgMove( int idx )
		{
			return ( BgMove )m_List[idx];
		}


		public int Count()
		{
			return m_List.Count;
		}


		public void Clear()
		{
			m_List.Clear();
		}
	}



	class BgMoveGenerator
	{
		private GammonInterface		m_BgGame;


		public BgMoveGenerator( GammonInterface game )
		{
			m_BgGame = game;
		}


		public void GenerateMoves( BgMoveList moveList )
		{
			moveList.Clear();
			GenerateMoves( moveList, null );
		}


		private void GenerateMoves( BgMoveList moveList, BgMove prevMove )
		{
			BgMoveList singleMoves = FindAllSingleMoves();

			if ( singleMoves.Count() > 0 )
			{
				for ( int i = 0; i < singleMoves.Count(); i++ )
				{
					BgMove currentMove = new BgMove( singleMoves.GetBgMove( i ).From, singleMoves.GetBgMove( i ).To, prevMove );

					m_BgGame.MakeMove( currentMove.From, currentMove.To );
					GenerateMoves( moveList, currentMove );

					m_BgGame.Undo();
				}
			}
			else
			{
				if ( prevMove != null )
					moveList.AddBgMove( BgMove.CloneFromFirst( prevMove.FirstMove ) );
			}
		}


		private BgMoveList FindAllSingleMoves()
		{
			BgMoveList result = new BgMoveList();

			int prevMoveVal = 0;
			for ( int i = 0; i < 4; i++ )
			{
				int moveVal = m_BgGame.GetMove( i );
				if ( moveVal != 0 && moveVal != prevMoveVal )
					FindSingleMoves( result, moveVal );
				prevMoveVal = moveVal;
			}

			return result;
		}		


		private void FindSingleMoves( BgMoveList moveList, int val )
		{
			for ( int i = m_BgGame.BoardSquareCount - 1; i > 0; i-- )
			{
				if ( m_BgGame.SquareOwner( i ) == m_BgGame.CurrentPlayer )
				{
					if ( m_BgGame.ValidateMove( i, i - val ) )
					{
						BgMove move = new BgMove( i, i - val, null );
						moveList.AddBgMove( move );
					}
				}
				if ( i <= 6 && m_BgGame.ValidateMove( i, 26 ) )
				{
					BgMove move = new BgMove( i, 26, null );
					moveList.AddBgMove( move );
				}
			}
		}
	}
}