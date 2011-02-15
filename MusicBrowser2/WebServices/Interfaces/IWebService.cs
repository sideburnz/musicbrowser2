namespace MusicBrowser.WebServices.Interfaces
{
    public interface IWebServiceDTO { }

    public interface IWebService
    {
        void setProvider(WebServiceProvider provider);
        IWebServiceDTO Fetch(IWebServiceDTO DTO);
    }
}
