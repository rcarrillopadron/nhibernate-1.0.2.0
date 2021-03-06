using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the exception messages that come out a FieldSetter when
	/// invalid values are passed in.
	/// </summary>
	[TestFixture]
	public class FieldSetterExceptionFixture
	{
		protected IPropertyAccessor _accessor;
		protected ISetter _setter;

		[SetUp]
		public void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field");
			_setter = _accessor.GetSetter( typeof(FieldSetterExceptionFixture.A), "Id" );
		}
		
		[Test]
		[ExpectedException( typeof(PropertyAccessException))]
		public void SetInvalidType()
		{
			A instance = new A();
			_setter.Set( instance, "wrong type" );
		}

		public class A
		{
			public int Id = 0;
		}
	}
}
