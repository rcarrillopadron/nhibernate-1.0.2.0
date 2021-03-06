using System;
using System.Collections;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// SqlTypeFactory provides Singleton access to the SqlTypes.
	/// </summary>
	[Serializable]
	public sealed class SqlTypeFactory
	{
		// key = typeof(sqlType).Name : ie - BinarySqlType(l), BooleanSqlType, DecimalSqlType(p,s)
		// value = SqlType
		private static Hashtable sqlTypes = Hashtable.Synchronized( new Hashtable( 41 ) );
		private static object lockObject = new object(); // not used !?!

		private SqlTypeFactory()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static AnsiStringSqlType GetAnsiString( int length )
		{
			string key = GetKeyForLengthBased( typeof( AnsiStringSqlType ).Name, length );
			AnsiStringSqlType returnSqlType = ( AnsiStringSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new AnsiStringSqlType( length );
				sqlTypes.Add( key, returnSqlType );
			}
			return returnSqlType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static AnsiStringFixedLengthSqlType GetAnsiStringFixedLength( int length )
		{
			string key = GetKeyForLengthBased( typeof( AnsiStringFixedLengthSqlType ).Name, length );
			AnsiStringFixedLengthSqlType returnSqlType = ( AnsiStringFixedLengthSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new AnsiStringFixedLengthSqlType( length );
				sqlTypes.Add( key, returnSqlType );
			}
			return returnSqlType;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static BinarySqlType GetBinary( int length )
		{
			string key = GetKeyForLengthBased( typeof( BinarySqlType ).Name, length );
			BinarySqlType returnSqlType = ( BinarySqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new BinarySqlType( length );
				sqlTypes.Add( key, returnSqlType );
			}
			return returnSqlType;

		}

		/// <summary></summary>
		public static BooleanSqlType GetBoolean()
		{
			string key = typeof( BooleanSqlType ).Name;
			BooleanSqlType returnSqlType = ( BooleanSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new BooleanSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;

		}

		/// <summary></summary>
		public static ByteSqlType GetByte()
		{
			string key = typeof( ByteSqlType ).Name;
			ByteSqlType returnSqlType = ( ByteSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new ByteSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static SByteSqlType GetSByte()
		{
			string key = typeof( SByteSqlType ).Name;
			SByteSqlType returnSqlType = ( SByteSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new SByteSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static CurrencySqlType GetCurrency()
		{
			string key = typeof( CurrencySqlType ).Name;

			CurrencySqlType returnSqlType = ( CurrencySqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new CurrencySqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static DateSqlType GetDate()
		{
			string key = typeof( DateSqlType ).Name;

			DateSqlType returnSqlType = ( DateSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new DateSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static DateTimeSqlType GetDateTime()
		{
			string key = typeof( DateTimeSqlType ).Name;

			DateTimeSqlType returnSqlType = ( DateTimeSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new DateTimeSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="precision"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static DecimalSqlType GetDecimal( byte precision, byte scale )
		{
			string key = GetKeyForPrecisionScaleBased( typeof( DecimalSqlType ).Name, precision, scale );

			DecimalSqlType returnSqlType = ( DecimalSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new DecimalSqlType( precision, scale );
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;

		}

		/// <summary></summary>
		public static DoubleSqlType GetDouble()
		{
			string key = typeof( DoubleSqlType ).Name;

			DoubleSqlType returnSqlType = ( DoubleSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new DoubleSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static GuidSqlType GetGuid()
		{
			string key = typeof( GuidSqlType ).Name;

			GuidSqlType returnSqlType = ( GuidSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new GuidSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static Int16SqlType GetInt16()
		{
			string key = typeof( Int16SqlType ).Name;

			Int16SqlType returnSqlType = ( Int16SqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new Int16SqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static UInt16SqlType GetUInt16()
		{
			string key = typeof( UInt16SqlType ).Name;

			UInt16SqlType returnSqlType = ( UInt16SqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new UInt16SqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static Int32SqlType GetInt32()
		{
			string key = typeof( Int32SqlType ).Name;

			Int32SqlType returnSqlType = ( Int32SqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new Int32SqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static UInt32SqlType GetUInt32()
		{
			string key = typeof( UInt32SqlType ).Name;

			UInt32SqlType returnSqlType = ( UInt32SqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new UInt32SqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static Int64SqlType GetInt64()
		{
			string key = typeof( Int64SqlType ).Name;

			Int64SqlType returnSqlType = ( Int64SqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new Int64SqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static UInt64SqlType GetUInt64()
		{
			string key = typeof( UInt64SqlType ).Name;

			UInt64SqlType returnSqlType = ( UInt64SqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new UInt64SqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static SingleSqlType GetSingle()
		{
			string key = typeof( SingleSqlType ).Name;

			SingleSqlType returnSqlType = ( SingleSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new SingleSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static StringSqlType GetString( int length )
		{
			string key = GetKeyForLengthBased( typeof( StringSqlType ).Name, length );

			StringSqlType returnSqlType = ( StringSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new StringSqlType( length );
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static StringFixedLengthSqlType GetStringFixedLength( int length )
		{
			string key = GetKeyForLengthBased( typeof( StringFixedLengthSqlType ).Name, length );

			StringFixedLengthSqlType returnSqlType = ( StringFixedLengthSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new StringFixedLengthSqlType( length );
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		/// <summary></summary>
		public static TimeSqlType GetTime()
		{
			string key = typeof( TimeSqlType ).Name;

			TimeSqlType returnSqlType = ( TimeSqlType ) sqlTypes[ key ];
			if( returnSqlType == null )
			{
				returnSqlType = new TimeSqlType();
				sqlTypes.Add( key, returnSqlType );
			}

			return returnSqlType;
		}

		private static string GetKeyForLengthBased( string name, int length )
		{
			return name + "(" + length + ")";
		}

		private static string GetKeyForPrecisionScaleBased( string name, byte precision, byte scale )
		{
			return name + "(" + precision + ", " + scale + ")";
		}
	}
}