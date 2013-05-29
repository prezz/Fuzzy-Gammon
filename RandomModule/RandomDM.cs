using System;
using GammonAgent;
using GammonGame;


namespace RandomModule
{
	public class RandomDM : AgentDecisionModule
	{
		private Random		m_Random;

		public RandomDM()
		{
			m_Random = new Random();
		}

		public override string NameId()
		{
			return "Random";
		}

		public override void GradeBoards( MoveRepresentationList list, BoardRepresentation initialBoard )
		{
			for ( int i = 0; i < list.Count(); i++ )
			{
				double s = m_Random.NextDouble() - m_Random.NextDouble();
				list.GetMoveRepresentation( i ).AddScore( s );
			}
		}

		public override void Learn( GameHistory history )
		{}

		public override bool Double( BoardRepresentation flippedBoard )
		{
			return false;
		}

		public override bool AcceptDouble( BoardRepresentation board )
		{
			return true;
		}

		public override void ShutDown()
		{}
	}
}

