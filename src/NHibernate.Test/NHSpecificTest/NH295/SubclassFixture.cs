using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH295
{
	[TestFixture]
	public class SubclassFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.NH295.Subclass.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void LoadByIDFailure()
		{
			UserGroup ug1 = new UserGroup();
			ug1.Name = "Group1";
			User ui1 = new User( "User1" );

			ISession s = OpenSession();
			object gid1 = s.Save( ug1 );
			object uid1 = s.Save( ui1 );
			s.Flush();
			s.Close();

			s = OpenSession();
			//Load user with USER NAME: 
			ICriteria criteria1 = s.CreateCriteria( typeof( User ) );
			criteria1.Add( Expression.Expression.Eq( "Name", "User1" ) );
			Assert.AreEqual( 1, criteria1.List().Count );
			s.Flush();
			s.Close();

			s = OpenSession();
			//Load group with USER NAME: 
			ICriteria criteria2 = s.CreateCriteria( typeof( UserGroup ) );
			criteria2.Add( Expression.Expression.Eq( "Name", "User1" ) );
			Assert.AreEqual( 0, criteria2.List().Count );
			s.Flush();
			s.Close();

			s = OpenSession();
			//Load group with GROUP NAME
			ICriteria criteria3 = s.CreateCriteria( typeof( UserGroup ) );
			criteria3.Add( Expression.Expression.Eq( "Name", "Group1" ) );
			Assert.AreEqual( 1, criteria3.List().Count );
			s.Flush();
			s.Close();

			s = OpenSession();
			//Load user with GROUP NAME
			ICriteria criteria4 = s.CreateCriteria( typeof( User ) );
			criteria4.Add( Expression.Expression.Eq( "Name", "Group1" ) );
			Assert.AreEqual( 0, criteria4.List().Count );
			s.Flush();
			s.Close();

			s = OpenSession();
			//Load group with USER IDENTITY
			ug1 = ( UserGroup ) s.Get( typeof( UserGroup ), uid1 );
			Assert.IsNull( ug1 );
			s.Flush();
			s.Close();

			s = OpenSession();
			//Load group with GROUP IDENTITY
			ui1 = ( User ) s.Get( typeof( User ), gid1 );
			Assert.IsNull( ui1 );
			s.Flush();
			s.Close();

			s = OpenSession();
			Party p = ( Party ) s.Get( typeof( Party ), uid1 );
			Assert.IsTrue( p is User );
			p = ( Party ) s.Get( typeof( Party ), gid1 );
			Assert.IsTrue( p is UserGroup );
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Delete( "from Party" );
			s.Flush();
			s.Close();
		}

		[Test]
		public void List()
		{
			using( ISession s = OpenSession() )
			{
				User user = new User();
				user.Name = "user";
				UserGroup group = new UserGroup();
				group.Name = "user";

				group.Users.Add( user );

				s.Save( group );
				s.Save( user );
				s.Flush();

				s.CreateCriteria( typeof( Party ) ).List();

				s.Delete( "from Party" );
				s.Flush();
			}
		}
	}
}