using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBug04.Shared
{
    /// <summary>
    /// Helper methods to implement the Set
    /// </summary>
    public static class ValueChangedHelper
    {
        /// <summary>
        /// Implements a Set property with EventCallback handler
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="property">the property backing field to set</param>
        /// <param name="value">the updated value to set</param>
        /// <param name="propertyChanged">EventCallback of T</param>
        /// <param name="equalityComparer">[optional] equality comparer to use (uses default if not specifed)</param>
        /// <returns></returns>
        public static bool Set<T>(ref T property,
                                  T value,
                                  in EventCallback<T> propertyChanged,
                                  IEqualityComparer<T> equalityComparer = null)
        {
            if ((equalityComparer ?? EqualityComparer<T>.Default).Equals(property, value))
            {
                return false;
            }
            // set the property backing field
            property = value;
            // raise a property changed event
            propertyChanged.InvokeAsync(value).SwallowExceptions();
            return true;
        }

        /// <summary>
        /// swallows exceptions
        /// </summary>
        /// <param name="task"></param>
        static void SwallowException(Task task) => _ = task?.Exception;

        /// <summary>
        /// Extension method to swallow errors
        /// </summary>
        /// <param name="task"></param>
        public static void SwallowExceptions(this Task task) => _ = task.ContinueWith(SwallowException, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

    }
}
