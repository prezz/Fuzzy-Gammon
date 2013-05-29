using System;
using GammonAgent;
using GammonGame;
using FuzzevalModule;


public class PreProcessingGrapper
{
	public static PreProcessingObject[] GetObjects()
	{
		PreProcessingObject[] result = {

		//Do we have contact?
			new Contact(),

		//Avoiding Single piece
			new OwnToBeHitFactor(),
			new OwnSinglePiecesInHome(),
			
		//Build blockades
			new OwnSquaresOwned(),
			new OwnSquaresOwnedHome(),
			new OwnStrongestConsecutiveBlockade(),

		//Pushing pieces forward
			new OwnPiecesNotInHomePipDistance(),
			new OwnOpponentHomeOutPipDistance(),
			new OwnPiecesNotInHome(),
			new OwnLastPiecePos(),
			new OwnPieceCountAtLastPos(),

		//Misc.
			new OpponentsOnBar(),
			new OwnBeardOff(),
			new OwnPipAdvantage()
		};
	
		return result;
	}
}


//-------------


public class Contact : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		bool opponentFound = false;
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 0; i < 26; i++ )
		{
			if ( board.GetPiecesAt( i ) < 0 )
			{
				opponentFound = true;
			}
			if ( board.GetPiecesAt( i ) > 0 && opponentFound )
			{
				result.CrispValue = 1;
				break;
			}
		}
		return result;
	}
}


//-------------


public class OwnToBeHitFactor : PreProcessingObject
{
	public void CalculateHitProbability( int position, int hitFrom, BoardRepresentation board, bool[,] hittingDices )
	{
		int distance = position - hitFrom;

		for ( int i = 1; i <= 6; i++ )
		{
			for ( int j = 1; j <= 6; j++ )
			{
				if ( distance == i || distance == j )
				{
					if ( board.GetPiecesAt( 0 ) == 0 )
					{
						hittingDices[i-1,j-1] = true;
					}
					else if ( board.GetPiecesAt( 0 ) < 0 && hitFrom == 0 )
					{
						hittingDices[i-1,j-1] = true;
					}
					else if ( board.GetPiecesAt( 0 ) == -1 && hitFrom != 0 )
					{
						if ( distance == i && board.GetPiecesAt( j ) < 2 )
						{
							hittingDices[i-1,j-1] = true;
						}
						else if ( distance == j && board.GetPiecesAt( i ) < 2 )
						{
							hittingDices[i-1,j-1] = true;
						}
					}
					else if ( i == j && ( board.GetPiecesAt( 0 ) == -1 || board.GetPiecesAt( 0 ) == -2 || board.GetPiecesAt( 0 ) == -3 ) && board.GetPiecesAt( i ) < 2 )
					{
						hittingDices[i-1,j-1] = true;
					}
				}
				else if ( distance == i + j )
				{
					if ( board.GetPiecesAt( 0 ) == 0 )
					{
						if ( board.GetPiecesAt( hitFrom + i ) < 2 || board.GetPiecesAt( hitFrom + j ) < 2 )
							hittingDices[i-1,j-1] = true;
					}
					else if ( board.GetPiecesAt( 0 ) == -1 && hitFrom == 0 )
					{
						if ( board.GetPiecesAt( hitFrom + i ) < 2 || board.GetPiecesAt( hitFrom + j ) < 2 )
							hittingDices[i-1,j-1] = true;
					}
					else if ( i == j && ( board.GetPiecesAt( 0 ) == -1 || board.GetPiecesAt( 0 ) == -2 ) && board.GetPiecesAt( i ) < 2 )
					{
						if ( board.GetPiecesAt( hitFrom + i ) < 2 )
							hittingDices[i-1,j-1] = true;
					}
					else if ( i == j && board.GetPiecesAt( 0 ) == -3 && hitFrom == 0 && board.GetPiecesAt( i ) < 2 )
					{
						hittingDices[i-1,j-1] = true;
					}
				}
				else if ( i == j && ( distance == 3 * i || distance == 4 * i ) )
				{
					if ( board.GetPiecesAt( 0 ) == 0 )
					{
						if ( distance == 3 * i )
						{
							if ( board.GetPiecesAt( hitFrom + i ) < 2 && board.GetPiecesAt( hitFrom + ( i * 2 ) ) < 2 )
								hittingDices[i-1,j-1] = true;
						}
						if ( distance == 4 * i )
						{
							if ( board.GetPiecesAt( hitFrom + i ) < 2 && board.GetPiecesAt( hitFrom + ( i * 2 ) ) < 2 && board.GetPiecesAt( hitFrom + ( i * 3 ) ) < 2 )
								hittingDices[i-1,j-1] = true;
						}
					}
					else if ( ( board.GetPiecesAt( 0 ) == -1 && hitFrom == 0 ) )
					{
						if ( distance == 3 * i )
						{
							if ( board.GetPiecesAt( hitFrom + i ) < 2 && board.GetPiecesAt( hitFrom + ( i * 2 ) ) < 2 )
								hittingDices[i-1,j-1] = true;
						}
						if ( distance == 4 * i )
						{
							if ( board.GetPiecesAt( hitFrom + i ) < 2 && board.GetPiecesAt( hitFrom + ( i * 2 ) ) < 2 && board.GetPiecesAt( hitFrom + ( i * 3 ) ) < 2 )
								hittingDices[i-1,j-1] = true;
						}
					}
					else if ( ( board.GetPiecesAt( 0 ) == -2 && hitFrom == 0 ) )
					{
						if ( distance == 3 * i )
						{
							if ( board.GetPiecesAt( hitFrom + i ) < 2 && board.GetPiecesAt( hitFrom + ( i * 2 ) ) < 2 )
								hittingDices[i-1,j-1] = true;
						}
					}
					else if ( board.GetPiecesAt( 0 ) == -1 && hitFrom != 0 && board.GetPiecesAt( i ) < 2 )
					{
						if ( distance == 3 * i )
						{
							if ( board.GetPiecesAt( hitFrom + i ) < 2 && board.GetPiecesAt( hitFrom + ( i * 2 ) ) < 2 )
								hittingDices[i-1,j-1] = true;
						}
					}
				}
			}
		}
	}
	
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		
		float r = 0.0f;
		for ( int i = 1; i < 25; i++ )
		{
			if ( board.GetPiecesAt( i ) == 1 )
			{
				bool[,] possibleDices = new bool[6,6];
				for ( int x = 0; x < 6; x++ )
					for ( int y = 0; y < 6; y++ )
						possibleDices[x,y] = false;
				
				for ( int j = 0; j < i; j++ )
					if ( board.GetPiecesAt( j ) < 0 )
						CalculateHitProbability( i, j, board, possibleDices );
				
				int waysToBeHit = 0;
				for ( int x = 0; x < 6; x++ )
					for ( int y = 0; y < 6; y++ )
						if ( possibleDices[x,y] )
							waysToBeHit++;
		
				float p = ( ((float)waysToBeHit) / 36.0f ) * 100;		
				r += ((24.0f - ((float)i-1))/24.0f) * p;
			}
		}

