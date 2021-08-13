namespace MSWordParserToHTML.Services
{
    public interface IDialogService
    {
        void ShowMessage(string text);
        void ShowFolder(string path);
        string FileBrowserDialog(string Extension = "*.docx");
        string FolderBrowserDialog();
    }
}
