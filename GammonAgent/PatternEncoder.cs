using System;
using GammonGame;

namespace GammonAgent
{
	class PatternEncoder
	{
		private char[] Base64Alphabet = {	'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
											'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
											'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
											'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
											'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
											'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
											'w', 'x', 'y', 'z', '0', '1', '2', '3',
											'4', '5', '6', '7', '8', '9', '+', '/' };


		public string Encode( GammonInterface game )
		{
			string result = BitStringPatternToStringPattern( NumberPatternToBitStringPattern( BoardToNumberPattern( game ) ) );
			result += game.GetDiceValue(0).ToString() + game.GetDiceValue(1).ToString();
			return result;
		}


		public int[] BoardToNumberPattern( GammonInterface game )
		{
			int[] result = new int[26];

			for ( int i = 0; i < game.BoardSquareCount; i++ )
			{
				if ( game.SquareOwner( i ) == game.CurrentPlayer )
					result[i] = game.SquareCheckerCount( i );
				else if ( game.SquareOwner( i ) == game.CurrentOpponentPlayer )
					result[i] = -( game.SquareCheckerCount( i ) );
				else
					result[i] = 0;
			}
			return result;		
		}


		public string NumberPatternToBitStringPattern( int[] numberPattern )
		{
			string result = "";

			for ( int i = 1; i < numberPattern.Length; i++ )
			{
				if ( numberPattern[i] > 0 )
				{
					for ( int j = 0; j <  numberPattern[i]; j++ )
						result += 1;
				}
				result += 0;
			}

			for ( int i = numberPattern.Length - 2; i >= 0; i-- )
			{
				if ( numberPattern[i] < 0 )
				{
					for ( int j = 0; j <  -( numberPattern[i] ); j++ )
						result += 1;
				}
				result += 0;
			}


			while ( result.Length < 84 )
				result += 0;

			return result;
		}


		public string BitStringPatternToStringPattern( string bitPattern )
		{
			byte[] patternArray = new byte[14];

			for ( int i = 0; i < bitPattern.Length; i += 6 )
				patternArray[i/6] = ByteStringToByte( bitPattern.Substring( i, 6 ) );

			string result = "";
			for ( int i = 0; i < patternArray.Length; i++ )
				result += Base64Alphabet[patternArray[i]];

			return result;
		}


		private byte ByteStringToByte( string ByteString )
		{
			byte result = 0;
			byte nextValue = 1;

			int stopIdx = ( ByteString.Length > 8 )? ByteString.Length - 8 : 0;

			for ( int i = ByteString.Length; i > stopIdx; i-- )
			{
				if ( ByteString[i-1] == '1' )
					result += nextValue;

				nextValue *= 2;
			}
			return result;
		}
	}
}


	/*
			private static string BitPatternToLittleEndian( string bitPattern )
			{
				string result = "";
				string bigEndian = "";
				string littleEndian = "";

				for ( int i = 0; i < bitPattern.Length; i += 8 )
				{
					littleEndian = "";
					bigEndian = bitPattern.Substring( i, 8 );

					for ( int j = 0; j < bigEndian.Length; j++ )
						littleEndian = bigEndian[j] + littleEndian;

					result += littleEndian;
				}

				return result;
			}
	*/

