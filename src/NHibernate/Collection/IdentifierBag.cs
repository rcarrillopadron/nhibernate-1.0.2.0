using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// An <c>IdentiferBag</c> implements "bag" semantics more efficiently than
	/// a regular <see cref="Bag" /> by adding a synthetic identifier column to the
	/// table.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The identifier is unique for all rows in the table, allowing very efficient
	/// updates and deletes.  The value of the identifier is never exposed to the 
	/// application. 
	/// </para>
	/// <para>
	/// <c>IdentifierBag</c>s may not be used for a many-to-one association.  Furthermore,
	/// there is no reason to use <c>inverse="true"</c>.
	/// </para>
	/// </remarks>
	[Serializable]
	public class IdentifierBag : PersistentCollection, IList
	{
		private IList values;	//element
		private IDictionary identifiers; //index -> id 

		internal IdentifierBag( ISessionImplementor session ) : base( session )
		{
		}

		internal IdentifierBag( ISessionImplementor session, ICollection coll ) : base( session )
		{
			IList list = coll as IList;

			if( list != null )
			{
				values = list;
			}
			else
			{
				values = new ArrayList();
				foreach( object obj in coll )
				{
					values.Add( obj );
				}
			}

			SetInitialized();
			IsDirectlyAccessible = true;
			identifiers = new Hashtable();
		}

		/// <summary>
		/// Initializes this Bag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the IdentifierBag.</param>
		/// <param name="disassembled">The disassembled IdentifierBag.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner )
		{
			BeforeInitialize( persister );
			object[ ] array = ( object[ ] ) disassembled;

			for( int i = 0; i < array.Length; i += 2 )
			{
				//object obj = persister.ElementType.Assemble( array[ i + 1 ], session, owner );
				//identifiers[ obj ] = persister.IdentifierType.Assemble( array[ i ], session, owner );
				identifiers[ i / 2 ] = persister.IdentifierType.Assemble( array[ i ], Session, owner );
				values.Add( persister.ElementType.Assemble( array[ i + 1 ], Session, owner ) );
			}

			SetInitialized();
		}

		public override bool IsWrapper( object collection )
		{
			return values == collection;
		}

		#region IList Members

		public int Add( object value )
		{
			Write();
			return values.Add( value );
		}

		public void Clear()
		{
			Write();
			values.Clear();
			identifiers.Clear();
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public object this[ int index ]
		{
			get
			{
				Read();
				return values[ index ];
			}
			set
			{
				Write();
				values[ index ] = value;
			}
		}

		public void Insert( int index, object value )
		{
			Write();
			BeforeAdd( index );
			values.Insert( index, value );
		}

		public void RemoveAt( int index )
		{
			Write();
			BeforeRemove( index );
			values.RemoveAt( index );
		}

		public void Remove( object value )
		{
			Write();
			int index = values.IndexOf( value );
			if( index >= 0 )
			{
				RemoveAt( index );
			}
		}

		public bool Contains( object value )
		{
			Read();
			return values.Contains( value );
		}

		public int IndexOf( object value )
		{
			Read();
			return values.IndexOf( value );
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		public override bool IsSynchronized
		{
			get { return false; }
		}

		public override int Count
		{
			get
			{
				Read();
				return values.Count;
			}
		}

		public override void CopyTo( Array array, int index )
		{
			Read();
			values.CopyTo( array, index );
		}

		public override object SyncRoot
		{
			get { return values.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		public override IEnumerator GetEnumerator()
		{
			Read();
			return values.GetEnumerator();
		}

		#endregion

		public override void BeforeInitialize( ICollectionPersister persister )
		{
			identifiers = new Hashtable();
			values = new ArrayList();
		}

		public override object Disassemble( ICollectionPersister persister )
		{
			object[ ] result = new object[ values.Count * 2 ];

			int i = 0;
			for( int j = 0; j < values.Count; j++ )
			{
				object val = values[ j ];
				result[ i++ ] = persister.IdentifierType.Disassemble( identifiers[ j ], Session );
				result[ i++ ] = persister.ElementType.Disassemble( val, Session );
			}

			return result;
		}

		public override bool Empty
		{
			get { return ( values.Count == 0 ); }
		}

		public override ICollection Entries()
		{
			return values;
		}

		public override bool EntryExists( object entry, int i )
		{
			return entry != null;
		}

		public override bool EqualsSnapshot( IType elementType )
		{
			IDictionary snap = ( IDictionary ) GetSnapshot();
			if( snap.Count != values.Count )
			{
				return false;
			}

			for( int i = 0; i < values.Count; i++ )
			{
				object val = values[ i ];
				object id = identifiers[ i ];
				if( id == null )
				{
					return false;
				}

				object old = snap[ id ];
				if( elementType.IsDirty( old, val, Session ) )
				{
					return false;
				}
			}

			return true;
		}

		public override ICollection GetDeletes( IType elemType )
		{
			IDictionary snap = ( IDictionary ) GetSnapshot();
			IList deletes = new ArrayList( snap.Keys );

			for( int i = 0; i < values.Count; i++ )
			{
				if( values[ i ] != null )
				{
					deletes.Remove( identifiers[ i ] );
				}
			}

			return deletes;
		}

		public override object GetIndex( object entry, int i )
		{
			return new NotImplementedException( "Bags don't have indexes" );
		}

		public override bool NeedsInserting( object entry, int i, IType elemType )
		{
			IDictionary snap = ( IDictionary ) GetSnapshot();
			object id = identifiers[ i ];

			return entry != null && ( id == null || snap[ id ] == null );
		}

		public override bool NeedsUpdating( object entry, int i, IType elemType )
		{
			if( entry == null )
			{
				return false;
			}
			IDictionary snap = ( IDictionary ) GetSnapshot();

			object id = identifiers[ i ];
			if( id == null )
			{
				return false;
			}

			object old = snap[ id ];
			return old != null && elemType.IsDirty( old, entry, Session );
		}

		public override object ReadFrom( IDataReader reader, ICollectionPersister persister, object owner )
		{
			object element = persister.ReadElement( reader, owner, Session );
			values.Add( element );
			identifiers[ values.Count - 1 ] = persister.ReadIdentifier( reader, Session );
			return element;
		}

		protected override ICollection Snapshot( ICollectionPersister persister )
		{
			IDictionary map = new Hashtable( values.Count );

			int i = 0;
			foreach( object obj in values )
			{
				object key = identifiers[ i++ ];
				map[ key ] = persister.ElementType.DeepCopy( obj );
			}

			return map;
		}

		public override ICollection GetOrphans( object snapshot )
		{
			/*
			IDictionary sn = ( IDictionary ) GetSnapshot();
			ArrayList result = new ArrayList();
			result.AddRange( sn.Values );
			PersistentCollection.IdentityRemoveAll( result, values, session );
			return result;
			*/

			return PersistentCollection.GetOrphans( ( ( IDictionary ) snapshot).Values, values, Session );
		}

		public override void PreInsert( ICollectionPersister persister )
		{
			try
			{
				int i = 0;
				foreach( object entry in values )
				{
					int loc = i++;
					if( !identifiers.Contains( loc ) ) 		// TODO: native ids
					{
						object id = persister.IdentifierGenerator.Generate( Session, entry );
						identifiers[ loc ] = id;
					}
				}
			}
			catch( Exception sqle )
			{
				throw new ADOException( "Could not generate idbag row id.", sqle );
			}
		}

		public override void WriteTo( IDbCommand st, ICollectionPersister persister, object entry, int i, bool writeOrder )
		{
			persister.WriteElement( st, entry, writeOrder, Session );
			// TODO: if not using identity columns:
			persister.WriteIdentifier( st, identifiers[ i ], writeOrder, Session );
		}

		private void BeforeRemove( int index ) 
		{
			// Move the identifier being removed to the end of the list (i.e. it isn't actually removed).
			object removedId = identifiers[ index ];
			int last = values.Count - 1;
			for( int i = index; i < last; i++ ) 
			{
				object id = identifiers[ i + 1 ];
				if( id == null ) 
				{
					identifiers.Remove( i );
				}
				else 
				{
					identifiers[ i ] = id;
				}
			}
			identifiers[ last ] = removedId;
		}

		private void BeforeAdd( int index ) 
		{
			for( int i = index; i < values.Count; i++ ) 
			{
				identifiers[ i + 1 ] = identifiers[ i ];
			}
			identifiers.Remove( index );
		}

	}
}