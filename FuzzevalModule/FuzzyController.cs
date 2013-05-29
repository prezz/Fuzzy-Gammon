using System;
using GammonAgent;
using GammonGame;


namespace FuzzevalModule
{
	class FuzzyController
	{
		private PreProcessor	m_PreProcessor;
		private Fuzzifier		m_Fuzzifier;
		private InferenceEngine	m_InferenceEngine;
		private Defuzzifier		m_Defuzzifier;


		public FuzzyController()
		{
			m_PreProcessor = new PreProcessor();
			PreProcessor.MakeReady( m_PreProcessor );

			m_Fuzzifier = new Fuzzifier();
			Fuzzifier.MakeReady( m_Fuzzifier );
			Fuzzifier.LoadMemberFunctions( m_Fuzzifier, ".\\Fuzzeval.mf" );

			m_InferenceEngine = new InferenceEngine();
			InferenceEngine.MakeReady( m_InferenceEngine );

			m_Defuzzifier = new Defuzzifier();
			Defuzzifier.MakeReady( m_Defuzzifier );
		}


		public LinguisticInputValue[] ObtainCrispInput( BoardRepresentation board )
		{
			return m_PreProcessor.Process( board );
		}


		public FuzzifiedValueContainer ObtainFuzzyValues( BoardRepresentation board )
		{
			return m_Fuzzifier.Fuzzificate( ObtainCrispInput( board ) );
		}


		public Rule[] ObtainRuleFulfilments( BoardRepresentation board )
		{
			return m_InferenceEngine.Infer( ObtainFuzzyValues( board ) );
		}


		public double ObtainBoardStrength( BoardRepresentation board )
		{
			return m_Defuzzifier.Defuzzificate( ObtainRuleFulfilments( board ) );
		}


		public int MembershipFunctionCount()
		{
			return m_Fuzzifier.MemberFunctionsCount();
		}


		public MfInterface GetMembershipFunction( int idx )
		{
			return m_Fuzzifier.GetMembershipFunction( idx );
		}


		public MfInterface GetMembershipFunction( string linguisticVar, string linguisticTerm )
		{
			return m_Fuzzifier.GetMembershipFunction( linguisticVar, linguisticTerm );
		}


		public MfInterface[] GetMembershipFunctions( string linguisticVar )
		{
			return m_Fuzzifier.GetMembershipFunctions( linguisticVar );
		}


		public int GetMembershipFunctionId( string linguisticVar, string linguisticTerm )
		{
			return m_Fuzzifier.GetMembershipFunctionId( linguisticVar, linguisticTerm );
		}


		public int RuleBaseRuleCount()
		{
			return m_InferenceEngine.GetRuleBase().RuleCount();
		}


		public Rule GetRuleBaseRule( int idx )
		{
			return m_InferenceEngine.GetRuleBase().GetRule( idx );
		}


		public void AddRule( Rule rule )
		{
			m_InferenceEngine.GetRuleBase().AddRule( rule );
		}


		public Rule RemoveRule( Rule rule )
		{
			return m_InferenceEngine.GetRuleBase().RemoveRule( rule );
		}

		public Rule RemoveRule( int idx )
		{
			return m_InferenceEngine.GetRuleBase().RemoveRule( idx );
		}

		public void ClearRuleBase()
		{
			m_InferenceEngine.GetRuleBase().Clear();
		}

		public int PreProcessorCount()
		{
			return m_PreProcessor.PreProcessorCount();
		}

		public PreProcessingObject GetPreProcessingObject( int idx )
		{
			return m_PreProcessor.GetPreProcessor( idx );
		}

		public int GetPreProcessorIndex( string LinguisticVar )
		{
			return m_PreProcessor.GetPreProcessorIdx( LinguisticVar );
		}
		
		public double GetCrispDefuzzifieValue( string identifier )
		{
			return m_Defuzzifier.GetOutputValue( identifier ).CrispValue;
		}

		public int DefuzzifieValueCount()
		{
			return m_Defuzzifier.Count();
		}

		public string GetDefuzzifieIdentifier( int idx )
		{
			return m_Defuzzifier.GetLinguisticIdentifier( idx );
		}

		public int GetDefuzzifieIdentifierIndex( string id )
		{
			return m_Defuzzifier.GetLinguisticIdentifierIndex( id );
		}

		public void Save()
		{
			Fuzzifier.SaveMemberFunctions( m_Fuzzifier, ".\\Fuzzeval.mf" );
		}
 	}
}
