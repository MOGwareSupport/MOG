/* Copyright (c) 2002 by Insight Enterprise Systems, Inc., and by Jason Smith. */ 
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Iesi.Collections
{
	/// <summary>
	/// Implements a <c>Set</c> that automatically changes from a list to a hash table
	/// when the size reaches a certain threshold.  This is good if you are unsure about
	/// whether you data-set will be tiny or huge.  Because this uses a dual implementation,
	/// iteration order is not guaranteed!
	/// </summary>
	public class HybridSet : DictionarySet
	{
		/// <summary>
		/// Creates a new set instance based on either a list or a hash table, depending on which 
		/// will be more efficient based on the data-set size.
		/// </summary>
		public HybridSet()
		{
			_set = new HybridDictionary(true);
		}


		/// <summary>
		/// Creates a new set instance based on either a list or a hash table, depending on which 
		/// will be more efficient based on the data-set size, and
		/// initializes it based on a collection of elements.
		/// </summary>
		/// <param name="initialValues">A collection of elements that defines the initial set contents.</param>
		public HybridSet(ICollection initialValues) : this()
		{
			this.AddAll(initialValues);
		}
	}
}
