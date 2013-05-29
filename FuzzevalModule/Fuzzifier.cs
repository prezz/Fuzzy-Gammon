using System;
using System.Collections;
using System.IO;


namespace FuzzevalModule
{
	class FuzzifiedValue
	{
		public string	VariableName;
		public string	TermName;
		public double	FuzzyValue;

		public FuzzifiedValue( string varName, string termName, double fuzzyVal )
		{
			VariableName = varName;
			TermName = termName;
			FuzzyValue = fuzzyVal;
		}
	}


	class FuzzifiedValueContainer
	{
		class FvKey
		{
			public string	VariableName;
			public string	TermName;

			public FvKey( string var, string term )
			{
				VariableName = var;
				TermName = term;
			}
		}

		private Hashtable	m_FuzzyValContainer;
		private ArrayList	m_FvKeys;


		public FuzzifiedValueContainer()
		{
			m_FuzzyValContainer = new Hashtable();
			m_FvKeys = new ArrayList();
		}


		public void Add( FuzzifiedValue fuzzyVal )
		{
			if ( m_FuzzyValContainer.ContainsKey( fuzzyVal.VariableName ) )
			{
				Hashtable ht = ( Hashtable )m_FuzzyValContainer[fuzzyVal.VariableName];
				if ( !ht.ContainsKey( fuzzyVal.TermName ) )
				{
					ht.Add( fuzzyVal.TermName, fuzzyVal );
					m_FvKeys.Add( new FvKey( fuzzyVal.VariableName, fuzzyVal.TermName ) );
				}
			}
			else
			{
				Hashtable ht = new Hashtable();
				ht.Add( fuzzyVal.TermName, fuzzyVal );
				m_FuzzyValContainer.Add( fuzzyVal.VariableName, ht );
				m_FvKeys.Add( new FvKey( fuzzyVal.VariableName, fuzzyVal.TermName ) );
			}
		}

		public int Count()
		{
			return m_FvKeys.Count;
		}


		public FuzzifiedValue Get( int idx )
		{
			FvKey k = ( FvKey )m_FvKeys[idx];
			return Get( k.VariableName, k.TermName );
		}


		public FuzzifiedValue Get( string variableName, string termName )
		{
			if ( m_FuzzyValContainer.ContainsKey( variableName ) )
			{
				Hashtable ht = ( Hashtable )m_FuzzyValContainer[variableName];
				if ( ht.ContainsKey( termName ) )
					return ( FuzzifiedValue )ht[termName];
			}
			LogWriter.Write( "FuzzifiedValueContainer.Get(...) failed to find " + variableName + " " + termName );
			return null;
		}
	}


	class Fuzzifier
	{
		class MfKey
		{
			public string Variable;
			public string Term;

			public MfKey( string var, string term )
			{
				Variable = var;
				Term = term;
			}
		}

		private Hashtable	m_MembershipFunctions;
		private ArrayList	m_MfKeys;

	
		public static void LoadMemberFunctions( Fuzzifier fuzzifier, string file )
		{
			try
			{
				if ( File.Exists( file ) )
				{
					fuzzifier.m_MembershipFunctions.Clear();
					
					StreamReader reader = new StreamReader( file );
					string line;
					char[] separator = {' ', '\t', '\n'};
					string[] tokens;
					while ( ( line = reader.ReadLine() ) != null )
					{
						line = line.Trim();
						if ( line.Length > 0 && line[0] != ';' )
						{
							tokens = line.Split( separator );
							MfInterface memberFunc = new MembershipFunction( tokens[0], tokens[1], double.Parse( tokens[2] ), double.Parse( tokens[3]), double.Parse( tokens[4] ) );
						
							if ( tokens.Length > 5 && tokens[5].Equals("*") )
								memberFunc.Lock( true );
						
							fuzzifier.AddMembershipFunction( memberFunc );
						}
					}
					reader.Close();
				}
			}
			catch ( Exception e )
			{
				LogWriter.Write( e.ToString() );
			}

		}

/*
		public static void LoadRedefinedMemberFunctions( Fuzzifier fuzzifier, string file )
		{
			try
			{
//				fuzzifier.m_MembershipFunctions.Clear();
					
				StreamReader reader = new StreamReader( file );
				string line;
				char[] separator = {' ', '\t', '\n'};
				string[] tokens;
				while ( ( line = reader.ReadLine() ) != null )
				{
					line = line.Trim();
					if ( line.Length > 0 && line[0] != ';' )
					{
						tokens = line.Split( separator );
						MfInterface[] memberFuncToWrap = fuzzifier.GetMembershipFunctions( tokens[1] );

						for ( int i = 0; i < memberFuncToWrap.Length; i++ )
						{
							MfInterface wrapperMf = new RedefinedMemberFunction( tokens[0], memberFuncToWrap[i] );
							fuzzifier.AddMembershipFunction( wrapperMf );
						}
					}
				}
				reader.Close();
			}
			catch ( Exception e )
			{
				LogWriter.Write( e.ToString() );
			}
		}
*/

