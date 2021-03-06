using System;
using System.Collections;
using System.Data;

using NHibernate.Transaction;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// This fixture contains no mappings, it is thus faster and can be used
	/// to run tests for basic features that don't require any mapping files
	/// to function.
	/// </summary>
	[TestFixture]
	public class EmptyMappingsFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		[Test]
		public void BeginWithIsolationLevel()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction( IsolationLevel.ReadCommitted ) )
			{
				AdoTransaction at = ( AdoTransaction ) t;
				Assert.AreEqual( IsolationLevel.ReadCommitted, at.IsolationLevel );
			}
		}

		[Test, ExpectedException( typeof( ObjectDisposedException ) )]
		public void ReconnectAfterClose()
		{
			using( ISession s = OpenSession() )
			{
				s.Close();
				s.Reconnect();
			}
		}

		[Test, ExpectedException( typeof( QueryException ) )]
		public void InvalidQuery()
		{
			using( ISession s = OpenSession() )
			{
				s.Find( "from SomeInvalidClass" );
			}
		}

		[Test, ExpectedException( typeof( ArgumentNullException ) )]
		public void NullInterceptor()
		{
			IInterceptor nullInterceptor = null;
			sessions.OpenSession(nullInterceptor).Close();
		}

		[Test]
		public void DisconnectShouldNotCloseUserSuppliedConnection()
		{
			IDbConnection conn = sessions.ConnectionProvider.GetConnection();
			try
			{
				using( ISession s = OpenSession() )
				{
					s.Disconnect();
					s.Reconnect( conn );
					Assert.AreSame( conn, s.Disconnect() );
					Assert.AreEqual( ConnectionState.Open, conn.State );
				}
			}
			finally
			{
				sessions.ConnectionProvider.CloseConnection( conn );
			}
		}
	}
}