		if ( r > 50.0f )
			r = 50.0f;

		result.CrispValue = (int)Math.Round( r, 0 );
		return result;
	}	
}


public class OwnSinglePiecesInHome : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 1; i < 7; i++ )
			if ( board.GetPiecesAt( i ) == 1 )
				result.CrispValue++;
		return result;
	}
}


//------------


public class OwnSquaresOwned : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 1; i < 25; i++ )
			if ( board.GetPiecesAt( i ) > 1 )
				result.CrispValue++;
		return result;
	}
}


public class OwnSquaresOwnedHome : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 1; i < 7; i++ )
			if ( board.GetPiecesAt( i ) > 1 )
				result.CrispValue++;
		return result;
	}
}


public class OwnStrongestConsecutiveBlockade : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		int currentConsecutiveOwned = 0;
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 1; i < 25; i++ )
		{
			if ( board.GetPiecesAt( i ) > 1 )
			{
				currentConsecutiveOwned++;
				if ( currentConsecutiveOwned > result.CrispValue )
					result.CrispValue = currentConsecutiveOwned;
			}
			else
			{
				currentConsecutiveOwned = 0;
			}
		}
		return result;
	}
}


public class OwnSquaresWithBlockBuildPossability : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 1; i < 25; i++ )
			if ( board.GetPiecesAt( i ) == 1 || board.GetPiecesAt( i ) > 2 )
				result.CrispValue++;
		return result;
	}
}


//----------


public class OwnPiecesNotInHomePipDistance : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 7; i < 26; i++ )
			if ( board.GetPiecesAt( i ) > 0 )
				result.CrispValue += ( board.GetPiecesAt( i ) * ( i - 6 ) );
			
		return result;
	}
}


public class OwnOpponentHomeOutPipDistance : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 19; i < 26; i++ )
			if ( board.GetPiecesAt( i ) > 0 )
				result.CrispValue += ( board.GetPiecesAt( i ) * ( i - 18 ) );
				
		if ( result.CrispValue > 24 )
			result.CrispValue = 24;
			
		return result;
	}
}


public class OwnPiecesNotInHome : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 7; i < 26; i++ )
			if ( board.GetPiecesAt( i ) > 0 )
				result.CrispValue += board.GetPiecesAt( i );
		return result;
	}
}


public class OwnLastPiecePos : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 1; i < 26; i++ )
			if ( board.GetPiecesAt( i ) > 0 )
				result.CrispValue = i;
		return result;
	}
}


public class OwnPieceCountAtLastPos : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );
		for ( int i = 25; i > 0; i-- )
		{
			if ( board.GetPiecesAt( i ) > 0 )
			{
				result.CrispValue = board.GetPiecesAt( i );
				break;
			}
		}
		return result;
	}
}


//------------


public class OpponentsOnBar : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), -board.GetPiecesAt( 0 ) );
		return result;
	}
}


public class OwnBeardOff : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), board.BearOffCountCurrent() );
		return result;
	}
}


public class OwnPipAdvantage : PreProcessingObject
{
	public override LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board )
	{
		LinguisticInputValue result = new LinguisticInputValue( GetName(), 0 );

		int ownPips = 0;
		for ( int i = 0; i < 26; i++ )
			if ( board.GetPiecesAt( i ) > 0 )
				ownPips += i * board.GetPiecesAt( i );

		int opponentPips = 0;
		for ( int i = 0; i < 26; i++ )
			if ( board.GetPiecesAt( i ) < 0 )
				opponentPips += ( 25 - i ) * ( -board.GetPiecesAt( i ) );
	
		int r = opponentPips - ownPips;
		
		if ( r > 50 )
			r = 50;
		if ( r < -50 )
			r = -50;
		
		result.CrispValue = r;
		return result;
	}
}
