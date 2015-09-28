using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
		/// <summary>
		/// Holds the _nested
		/// </summary> 
		private ComponentCollection _nested;

		/// <summary>
		/// Gets or sets the _nested
		/// </summary> 
		public ComponentCollection Nested
		{
			get { return _nested; }
			set { _nested = value; }
		}