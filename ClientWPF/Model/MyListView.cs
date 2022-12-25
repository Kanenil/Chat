using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;

namespace ClientWPF.Model
{
    public class MyListView : ListView
    {
        public MyListView()
        {
            var scroll = FindScrollViewer(this);
            scroll.ScrollChanged += Scrolled;
        }

        protected override void OnItemsSourceChanged(
                                        IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            var scroll = FindScrollViewer(this);
            scroll.ScrollToVerticalOffset(_scrollOffset);

            //var list = (IEnumerable<MessageModel>)base.ItemsSource;
            //if (list.Count() != 0)
            //    base.ScrollIntoView(list.Last());

        }
        private double _scrollOffset;
        private void Scrolled(object sender, ScrollChangedEventArgs e)
        {
            var scroll = FindScrollViewer(this);
            scroll.ScrollChanged += Scrolled;
            _scrollOffset = scroll.VerticalOffset;
        }

        private ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer)
                return d as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var sw = FindScrollViewer(VisualTreeHelper.GetChild(d, i));
                if (sw != null) return sw;
            }
            return null;
        }

    }
}
