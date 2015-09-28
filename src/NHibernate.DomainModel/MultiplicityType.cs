using System;
using System.Data;

namespace NHibernate.DomainModel
{
	public class MultiplicityType : ICompositeUserType 
	{
		{
			get { return PROP_NAMES; }
		{
			get { return TYPES; }
		{
		{
			{
			{
		public System.Type ReturnedClass
		{
			get { return typeof(Multiplicity); }
		public new bool Equals(object x, object y) 
		{
			Multiplicity my = (Multiplicity) y;
			if (mx==my) return true;
			if (mx==null || my==null) return false;
			return mx.count==my.count && mx.glarch==my.glarch;
			int c = (int) NHibernateUtil.Int32.NullSafeGet( rs, names[0], session, owner);
			GlarchProxy g = (GlarchProxy) NHibernateUtil.Entity(typeof(Glarch)).NullSafeGet(rs, names[1], session, owner);
			Multiplicity m = new Multiplicity();
			m.count = ( c==0 ? 0 : c );
			m.glarch = g;
			return m;
		}

		public void NullSafeSet(IDbCommand st, Object value, int index, Engine.ISessionImplementor session)
			Multiplicity o = (Multiplicity) value;
			GlarchProxy g;
			int c;
			if (o==null) 
			{
				g=null;
				c=0;
			}
			else 
			{
				g = o.glarch;
				c = o.count;
			}
			NHibernateUtil.Int32.NullSafeSet(st, c, index, session);
			NHibernateUtil.Entity(typeof(Glarch)).NullSafeSet(st, g, index+1, session);
		{
			Multiplicity v = (Multiplicity) value;
			Multiplicity m = new Multiplicity();
			m.count = v.count;
			m.glarch = v.glarch;
			return m;
		public bool IsMutable
		{
			get { return true; }
		public object Assemble(object cached, Engine.ISessionImplementor session, Object owner) 
		{
		public object Disassemble(Object value, Engine.ISessionImplementor session) 
		{