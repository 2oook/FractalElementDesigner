using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Controls
{
    public class SolutionTreeView : TreeView, ITree
    {
        #region ITree

        public IEnumerable<ITreeItem> GetItems()
        {
            return this.Items.Cast<FrameworkElement>().Cast<ITreeItem>();
        }

        public void Add(ITreeItem item)
        {
            this.Items.Add(item as FrameworkElement);
        }

        public void Remove(ITreeItem item)
        {
            this.Items.Remove(item as FrameworkElement);
        }

        public void Clear()
        {
            this.Items.Clear();
        }

        public object GetSelectedItem()
        {
            return this.SelectedItem;
        }

        #endregion

        #region IUid

        public string GetUid()
        {
            return this.Uid;
        }

        public void SetUid(string uid)
        {
            this.Uid = uid;
        }

        #endregion

        #region ITag

        public object GetTag()
        {
            return this.Tag;
        }

        public void SetTag(object tag)
        {
            this.Tag = tag;
        }

        #endregion

        #region IData

        public object GetData()
        {
            return null;
        }

        public void SetData(object data)
        {
        }

        #endregion

        #region ISelected

        public bool GetSelected()
        {
            return false;
        }

        public void SetSelected(bool selected)
        {
        }

        #endregion
    } 
}
