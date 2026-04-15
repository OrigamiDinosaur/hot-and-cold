using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace Apache.Core.Extensions {
	public static class CollectionExtensions {

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static readonly Random shuffleRandom = new Random();

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------
		
		/// <summary>Returns <c>true</c> if the list is either null or empty, otherwise <c>false</c>.</summary>
		public static bool IsNullOrEmpty<T>(this IList<T> self) {
			return (self == null || self.Count == 0);
		}

		/// <summary>
		/// Returns a random element from an <c>IEnumerable</c>. Special case for <c>int</c> type.
		/// </summary>
		/// <remarks>
		/// RandomOrDefault is much faster when used on a collection of <c>int</c> or <c>float</c>.
		/// </remarks>
		/// <remarks>
		/// Count less than 100 was optimised from <see href="https://bit.ly/2Qsh1lr">here</see>.
		/// </remarks>
		public static int RandomOrDefault(this IEnumerable<int> self) {
			int count = self.Count();
			int index = UnityEngine.Random.Range(0, count);

			// if we have more than 100, just use random element at.
			if (count > 100) return self.ElementAt(index);

			// when the collection has 100 elements or fewer, get the random element by traversing the collection one element at a time.
			using (IEnumerator<int> enumerator = self.GetEnumerator()) {
				while (index >= 0 && enumerator.MoveNext()) {
					index--;
				}
				return enumerator.Current;
			}
		}

		/// <summary>
		/// Returns a random element from an <c>IEnumerable</c>. Special case for <c>float</c> type.
		/// </summary>
		/// <remarks>
		/// RandomOrDefault is much faster when used on a collection of <c>int</c> or <c>float</c>.
		/// </remarks>
		/// <remarks>
		/// Count less than 100 was optimised from <see href="https://bit.ly/2Qsh1lr">here</see>.
		/// </remarks>
		public static float RandomOrDefault(this IEnumerable<float> self) {
			int count = self.Count();
			int index = UnityEngine.Random.Range(0, count);

			// if we have more than 100, just use random element at.
			if (count > 100) return self.ElementAt(index);

			// when the collection has 100 elements or fewer, get the random element by traversing the collection one element at a time.
			using (IEnumerator<float> enumerator = self.GetEnumerator()) {
				while (index >= 0 && enumerator.MoveNext()) {
					index--;
				}
				return enumerator.Current;
			}
		}

		/// <summary>
		/// Returns a random element from an <c>IEnumerable</c>.
		/// </summary>
		public static T RandomOrDefault<T>(this IEnumerable<T> self) {
			// ReSharper disable once RedundantTypeSpecificationInDefaultExpression
			T randomElement = default(T);
			int count = 0;
			foreach (T element in self) {
				++count;
				if (UnityEngine.Random.value <= (1f / count)) {
					randomElement = element;
				}
			}
			return randomElement;
		}

		/// <summary>
		/// Returns a random element from an <c>IEnumerable</c> which satisfies the given predicate.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="predicate">A predicate which each element must satisfy.</param>
		public static T RandomOrDefault<T>(this IEnumerable<T> self, Func<T, bool> predicate) {
			// ReSharper disable once RedundantTypeSpecificationInDefaultExpression
			T randomElement = default(T);
			int count = 0;
			foreach (T element in self) {
				if (!predicate(element)) continue;
				++count;
				if (UnityEngine.Random.value <= (1f / count)) {
					randomElement = element;
				}
			}
			return randomElement;
		}

		/// <summary>Shuffle the elements in the <c>IList</c> using the Fisher Yates method.</summary>
		/// <remarks><see href="https://bit.ly/2Phy25m">See here</see> for more info on the Fisher Yates method.</remarks>
		public static void Shuffle<T>(this IList<T> self) {
			int n = self.Count;
			while (n > 1) {
				n--;
				int k = shuffleRandom.Next(n + 1);
				T value = self[k];
				self[k] = self[n];
				self[n] = value;
			}
		}

		/// <summary>
		/// Performs an action passing each item.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action) {
			foreach (T item in self) {
				action(item);
			}
			return self;
		}

		/// <summary>
		/// Performs an action passing each item and an counter representing the ID of the active item.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T, int> action) {
			int counter = 0;
			foreach (T item in self) {
				action(item, counter++);
			}
			return self;
		}

		/// <summary>
		/// Converts each item in the collection.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="converter">Func to convert the items.</param>
		public static IEnumerable<T> Convert<T>(this IEnumerable self, Func<object, T> converter) {
			foreach (T item in self) {
				yield return converter(item);
			}
		}

		/// <summary>
		/// Adds an item to the beginning of a collection.
		/// </summary>
		/// <param name="self">The collection.</param>
		/// <param name="prepend">Func to create the item to prepend.</param>
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, Func<T> prepend) {
			yield return prepend();
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the beginning of a collection.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="prepend">The item to prepend.</param>
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, T prepend) {
			yield return prepend;
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds a collection to the beginning of another collection.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="prepend">The collection to prepend.</param>
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, IEnumerable<T> prepend) {
			foreach (T item in prepend) {
				yield return item;
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">Func to create the item to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, bool condition, Func<T> prepend) {
			if (condition) {
				yield return prepend();
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">The item to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, bool condition, T prepend) {
			if (condition) {
				yield return prepend;
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds a collection to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">The collection to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, bool condition, IEnumerable<T> prepend) {
			if (condition) {
				foreach (T item in prepend) {
					yield return item;
				}
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">Func to create the item to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, Func<bool> condition, Func<T> prepend) {
			if (condition()) {
				yield return prepend();
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">The item to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, Func<bool> condition, T prepend) {
			if (condition()) {
				yield return prepend;
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds a collection to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">The collection to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, Func<bool> condition, IEnumerable<T> prepend) {
			if (condition()) {
				foreach (T item in prepend) {
					yield return item;
				}
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">Func to create the item to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, Func<IEnumerable<T>, bool> condition, Func<T> prepend) {
			if (condition(self)) {
				yield return prepend();
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">The item to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, Func<IEnumerable<T>, bool> condition, T prepend) {
			if (condition(self)) {
				yield return prepend;
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds a collection to the beginning of another collection, if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="prepend">The collection to prepend.</param>
		public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> self, Func<IEnumerable<T>, bool> condition, IEnumerable<T> prepend) {
			if (condition(self)) {
				foreach (T item in prepend) {
					yield return item;
				}
			}
			foreach (T item in self) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the end of a collection.
		/// </summary>>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="append">Func to create the item to append.</param>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> self, Func<T> append) {
			foreach (T item in self) {
				yield return item;
			}
			yield return append();
		}

		/// <summary>
		/// Adds an item to the end of a collection.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="append">The item to append.</param>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> self, T append) {
			foreach (T item in self) {
				yield return item;
			}
			yield return append;
		}

		/// <summary>
		/// Adds a collection to the end of another collection.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="append">The collection to append.</param>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> self, IEnumerable<T> append) {
			foreach (T item in self) {
				yield return item;
			}
			foreach (T item in append) {
				yield return item;
			}
		}

		/// <summary>
		/// Adds an item to the end of a collection if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="append">Func to create the item to append.</param>
		public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> self, bool condition, Func<T> append) {
			foreach (T item in self) {
				yield return item;
			}
			if (condition) {
				yield return append();
			}
		}

		/// <summary>
		/// Adds an item to the end of a collection if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="append">The item to append.</param>
		public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> self, bool condition, T append) {
			foreach (T item in self) {
				yield return item;
			}
			if (condition) {
				yield return append;
			}
		}

		/// <summary>
		/// Adds a collection to the end of another collection if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="append">The collection to append.</param>
		public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> self, bool condition, IEnumerable<T> append) {
			foreach (T item in self) {
				yield return item;
			}
			if (condition) {
				foreach (T item in append) {
					yield return item;
				}
			}
		}

		/// <summary>
		/// Adds an item to the end of a collection if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="append">Func to create the item to append.</param>
		public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> self, Func<bool> condition, Func<T> append) {
			foreach (T item in self) {
				yield return item;
			}
			if (condition()) {
				yield return append();
			}
		}

		/// <summary>
		/// Adds an item to the end of a collection if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="append">The item to append.</param>
		public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> self, Func<bool> condition, T append) {
			foreach (T item in self) {
				yield return item;
			}
			if (condition()) {
				yield return append;
			}
		}

		/// <summary>
		/// Adds a collection to the end of another collection if a condition is met.
		/// </summary>
		/// <param name="self">The <c>IEnumerable</c>.</param>
		/// <param name="condition">The condition.</param>
		/// <param name="append">The collection to append.</param>
		public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> self, Func<bool> condition, IEnumerable<T> append) {
			foreach (T item in self) {
				yield return item;
			}
			if (condition()) {
				foreach (T item in append) {
					yield return item;
				}
			}
		}

		/// <summary>Converts a colletion to a <c>HashSet</c>.</summary>
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self) {
			return new HashSet<T>(self);
		}

		/// <summary>
		/// Adds a collection to a <c>HashSet</c>.
		/// </summary>
		/// <param name="self">The <c>HashSet</c>.</param>
		/// <param name="range">The collection.</param>
		public static void AddRange<T>(this HashSet<T> self, IEnumerable<T> range) {
			foreach (T value in range) {
				self.Add(value);
			}
		}
	}
}