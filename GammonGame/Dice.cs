using System;

namespace GammonGame
{
	class Dice
	{
		public static Random	m_Random = null;
		private int				m_Value;
		private int				m_NextVal;


		public Dice()
		{
			m_NextVal = 0;
			Roll();
		}


		public void SetNextRoll( int val )
		{
			if ( val > 0 && val < 7 )
				m_NextVal = val;
		}


		public void Roll()
		{
			if ( m_NextVal != 0 )
			{
				m_Value = m_NextVal;
				m_NextVal = 0;
			}
			else
			{
				m_Value = m_Random.Next( 1, 7 );
			}
		}


		public int Value
		{
			get 
			{
				return m_Value;
			}

			set 
			{
				if ( value >= 1 && value <= 6 )
					m_Value = value;
			}
		}
	}



	class BgDiceCup
	{
		private Random	m_NewRanomizerDecider;
		private Dice[]	m_Dices;
		private bool	m_UsingSeed;


		public BgDiceCup()
		{
			m_NewRanomizerDecider = new Random();

			Dice.m_Random = new Random();
			m_UsingSeed = false;

			m_Dices = new Dice[2];
			m_Dices[0] = new Dice();
			m_Dices[1] = new Dice();

			Roll();
		}


		public void SetSeed( int seed )
		{
			Dice.m_Random = new Random( seed );
			m_UsingSeed = true;
		}


		public void SetRandomSeed()
		{
			if ( m_UsingSeed )
			{
				Dice.m_Random = new Random();
				m_UsingSeed = false;
			}
		}


		public bool UsingSeed
		{
			get{ return m_UsingSeed; }
		}


		public void Roll()
		{
			if ( !m_UsingSeed )
				if ( m_NewRanomizerDecider.Next( 50 ) == 25 )
					Dice.m_Random = new Random();

			for ( int i = 0; i < m_Dices.Length; i++ )
				m_Dices[i].Roll();
		}


		public Dice GetDice( int idx )
		{
			return m_Dices[idx];
		}
	}
}
