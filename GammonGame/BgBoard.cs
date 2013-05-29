using System;

namespace GammonGame
{
	public enum BgPlayer { None, Dark, Light };



	class BgSquare
	{
		private BgPlayer	m_Owner;
		private int			m_Count;


		public BgSquare()
		{
			m_Owner = BgPlayer.None;
			m_Count = 0;
		}


		public BgSquare( BgPlayer owner, int count ):this()
		{
			if ( owner != BgPlayer.None && count > 0 )
			{
				m_Owner = owner;
				m_Count = count;
			}
		}


		public BgPlayer Owner
		{
			get
			{
				return m_Owner;
			}

			set
			{
				if ( value == BgPlayer.None )
					m_Count = 0;
				else if ( value != BgPlayer.None && m_Count == 0 )
					m_Count = 1;

				m_Owner = value;
			}
		}


		public int Count
		{
			get
			{
				return m_Count;
			}

			set
			{
				if ( value < 1 )
				{
					m_Owner = BgPlayer.None;
					m_Count = 0;
				}
				else
				{
					m_Count = value;
				}
			}
		}


		public bool PutPiece( BgPlayer piece )
		{
			if ( m_Owner == BgPlayer.None )
			{
				m_Owner = piece;
				m_Count = 1;
				return true;
			}
			else if ( m_Owner == piece )
			{
				m_Count++;
				return true;
			}
			else
			{
				return false;
			}
		}


		public BgPlayer RemovePiece()
		{
			if ( m_Count > 0 )
			{
				BgPlayer result = m_Owner;
				m_Count--;
				if ( m_Count == 0 )
					m_Owner = BgPlayer.None;
				return result;
			}
			else
			{
				return BgPlayer.None;
			}
		}
	}



	class BgBoard
	{
		private BgSquare[]		m_Squares;
		private BgSquare		m_DarkOffBoardSquare;
		private BgSquare		m_LightOffBoardSquare;


		public BgBoard( bool nackgammon )
		{
			m_DarkOffBoardSquare = new BgSquare();
			m_LightOffBoardSquare = new BgSquare();

			m_Squares = new BgSquare[26];
			for ( int i = 0; i < m_Squares.Length; i++ )
				m_Squares[i] = new BgSquare();
			
			SetStartLayout( nackgammon );
		}


		public void ClearBoard()
		{
			m_DarkOffBoardSquare.Owner = BgPlayer.Dark;
			m_DarkOffBoardSquare.Count = 15;

			m_LightOffBoardSquare.Owner = BgPlayer.Light;
			m_LightOffBoardSquare.Count = 15;
			
			for ( int i = 0; i < m_Squares.Length; i++ )
				m_Squares[i].Owner = BgPlayer.None;
		}


		public void SetStartLayout( bool nackgammonSetup )
		{
			ClearBoard();

			m_DarkOffBoardSquare.Owner = BgPlayer.None;
			m_LightOffBoardSquare.Owner = BgPlayer.None;

			m_Squares[6].Owner = BgPlayer.Dark;
			m_Squares[6].Count = 5;
			m_Squares[8].Owner = BgPlayer.Dark;
			m_Squares[8].Count = 3;
			m_Squares[13].Owner = BgPlayer.Dark;
			m_Squares[13].Count = 5;
			m_Squares[24].Owner = BgPlayer.Dark;
			m_Squares[24].Count = 2;

			m_Squares[1].Owner = BgPlayer.Light;
			m_Squares[1].Count = 2;
			m_Squares[12].Owner = BgPlayer.Light;
			m_Squares[12].Count = 5;
			m_Squares[17].Owner = BgPlayer.Light;
			m_Squares[17].Count = 3;
			m_Squares[19].Owner = BgPlayer.Light;
			m_Squares[19].Count = 5;

			if ( nackgammonSetup )
			{
				m_Squares[6].Count = 4;
				m_Squares[12].Count = 4;
				m_Squares[13].Count = 4;
				m_Squares[19].Count = 4;
				m_Squares[2].Owner = BgPlayer.Light;
				m_Squares[2].Count = 2;
				m_Squares[23].Owner = BgPlayer.Dark;
				m_Squares[23].Count = 2;
			}


/*
			m_Squares[1].Owner = BgPlayer.Light;
			m_Squares[1].Count = 2;

			m_DarkOffBoardSquare.Owner = BgPlayer.Dark;
			m_DarkOffBoardSquare.Count = 8;

			m_Squares[2].Owner = BgPlayer.Dark;
			m_Squares[2].Count = 4;			
			m_Squares[3].Owner = BgPlayer.Dark;
			m_Squares[3].Count = 3;

			m_Squares[19].Owner = BgPlayer.Light;
			m_Squares[19].Count = 2;
			m_Squares[20].Owner = BgPlayer.Light;
			m_Squares[20].Count = 2;
			m_Squares[21].Owner = BgPlayer.Light;
			m_Squares[21].Count = 2;
			m_Squares[22].Owner = BgPlayer.Light;
			m_Squares[22].Count = 2;
			m_Squares[23].Owner = BgPlayer.Light;
			m_Squares[23].Count = 3;
			m_Squares[24].Owner = BgPlayer.Light;
			m_Squares[24].Count = 2;
*/
		}


		public int SquareCount
		{
			get
			{
				return m_Squares.Length;
			}
		}


		public BgSquare GetSquare( int idx, bool switched )
		{
			int index = (switched)? m_Squares.Length - 1 - idx : idx;
			return m_Squares[index];
		}


		public BgSquare GetOffBoardSquare( BgPlayer player )
		{
			if ( player == BgPlayer.Light )
				return m_LightOffBoardSquare;
			else if ( player == BgPlayer.Dark )
				return m_DarkOffBoardSquare;
			else
				return null;
		}
	}
}
