using System;

namespace FuzzevalModule
{
	interface MfInterface
	{
		void IncreaseMembership( double adjustment );
		void DecreaseMembership( double adjustment );
		void AdjustToTargetPoint( double point, double rate );
		double FuzzifieValue( int val );
		string VariableName();
		string TermName();
		double BaseLeft();
		double CenterMax();
		double BaseRight();
		void Lock( bool locked );
		bool IsLocked();
		bool SaveAble();
	}



	class MembershipFunction : MfInterface
	{
		private string	m_VariableName;
		private string	m_TermName;
		private double	m_BaseLeft;
		private double	m_CenterMax;
		private double	m_BaseRight;
		private bool	m_Locked;


		public MembershipFunction( string variableName, string termName, double baseLeft, double centerMax, double baseRight )
		{
			m_Locked = false;

			m_VariableName = variableName;
			m_TermName = termName;

			if ( baseLeft <= centerMax && centerMax <= baseRight )
			{
				m_BaseLeft = baseLeft;
				m_CenterMax = centerMax;
				m_BaseRight = baseRight;
			}
			else
			{
				m_BaseLeft = 0.0;
				m_CenterMax = 0.0;
				m_BaseRight = 0.0;
			}
		}

		public void IncreaseMembership( double adjustment )
		{
			if ( !m_Locked )
			{
				if ( m_BaseLeft == m_CenterMax )
				{
					m_BaseRight += adjustment;
				}
				else if ( m_CenterMax == m_BaseRight )
				{
					m_BaseLeft -= adjustment;
				}
				else
				{
					m_BaseRight += adjustment;
					m_BaseLeft -= adjustment;
				}
			}
		}

		public void DecreaseMembership( double adjustment )
		{
			if ( !m_Locked )
			{
				if ( m_BaseLeft == m_CenterMax )
				{
					if ( ( m_BaseRight - adjustment ) > m_CenterMax )
						m_BaseRight -= adjustment;
				}
				else if ( m_CenterMax == m_BaseRight )
				{
					if ( ( m_BaseLeft + adjustment ) < m_CenterMax )
						m_BaseLeft += adjustment;
				}
				else
				{
					if ( ( m_BaseRight - adjustment ) > m_CenterMax )
						m_BaseRight -= adjustment;

					if ( ( m_BaseLeft + adjustment ) < m_CenterMax )
						m_BaseLeft += adjustment;
				}

			}
		}

		public void AdjustToTargetPoint( double point, double rate )
		{
			if ( !m_Locked )
			{
				if ( m_BaseLeft == m_CenterMax )
				{
					if ( m_BaseRight > point )
						m_BaseRight -= rate * ( m_BaseRight - point );
					else if ( m_BaseRight < point )
						m_BaseRight += rate * ( point - m_BaseRight );
				}
				else if ( m_CenterMax == m_BaseRight )
				{
					if ( m_BaseLeft > point )
						m_BaseLeft -= rate * ( m_BaseLeft - point );
					else if ( m_BaseLeft < point )
						m_BaseLeft += rate * ( point - m_BaseLeft );
				}
				else
				{
					if ( m_CenterMax > point )
						m_CenterMax -= rate * ( m_CenterMax - point );
					else if ( m_CenterMax < point )
						m_CenterMax += rate * ( point - m_CenterMax );
				}
			}
		}

		public double FuzzifieValue( int val )
		{
			if ( val < m_BaseLeft || val > m_BaseRight )
				return 0;

			if ( val == m_CenterMax )
				return 1;

			double y2 = 1;
			double x2 = m_CenterMax;
			double y1 = 0;
			double x1 =( val < m_CenterMax )? m_BaseLeft : m_BaseRight;

			if ( x1 == x2 )
				LogWriter.Write( "divide by zero in MembershipFunction " + m_VariableName + " " + m_TermName );

			double a = (y2 - y1 ) / ( x2 - x1 );
			double b = y1 - ( a * x1 );

			return ( a * val ) + b;
		}


		public string VariableName()
		{
			return m_VariableName;
		}


		public string TermName()
		{
			return m_TermName;
		}

		public double BaseLeft()
		{
			return m_BaseLeft;
		}

		public double CenterMax()
		{
			return m_CenterMax;
		}

		public double BaseRight()
		{
			return m_BaseRight;
		}

		public void Lock( bool locked )
		{
			m_Locked = locked;
		}

		public bool IsLocked()
		{
			return m_Locked;
		}

		public bool SaveAble()
		{
			return true;
		}
	}



	class RedefinedMemberFunction : MfInterface
	{
		private string			m_VariableName;
		private MfInterface		m_WrappedMf;


		public RedefinedMemberFunction( string variableName, MfInterface wrappedMf )
		{
			m_VariableName = variableName;
			m_WrappedMf = wrappedMf;
		}

		public void IncreaseMembership( double adjustment )
		{}

		public void DecreaseMembership( double adjustment )
		{}

		public void AdjustToTargetPoint( double point, double rate )
		{}

		public double FuzzifieValue( int val )
		{
			return m_WrappedMf.FuzzifieValue( val );
		}

		public string VariableName()
		{
			return m_VariableName;
		}

		public string TermName()
		{
			return m_WrappedMf.TermName();
		}

		public double BaseLeft()
		{
			return m_WrappedMf.BaseLeft();
		}

		public double CenterMax()
		{
			return m_WrappedMf.CenterMax();
		}

		public double BaseRight()
		{
			return m_WrappedMf.BaseRight();
		}

		public void Lock( bool locked )
		{}

		public bool IsLocked()
		{
			return true;
		}

		public bool SaveAble()
		{
			return false;
		}
	}
}	


