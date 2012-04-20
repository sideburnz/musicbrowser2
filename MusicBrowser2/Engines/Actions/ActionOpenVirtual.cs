using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionOpenView : baseActionCommand
    {
        private const string LABEL = "Open";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconOpen";

        private string _title;

        public ActionOpenView(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionOpenView()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Label = LABEL + " " + value;
            }
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionOpenView(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            baseEntity e = new View();
            e.Title = Title;
            Application.GetReference().Navigate(e);
        }

    }
}
