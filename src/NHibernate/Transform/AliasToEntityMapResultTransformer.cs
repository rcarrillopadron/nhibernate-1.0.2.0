using System;
using System.Collections;

namespace NHibernate.Transform
{
	public class AliasToEntityMapResultTransformer : IResultTransformer
	{
		public object TransformTuple(object[] tuple, string[] aliases)
		{
			IDictionary result = new Hashtable();
			for( int i = 0; i < tuple.Length; i++ )
			{
				string alias = aliases[ i ];
				if( !alias.EndsWith( "_" ) )
				{
					// TODO: Incredibly dodgy!! what if the user defines an alias ending in "_"
					result[ alias ] = tuple[ i ];
				}
			}

			return result;
		}

		public IList TransformList(IList collection)
		{
			return collection;
		}
	}
}
