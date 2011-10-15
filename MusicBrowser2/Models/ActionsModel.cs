using System;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Actions;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{
    public class ActionsModel : BaseModel
    {
        //TODO: this should also do the work for the Actions - marshal the Execute calls

        #region singleton
        static ActionsModel _instance;
        
        public static ActionsModel GetInstance
        {
            get 
            {
            if (_instance != null) return _instance;
            _instance = new ActionsModel();
            return _instance;
            }
        }
        #endregion


        public bool _visible = false;

        public bool Visible
        {
            get 
            { 
                return _visible; 
            }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    FirePropertyChanged("Visible");
                }
            }
        }

        public void ExecuteAction(baseActionCommand action, Entity entity, String context )
        {
            try
            {
                action.Execute(entity, context);
            }
            catch
            {
            }
        }

    }

}
