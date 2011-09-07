using System;
using System.Runtime.InteropServices;

namespace MusicBrowser.WebServices.Helper
{
    public class Externals
    {
        //Private Declare Function UrlEscape Lib "Shlwapi.dll" Alias "UrlEscapeA" ( _
        //ByVal pszURL As String, ByVal pszEscaped As String, ByRef pcchEscaped As Long, _
        //ByVal dwFlags As Long) As Long

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern int UrlEscape(
        [In]      string pszURL,
        [Out]     string pszEscaped,
        [In, Out] ref uint urlLength,
        [In]      uint dwFlags
        );

        const UInt32 UrlEscapePercent = 0x1000;
        const UInt32 URL_ESCAPE_SEGMENT_ONLY = 0x00002000;
        const Int32 UrlMax = 3000;
        const UInt32 UrlSuccess = 0;


        public static string EncodeURL(string strData)
        {
            UInt32 pos = UrlMax;
            string strTemp = new string(' ', UrlMax);

            if (UrlEscape(strData, strTemp, ref pos, UrlEscapePercent + URL_ESCAPE_SEGMENT_ONLY) == UrlSuccess)
            {
                return strTemp.Substring(0, Int32.Parse(pos.ToString()));
            }
            return string.Empty;
        }
    }
}
