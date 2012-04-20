using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public static class Factory
    {
        private struct ActionConfigEntry
        {
            public baseActionCommand OnRecord;
            public baseActionCommand OnEnter;
            public baseActionCommand OnPlay;
            public baseActionCommand OnStar;

            public IList<baseActionCommand> MenuOptions;
        }

        private static readonly List<baseActionCommand> AvailableActions = GetAvailableActions();
        private static IDictionary<String, ActionConfigEntry> _actionConfig;
        private static readonly Dictionary<string, baseActionCommand> AugmentedActions = new Dictionary<string, baseActionCommand>();

        private static baseActionCommand ActionFactory(String name)
        {
            foreach (baseActionCommand action in AvailableActions)
            {
                if (action.ToString().EndsWith(".action" + name, StringComparison.OrdinalIgnoreCase))
                {
                    return action;
                }
            }

            return new ActionNoOperation();
        }

        private static baseActionCommand ActionFactory(XmlNode node)
        {
            if (node != null)
            {
                return ActionFactory(node.InnerText);
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetEnterAction(baseEntity entity)
        {
            if (_actionConfig == null)
            {
                _actionConfig = GetActionConfig();
            }

            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnEnter.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnEnter.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetPlayAction(baseEntity entity)
        {
            if (_actionConfig == null)
            {
                _actionConfig = GetActionConfig();
            }

            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnPlay.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnPlay.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetStarAction(baseEntity entity)
        {
            if (_actionConfig == null)
            {
                _actionConfig = GetActionConfig();
            }

            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnStar.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnStar.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetRecordAction(baseEntity entity)
        {
            if (_actionConfig == null)
            {
                _actionConfig = GetActionConfig();
            }

            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnRecord.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnRecord.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static List<baseActionCommand> GetActionList(baseEntity entity)
        {
            if (_actionConfig == null)
            {
                _actionConfig = GetActionConfig();
            }

            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].MenuOptions.Count > 0)
                    {
                        List<baseActionCommand> ret = new List<baseActionCommand>();
                        foreach (baseActionCommand action in _actionConfig[leaf].MenuOptions)
                        {
                            ret.Add(action.NewInstance(entity));
                        }
                        foreach(string key in AugmentedActions.Keys)
                        {
                            if (key.StartsWith(entity.Kind))
                            {
                                ret.Add(AugmentedActions[key]);
                            }
                        }
                        ret.Add(new ActionCloseMenu());
                        return ret;
                    }
                }
            }
            return null;
        }

        private static List<baseActionCommand> GetAvailableActions()
        {
            List<baseActionCommand> ret = new List<baseActionCommand>();

            ret.Add(new ActionCloseMenu());
            ret.Add(new ActionNoOperation()); // AKA Do Nothing
            ret.Add(new ActionOpen());
            ret.Add(new ActionPause());
            ret.Add(new ActionPlay());
            ret.Add(new ActionPreviousPage());
            ret.Add(new ActionQueue());
            ret.Add(new ActionRefreshMetadata());
            ret.Add(new ActionRestart());
            ret.Add(new ActionShowActions());
            ret.Add(new ActionShowKeyboard());
            ret.Add(new ActionShowSearch());
            ret.Add(new ActionShowSettings());
            ret.Add(new ActionShuffle());
            ret.Add(new ActionSkipBack());
            ret.Add(new ActionSkipForward());
            ret.Add(new ActionStop());
            ret.Add(new ActionToggleWatched());

            return ret;
        }

        public static void RegisterAction(baseActionCommand action, string kind)
        {
            AugmentedActions.Add(kind + ":" + action.Label, action);
        }

        private static IDictionary<String, ActionConfigEntry> GetActionConfig()
        {
            IDictionary<String, ActionConfigEntry> actions = new Dictionary<String, ActionConfigEntry>();

            string configFile = Path.Combine(Util.Helper.AppFolder, "actions.config");
            XmlDocument xml = new XmlDocument();
            try
            {
                if (File.Exists(configFile))
                {
                    LoggerEngineFactory.Debug("ActionsFactory", "using actions override file");
                    xml.Load(configFile);
                }
                else
                {
                    xml.LoadXml(Resources.ActionConfig);
                }
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

                    entry.OnEnter = ActionFactory(node.SelectSingleNode("OnEnter"));
                    entry.OnPlay = ActionFactory(node.SelectSingleNode("OnPlay"));
                    entry.OnRecord = ActionFactory(node.SelectSingleNode("OnRecord"));
                    entry.OnStar = ActionFactory(node.SelectSingleNode("OnStar"));

                    foreach (XmlNode item in node.SelectNodes("MenuItems/Item"))
                    {
                        entry.MenuOptions.Add(ActionFactory(item.InnerText));
                    }

                    actions.Add(node.Attributes["name"].InnerText, entry);
                }
                catch (Exception e)
                {
                    LoggerEngineFactory.Error(e);
                    throw;
                }
            }

            return actions;
        }
    }
}
