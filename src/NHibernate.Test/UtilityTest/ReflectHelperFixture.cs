using System;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Summary description for ReflectHelperFixture.
	/// </summary>
	[TestFixture]
	public class ReflectHelperFixture
	{
		[Test]
		public void GetConstantValueEnum()
		{
			object result = ReflectHelper.GetConstantValue( "NHibernate.DomainModel.FooStatus, NHibernate.DomainModel", "ON" );
			Assert.AreEqual( 1, ( int ) result, "Should have found value of 1" );
		}

		[Test]
		public void OverridesEquals()
		{
			Assert.IsFalse( ReflectHelper.OverridesEquals( this.GetType() ), "ReflectHelperFixture does not override equals" );
			Assert.IsTrue( ReflectHelper.OverridesEquals( typeof( string ) ), "String does override equals" );
			Assert.IsFalse( ReflectHelper.OverridesEquals( typeof( IDisposable ) ), "IDisposable does not override equals" );
			Assert.IsTrue( ReflectHelper.OverridesEquals( typeof( BRhf ) ), "Base class overrides equals" );
		}

		[Test]
		public void NoTypeFoundReturnsNull()
		{
			System.Type noType = ReflectHelper.TypeFromAssembly( "noclass", "noassembly" );
			Assert.IsNull( noType );
		}

		[Test]
		public void TypeFoundInNotLoadedAssembly()
		{
			System.Type httpRequest = ReflectHelper.TypeFromAssembly( "System.Web.HttpRequest", "System.Web" );
			Assert.IsNotNull( httpRequest );

			System.Type sameType = ReflectHelper.TypeFromAssembly( "System.Web.HttpRequest", "System.Web" );
			Assert.AreEqual( httpRequest, sameType, "should be the exact same Type" );
		}
	}

	public class ARhf
	{
		public override bool Equals( object obj )
		{
			return base.Equals( obj );
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class BRhf : ARhf
	{
	}

}