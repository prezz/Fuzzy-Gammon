using System;
using GammonAgent;
using GammonGame;

namespace FuzzevalModule
{
	public class FuzzevalDM : AgentDecisionModule
	{
		private FuzzyController		m_FuzzyController;
		private FunctionCallibrator	m_FuncCallibrator;
		private bool				m_Trained;


		public FuzzevalDM()
		{
			m_Trained = false;
			m_FuzzyController = new FuzzyController();
			m_FuncCallibrator = new FunctionCallibrator( m_FuzzyController );
		}


		public override string NameId()
		{
			return "Fuzzeval";
		}


		public override void GradeBoards( MoveRepresentationList list, BoardRepresentation initialBoard )
		{
			for ( int i = 0; i < list.Count(); i++ )
			{
				double s = m_FuzzyController.ObtainBoardStrength( list.GetMoveRepresentation( i ) );
				list.GetMoveRepresentation( i ).AddScore( s );
			}
		}


		public override void Learn( GameHistory history )
		{
			m_Trained = true;

			BoardRepresentation[] looserBoards = new BoardRepresentation[history.Count() / 2];
			int looserIdx = looserBoards.Length;
			for ( int i = history.Count()-2; i >= 0; i -= 2 )
				looserBoards[--looserIdx] = history.Peek( i );

			BoardRepresentation[] winnerBoards = new BoardRepresentation[history.Count() - looserBoards.Length];
			int winnerIdx = winnerBoards.Length;
			for ( int i = history.Count()-1; i >= 0; i -= 2 )
				winnerBoards[--winnerIdx] = history.Peek( i );

			m_FuncCallibrator.Callibrate( winnerBoards, looserBoards );
		}


		public override bool Double( BoardRepresentation flippedBoard )
		{
			return false;
		}


		public override bool AcceptDouble( BoardRepresentation board )
		{
			return true;
		}

		
		public override void ShutDown()
		{
			if ( m_Trained )
				m_FuzzyController.Save();
		}
	}
}
