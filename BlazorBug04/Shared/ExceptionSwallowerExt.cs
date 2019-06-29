using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBug04.Shared
{
    // with thanks to @joshlang    
    public static class ValueChangedHelper
    {
        /// <summary>
        /// Implements a Set property with EventCallback handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="propertyChanged"></param>
        /// <param name="equalityComparer"></param>
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

            property = value;
            propertyChanged.InvokeAsync(value).SwallowExceptions();
            return true;
        }

        static void SwallowException(Task task) => _ = task?.Exception;

        public static void SwallowExceptions(this Task task) => _ = task.ContinueWith(SwallowException, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

    }
}
