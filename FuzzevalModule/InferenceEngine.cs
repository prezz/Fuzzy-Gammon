using System;
using System.Collections;
using System.IO;

namespace FuzzevalModule
{
	class RuleCondition
	{
		public string	VariableName;
		public string	TermName;
		public double	Fulfillment;

		public RuleCondition( string varName, string termName )
		{
			VariableName = varName;
			TermName = termName;
			Fulfillment = 0.0;
		}

		public override bool Equals(object obj)
		{
			RuleCondition r = ( RuleCondition )obj;
			if ( this.VariableName.Equals( r.VariableName ) && this.TermName.Equals( r.TermName ) )
				return true;
			else
				return false;
		}

		public override int GetHashCode()
		{ return base.GetHashCode (); }
	}


	class RuleConclusion
	{
		public string Identifier;
		public double Fulfillment;

		public RuleConclusion( string identifier )
		{
			Identifier = identifier;
			Fulfillment = 0.0;
		}

		public override bool Equals(object obj)
		{
			RuleConclusion r = ( RuleConclusion )obj;
			if ( this.Identifier.Equals( r.Identifier ) )
				return true;
			else
				return false;
		}

		public override int GetHashCode()
		{ return base.GetHashCode (); }

	}


	class Rule
	{
		private RuleConclusion	m_Conclusion;
		private ArrayList		m_Conditions;


		public Rule()
		{
			m_Conditions = new ArrayList();
			m_Conclusion = null;

		}

		public void SetConclusion( RuleConclusion conclusion )
		{
			m_Conclusion = conclusion;
		}

		public RuleConclusion GetConclusion()
		{
			return m_Conclusion;
		}

		public void AddCondition( RuleCondition condition )
		{
			for ( int i = 0; i < ConditionCount(); i++ )
			{
				if ( condition.Equals( GetCondition( i ) ) )
					return;
			}
			m_Conditions.Add( condition );
		}

		public int ConditionCount()
		{
			return m_Conditions.Count;
		}

		public RuleCondition GetCondition( int idx )
		{
			return ( RuleCondition )m_Conditions[idx];
		}

		public void CalculateConclusionFulfillments()
		{
			if ( m_Conditions.Count == 0 )
				return;

			m_Conclusion.Fulfillment = ( ( RuleCondition )m_Conditions[0] ).Fulfillment;
			for ( int i = 1; i < m_Conditions.Count; i++ )
				if ( m_Conclusion.Fulfillment > ( ( RuleCondition )m_Conditions[i] ).Fulfillment )
					m_Conclusion.Fulfillment = ( ( RuleCondition )m_Conditions[i] ).Fulfillment;
		}

		public void ResetFulfillmentValues()
		{
			m_Conclusion.Fulfillment = 0.0;
			for ( int i = 0; i < m_Conditions.Count; i++ )
				( ( RuleCondition )m_Conditions[i] ).Fulfillment = 0.0;
		}

		public override bool Equals(object obj)
		{
			Rule r = ( Rule )obj;

			if ( ConditionCount() != r.ConditionCount() )
				return false;

			if ( !GetConclusion().Equals( r.GetConclusion() ) )
				return false;

			for ( int i = 0; i < ConditionCount(); i++ )
			{
				bool matchFound = false;
				for ( int j = 0; j < r.ConditionCount() && !matchFound; j++ )
				{
					if ( GetCondition( i ).Equals( r.GetCondition( j ) ) )
						matchFound = true;
				}
				if ( !matchFound )
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int result = 0;
			
			string s = GetConclusion().Identifier;
			for ( int i = 0; i < s.Length; i++ )
				result += s[i];

			for ( int j = 0; j < ConditionCount(); j++ )
			{
				s = GetCondition( j ).VariableName + GetCondition( j ).TermName;
				for ( int i = 0; i < s.Length; i++ )
					result += s[i];
			}

			return result;
		}
	}
	
	
	class RuleBase
	{
		private ArrayList	m_Rules;


		public static void MakeReady( RuleBase ruleBase )
		{
			ruleBase.m_Rules.Clear();

			string line;
			char[] separator = {' ', '\t', '\n'};
			string[] tokens;
			string[] s = Rules.GetRules();
			for ( int i = 0; i < s.Length; i++ )
			{
				line = s[i].Trim();
				if ( line.Length > 0 && line[0] != ';' )
				{
					Rule rule = new Rule();

					tokens = line.Split( separator );
					int idx = 0;
					while ( !tokens[idx].Equals( "then" ) )
					{
						RuleCondition ruleCondition = new RuleCondition( tokens[idx+1], tokens[idx+3] );
						rule.AddCondition( ruleCondition );
						idx += 4;
					}
					RuleConclusion ruleConclusion = new RuleConclusion( tokens[idx+2] );
					rule.SetConclusion( ruleConclusion );

					ruleBase.AddRule( rule );
				}
			}
		}


		public RuleBase()
		{
			m_Rules = new ArrayList();
		}

		public void AddRule( Rule rule )
		{
			for ( int i = 0; i < RuleCount(); i++ )
				if ( GetRule( i ).Equals( rule ) )
					return;

			m_Rules.Add( rule );
		}

		public Rule GetRule( int idx )
		{
			return ( Rule )m_Rules[idx];
		}

		public Rule RemoveRule( int idx )
		{
			Rule result = ( Rule )m_Rules[idx];
			m_Rules.RemoveAt( idx );
			return result;
		}

		public void Clear()
		{
			m_Rules.Clear();
		}

		public Rule RemoveRule( Rule rule )
		{
			for ( int i = 0; i < RuleCount(); i++ )
				if ( ( ( Rule )m_Rules[i] ).Equals( rule ) )
					return RemoveRule( i );
			return null;
		}

		public int RuleCount()
		{
			return m_Rules.Count;
		}

		public void ResetFulfillmentValues()
		{
			for ( int i = 0; i < m_Rules.Count; i++ )
				( ( Rule )m_Rules[i] ).ResetFulfillmentValues();
		}
	}
	

	class InferenceEngine
	{
		private RuleBase	m_RuleBase;


		public static void MakeReady( InferenceEngine infEng )
		{
			RuleBase.MakeReady( infEng.m_RuleBase );
		}

		public InferenceEngine()
		{
			m_RuleBase = new RuleBase();
		}

		public RuleBase GetRuleBase()
		{
			return m_RuleBase;
		}

		public Rule[] Infer( FuzzifiedValueContainer fuzzyContainer )
		{
			Rule[] result = new Rule[m_RuleBase.RuleCount()];
			m_RuleBase.ResetFulfillmentValues();

			for ( int i = 0; i < m_RuleBase.RuleCount(); i++ )
			{
				Rule currentRule = m_RuleBase.GetRule( i );
				for ( int j = 0; j < currentRule.ConditionCount(); j++ )
				{
					RuleCondition currentCondition = currentRule.GetCondition( j );
					FuzzifiedValue fuzzyVal = null;
					if ( ( fuzzyVal = fuzzyContainer.Get( currentCondition.VariableName, currentCondition.TermName ) ) != null )
						currentCondition.Fulfillment = fuzzyVal.FuzzyValue;
					else
						LogWriter.Write( "Error in rule " + i + " condition " + j ); 
				}
				currentRule.CalculateConclusionFulfillments();
				result[i] = currentRule;
			}
			return result;
		}
	}
}
