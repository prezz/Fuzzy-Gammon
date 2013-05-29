using System;
using System.Collections;
using System.IO;

namespace FuzzevalModule
{
	class LinguisticOutputValue
	{
		public string Identifier;
		public double CrispValue;

		public LinguisticOutputValue( string identifier, double crispValue )
		{
			Identifier = identifier;
			CrispValue = crispValue;
		}
	}


	class Defuzzifier
	{
		private Hashtable	m_OutputValues;
		private ArrayList	m_Keys;


		public static void MakeReady( Defuzzifier defuzzifier )
		{
			defuzzifier.m_OutputValues.Clear();
			
			string line;
			char[] separator = {' ', '\t', '\n'};
			string[] tokens;
			string[] s = RuleOutputs.GetOutputs();
			for ( int i = 0; i < s.Length; i++ )
			{
				line = s[i].Trim();
				if ( line.Length > 0 && line[0] != ';' )
				{
					tokens = line.Split( separator );
					LinguisticOutputValue OutVal = new LinguisticOutputValue( tokens[0], double.Parse( tokens[1] ) );
					defuzzifier.AddLinguisticOutVal( OutVal );
				}
			}
		}


		public Defuzzifier()
		{
			m_OutputValues = new Hashtable();
			m_Keys = new ArrayList();
		}


		public int Count()
		{
			return m_Keys.Count;
		}


		public void AddLinguisticOutVal( LinguisticOutputValue outVal )
		{
			if ( !m_OutputValues.ContainsKey( outVal.Identifier ) )
			{
				m_OutputValues.Add( outVal.Identifier, outVal );
				m_Keys.Add( outVal.Identifier );
			}
		}


		public string GetLinguisticIdentifier( int idx )
		{
			return ( string )m_Keys[idx];
		}


		public int GetLinguisticIdentifierIndex( string id )
		{
			for ( int i = 0; i < m_Keys.Count; i++ )
			{
				if ( id.Equals( ( string )m_Keys[i] ) )
					return i;
			}
			return -1;
		}


		public LinguisticOutputValue GetOutputValue( string identifier )
		{
			if ( m_OutputValues.ContainsKey( identifier ) )
			{
				return ( LinguisticOutputValue )m_OutputValues[identifier];
			}
			else
			{
				LogWriter.Write( "Defuzzifier failed to find defined value for " + identifier );
				return null;
			}
		}


		public double Defuzzificate( Rule[] rule )
		{
			double numerator = 0.0;
			double denominator = 0.0;

			for ( int i = 0; i < rule.Length; i++ )
			{
				numerator += GetOutputValue( rule[i].GetConclusion().Identifier ).CrispValue * rule[i].GetConclusion().Fulfillment;
				denominator += rule[i].GetConclusion().Fulfillment;
			}
			return ( denominator != 0.0 )? ( numerator / denominator ) : 0.0;
		}
	}
}
