namespace MusicBrowser.Providers.Background
{
    public interface IBackgroundTaskable
    {
        string Title { get; }
        void Execute();
    }
}
