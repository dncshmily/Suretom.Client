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
        public static void ObservableMySort(this ObservableCollection<BatchImportProcessInfo> observableCollection)
        {
            var a = observableCollection.OrderByDescending(f => f.Id).ToList();

            observableCollection.Clear();
            foreach (var b in a)
            {
                observableCollection.Add(b);
            }
        }
    }
}