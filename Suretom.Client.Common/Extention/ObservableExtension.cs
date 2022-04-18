using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Common
{
    /// <summary>
    ///
    /// </summary>
    public static class ObservableExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void ToSortDesc<T>(this ObservableCollection<T> collection) where T : IComparable<T>
        {
            List<T> sortedList = collection.OrderByDescending(x => x).ToList();//这里用降序

            for (int i = 0; i < sortedList.Count(); i++)
            {
                collection.Move(collection.IndexOf(sortedList[i]), i);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void ToSort<T>(this ObservableCollection<T> collection) where T : IComparable<T>
        {
            List<T> sortedList = collection.OrderBy(x => x).ToList();//这里用降序

            for (int i = 0; i < sortedList.Count(); i++)
            {
                collection.Move(collection.IndexOf(sortedList[i]), i);
            }
        }
    }
}