using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Extends the MapType to provide Sorting.
	/// </summary>
	public class SortedMapType : MapType
	{
		private IComparer comparer;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <param name="comparer"></param>
		public SortedMapType( string role, IComparer comparer ) : base( role )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override PersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			SortedMap sortedMap = new SortedMap( session, comparer );
			return sortedMap;
		}

		//public System.Type ReturnedClass {get;} -> was overridden in H2.0.3
		// because they have different Interfaces for Sorted/UnSorted - since .NET
		// doesn't have that I don't need to override it.

		/// <summary>
		/// Wraps an <see cref="IDictionary"/> in a <see cref="SortedMap"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IDictionary"/>.</param>
		/// <returns>
		/// An <see cref="SortedMap"/> that wraps the non NHibernate <see cref="IDictionary"/>.
		/// </returns>
		public override PersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new SortedMap( session, ( IDictionary ) collection, comparer );
		}
	}
}