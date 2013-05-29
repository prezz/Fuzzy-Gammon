using System;
using System.Collections;
using GammonGame;


namespace GammonAgent
{
	public class MoveRepresentation : BoardRepresentation
	{
		private BgMove					m_Move;
		private double					m_Score;


		public MoveRepresentation( GammonInterface game, BgMove move ) : base( game )
		{
			m_Score = 0.0;
			m_Move = move;

			BgMove m = m_Move.FirstMove;
			while ( m != null )
			{
				if ( m.To != 26 && m_BoardPattern[m.To] == -1 )
				{
					m_BoardPattern[m.To] = 0;
					m_BoardPattern[0]--;
				}
				
				m_BoardPattern[m.From]--;
				if ( m.To != 26 )
					m_BoardPattern[m.To]++;

				m = m.NextMove;
			}
		}


		private MoveRepresentation( GammonInterface game ) : base( game )
		{}


		public BgMove GetMoves()
		{
			return m_Move;
		}


		public void AddScore( double score )
		{
			m_Score += score;
		}


		public double GetScore()
		{
			return m_Score;
		}

		public MoveRepresentation GetClone()
		{
			MoveRepresentation result = new MoveRepresentation( null );

			result.m_BoardPattern = new int[m_BoardPattern.Length];
			for ( int i = 0; i < m_BoardPattern.Length; i++ )
				result.m_BoardPattern[i] = m_BoardPattern[i];

			result.m_Move = BgMove.CloneFromFirst( m_Move.FirstMove );
			result.m_Score = m_Score;

			return result;
		}
	}



	public class MoveRepresentationList
	{

		class MoveRepresentationComparer : IComparer
		{
			public int Compare( object x, object y )
			{
				if ( ( ( MoveRepresentation )x ).GetScore() < ( ( MoveRepresentation )y ).GetScore() )
					return 1;
				else if ( ( ( MoveRepresentation )x ).GetScore() > ( ( MoveRepresentation )y ).GetScore() )
					return -1;
				else
					return 0;
			}
		}


		private ArrayList						m_List;
		private MoveRepresentationComparer		m_ListSort;


		public MoveRepresentationList()
		{
			m_List = new ArrayList();
			m_ListSort = new MoveRepresentationComparer();
		}


		public void SortByScore()
		{
			m_List.Sort( m_ListSort );
		}


		public void AddMoveRepresentation( MoveRepresentation MoveRepresentation, bool addIfExists )
		{
			if ( addIfExists || !DoesMoveRepresentationExist( MoveRepresentation ) )
				m_List.Add( MoveRepresentation );
		}


		private bool DoesMoveRepresentationExist( MoveRepresentation move )
		{
			for ( int i = 0; i < m_List.Count; i++ )
				if ( ( ( MoveRepresentation )m_List[i] ).PatternMatches( move ) )
					return true;

			return false;
		}
	

		public MoveRepresentation GetMoveRepresentation( int idx )
		{
			return ( MoveRepresentation )m_List[idx];
		}


		public int Count()
		{
			return m_List.Count;
		}


		public void Clear()
		{
			m_List.Clear();
		}
	}



	class MoveRepresentationGenerator
	{
		private GammonInterface			m_BgGame;
		private BgMoveList				m_MoveList;
		private BgMoveGenerator			m_MoveGenerator;

		private PatternEncoder			m_PatternEncoder;
		private string					m_CurrentGeneratedPattern;
		private MoveRepresentationList	m_CurrentGeneratedMoves;




		public MoveRepresentationGenerator( GammonInterface game )
		{
			m_BgGame = game;
			m_MoveList = new BgMoveList();
			m_MoveGenerator = new BgMoveGenerator( game );
			m_PatternEncoder = new PatternEncoder();
			m_CurrentGeneratedMoves = new MoveRepresentationList();
		}


		public void Reset()
		{
			m_CurrentGeneratedPattern = "";
		}


		public void GeneratePossibleRepresentations( MoveRepresentationList knowledgeList )
		{
			string currentPattern = m_PatternEncoder.Encode( m_BgGame );

			if ( !currentPattern.Equals( m_CurrentGeneratedPattern ) )
			{
				m_CurrentGeneratedMoves.Clear();
				m_MoveGenerator.GenerateMoves( m_MoveList );

				for ( int i = 0; i < m_MoveList.Count(); i++ )
				{
					MoveRepresentation possibleMove = new MoveRepresentation( m_BgGame, m_MoveList.GetBgMove( i ) );
					m_CurrentGeneratedMoves.AddMoveRepresentation( possibleMove, false );
				}

				m_CurrentGeneratedPattern = currentPattern;
			}

			knowledgeList.Clear();

			for ( int i = 0; i < m_CurrentGeneratedMoves.Count(); i++ )
				knowledgeList.AddMoveRepresentation( m_CurrentGeneratedMoves.GetMoveRepresentation( i ).GetClone(), true );
		}
	}
}
