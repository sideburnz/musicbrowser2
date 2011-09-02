namespace MusicBrowser.WebServices.Interfaces
{
    public interface IWebServiceDTO
    {
        WebServiceStatus Status { get; set; }
        string Error { get; set; }
    }

    public interface IWebService
    {
        void SetProvider(WebServiceProvider provider);
        IWebServiceDTO Fetch(IWebServiceDTO dto);
    }

    public enum WebServiceStatus
    {
        Success,
        Warning,
        Error
    }
}
