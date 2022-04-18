using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Common
{
    public static class ObservableHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="observableCollection"></param>
        public static void ObservableMySort(this ObservableCollection<BatchImportProcessInfo> observableCollection)
        {
            var a = observableCollection.OrderByDescending(f => f.Id).ToList();

            observableCollection.Clear();
            foreach (var b in a)
            {
                observableCollection.Add(b);
            }
        }

        public static void ObservableMySort(this ObservableCollection<CourseDto> observableCollection, int type = 0)
        {
            var a = type==0 ? observableCollection.OrderByDescending(f => f.Id).ToList() : observableCollection.OrderByDescending(f => f.Id).ToList();

            observableCollection.Clear();
            foreach (var b in a)
            {
                observableCollection.Add(b);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observableCollection"></param>
        public static void ObservableMySort<T>(this ObservableCollection<T> observableCollection) where T : IComparable<T>
        {
            var a = observableCollection.OrderByDescending(x => x).ToList();

            observableCollection.Clear();
            foreach (var b in a)
            {
                observableCollection.Add(b);
            }
        }
    }
}