using System;
using System.Collections;
using System.Data;
using System.Reflection;

using log4net;

using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace NHibernate.Test
{
	public abstract class TestCase
	{
		private const bool OutputDdl = false;
		protected Configuration cfg;
		protected Dialect.Dialect dialect;
		protected ISessionFactory sessions;

		private static readonly ILog log =
			LogManager.GetLogger( typeof( TestCase ) );

		private ISession lastOpenedSession;
		private DebugConnectionProvider connectionProvider;

		/// <summary>
		/// Mapping files used in the TestCase
		/// </summary>
		protected abstract IList Mappings { get; }

		/// <summary>
		/// Assembly to load mapping files from (default is NHibernate.DomainModel).
		/// </summary>
		protected virtual string MappingsAssembly
		{
			get { return "NHibernate.DomainModel"; }
		}

		/// <summary>
		/// Creates the tables used in this TestCase
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			try
			{
				ExportSchema();
			}
			catch( Exception e )
			{
				log.Error( "Error while setting up the database schema, ignoring the fixture", e );
				Assert.Ignore( "Error while setting up the database schema: " + e );
			}
		}

		/// <summary>
		/// Removes the tables used in this TestCase.
		/// </summary>
		/// <remarks>
		/// If the tables are not cleaned up sometimes SchemaExport runs into
		/// Sql errors because it can't drop tables because of the FKs.  This 
		/// will occur if the TestCase does not have the same hbm.xml files
		/// included as a previous one.
		/// </remarks>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			DropSchema();
		}

		protected virtual void OnSetUp()
		{
		}

		/// <summary>
		/// Set up the test. This method is not overridable, but it calls
		/// <see cref="OnSetUp" /> which is.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			OnSetUp();
		}

		protected virtual void OnTearDown()
		{
		}

		/// <summary>
		/// Checks that the test case cleans up after itself. This method
		/// is not overridable, but it calls <see cref="OnTearDown" /> which is.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			OnTearDown();

			bool wasClosed = CheckSessionWasClosed();
			bool wasCleaned = CheckDatabaseWasCleaned();
			bool wereConnectionsClosed = CheckConnectionsWereClosed();
			bool fail = !wasClosed || !wasCleaned || !wereConnectionsClosed;

			if( fail )
			{
				Assert.Fail( "Test didn't clean up after itself" );
			}
		}

		private bool CheckSessionWasClosed()
		{
			if( lastOpenedSession != null && lastOpenedSession.IsOpen )
			{
				log.Error( "Test case didn't close a session, closing" );
				lastOpenedSession.Close();
				return false;
			}

			return true;
		}

		private bool CheckDatabaseWasCleaned()
		{
			if( sessions.GetAllClassMetadata().Count == 0 )
			{
				// Return early in the case of no mappings, also avoiding
				// a warning when executing the HQL below.
				return true;
			}

			int objectCount;
			using( ISession s = sessions.OpenSession() )
			{
				objectCount = s.CreateQuery( "from System.Object o" ).List().Count;
			}

			if( objectCount > 0 )
			{
				log.Error( "Test case didn't clean up the database after itself, re-creating the schema" );
				DropSchema();
				ExportSchema();
				return false;
			}

			return true;
		}

		private bool CheckConnectionsWereClosed()
		{
			if( connectionProvider == null || !connectionProvider.HasOpenConnections )
			{
				return true;
			}

			log.Error( "Test case didn't close all open connections, closing" );
			connectionProvider.CloseAllConnections();
			return false;
		}

		private void ExportSchema()
		{
			ExportSchema( Mappings, MappingsAssembly );
		}

		private void ExportSchema( IList files, string assemblyName )
		{
			cfg = new Configuration();

			for( int i = 0; i < files.Count; i++ )
			{
				cfg.AddResource( assemblyName + "." + files[ i ].ToString(), Assembly.Load( assemblyName ) );
			}

			new SchemaExport( cfg ).Create( OutputDdl, true );

			sessions = cfg.BuildSessionFactory();
			dialect = Dialect.Dialect.GetDialect();
			connectionProvider = sessions.ConnectionProvider as DebugConnectionProvider;
		}

		/// <summary>
		/// Drops the schema that was built with the TestCase's Configuration.
		/// </summary>
		private void DropSchema()
		{
			sessions.Close();
			sessions = null;
			dialect = null;
			connectionProvider = null;
			lastOpenedSession = null;

			new SchemaExport( cfg ).Drop( OutputDdl, true );
			cfg = null;
		}

		public void ExecuteStatement( string sql )
		{
			ExecuteStatement( sql, true );
		}

		public void ExecuteStatement( string sql, bool error )
		{
			IDbConnection conn = null;
			IDbTransaction tran = null;
			try
			{
				if( cfg == null )
				{
					cfg = new Configuration();
				}
				IConnectionProvider prov = ConnectionProviderFactory.NewConnectionProvider( cfg.Properties );
				conn = prov.GetConnection();
				tran = conn.BeginTransaction();
				IDbCommand comm = conn.CreateCommand();
				comm.CommandText = sql;
				comm.Transaction = tran;
				comm.CommandType = CommandType.Text;
				comm.ExecuteNonQuery();
				tran.Commit();
			}
			catch( Exception )
			{
				if( tran != null )
				{
					tran.Rollback();
				}
				if( error )
				{
					throw;
				}
			}
			finally
			{
				if( conn != null )
				{
					conn.Close();
				}
			}
		}

		protected ISession OpenSession()
		{
			lastOpenedSession = sessions.OpenSession();
			return lastOpenedSession;
		}
	}
}