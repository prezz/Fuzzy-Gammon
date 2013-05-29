using System;
using GammonGame;

namespace GammonAgent
{
	public abstract class AgentDecisionModule
	{
		public abstract string NameId();
		public abstract void GradeBoards( MoveRepresentationList list, BoardRepresentation initialBoard );
		public abstract void Learn( GameHistory history );
		public abstract bool Double( BoardRepresentation flippedBoard );
		public abstract bool AcceptDouble( BoardRepresentation board );
		public abstract void ShutDown();
	}
}
