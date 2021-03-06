using System;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest 
{

	/// <summary>
	/// Test Fixture for TypeFactory.
	/// </summary>
	[TestFixture]
	public class TypeFactoryFixture 
	{

		/// <summary>
		/// Test that calling GetGuidType multiple times returns the
		/// exact same GuidType object by reference.
		/// </summary>
		[Test]
		public void GetGuidSingleton() 
		{
			NullableType guidType = NHibernateUtil.Guid;
			NullableType guidType2 = NHibernateUtil.Guid;

			Assert.AreSame(guidType, guidType2);
		}

		/// <summary>
		/// Test that Strings with different lengths return different StringTypes.
		/// </summary>
		[Test]
		public void GetStringWithDiffLength() 
		{
			NullableType string25 = TypeFactory.GetStringType(25);
			NullableType string30 = TypeFactory.GetStringType(30);

			Assert.IsFalse(string25==string30, "string25 & string30 should be different strings");
		}
	}
}
