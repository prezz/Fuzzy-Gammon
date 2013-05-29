using System;
using FuzzevalModule;


public class MemberFunctionGrapper
{
	public static string[] GetFunctions()
	{
		string[] s = {
						"Contact False 0 0 0.5 *",
						"Contact True 0.5 1 1 *",
						"OwnOpponentHomeOutPipDistance Low 0 0 2.57",
						"OwnOpponentHomeOutPipDistance High 2.57 24 24",
						"OwnSquaresOwnedHome Few 0 0 2.86",
						"OwnSquaresOwnedHome Many 2.86 6 6",
						"OwnPieceCountAtLastPos Few 0 0 1.74",
						"OwnPieceCountAtLastPos Many 1.74 15 15",
						"OwnStrongestConsecutiveBlockade Short 0 0 2.75",
						"OwnStrongestConsecutiveBlockade Long 2.75 7 7",
						"OwnPiecesNotInHomePipDistance Short 0 0 24.00",
						"OwnPiecesNotInHomePipDistance Long 24.00 285 285",
						"OwnLastPiecePos Front 0 0 12.87",
						"OwnLastPiecePos Back 12.87 25 25",
						"OwnPipAdvantage Low -50 -50 18.04",
						"OwnPipAdvantage High 18.04 50 50",
						"OwnBeardOff Few 0 0 2.74",
						"OwnBeardOff Many 2.74 15 15",
						"OwnToBeHitFactor Low 0 0 6.07",
						"OwnToBeHitFactor High 6.07 50 50",
						"OwnSinglePiecesInHome Few 0 0 0.28",
						"OwnSinglePiecesInHome Many 0.28 6 6",
						"OwnPiecesNotInHome Few 0 0 3.84",
						"OwnPiecesNotInHome Many 3.84 15 15",
						"OpponentsOnBar Few 0 0 0.33",
						"OpponentsOnBar Many 0.33 15 15",
						"OwnSquaresOwned Few 0 0 4.06",
						"OwnSquaresOwned Many 4.06 7 7"
					 };
		return s;
	}
}
