/* Copyright (c) 2002 by Insight Enterprise Systems, Inc., and by Jason Smith. */ 
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace Iesi.Collections
{
	/// <summary>
	/// <p>A collection that contains no duplicate elements.  This class models the mathematical
	/// <c>Set</c> abstraction, and is the base class for all other <c>Set</c> implementations.</p>
	/// 
	/// <p><c>Set</c> overrides the <c>Equals()</c> method to test for "equivalency": whether the 
	/// two sets contain the same elements.  The "==" and "!=" operators are not overridden by 
	/// design, since it is often desirable to compare object references for equality.</p>
	/// 
	/// <p>Also, the <c>GetHashCode()</c> method is not overridden on any of the set implementations, since none
	/// of them are truely immutable.  This is by design, and it is the way almost all collection in 
	/// the .NET framework function.  So as a general rule, don't store collection objects inside <c>Set</c>
	/// instances.  You would typically want to use a keyed <c>IDictionary</c> instead.</p>
	/// 
	/// <p>None of the <c>Set</c> implementations in this library are guranteed to be thread-safe
	/// in any way unless wrapped in a <c>SynchronizedSet</c>.</p>
	/// 
	/// <p>The following table summarizes the binary operators that are supported by the <c>Set</c> class.</p>
	/// <list type="table">
	///		<listheader>
	///			<term>Operation</term>
	///			<term>Description</term>
	///			<term>Method</term>
	///			<term>Operator</term>
	///		</listheader>
	///		<item>
	///			<term>Union (OR)</term>
	///			<term>Element included in result if it exists in either <c>A</c> OR <c>B</c>.</term>
	///			<term><c>Union()</c></term>
	///			<term><c>|</c></term>
	///		</item>
	///		<item>
	///			<term>Intersection (AND)</term>
	///			<term>Element included in result if it exists in both <c>A</c> AND <c>B</c>.</term>
	///			<term><c>InterSect()</c></term>
	///			<term><c>&amp;</c></term>
	///		</item>
	///		<item>
	///			<term>Exclusive Or (XOR)</term>
	///			<term>Element included in result if it exists in one, but not both, of <c>A</c> and <c>B</c>.</term>
	///			<term><c>ExclusiveOr()</c></term>
	///			<term><c>^</c></term>
	///		</item>
	///		<item>
	///			<term>Minus (n/a)</term>
	///			<term>Take all the elements in <c>A</c>.  Now, if any of them exist in <c>B</c>, remove
	///			them.  Note that unlike the other operators, <c>A - B</c> is not the same as <c>B - A</c>.</term>
	///			<term><c>Minus()</c></term>
	///			<term><c>-</c></term>
	///		</item>
	/// </list>
	/// </summary>
	public abstract class Set : ICollection, ICloneable
	{
		/// <summary>
		/// Performs a "union" of the two sets, where all the elements
		/// in both sets are present.  That is, the element is included if it is in either <c>a</c> or <c>b</c>.
		/// Neither this set nor the input set are modified during the operation.  The return value
		/// is a <c>Clone()</c> of this set with the extra elements added in.
		/// </summary>
		/// <param name="a">A collection of elements.</param>
		/// <returns>A new <c>Set</c> containing the union of this <c>Set</c> with the specified collection.
		/// Neither of the input objects is modified by the union.</returns>
		public Set Union(Set a)
		{
			Set resultSet = (Set)this.Clone();
			if(a != null)
				resultSet.AddAll(a);
			return resultSet;
		}
		
		/// <summary>
		/// Performs a "union" of two sets, where all the elements
		/// in both are present.  That is, the element is included if it is in either <c>a</c> or <c>b</c>.
		/// The return value is a <c>Clone()</c> of one of the sets (<c>a</c> if it is not <c>null</c>) with elements of the other set
		/// added in.  Neither of the input sets is modified by the operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>A set containing the union of the input sets.  <c>null</c> if both sets are <c>null</c>.</returns>
		public static Set Union(Set a, Set b)
		{
			if (a == null && b == null)
				return null;
			else if(a == null)
				return (Set)b.Clone();
			else if(b == null)
				return (Set)a.Clone();
			else
				return a.Union(b);
		}

		/// <summary>
		/// Performs a "union" of two sets, where all the elements
		/// in both are present.  That is, the element is included if it is in either <c>a</c> or <c>b</c>.
		/// The return value is a <c>Clone()</c> of one of the sets (<c>a</c> if it is not <c>null</c>) with elements of the other set
		/// added in.  Neither of the input sets is modified by the operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>A set containing the union of the input sets.  <c>null</c> if both sets are <c>null</c>.</returns>
		public static Set operator | (Set a, Set b)
		{
			return Union(a, b);
		}

		/// <summary>
		/// Performs an "intersection" of the two sets, where only the elements
		/// that are present in both sets remain.  That is, the element is included if it exists in
		/// both sets.  The <c>Intersect()</c> operation does not modify the input sets.  It returns
		/// a <c>Clone()</c> of this set with the appropriate elements removed.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>The intersection of this set with <c>a</c>.</returns>
		public Set Intersect(Set a)
		{
			Set resultSet = (Set)this.Clone();
			if(a != null)
				resultSet.RetainAll(a);
			else
				resultSet.Clear();
			return resultSet;
		}

		/// <summary>
		/// Performs an "intersection" of the two sets, where only the elements
		/// that are present in both sets remain.  That is, the element is included only if it exists in
		/// both <c>a</c> and <c>b</c>.  Neither input object is modified by the operation.
		/// The result object is a <c>Clone()</c> of one of the input objects (<c>a</c> if it is not <c>null</c>) containing the
		/// elements from the intersect operation. 
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>The intersection of the two input sets.  <c>null</c> if both sets are <c>null</c>.</returns>
		public static Set Intersect(Set a, Set b)
		{
			if(a == null && b == null)
				return null;
			else if(a == null)
			{
				Set resultSet = (Set)((Set)b).Clone();
				resultSet.Clear();
				return resultSet;
			}
			else if(b == null)
			{
				Set resultSet = (Set)((Set)a).Clone();
				resultSet.Clear();
				return resultSet;
			}
			else
				return a.Intersect(b);
		}

		/// <summary>
		/// Performs an "intersection" of the two sets, where only the elements
		/// that are present in both sets remain.  That is, the element is included only if it exists in
		/// both <c>a</c> and <c>b</c>.  Neither input object is modified by the operation.
		/// The result object is a <c>Clone()</c> of one of the input objects (<c>a</c> if it is not <c>null</c>) containing the
		/// elements from the intersect operation. 
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>The intersection of the two input sets.  <c>null</c> if both sets are <c>null</c>.</returns>
		public static Set operator & (Set a, Set b)
		{
			return Intersect(a, b);
		}

		/// <summary>
		/// Performs a "minus" of set <c>b</c> from set <c>a</c>.  This returns a set of all
		/// the elements in set <c>a</c>, removing the elements that are also in set <c>b</c>.
		/// The original sets are not modified during this operation.  The result set is a <c>Clone()</c>
		/// of this <c>Set</c> containing the elements from the operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>A set containing the elements from this set with the elements in <c>a</c> removed.</returns>
		public Set Minus(Set a)
		{
			Set resultSet = (Set)this.Clone();
			if(a != null)
				resultSet.RemoveAll(a);
			return resultSet;
		}

		/// <summary>
		/// Performs a "minus" of set <c>b</c> from set <c>a</c>.  This returns a set of all
		/// the elements in set <c>a</c>, removing the elements that are also in set <c>b</c>.
		/// The original sets are not modified during this operation.  The result set is a <c>Clone()</c>
		/// of set <c>a</c> containing the elements from the operation. 
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>A set containing <c>A - B</c> elements.  <c>null</c> if <c>a</c> is <c>null</c>.</returns>
		public static Set Minus(Set a, Set b)
		{
			if(a == null)
				return null;
			else
				return a.Minus(b);
		}

		/// <summary>
		/// Performs a "minus" of set <c>b</c> from set <c>a</c>.  This returns a set of all
		/// the elements in set <c>a</c>, removing the elements that are also in set <c>b</c>.
		/// The original sets are not modified during this operation.  The result set is a <c>Clone()</c>
		/// of set <c>a</c> containing the elements from the operation. 
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>A set containing <c>A - B</c> elements.  <c>null</c> if <c>a</c> is <c>null</c>.</returns>
		public static Set operator - (Set a, Set b)
		{
			return Minus(a, b);
		}


		/// <summary>
		/// Performs an "exclusive-or" of the two sets, keeping only the elements that
		/// are in one of the sets, but not in both.  The original sets are not modified
		/// during this operation.  The result set is a <c>Clone()</c> of this set containing
		/// the elements from the exclusive-or operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>A set containing the result of <c>a ^ b</c>.</returns>
		public Set ExclusiveOr(Set a)
		{
			Set resultSet = (Set)this.Clone();
			foreach(object element in a)
			{
				if(resultSet.Contains(element))
					resultSet.Remove(element);
				else
					resultSet.Add(element);
			}
			return resultSet;
		}

		/// <summary>
		/// Performs an "exclusive-or" of the two sets, keeping only the elements that
		/// are in one of the sets, but not in both.  The original sets are not modified
		/// during this operation.  The result set is a <c>Clone()</c> of one of the sets
		/// (<c>a</c> if it is not <c>null</c>) containing
		/// the elements from the exclusive-or operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>A set containing the result of <c>a ^ b</c>.  <c>null</c> if both sets are <c>null</c>.</returns>
		public static Set ExclusiveOr(Set a, Set b)
		{
			if(a == null && b == null)
				return null;
			else if(a == null)
				return (Set)b.Clone();
			else if(b == null)
				return (Set)a.Clone();
			else
				return a.ExclusiveOr(b);
		}

		/// <summary>
		/// Performs an "exclusive-or" of the two sets, keeping only the elements that
		/// are in one of the sets, but not in both.  The original sets are not modified
		/// during this operation.  The result set is a <c>Clone()</c> of one of the sets
		/// (<c>a</c> if it is not <c>null</c>) containing
		/// the elements from the exclusive-or operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <param name="b">A set of elements.</param>
		/// <returns>A set containing the result of <c>a ^ b</c>.  <c>null</c> if both sets are <c>null</c>.</returns>
		public static Set operator ^ (Set a, Set b)
		{
			return ExclusiveOr(a, b);
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// </summary>
		/// <param name="o">The object to add to the set.</param>
		/// <returns><c>true</c> is the object was added, <c>false</c> if it was already present.</returns>
		public abstract bool Add(object o);

		/// <summary>
		/// Adds all the elements in the specified collection to the set if they are not already present.
		/// </summary>
		/// <param name="c">A collection of objects to add to the set.</param>
		/// <returns><c>true</c> is the set changed as a result of this operation, <c>false</c> if not.</returns>
		public abstract bool AddAll(ICollection c);

		/// <summary>
		/// Removes all objects from the set.
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Returns <c>true</c> if this set contains the specified element.
		/// </summary>
		/// <param name="o">The element to look for.</param>
		/// <returns><c>true</c> if this set contains the specified element, <c>false</c> otherwise.</returns>
		public abstract bool Contains(object o);

		/// <summary>
		/// Returns <c>true</c> if the set contains all the elements in the specified collection.
		/// </summary>
		/// <param name="c">A collection of objects.</param>
		/// <returns><c>true</c> if the set contains all the elements in the specified collection, <c>false</c> otherwise.</returns>
		public abstract bool ContainsAll(ICollection c);

		/// <summary>
		/// Returns <c>true</c> if this set contains no elements.
		/// </summary>
		public abstract bool IsEmpty{get;}

		/// <summary>
		/// Removes the specified element from the set.
		/// </summary>
		/// <param name="o">The element to be removed.</param>
		/// <returns><c>true</c> if the set contained the specified element, <c>false</c> otherwise.</returns>
		public abstract bool Remove(object o);

		/// <summary>
		/// Remove all the specified elements from this set, if they exist in this set.
		/// </summary>
		/// <param name="c">A collection of elements to remove.</param>
		/// <returns><c>true</c> if the set was modified as a result of this operation.</returns>
		public abstract bool RemoveAll(ICollection c);


		/// <summary>
		/// Retains only the elements in this set that are contained in the specified collection.
		/// </summary>
		/// <param name="c">Collection that defines the set of elements to be retained.</param>
		/// <returns><c>true</c> if this set changed as a result of this operation.</returns>
		public abstract bool RetainAll(ICollection c);

		/// <summary>
		/// Returns a clone of the <c>Set</c> instance.  This will work for derived <c>Set</c>
		/// classes if the derived class implements a constructor that takes an <c>ICollection</c>
		/// or <c>ISet</c> as a parameter, and if you haven't implemented any new fields.
		/// </summary>
		/// <returns>A clone of this object.</returns>
		public virtual object Clone()
		{
			//COOL .NET TRICK: Clone() implemented using reflection...
			//
			// The clone is constructed by creating a new 
			// instance of this <c>Set</c> based on reflection, calling a constructor
			// that will accept <c>this</c> as an argument.  So in other words, if the <c>Set</c>
			// can be constructed with an <c>ICollection</c>, there is no need to override the
			// clone method!
			return Activator.CreateInstance(this.GetType(), new object[] {this});
		}


		/// <summary>
		/// Copies the elements in the <c>Set</c> to an array.  The type of array needs
		/// to be compatible with the objects in the <c>Set</c>, obviously.
		/// </summary>
		/// <param name="array">An array that will be the target of the copy operation.</param>
		/// <param name="index">The zero-based index where copying will start.</param>
		public abstract void        CopyTo(Array array, int index);

		/// <summary>
		/// The number of elements currently contained in this collection.
		/// </summary>
		public abstract int         Count{get;}

		/// <summary>
		/// Returns <c>true</c> if the <c>Set</c> is synchronized across threads.  Note that
		/// enumeration is inherently not thread-safe.  Use the <c>SyncRoot</c> to lock the
		/// object during enumeration.
		/// </summary>
		public abstract bool        IsSynchronized{get;}

		/// <summary>
		/// An object that can be used to synchronize this collection to make it thread-safe.
		/// When implementing this, if your object uses a base object, like an <c>IDictionary</c>,
		/// or anything that has a <c>SyncRoot</c>, return that object instead of "<c>this</c>".
		/// </summary>
		public abstract object      SyncRoot{get;}
		
		/// <summary>
		/// Gets an enumerator for the elements in the <c>Set</c>.
		/// </summary>
		/// <returns>An <c>IEnumerator</c> over the elements in the <c>Set</c>.</returns>
		public abstract IEnumerator GetEnumerator();
		
		/// <summary>
		/// This method will test the <c>Set</c> against another <c>Set</c> for "equality".
		/// In this case, "equality" means that the two sets contain the same elements.
		/// The "==" and "!=" operators are not overridden by design.  If you wish to check
		/// for "equivalent" <c>Set</c> instances, use <c>Equals()</c>.  If you wish to check
		/// to see if two references are actually the same object, use "==" and "!=".  
		/// </summary>
		/// <param name="o">A <c>Set</c> object to compare to.</param>
		/// <returns></returns>
		public override Int32 GetHashCode()	{	return 0;	}
		public override bool Equals(object o)
		{
			if(o == null || !(o is Set) || ((Set)o).Count != this.Count)
				return false;
			else 
			{
				foreach(object elt in ((Set)o))
				{
					if(!this.Contains(elt))
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Provides a thread-safe <c>Set</c> wrapper.  
		/// </summary>
		/// <param name="basisSet">The <c>Set</c> object that this call wraps.</param>
		/// <returns>A thread-safe <c>Set</c>.</returns>
		public static Set Synchronized(Set basisSet)
		{
			return new SynchronizedSet(basisSet);
		}

		/// <summary>
		/// Provides an immutable (read-only) <c>Set</c> wrapper.
		/// </summary>
		/// <param name="basisSet">The <c>Set</c> object that this call wraps.</param>
		/// <returns>An immutable (read-only) <c>Set</c>.</returns>
		public static Set Immutable(Set basisSet)
		{
			return new ImmutableSet(basisSet);
		}
	}
}
