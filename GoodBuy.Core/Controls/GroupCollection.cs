using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoodBuy.Core.Controls
{
    public class GroupCollection<TKey, TItem> : ObservableCollection<TItem>
    {
        public TKey Key { get; private set; }

        public GroupCollection(TKey key, IEnumerable<TItem> items)
        {
            this.Key = key;
            foreach (var item in items)
                this.Items.Add(item);
        }
    }
}


