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
            public baseActionCommand OnRecord;
            public baseActionCommand OnStar;

            public baseActionCommand OnPlus;
            public baseActionCommand OnMinus;

            public baseActionCommand OnEnter;
            public baseActionCommand OnPlay;
            public IList<baseActionCommand> MenuOptions;
        }

        private static readonly IEnumerable<baseActionCommand> _availableActions = GetAvailableActions();
        private static readonly IDictionary<String, ActionConfigEntry> _actionConfig = GetActionConfig();

        public static baseActionCommand ActionFactory(String name)
        {
            foreach (baseActionCommand action in _availableActions)
            {
                if (action.ToString().EndsWith(".action" + name, StringComparison.OrdinalIgnoreCase))
                {
                    return action;
                }
            }

            return new ActionNoOperation();
        }

        public static baseActionCommand ActionFactory(XmlNode node)
        {
            if (node != null)
            {
                return ActionFactory(node.InnerText);
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

        public static baseActionCommand GetPlusAction(Entity entity)
        {
            return _actionConfig[entity.KindName].OnPlus.NewInstance(entity);
        }

        public static baseActionCommand GetMinusAction(Entity entity)
        {
            return _actionConfig[entity.KindName].OnMinus.NewInstance(entity);
        }

        public static baseActionCommand GetRecordAction(Entity entity)
        {
            return _actionConfig[entity.KindName].OnRecord.NewInstance(entity);
        }

        public static baseActionCommand GetStarAction(Entity entity)
        {
            return _actionConfig[entity.KindName].OnStar.NewInstance(entity);
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

            ret.Add(new ActionCloseMenu());
            ret.Add(new ActionCycleViews());
            ret.Add(new ActionNoOperation()); // AKA Do Nothing
            //the ActionOn____ actions should never be factored
            ret.Add(new ActionOpen());
            ret.Add(new ActionPause());
            ret.Add(new ActionPlay());
            ret.Add(new ActionPlayEntireLibrary());
            ret.Add(new ActionPlayFavourites());
            ret.Add(new ActionPlayNewlyAdded());
            ret.Add(new ActionPlayPopularLastFM());
            ret.Add(new ActionPlayRandomPopular());
            ret.Add(new ActionPreviousPage());
            ret.Add(new ActionQueue());
//          ret.Add(new ActionRate());
            ret.Add(new ActionRateLess());
            ret.Add(new ActionRateMore());
            ret.Add(new ActionRefreshMetadata());
//          ret.Add(new ActionSetBooleanSetting());
//          ret.Add(new ActionSetSetting());
            ret.Add(new ActionShowActions());
            ret.Add(new ActionShowKeyboard());
            ret.Add(new ActionShowSearch());
            ret.Add(new ActionShowSettings());
            ret.Add(new ActionSkipBack());
            ret.Add(new ActionSkipForward());
            ret.Add(new ActionStop());

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

                    //todo: this needs to be able deal with non-existant nodes
                    entry.OnEnter = ActionFactory(node.SelectSingleNode("OnEnter"));
                    entry.OnPlay = ActionFactory(node.SelectSingleNode("OnPlay"));
                    entry.OnRecord = ActionFactory(node.SelectSingleNode("OnRecord"));
                    entry.OnStar = ActionFactory(node.SelectSingleNode("OnStar"));
                    entry.OnPlus = ActionFactory(node.SelectSingleNode("OnPlus"));
                    entry.OnMinus = ActionFactory(node.SelectSingleNode("OnMinus"));

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
