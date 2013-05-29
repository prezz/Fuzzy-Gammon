using System;
using GammonGame;


namespace FuzzevalModule
{
	class AvgLinguisticInputResult
	{
		public string	VariableName;
		public double	CrispValue;

		public AvgLinguisticInputResult( string varName, double crispVal )
		{
			VariableName = varName;
			CrispValue = crispVal;
		}
	}



	class FunctionCallibrator
	{
		private FuzzyController		m_Controller;


		public FunctionCallibrator( FuzzyController controller )
		{
			m_Controller = controller;
		}


		private AvgLinguisticInputResult[] GetAverageInput( BoardRepresentation[] boards )
		{
			AvgLinguisticInputResult[] result = new AvgLinguisticInputResult[m_Controller.PreProcessorCount()];
			LinguisticInputValue[] inValues = m_Controller.ObtainCrispInput( boards[0] );

			for ( int i = 0; i < inValues.Length; i++ )
				result[i] = new AvgLinguisticInputResult( inValues[i].VariableName, ( double )inValues[i].CrispValue );

			for ( int i = 1; i < boards.Length; i++ )
			{
				inValues = m_Controller.ObtainCrispInput( boards[i] );
				for ( int j = 0; j < inValues.Length; j++ )
					result[j].CrispValue += ( double )inValues[j].CrispValue;
			}

			for ( int i = 0; i < result.Length; i++ )
				result[i].CrispValue = result[i].CrispValue / ( double )boards.Length;

			return result;
		}


		public void Callibrate( BoardRepresentation[] winnerBoards, BoardRepresentation[] loserBoards )
		{
			AvgLinguisticInputResult[] avgWinnerInput = GetAverageInput( winnerBoards );

			for ( int i = 0; i < avgWinnerInput.Length; i++ )
			{
				MfInterface[] memberFunctions = m_Controller.GetMembershipFunctions( avgWinnerInput[i].VariableName );

				for ( int j = 0; j < memberFunctions.Length; j++ )
					memberFunctions[j].AdjustToTargetPoint( avgWinnerInput[i].CrispValue, 0.05 );
			}
		}
	}
}
