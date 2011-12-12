using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    // for this to work the page needs to feed the data into the dataset here and then from here into the list shown on the scren
    public abstract class IKeyboardHandler
    {
        private int _index = 0;
        private EntityCollection _rawdata;
        private EntityCollection _processeddata;
        private string _value = String.Empty;

        public int Index 
        { 
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                if (!(OnDataChanged == null))
                {
                    OnDataChanged("Index");
                }
            }
        }

        public EntityCollection RawDataSet
        {
            set
            {
                _rawdata = value;
                _processeddata = value;
                if (!(OnDataChanged == null))
                {
                    OnDataChanged("DataSet");
                }
            }
            protected get
            {
                return _rawdata;
            }
        }

        public EntityCollection DataSet
        {
            get
            {
                if (String.IsNullOrEmpty(Value))
                {
                    return _rawdata;
                }
                return _processeddata;
            }
            protected set
            {
                _processeddata = value;
                if (!(OnDataChanged == null))
                {
                    OnDataChanged("DataSet");
                }
            }
        }

        public string Value 
        { 
            get
            {
                return _value;
            } 
            set
            {
                _value = value;
                DoService();
                if (!(OnDataChanged == null))
                {
                    OnDataChanged("Value");
                }
            }
        }

        public abstract void DoService();

        public delegate void DataChangedHandler(String key);
        public static event DataChangedHandler OnDataChanged;
    }
}
