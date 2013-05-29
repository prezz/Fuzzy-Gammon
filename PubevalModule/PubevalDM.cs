using System;
using GammonAgent;
using GammonGame;


namespace PubevalModule
{
	public class PubevalDM : AgentDecisionModule
	{
		private float[] wr = 
		{
			0.00000f, -0.17160f, 0.27010f, 0.29906f, -0.08471f, 0.00000f, -1.40375f,
			-1.05121f, 0.07217f, -0.01351f, 0.00000f, -1.29506f, -2.16183f, 0.13246f,
			-1.03508f, 0.00000f, -2.29847f, -2.34631f, 0.17253f, 0.08302f, 0.00000f,
			-1.27266f, -2.87401f, -0.07456f, -0.34240f, 0.00000f, -1.34640f, -2.46556f,
			-0.13022f, -0.01591f, 0.00000f, 0.27448f, 0.60015f, 0.48302f, 0.25236f, 0.00000f,
			0.39521f, 0.68178f, 0.05281f, 0.09266f, 0.00000f, 0.24855f, -0.06844f, -0.37646f,
			0.05685f, 0.00000f, 0.17405f, 0.00430f, 0.74427f, 0.00576f, 0.00000f, 0.12392f,
			0.31202f, -0.91035f, -0.16270f, 0.00000f, 0.01418f, -0.10839f, -0.02781f, -0.88035f,
			0.00000f, 1.07274f, 2.00366f, 1.16242f, 0.22520f, 0.00000f, 0.85631f, 1.06349f,
			1.49549f, 0.18966f, 0.00000f, 0.37183f, -0.50352f, -0.14818f, 0.12039f, 0.00000f,
			0.13681f, 0.13978f, 1.11245f, -0.12707f, 0.00000f, -0.22082f, 0.20178f, -0.06285f,
			-0.52728f, 0.00000f, -0.13597f, -0.19412f, -0.09308f, -1.26062f, 0.00000f, 3.05454f,
			5.16874f, 1.50680f, 5.35000f, 0.00000f, 2.19605f, 3.85390f, 0.88296f, 2.30052f,
			0.00000f, 0.92321f, 1.08744f, -0.11696f, -0.78560f, 0.00000f, -0.09795f, -0.83050f,
			-1.09167f, -4.94251f, 0.00000f, -1.00316f, -3.66465f, -2.56906f, -9.67677f,
			0.00000f, -2.77982f, -7.26713f, -3.40177f, -12.32252f, 0.00000f, 3.42040f
		};
		private float[] wc = 
		{
			0.25696f, -0.66937f, -1.66135f, -2.02487f, -2.53398f, -0.16092f, -1.11725f,
			-1.06654f, -0.92830f, -1.99558f, -1.10388f, -0.80802f, 0.09856f, -0.62086f,
			-1.27999f, -0.59220f, -0.73667f, 0.89032f, -0.38933f, -1.59847f, -1.50197f,
			-0.60966f, 1.56166f, -0.47389f, -1.80390f, -0.83425f, -0.97741f, -1.41371f,
			0.24500f, 0.10970f, -1.36476f, -1.05572f, 1.15420f, 0.11069f, -0.38319f, -0.74816f,
			-0.59244f, 0.81116f, -0.39511f, 0.11424f, -0.73169f, -0.56074f, 1.09792f, 0.15977f,
			0.13786f, -1.18435f, -0.43363f, 1.06169f, -0.21329f, 0.04798f, -0.94373f, -0.22982f,
			1.22737f, -0.13099f, -0.06295f, -0.75882f, -0.13658f, 1.78389f, 0.30416f, 0.36797f,
			-0.69851f, 0.13003f, 1.23070f, 0.40868f, -0.21081f, -0.64073f, 0.31061f, 1.59554f,
			0.65718f, 0.25429f, -0.80789f, 0.08240f, 1.78964f, 0.54304f, 0.41174f, -1.06161f,
			0.07851f, 2.01451f, 0.49786f, 0.91936f, -0.90750f, 0.05941f, 1.83120f, 0.58722f,
			1.28777f, -0.83711f, -0.33248f, 2.64983f, 0.52698f, 0.82132f, -0.58897f, -1.18223f,
			3.35809f, 0.62017f, 0.57353f, -0.07276f, -0.36214f, 4.37655f, 0.45481f, 0.21746f,
			0.10504f, -0.61977f, 3.54001f, 0.04612f, -0.18108f, 0.63211f, -0.87046f, 2.47673f,
			-0.48016f, -1.27157f, 0.86505f, -1.11342f, 1.24612f, -0.82385f, -2.77082f,
			1.23606f, -1.59529f, 0.10438f, -1.30206f, -4.11520f, 5.62596f, -2.75800f
		};
		private float[] x;


		public PubevalDM()
		{
			x = new float[122];
		}


		public override string NameId()
		{
			return "Pubeval";
		}


		public override void GradeBoards( MoveRepresentationList list, BoardRepresentation initialBoard )
		{
			int race = 1;
			bool opponentFound = false;
			for ( int i = 0; i < initialBoard.SquareCount() && race == 1; i++ )
			{
				if ( !opponentFound && initialBoard.GetPiecesAt( i ) < 0 )
					opponentFound = true;

				if ( opponentFound && initialBoard.GetPiecesAt( i ) > 0 )
					race = 0;
			}
			
			for ( int i = 0; i < list.Count(); i++ )
			{
				int[] pubevalBoard = new int[28];
				MoveRepresentation currentBoard = list.GetMoveRepresentation( i );

				for ( int j = 0; j < currentBoard.SquareCount(); j++ )
					pubevalBoard[j] = currentBoard.GetPiecesAt( j );

				pubevalBoard[26] = currentBoard.BearOffCountCurrent();
				pubevalBoard[27] = currentBoard.BearOffCountOpponent();

				double s = PubevalGrade( race, pubevalBoard );
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


		private void setx( int[] pos )
		{
			for( int i = 0; i < 122; i++ )
				x[i] = 0.0f;

			/* first encode board locations 24-1 */
			int jm1;
			float n;
			for( int j = 1; j <= 24; j++ )
			{
				jm1 = j - 1;
				n = pos[25-j];
				if( n != 0 )
				{
					if( n == -1 )
						x[5*jm1+0] = 1.0f;
					if( n == 1 )
						x[5*jm1+1] = 1.0f;
					if( n >= 2 )
						x[5*jm1+2] = 1.0f;
					if( n == 3 )
						x[5*jm1+3] = 1.0f;
					if( n>=4 )
						x[5*jm1+4] = (float)( n-3.0f )/2.0f;
				}
			}

			/* encode opponent barmen */
			x[120] = -(float)((float)pos[0])/2.0f;
			/* encode computer's menoff */
			x[121] = (float)((float)pos[26])/15.0f;
		}

		//race is decided by initial board boards are generated from not the actual board
		private float PubevalGrade( int race, int[] pos )
		{
			/* all men off, best possible move */
			if ( pos[26] == 15 )
				return(99999999.0f);

			setx( pos );

			float score = 0.0f;
			if( race == 1 )
			{
				for( int i = 0; i < 122; i++ )
					score += wr[i]*x[i];
			}
			else
			{ 
				for( int i = 0; i < 122; i++ )
					score += wc[i]*x[i];
			}
			return score;
		}
	}
}
