using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Runtime.Serialization;

namespace MusicBrowser.Models
{
    [Serializable]
    public class BaseModel : IModelItem
    {
        #region IModelItem Members

        // implemented as part of the default implementation
        public string Description { get; set; }
        public bool Selected { get; set; }
        public Guid UniqueId { get; set; }

        #endregion

        #region IPropertyObject Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IModelItemOwner Members

        readonly List<ModelItem> _items = new List<ModelItem>();

        public void RegisterObject(ModelItem modelItem)
        {
            _items.Add(modelItem);
        }

        public void UnregisterObject(ModelItem modelItem)
        {
            if (_items.Exists(i => i == modelItem))
            {
                modelItem.Dispose();
            }
        }

        #endregion

        #region change in properties handler

        protected void FirePropertiesChanged(params string[] properties)
        {
            RunInUIThread(() =>
            {
                if (PropertyChanged != null && properties != null)
                {
                    foreach (var property in properties)
                    {
                        PropertyChanged(this, property);
                    }
                }
            });
        }

        protected void FirePropertyChanged(string property)
        {
            RunInUIThread(() =>
            {
                if (PropertyChanged != null && property != null)
                {
                    PropertyChanged(this, property);
                }
            });
        }

        private static void RunInUIThread(Action action)
        {
            Microsoft.MediaCenter.UI.Application.DeferredInvoke(_ => action());
        }

        #endregion
    }
}
