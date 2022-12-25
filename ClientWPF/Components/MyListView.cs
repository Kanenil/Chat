using ClientWPF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientWPF.Components
{
    public class MyListView : ListView
    {
        protected override void OnItemsSourceChanged(
                                        IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            var list = (IEnumerable<MessageModel>)base.ItemsSource;

            if (list != null)
                if (list.Count() != 0)
                    base.ScrollIntoView(list.Last());

        }

    }
}
