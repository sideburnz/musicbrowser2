using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Engines.Logging;

//TODO: tidy this up

namespace MusicBrowser.Actions
{
    static class Helper
    {
        private struct ActionConfigEntry
        {
            //TODO: implement additional shortcuts
            public baseActionCommand OnRecord;  // default to refresh metadata
            public baseActionCommand OnHash; // default to cycle through starts (0,1,2,3,4,5)
            public baseActionCommand OnStar; // default to mark as favorite

            public baseActionCommand OnEnter;
            public baseActionCommand OnPlay;
            public IList<baseActionCommand> MenuOptions;
        }

        private static readonly IEnumerable<baseActionCommand> _availableActions = GetAvailableActions();
        private static readonly IDictionary<String, ActionConfigEntry> _actionConfig = GetActionConfig();

        public static baseActionCommand ActionFactory(String name)
        {
            foreach (baseActionCommand action in GetAvailableActions())
            {
                if (action.ToString().Equals("musicbrowser.actions.action" + name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return action;
                }
            }

            return new ActionNoOperation();
        }

        public static baseActionCommand GetEnterAction(Entity entity)
        {
            return _actionConfig[entity.KindName].OnEnter.NewInstance(entity);
        }

        public static baseActionCommand GetPlayAction(Entity entity)
        {
            return _actionConfig[entity.KindName].OnPlay.NewInstance(entity);
        }

        public static IList<baseActionCommand> GetActionList(Entity entity)
        {
            List<baseActionCommand> ret = new List<baseActionCommand>();

            foreach (baseActionCommand action in _actionConfig[entity.KindName].MenuOptions)
            {
                ret.Add(action.NewInstance(entity));
            }

            return ret;
        }

        private static IEnumerable<baseActionCommand> GetAvailableActions()
        {
            List<baseActionCommand> ret = new List<baseActionCommand>();

            // commented actions are internal use only

            ret.Add(new ActionCloseMenu());
            ret.Add(new ActionCycleViews());
//            ret.Add(new ActionDefaultAction());
            ret.Add(new ActionNoOperation());
//            ret.Add(new ActionOnEnter());
//            ret.Add(new ActionOnPlay());
            ret.Add(new ActionOpen());
            ret.Add(new ActionPlay());
            ret.Add(new ActionPlayEntireLibrary());
            ret.Add(new ActionPlayFavourites());
            ret.Add(new ActionPlayNewlyAdded());
            ret.Add(new ActionPlayPopularLastFM());
            ret.Add(new ActionPlayRandomPopular());
            ret.Add(new ActionPreviousPage());
            ret.Add(new ActionQueue());
            ret.Add(new ActionRefreshMetadata());
//            ret.Add(new ActionSetSetting());
            ret.Add(new ActionShowActions());
            ret.Add(new ActionShowKeyboard());
            ret.Add(new ActionShowSearch());
            ret.Add(new ActionShowSettings());

            return ret;
        }

        private static IDictionary<String, ActionConfigEntry> GetActionConfig()
        {
            IDictionary<String, ActionConfigEntry> actions = new Dictionary<String, ActionConfigEntry>();

            string configFile = Path.Combine(Util.Helper.AppFolder, "actions.config");
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(configFile);
            }
            catch (Exception e)
            {
                LoggerEngineFactory.Error(e);
                throw e;
            }

            XmlNodeList nodes = xml.SelectNodes("ActionConfig/Entity");
            foreach(XmlNode node in nodes)
            {
                try
                {
                    ActionConfigEntry entry = new ActionConfigEntry();
                    entry.MenuOptions = new List<baseActionCommand>();

                    entry.OnEnter = ActionFactory(node.SelectSingleNode("OnEnter").InnerText);
                    entry.OnPlay = ActionFactory(node.SelectSingleNode("OnPlay").InnerText);

                    foreach (XmlNode item in node.SelectNodes("MenuItems/Item"))
                    {
                        entry.MenuOptions.Add(ActionFactory(item.InnerText));
                    }
                    entry.MenuOptions.Add(new ActionCloseMenu());

                    actions.Add(node.Attributes["name"].InnerText, entry);
                }
                catch (Exception e)
                {
                    LoggerEngineFactory.Error(e);
                    throw e;
                }
            }

            return actions;
        }
    }
}
