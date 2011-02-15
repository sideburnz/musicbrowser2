using System.Collections.Generic;
using System.Text;
using MusicBrowser.Entities.Interfaces;

namespace MusicBrowser.Models
{
    public sealed class Breadcrumbs
    {
        readonly List<IEntity> _entities = new List<IEntity>();

        public Breadcrumbs(Breadcrumbs crumbs)
        {
            foreach (IEntity item in crumbs.Crumbs)
            {
                _entities.Add(item);
            }
        }

        public Breadcrumbs()
        {
        }

        public void Add(IEntity entity)
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

        private IEnumerable<IEntity> Crumbs
        {
            get { return _entities; }
        }
    }
}
