namespace CyberFeedForward.TheMadArchivist.ViewModels;

public sealed class HomePageViewModel
{
    public HomePageViewModel()
    {
        FolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    }

    public string Title => "Home";
    public string Description => "Welcome to The Mad Archivist.";

    public string FolderPath { get; }
}
