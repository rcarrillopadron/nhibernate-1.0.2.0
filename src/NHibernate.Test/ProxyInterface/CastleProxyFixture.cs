using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace NHibernate.Test.ProxyInterface
{
	/// <summary>
	/// Summary description for CastleProxyFixture.
	/// </summary>
	[TestFixture]
	public class CastleProxyFixture : TestCase 
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "ProxyInterface.CastleProxyImpl.hbm.xml" };
			}
		}

		[Test]
		public void Proxy() 
		{
			ISession s = OpenSession();
			CastleProxy ap = new CastleProxyImpl();
			ap.Id = 1;
			ap.Name = "first proxy";
			s.Save( ap );
			s.Flush();
			s.Close();

			s = OpenSession();
			ap = (CastleProxy)s.Load( typeof(CastleProxyImpl), ap.Id ); 
			Assert.IsFalse( NHibernateUtil.IsInitialized( ap ) );
			int id = ap.Id;
			Assert.IsFalse( NHibernateUtil.IsInitialized( ap ), "get id should not have initialized it." );
			string name = ap.Name;
			Assert.IsTrue(  NHibernateUtil.IsInitialized( ap ), "get name should have initialized it." );
			s.Delete( ap );
			s.Flush();
			s.Close();
		}

		private void SerializeAndDeserialize(ref ISession s)
		{
			// Serialize the session
			using( Stream stream = new MemoryStream() )
			{
				IFormatter formatter = new BinaryFormatter( );
				formatter.Serialize(stream, s);

				// Close the original session
				s.Close();

				// Deserialize the session
				stream.Position = 0;
				s = (ISession)formatter.Deserialize(stream);
			}
		}

		[Test]
		public void ProxySerialize() 
		{
			ISession s = OpenSession();
			CastleProxy ap = new CastleProxyImpl();
			ap.Id = 1;
			ap.Name = "first proxy";
			s.Save( ap );
			s.Flush();
			s.Close();

			s = OpenSession();
			ap = (CastleProxy)s.Load( typeof(CastleProxyImpl), ap.Id ); 
			Assert.AreEqual( 1, ap.Id );
			s.Disconnect();

			SerializeAndDeserialize(ref s);

			s.Reconnect();
			s.Disconnect();

			// serialize and then deserialize the session again - make sure Castle.DynamicProxy
			// has no problem with serializing two times - earlier versions of it did.
			SerializeAndDeserialize(ref s);

			s.Close();

			s = OpenSession();
			s.Delete( ap );
			s.Flush();
			s.Close();
		}

		[Test]
		public void SerializeNotFoundProxy() 
		{
			ISession s = OpenSession();
			// this does not actually exists in db
			CastleProxy notThere = (CastleProxy)s.Load( typeof(CastleProxyImpl), 5 ); 
			Assert.AreEqual( 5, notThere.Id );
			s.Disconnect();

			// serialize and then deserialize the session.
			SerializeAndDeserialize(ref s);

			Assert.IsNotNull( s.Load( typeof(CastleProxyImpl), 5 ), "should be proxy - even though it doesn't exists in db" );			
			s.Close();
		}

	}
}
