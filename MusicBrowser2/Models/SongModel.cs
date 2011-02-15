using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.Entities.Kinds;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Models
{
    public class SongModel : BaseModel
    {
        Song _song;

        public SongModel(Song song)
        {
            _song = song;
        }

        public IEntity Entity { get { return _song; } }

        public string Title { get { return _song.Title; } }
        public string Album { get { return GetProperty("album"); } }
        public string Artist { get { return GetProperty("artist"); } }
        public string Year { get { return GetProperty("year"); } }
        public Image Thumb { get { return _song.Icon; } }

        public string Summary { get { return _song.Summary; } }


        private string GetProperty(string key)
        {
            if (_song.Properties.ContainsKey(key))
            {
                return _song.Properties[key];
            }
            return string.Empty;
        }
    }
}
