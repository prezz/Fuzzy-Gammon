using System;

namespace GammonGame
{
	class Cube
	{
		private int			m_Value;
		private BgPlayer	m_Owner;


		public Cube()
		{
			Reset();
		}


		public void Reset()
		{
			m_Value = 1;
			m_Owner = BgPlayer.None;
		}


		public void Double( BgPlayer player )
		{
			if ( m_Owner == BgPlayer.None || m_Owner == player )
				m_Value *= 2;
		}


		public int Value
		{
			get
			{
				return m_Value;
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
				m_Owner = value;
			}
		
		}
	}
}
