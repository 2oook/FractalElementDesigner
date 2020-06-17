using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public interface ITreeItem : IData, IUid, ITag, ISelected
    {
    	IEnumerable<ITreeItem> GetItems();
        int GetItemsCount();

        ITreeItem GetItem(int index);
        int GetItemIndex(ITreeItem item);

        void Add(ITreeItem item);
        void Remove(ITreeItem item);
        void Clear();

        object GetParent();

        void PushIntoView();
    }
}