		public static void SaveMemberFunctions( Fuzzifier fuzzifier, string file )
		{
			try
			{
				StreamWriter writer = new StreamWriter( file, false );
			
				IDictionaryEnumerator htEnumerator = fuzzifier.m_MembershipFunctions.GetEnumerator();
				while ( htEnumerator.MoveNext() )
				{
					ArrayList currentFunctions = ( ArrayList )htEnumerator.Value;
					for ( int i = 0; i < currentFunctions.Count; i++ )
					{
						string locked = "";
						MfInterface memberFuncToSave = ( MfInterface )currentFunctions[i];

						if ( memberFuncToSave.IsLocked() )
							locked = " *";

						if ( memberFuncToSave.SaveAble() )
							writer.WriteLine( memberFuncToSave.VariableName() + " " + memberFuncToSave.TermName() + " " + memberFuncToSave.BaseLeft() + " " + memberFuncToSave.CenterMax() + " " + memberFuncToSave.BaseRight() + locked );
					}
				}
				writer.Close();
			}
			catch ( Exception )
			{}
		}


		public static void MakeReady( Fuzzifier fuzzifier )
		{
			fuzzifier.m_MembershipFunctions.Clear();

			string line;
			char[] separator = {' ', '\t', '\n'};
			string[] tokens;
			string[] s = MemberFunctionGrapper.GetFunctions();
			for ( int i = 0; i < s.Length; i++ )
			{
				line = s[i].Trim();
				if ( line.Length > 0 && line[0] != ';' )
				{
					tokens = line.Split( separator );
					MfInterface memberFunc = new MembershipFunction( tokens[0], tokens[1], double.Parse( tokens[2] ), double.Parse( tokens[3]), double.Parse( tokens[4] ) );
						
					if ( tokens.Length > 5 && tokens[5].Equals("*") )
						memberFunc.Lock( true );
						
					fuzzifier.AddMembershipFunction( memberFunc );
				}
			}
		}


		public Fuzzifier()
		{
			m_MembershipFunctions = new Hashtable();
			m_MfKeys = new ArrayList();
		}
        

		public void AddMembershipFunction( MfInterface memberFunc )
		{
			if ( m_MembershipFunctions.ContainsKey( memberFunc.VariableName() ) )
			{
				ArrayList al = ( ArrayList )m_MembershipFunctions[memberFunc.VariableName()];
				for ( int i = 0; i < al.Count; i++ )
					if ( ( ( MfInterface )al[i] ).TermName().Equals( memberFunc.TermName() ) )
						return;
				al.Add( memberFunc );
				m_MfKeys.Add( new MfKey( memberFunc.VariableName(), memberFunc.TermName() ) );
			}
			else
			{
				ArrayList al = new ArrayList();
				al.Add( memberFunc );
				m_MembershipFunctions.Add( memberFunc.VariableName(), al );
				m_MfKeys.Add( new MfKey( memberFunc.VariableName(), memberFunc.TermName() ) );
			}
		}


		public MfInterface[] GetMembershipFunctions( string lingisticVar )
		{
			if ( m_MembershipFunctions.ContainsKey( lingisticVar ) )
			{
				ArrayList al = ( ArrayList )m_MembershipFunctions[lingisticVar];
				MfInterface[] result = new MfInterface[al.Count];
				for ( int i = 0; i < al.Count; i++ )
					result[i] = ( MfInterface )al[i];
				return result;
			}
			LogWriter.Write( "Fuzzifier does not contain membership functions for " + lingisticVar );
			return null;
		}


		public int MemberFunctionsCount()
		{
			return m_MfKeys.Count;
		}


		public MfInterface GetMembershipFunction( int idx )
		{
			MfKey k = ( MfKey )m_MfKeys[ idx ];
			return GetMembershipFunction( k.Variable, k.Term );
		}


		public MfInterface GetMembershipFunction( string linguisticVar, string linguisticTerm )
		{
			MfInterface[] func = GetMembershipFunctions( linguisticVar );

			for ( int i = 0; i < func.Length; i++ )
				if ( func[i].TermName().Equals( linguisticTerm ) )
					return func[i];

			return null;
		}


		public int GetMembershipFunctionId( string linguisticVar, string linguisticTerm )
		{
			for ( int i = 0; i < MemberFunctionsCount(); i++ )
			{
				MfInterface mf = GetMembershipFunction( i );
				if ( mf.VariableName().Equals( linguisticVar ) && mf.TermName().Equals( linguisticTerm ) )
					return i;
			}
			return -1;
		}


		public FuzzifiedValueContainer Fuzzificate( LinguisticInputValue[] inputs )
		{
			FuzzifiedValueContainer result = new FuzzifiedValueContainer();

			for ( int i = 0; i < inputs.Length; i++ )
			{
				MfInterface[] memberFunc = GetMembershipFunctions( inputs[i].VariableName );
				if ( memberFunc != null )
				{
					for ( int j = 0; j < memberFunc.Length; j++ )
					{
						FuzzifiedValue fuzzyVal = new FuzzifiedValue( memberFunc[j].VariableName(), memberFunc[j].TermName(), memberFunc[j].FuzzifieValue( inputs[i].CrispValue ) );
						result.Add( fuzzyVal );
					}
				}
				else
				{
					LogWriter.Write( "Could not fuzzificate " + inputs[i].VariableName );
				}
			}
			return result;
		}
	}
}
