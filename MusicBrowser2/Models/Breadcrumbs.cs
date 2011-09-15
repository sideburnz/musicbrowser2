using System.Collections.Generic;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{
    public sealed class Breadcrumbs
    {
        readonly List<Entity> _entities = new List<Entity>();

        public Breadcrumbs(Breadcrumbs crumbs)
        {
            foreach (Entity item in crumbs.Crumbs)
            {
                _entities.Add(item);
            }
        }

        public Breadcrumbs()
        {
        }

        public void Add(Entity entity)
        {
            _entities.Add(entity);
        }

        public string Path
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < (_entities.Count - 1); i++)
                {
                    sb.Append(_entities[i].Description);
                    sb.Append(" > ");
                }
                return sb.ToString();
            }
        }

        private IEnumerable<Entity> Crumbs
        {
            get { return _entities; }
        }
    }
}
