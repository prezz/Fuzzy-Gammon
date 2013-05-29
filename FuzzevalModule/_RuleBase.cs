using System;
using FuzzevalModule;


public class Rules
{
	public static string[] GetRules()
	{
		string[] s = {
						//-----------------CONTACT----------------

						"if OwnToBeHitFactor is Low and Contact is True then output Good",
						"if OwnToBeHitFactor is High and Contact is True then output Weak",

						"if OwnSinglePiecesInHome is Few and Contact is True then output Good",
						"if OwnSinglePiecesInHome is Many and Contact is True then output Weak",

						"if OwnSquaresOwned is Few and Contact is True then output Weak",
						"if OwnSquaresOwned is Many and Contact is True then output Good",

						"if OwnSquaresOwnedHome is Few and Contact is True then output Weak",
						"if OwnSquaresOwnedHome is Many and Contact is True then output Good",

						"if OwnStrongestConsecutiveBlockade is Short and Contact is True then output Weak",
						"if OwnStrongestConsecutiveBlockade is Long and Contact is True then output Good",

						"if OwnOpponentHomeOutPipDistance is Low and Contact is True then output Good",
						"if OwnOpponentHomeOutPipDistance is High and Contact is True then output Weak",

						"if OwnPiecesNotInHome is Few and Contact is True then output Good",
						"if OwnPiecesNotInHome is Many and Contact is True then output Weak",

						"if OwnLastPiecePos is Front and Contact is True then output Good",
						"if OwnLastPiecePos is Back and Contact is True then output Weak",

						"if OwnPieceCountAtLastPos is Few and Contact is True then output Good",
						"if OwnPieceCountAtLastPos is Many and Contact is True then output Weak",

						"if OpponentsOnBar is Few and Contact is True then output Weak",
						"if OpponentsOnBar is Many and Contact is True then output Good",

						"if OwnBeardOff is Few and Contact is True then output Weak",
						"if OwnBeardOff is Many and Contact is True then output Good",

						//-------------NO CONTACT---------------------------

						"if OwnToBeHitFactor is Low and Contact is False then output Good",
						"if OwnToBeHitFactor is High and Contact is False then output Weak",

						"if OwnBeardOff is Few and Contact is False then output Weak",
						"if OwnBeardOff is Many and Contact is False then output Good",

						"if OwnPipAdvantage is Low and Contact is False then output Weak",
						"if OwnPipAdvantage is High and Contact is False then output Good",

						"if OwnPiecesNotInHomePipDistance is Short and Contact is False then output Good",
						"if OwnPiecesNotInHomePipDistance is Long and Contact is False then output Weak"
					 };
		return s;
	}
}
