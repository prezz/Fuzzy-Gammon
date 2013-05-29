using System;

namespace GammonGame
{
	struct UndoMove
	{
		public int	From;
		public int	To;
		public int	Value;
		public bool	HitOpponent;
		public int	MoveId;
	}



	class UndoStack
	{
		private int			m_Top;
		private UndoMove[]	m_UndoMoves;
		

		public UndoStack()
		{
			m_Top = 0;
			m_UndoMoves = new UndoMove[4];
		}


		public void Clear()
		{
			m_Top = 0;
		}


		public bool Empty()
		{
			return ( m_Top == 0 );
		}


		public int Size()
		{
			return m_Top;
		}


		public void PushMove( int from, int to, int val, bool opponentHit, int moveId )
		{
			m_UndoMoves[m_Top].From = from;
			m_UndoMoves[m_Top].To = to;
			m_UndoMoves[m_Top].Value = val;
			m_UndoMoves[m_Top].HitOpponent = opponentHit;
			m_UndoMoves[m_Top].MoveId = moveId;
			m_Top++;
		}


		public UndoMove PopMove()
		{
			return m_UndoMoves[--m_Top];
		}
	}
}
