using System;
using System.Collections;


namespace GammonGame
{
	public class GameHistory
	{
		private ArrayList		m_List;
		private BgPlayer		m_Winner;
		private int				m_WinType;


		public GameHistory()
		{
			m_List = new ArrayList();
			m_Winner = BgPlayer.None;
			m_WinType = 0;
		}


		public void Add( BoardRepresentation rep )
		{
			m_List.Add( rep );
		}

		public BoardRepresentation PeekFirst()
		{
			return Peek( 0 );
		}


		public BoardRepresentation PeekLast()
		{
			return Peek( m_List.Count-1 );
		}


		public BoardRepresentation RemoveFirst()
		{
			return Remove( 0 );
		}


		public BoardRepresentation RemoveLast()
		{
			return Remove( m_List.Count-1 );
		}


		public BoardRepresentation Peek( int idx )
		{
			if ( m_List.Count > idx )
				return ( BoardRepresentation )m_List[idx];
			else
				return null;
		}


		public BoardRepresentation Remove( int idx )
		{
			if ( m_List.Count > idx )
			{
				BoardRepresentation result = Peek( idx );
				m_List.RemoveAt( idx );
				return result;
			}
			else
			{
				return null;
			}
		}


		public int Count()
		{
			return m_List.Count;
		}
	

		public void Clear()
		{
			m_List.Clear();
			m_Winner = BgPlayer.None;
		}


		public BgPlayer Winner
		{
			get {return m_Winner;}
			set { m_Winner = value;}
		}


		public int WinType
		{
			get {return m_WinType;}
			set { m_WinType = value;}
		}
	}
}
