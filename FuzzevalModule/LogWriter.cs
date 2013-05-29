using System;
using System.IO;

namespace FuzzevalModule
{
	class LogWriter
	{
		private static string	m_Filename = ".\\FuzzevalLog.txt";
		private static bool		m_Append = false;

		public static void Write( string line )
		{
			try
			{
				StreamWriter logWriter = new StreamWriter( m_Filename, m_Append );

				if ( m_Append == false )
					logWriter.WriteLine( "Log created: " + DateTime.Now.ToString() );

				logWriter.WriteLine( line );
				logWriter.Close();		
				m_Append = true;
			}
			catch ( Exception )
			{}
		}
	}
}
