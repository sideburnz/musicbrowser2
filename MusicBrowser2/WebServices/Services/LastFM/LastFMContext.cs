//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MusicBrowser.Providers;

//namespace MusicBrowser.Metadata.LastFM
//{
//    public class LastFMContext
//    {
//        #region singleton
//        static LastFMContext _instance;
//        static readonly object Padlock = new object();

//        LastFMContext()
//        { }

//        public static LastFMContext Instance
//        {
//            get
//            {
//                lock (Padlock)
//                {
//                    if (_instance != null) return _instance;
//                    _instance = new LastFMContext();
//                    return _instance;
//                }
//            }
//        }
//        #endregion

//        private string _LFMSessionKey = string.Empty;
//        public string SessionKey
//        {
//            get
//            {
//                if (_LFMSessionKey == "not authenticated")
//                {
//                    return string.Empty;
//                }
//                if (!String.IsNullOrEmpty(_LFMSessionKey))
//                {
//                    return _LFMSessionKey;
//                }

//                string LFMUser = Util.Config.Instance.GetSetting("LastFMUserName").ToLower();
//                string LFMPass = Util.Config.Instance.GetSetting("LastFMPasswordHash").ToLower();
//                string LFMAuthToken = LastFMProvider.getAuthToken(LFMUser, LFMPass);

//                SessionService.SessionDTO DTO = new SessionService.SessionDTO();
//                DTO.AuthToken = LFMAuthToken;
//                DTO.Username = LFMUser.ToLower();

//                LastFM.SessionService.DoService(ref DTO);
//                _LFMSessionKey = DTO.SessionKey;

//                if (_LFMSessionKey != "not authenticated")
//                {
//                    Logging.LoggerFactory.SelectedLogger.LogInfo(String.Format("Authenticated for Last.fm {0}", _LFMSessionKey));
//                    return _LFMSessionKey;
//                }
//                else
//                {
//                    Logging.LoggerFactory.SelectedLogger.LogInfo(String.Format("Unable to authenicate for Last.fm. Response: {0}", DTO.Response));
//                    return string.Empty;
//                }
//            }
//        }
//    }
//}
