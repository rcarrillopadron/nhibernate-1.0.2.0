using System;
using System.Collections;

using NUnit.Framework;
using NHibernate;
using NHibernate.DomainModel;


namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for ABCTest.
	/// </summary>
	[TestFixture]
	public class ABCTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[] { "ABC.hbm.xml"};
			}
		}

		[Test]
		public void HigherLevelIndexDefinition()
		{
			string[] commands = cfg.GenerateSchemaCreationScript( dialect );
			int max = commands.Length;
			bool found = false;
			
			foreach( string command in commands )
			{
				System.Console.WriteLine("Checking command : " + command);
				found = command.IndexOf("create index indx_a_name") >= 0;
				if (found)
					break;
			}
			Assert.IsTrue( found, "Unable to locate indx_a_name index creation" );
		}

		[Test]
		public void Subselect()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				B b = new B();
				IDictionary map = new Hashtable();
				map["a"] = "a";
				map["b"] = "b";
				b.Map = map;
				s.Save(b);
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				B b = (B) s.CreateQuery( "from B" ).UniqueResult();
				t.Commit();
			}

			if( dialect is Dialect.FirebirdDialect )
			{
				// Firebird has problems deleting the map contents
				ExecuteStatement( "delete from Map" );
				ExecuteStatement( "delete from A" );
			}
			else
			{
				using( ISession s = OpenSession() )
				using( ITransaction t = s.BeginTransaction() )
				{
					s.Delete( "from B" );
					t.Commit();
				}
			}
		}

		[Test]
		public void Subclassing() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			C1 c1 = new C1();
			D d = new D();
			d.Amount =213.34f;
			// id used to be a increment
			c1.Id = 1;
			c1.Address = "foo bar";
			c1.Count = 23432;
			c1.Name ="c1";
			c1.D = d;
			s.Save(c1);
			d.Id = c1.Id;
			s.Save(d);
		
			Assert.IsTrue( s.Find("from c in class C2 where 1=1 or 1=1").Count ==0 );
		
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(A), c1.Id );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.Amount>213.3f
				);
			t.Commit();
			s.Close();
		
			s = OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(B), c1.Id );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.Amount>213.3f
				);
			t.Commit();
			s.Close();
		
			s = OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(C1), c1.Id );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.Amount>213.3f
				);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Find("from b in class B");
			t.Commit();
			s.Close();

			// need to clean up the objects created by this test or Subselect() will fail
			// because there are rows in the table.  There must be some difference in the order
			// that NUnit and JUnit run their tests.
			s = OpenSession();
			t = s.BeginTransaction();

			IList aList = s.Find( "from A" );
			IList dList = s.Find( "from D" );

			foreach( A aToDelete in aList )
			{
				s.Delete( aToDelete );
			}

			foreach( D dToDelete in dList )
			{
				s.Delete( dToDelete );
			}

			t.Commit();
			s.Close();
		}
	}
}
