using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Configurator
{
    public partial class frmMain : Form
    {
        private MusicBrowser.Util.Config _config = MusicBrowser.Util.Config.GetInstance();
        private string[] _views = { "List", "Strip", "Thumb" };

        public frmMain()
        {
            InitializeComponent();
        }

        #region Display Settings

        private void chk_ShowClock_CheckedChanged(object sender, EventArgs e)
        {
            _config.SetSetting("ShowClock", chk_ShowClock.Checked.ToString());
        }


        #endregion

        private void frmMain_Load(object sender, EventArgs e)
        {
            chk_ShowClock.Checked = _config.GetBooleanSetting("ShowClock");
            chk_EnableFanArt.Checked = _config.GetBooleanSetting("EnableFanArt");
            chk_UseFolderImageForTracks.Checked = _config.GetBooleanSetting("UseFolderImageForTracks");
            chk_PutDiscInTrackNo.Checked = _config.GetBooleanSetting("PutDiscInTrackNo");
            chk_AutoLoadNowPlaying.Checked = _config.GetBooleanSetting("AutoLoadNowPlaying");
            chk_UseInternetProviders.Checked = _config.GetBooleanSetting("UseInternetProviders");
            chk_LogStatsOnClose.Checked = _config.GetBooleanSetting("LogStatsOnClose");

            txt_LastFMUsername.Enabled = chk_UseInternetProviders.Checked;
            txt_LastFMUsername.Text = _config.GetStringSetting("LastFMUserName");

            num_AutoPlaylistSize.Value = _config.GetIntSetting("AutoPlaylistSize");

            cmb_AlbumView.DataSource = _views.Clone();
            cmb_AlbumView.SelectedItem = _config.GetStringSetting("Album.View");
            cmb_AlbumView.SelectedIndexChanged += new System.EventHandler(this.cmb_AlbumView_SelectedIndexChanged);

            cmb_ArtistView.DataSource = _views.Clone();
            cmb_AlbumView.SelectedItem = _config.GetStringSetting("Artist.View");
            cmb_ArtistView.SelectedIndexChanged += new System.EventHandler(this.cmb_ArtistView_SelectedIndexChanged);

            cmb_GenreView.DataSource = _views.Clone();
            cmb_GenreView.SelectedItem = _config.GetStringSetting("Genre.View");
            cmb_GenreView.SelectedIndexChanged += new System.EventHandler(this.cmb_GenreView_SelectedIndexChanged);

            cmb_GroupsView.DataSource = _views.Clone();
            cmb_GroupsView.SelectedItem = _config.GetStringSetting("Group.View");
            cmb_GroupsView.SelectedIndexChanged += new System.EventHandler(this.cmb_GroupsView_SelectedIndexChanged);

            cmb_HomeView.DataSource = _views.Clone();
            cmb_HomeView.SelectedItem = _config.GetStringSetting("Home.View");
            cmb_HomeView.SelectedIndexChanged += new System.EventHandler(this.cmb_HomeView_SelectedIndexChanged);

            cmb_VirtualView.DataSource = _views.Clone();
            cmb_VirtualView.SelectedItem = _config.GetStringSetting("Virtual.View");
            cmb_VirtualView.SelectedIndexChanged += new System.EventHandler(this.cmb_VirtualView_SelectedIndexChanged);
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmd_ClearCache_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void cmb_HomeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _config.SetSetting("Home.View", cmb_HomeView.SelectedItem.ToString());
        }

        private void cmb_GroupsView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _config.SetSetting("Group.View", cmb_GroupsView.SelectedItem.ToString());
        }

        private void cmb_GenreView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _config.SetSetting("Genre.View", cmb_GenreView.SelectedItem.ToString());
        }

        private void cmb_ArtistView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _config.SetSetting("Artist.View", cmb_ArtistView.SelectedItem.ToString());
        }

        private void cmb_AlbumView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _config.SetSetting("Album.View", cmb_AlbumView.SelectedItem.ToString());
        }

        private void cmb_VirtualView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _config.SetSetting("Virtual.View", cmb_VirtualView.SelectedItem.ToString());
        }

        private void chk_EnableFanArt_CheckedChanged(object sender, EventArgs e)
        {
            _config.SetSetting("EnableFanArt", chk_EnableFanArt.Checked.ToString());
        }

        private void chk_UseFolderImageForTracks_CheckedChanged(object sender, EventArgs e)
        {
            _config.SetSetting("UseFolderImageForTracks", chk_UseFolderImageForTracks.Checked.ToString());
        }

        private void chk_PutDiscInTrackNo_CheckedChanged(object sender, EventArgs e)
        {
            _config.SetSetting("PutDiscInTrackNo", chk_PutDiscInTrackNo.Checked.ToString());
        }

        private void chk_AutoLoadNowPlaying_CheckedChanged(object sender, EventArgs e)
        {
            _config.SetSetting("AutoLoadNowPlaying", chk_AutoLoadNowPlaying.Checked.ToString());
        }

        private void chk_UseInternetProviders_CheckedChanged(object sender, EventArgs e)
        {
            _config.SetSetting("UseInternetProviders", chk_UseInternetProviders.Checked.ToString());
            txt_LastFMUsername.Enabled = chk_UseInternetProviders.Checked;
        }

        private void txt_LastFMUsername_TextChanged(object sender, EventArgs e)
        {
            _config.SetSetting("LastFMUserName", txt_LastFMUsername.Text);
        }

        private void num_AutoPlaylistSize_ValueChanged(object sender, EventArgs e)
        {
            _config.SetSetting("AutoPlaylistSize", ((int)num_AutoPlaylistSize.Value).ToString());
        }

        private void chk_LogStatsOnClose_CheckedChanged(object sender, EventArgs e)
        {
            _config.SetSetting("LogStatsOnClose", chk_LogStatsOnClose.Checked.ToString());
        }

   }
}
