using System;
using System.Collections;
using GammonAgent;
using GammonGame;


namespace FuzzevalModule
{
	public class LinguisticInputValue
	{
		public string	VariableName;
		public int		CrispValue;

		public LinguisticInputValue( string varName, int crispVal )
		{
			VariableName = varName;
			CrispValue = crispVal;
		}
	}
	

	public abstract class PreProcessingObject
	{
		public abstract LinguisticInputValue CreateLinguisticInputValue( BoardRepresentation board );

		public string GetName()
		{
			return this.GetType().ToString();
		}
	}



	class PreProcessor
	{
		private ArrayList	m_PreProcessorObjects;


		public static void MakeReady( PreProcessor preProc )
		{
			PreProcessingObject[] obj = PreProcessingGrapper.GetObjects();
			for ( int i = 0; i < obj.Length; i++ )
				preProc.AddPreProcessorObject( obj[i] );
		}


		public PreProcessor()
		{
			m_PreProcessorObjects = new ArrayList();
		}


		public int PreProcessorCount()
		{
			return m_PreProcessorObjects.Count;
		}


		public PreProcessingObject GetPreProcessor( int idx )
		{
			return ( PreProcessingObject )m_PreProcessorObjects[idx];
		}


		public int GetPreProcessorIdx( string LinguisticVar )
		{
			for ( int i = 0; i < m_PreProcessorObjects.Count; i++ )
				if ( ( ( PreProcessingObject )m_PreProcessorObjects[i] ).GetName().Equals( LinguisticVar ) )
					return i;
			
			return -1;
		}


		public void AddPreProcessorObject( PreProcessingObject PreProcObj )
		{
			m_PreProcessorObjects.Add( PreProcObj );
		}


		public LinguisticInputValue[] Process( BoardRepresentation board )
		{
			LinguisticInputValue[] result = new LinguisticInputValue[m_PreProcessorObjects.Count];
			for ( int i = 0; i < m_PreProcessorObjects.Count; i++ )
				result[i] = ( ( PreProcessingObject )m_PreProcessorObjects[i] ).CreateLinguisticInputValue( board );
			return result;
		}
	}
}
