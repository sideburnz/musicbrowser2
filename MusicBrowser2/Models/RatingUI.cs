using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Models
{
    public class RatingUI : BaseModel
    {
        private const string IconStar = "resx://MusicBrowser/MusicBrowser.Resources/IconStar";
        private const string IconHalfstar = "resx://MusicBrowser/MusicBrowser.Resources/IconHalfStar";
        private const string IconLoved = "resx://MusicBrowser/MusicBrowser.Resources/IconFavorite";

        private int _rating;
        public int Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                _rating = value;
                FirePropertyChanged("Rating");
                FirePropertyChanged("Stars");
            }
        }

        private bool _loved;
        public bool Loved
        {
            get
            {
                return _loved;
            }
            set
            {
                _loved = value;
                FirePropertyChanged("Loved");
                FirePropertyChanged("Stars");
            }
        }

        public List<Image> Stars
        {
            get
            {
                List<Image> res = new List<Image>();

                    for (int i = 0; i < Math.Floor(_rating / 20.00); i++)
                    {
                        res.Add(new Image(IconStar));
                    }

                if((int)Math.Floor(_rating / 10.00) % 2 == 1)
                {
                    res.Add(new Image(IconHalfstar));
                }

                if (Loved)
                {
                    res.Add(new Image(IconLoved));
                }

                return res;
            }
        }
    }
}
