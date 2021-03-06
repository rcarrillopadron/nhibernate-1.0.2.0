using System;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NExpression = NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Summary description for NullExpressionFixture.
	/// </summary>
	[TestFixture]
	public class NullExpressionFixture : BaseExpressionFixture
	{
		
		[Test]
		public void NullSqlStringTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion expression = NExpression.Expression.IsNull("Address");

			SqlString sqlString = expression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "simple_alias.address is null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}
	}
}
