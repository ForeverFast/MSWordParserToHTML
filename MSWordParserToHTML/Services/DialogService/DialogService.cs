using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Windows;

namespace MSWordParserToHTML.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string text) => MessageBox.Show(text);

        public void ShowFolder(string path) => Process.Start("explorer", path);

        public string FileBrowserDialog(string Extension = "*.docx")
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filters.Add(new CommonFileDialogFilter("Файлы", Extension));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                return dlg.FileName;
            else
                return "";
        }

        public string FolderBrowserDialog()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                return dlg.FileName;
            else
                return "";
        }
    }
}
