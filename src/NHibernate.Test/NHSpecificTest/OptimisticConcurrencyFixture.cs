using System;
using System.Collections;

using NHibernate.DomainModel;
using NHibernate.DomainModel.NHSpecific;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class OptimisticConcurrencyFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[ ] {"Multi.hbm.xml", "NHSpecific.Optimistic.hbm.xml"}; }
		}

		[Test]
		[ExpectedException( typeof( StaleObjectStateException ) )]
		public void StaleObjectStateCheckWithNormalizedEntityPersister()
		{
			Top top = new Top();
			top.Name = "original name";

			try
			{
				using( ISession session = OpenSession() )
				{
					session.Save( top );
					session.Flush();

					using( ISession concurrentSession = OpenSession() )
					{
						Top sameTop = ( Top ) concurrentSession.Get( typeof( Top ), top.Id );
						sameTop.Name = "another name";
						concurrentSession.Flush();
					}

					top.Name = "new name";
					session.Flush();
				}
			}
			finally
			{
				using( ISession session = OpenSession() )
				{
					session.Delete( "from Top" );
					session.Flush();
				}
			}
		}

		[Test]
		[ExpectedException( typeof( StaleObjectStateException ) )]
		[Ignore( "Not implemented yet, throws HibernateException instead of StaleObjectStateException" )]
		public void StaleObjectStateCheckWithEntityPersisterAndOptimisticLock()
		{
			Optimistic optimistic = new Optimistic();
			optimistic.String = "original string";

			try
			{
				using( ISession session = OpenSession() )
				{
					session.Save( optimistic );
					session.Flush();

					using( ISession concurrentSession = OpenSession() )
					{
						Optimistic sameOptimistic = ( Optimistic ) concurrentSession.Get( typeof( Optimistic ), optimistic.Id );
						sameOptimistic.String = "another string";
						concurrentSession.Flush();
					}

					optimistic.String = "new string";
					session.Flush();
				}
			}
			finally
			{
				using( ISession session = OpenSession() )
				{
					session.Delete( "from Optimistic" );
					session.Flush();
				}
			}
		}
	}
}