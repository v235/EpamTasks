using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Task.Generics {

	public static class ListConverter {

		private static char ListSeparator = ',';  // Separator used to separate values in string

		/// <summary>
		///   Converts a source list into a string representation
		/// </summary>
		/// <typeparam name="T">type  of list items</typeparam>
		/// <param name="list">source list</param>
		/// <returns>
		///   Returns the string representation of a list 
		/// </returns>
		/// <example>
		///   { 1,2,3,4,5 } => "1,2,3,4,5"
		///   { '1','2','3','4','5'} => "1,2,3,4,5"
		///   { true, false } => "True,False"
		///   { ConsoleColor.Black, ConsoleColor.Blue, ConsoleColor.Cyan } => "Black,Blue,Cyan"
		///   { new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 30) } => "01:00:00,00:00:30",
		/// </example>
		public static string ConvertToString<T>(this IEnumerable<T> list)
		{
            StringBuilder result = new StringBuilder();
            foreach (T l in list)
            {
                result.Append(l.ToString() + ListSeparator);
            }
            result.Length--;
            return result.ToString();       
		}

		/// <summary>
		///   Converts the string respresentation to the list of items
		/// </summary>
		/// <typeparam name="T">required type of output items</typeparam>
		/// <param name="list">string representation of the list</param>
		/// <returns>
		///   Returns the list of items from specified string
		/// </returns>
		/// <example>
		///  "1,2,3,4,5" for int => {1,2,3,4,5}
		///  "1,2,3,4,5" for char => {'1','2','3','4','5'}
		///  "1,2,3,4,5" for string => {"1","2","3","4","5"}
		///  "true,false" for bool => { true, false }
		///  "Black,Blue,Cyan" for ConsoleColor => { ConsoleColor.Black, ConsoleColor.Blue, ConsoleColor.Cyan }
		///  "1:00:00,0:00:30" for TimeSpan =>  { new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 30) },
		///  </example>
		public static IEnumerable<T> ConvertToList<T>(this string list) {

            List<T> result = new List<T>();
            for (int i = 0; i < list.Split(ListSeparator).Length; i++)
            {
                result.Add((T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(list.Split(ListSeparator)[i]));
            }
            return result;
		}

	}

	public static class ArrayExtentions {

		/// <summary>
		///   Swaps the one element of source array with another
		/// </summary>
		/// <typeparam name="T">required type of</typeparam>
		/// <param name="array">source array</param>
		/// <param name="index1">first index</param>
		/// <param name="index2">second index</param>
		public static void SwapArrayElements<T>(this T[] array, int index1, int index2)
		{
            T buf = array[index1];
            array[index1] = array[index2];
            array[index2] = buf;
		}

	    /// <summary>
	    ///   Sorts the tuple array by specified column in ascending or descending order
	    /// </summary>
	    /// <param name="array">source array</param>
	    /// <param name="sortedColumn">index of column</param>
	    /// <param name="ascending">true if ascending order required; otherwise false</param>
	    /// <example>
	    ///   source array : 
	    ///   { 
	    ///     { 1, "a", false },
	    ///     { 3, "b", false },
	    ///     { 2, "c", true  }
	    ///   }
	    ///   result of SortTupleArray(array, 0, true) is sort rows by first column in a ascending order: 
	    ///   { 
	    ///     { 1, "a", false },
	    ///     { 2, "c", true  },
	    ///     { 3, "b", false }
	    ///   }
	    ///   result of SortTupleArray(array, 1, false) is sort rows by second column in a descending order: 
	    ///   {
	    ///     { 2, "c", true  },
	    ///     { 3, "b", false }
	    ///     { 1, "a", false },
	    ///   }
	    /// </example>
	    public static void SortTupleArray<T1, T2, T3>(this Tuple<T1, T2, T3>[] array, int sortedColumn, bool ascending)
	    {

	        if (sortedColumn >= array.Length)
	        {
                throw new IndexOutOfRangeException("Индекс вне границ кортежа");
	        }
	        switch (sortedColumn)
	        {
	            case 0:
	                if (ascending)
	                {
                        array = array.OrderBy(p => p.Item1).ToArray();                      
	                }
	                else
	                {
                        array = array.OrderByDescending(p => p.Item1).ToArray();                       
	                }
	                break;
	            case 1:
	                if (ascending)
	                {
                        array = array.OrderBy(p => p.Item2).ToArray();                      
	                }
	                else
	                {
                        array = array.OrderByDescending(p => p.Item2).ToArray();                       
	                }
	                break;
	            case 2:
	               if (ascending)
	                {
                        array = array.OrderBy(p => p.Item3).ToArray();                      
	                }
	                else
	                {
                        array = array.OrderByDescending(p => p.Item3).ToArray();                       
	                }
	                break;
	        }

	    }

	}

	/// <summary>
	///   Generic singleton class
	/// </summary>
	/// <example>
	///   This code should return the same MyService object every time:
	///   MyService singleton = Singleton<MyService>.Instance;
	/// </example>
    public static class Singleton<T>
        where T : class, new()
    {
        private static T _instance;

        public static T Instance
        {

            get
            {
                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }
    }



	public static class FunctionExtentions {
		/// <summary>
		///   Tries to invoke the specified function up to 3 times if the result is unavailable 
		/// </summary>
		/// <param name="function">specified function</param>
		/// <returns>
		///   Returns the result of specified function, if WebException occurs duaring request then exception should be logged into trace 
		///   and the new request should be started (up to 3 times).
		/// </returns>
		/// <example>
		///   Sometimes if network is unstable it is required to try several request to get data:
		///   
		///   Func<string> f1 = ()=>(new System.Net.WebClient()).DownloadString("http://www.google.com/");
		///   string data = f1.TimeoutSafeInvoke();
		///   
		///   If the first attemp to download data is failed by WebException then exception should be logged to trace log and the second attemp should be started.
		///   The second attemp has the same workflow.
		///   If the third attemp fails then this exception should be rethrow to the application.
		/// </example>
        public static int count=0;
		public static T TimeoutSafeInvoke<T>(this Func<T> function)
		{
            try
            {
                switch (count)
                {
                    case 0:
                        return function();
                    case 1:
                        throw new WebException("The operation has timed out", WebExceptionStatus.Timeout);
                    case 2:
                        throw new WebException("The operation has timed out", WebExceptionStatus.Timeout);
                }
                return function();
            }
            catch (WebException ex)
            {
                Trace.WriteLine(ex.Status, "WebException");
                count++;
                switch (count)
                {
                    case 1:
                        function.TimeoutSafeInvoke();
                        break;
                    case 2:
                        return function();
                    case 3:
                        return function();
                }
               
            }

            return function();
		}


		/// <summary>
		///   Combines several predicates using logical AND operator 
		/// </summary>
		/// <param name="predicates">array of predicates</param>
		/// <returns>
		///   Returns a new predicate that combine the specified predicated using AND operator
		/// </returns>
		/// <example>
		///   var result = CombinePredicates(new Predicate<string>[] {
		///            x=> !string.IsNullOrEmpty(x),
		///            x=> x.StartsWith("START"),
		///            x=> x.EndsWith("END"),
		///            x=> x.Contains("#")
		///        })
		///   should return the predicate that identical to 
		///   x=> (!string.IsNullOrEmpty(x)) && x.StartsWith("START") && x.EndsWith("END") && x.Contains("#")
		///
		///   The following example should create predicate that returns true if int value between -10 and 10:
		///   var result = CombinePredicates(new Predicate<int>[] {
		///            x=> x>-10,
		///            x=> x<10
		///       })
		/// </example>
		public static Predicate<T> CombinePredicates<T>(Predicate<T>[] predicates) {
            Func<string> f1 = () => (new System.Net.WebClient()).DownloadString("http://www.google.com/");
               string data = f1.TimeoutSafeInvoke();
			// TODO : Implement CombinePredicates<T>
			throw new NotImplementedException();
		}

	}


}
