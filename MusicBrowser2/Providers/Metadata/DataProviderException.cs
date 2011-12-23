using System;
using System.Text;

namespace MusicBrowser.Interfaces
{
    /// <summary>
    /// A DataProvider specific error, allows validation and format checks on params
    /// to throw a specific error
    /// </summary>
    public class DataProviderException: Exception
    {
        private readonly string _message;

        public DataProviderException(string message, params string[] fields)
        {
            _message = message;

            StringBuilder sb = new StringBuilder();
            foreach (string field in fields )
            {
                sb.Append(field + " ");
            }
            base.Source = sb.ToString();
        }

        public override string Message { get { return _message; } }

    }
}
