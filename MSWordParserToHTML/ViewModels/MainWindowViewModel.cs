using MSWordParserToHTML.Commands;
using MSWordParserToHTML.Models;
using MSWordParserToHTML.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MSWordParserToHTML.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IParserService _parserService;
        private readonly IDialogService _dialogService;

        #region Поля
        private TextData _inputTextData;
        private TextData _outputTextData;
        private string _targetFilePath;
        #endregion

      

        public TextData InputTextData { get => _inputTextData; set => SetProperty(ref _inputTextData, value); }
        public TextData OutputTextData { get => _outputTextData; set => SetProperty(ref _outputTextData, value); }
        public string TargetFilePath { get => _targetFilePath; set => SetProperty(ref _targetFilePath, value); }


        #region Команды
        public ICommand SetFileCommand { get; }
        public ICommand StartProcessingCommand { get; }

        private void SetFileExecute(object parameter)
        {
            TargetFilePath = _dialogService.FileBrowserDialog();
        }

        private void StartProcessingExecute(object parameter)
        {
            if (!string.IsNullOrEmpty(TargetFilePath))
            {         
                _parserService.Start(TargetFilePath);
                InputTextData = _parserService.InputTextData;
                OutputTextData = _parserService.OutputTextData;
            }
            else
                _dialogService.ShowMessage("Выберите файл!");
        }

        #endregion

        #region Конструкторы
        public MainWindowViewModel(IParserService parserService,
            IDialogService dialogService) : base()
        {
            _parserService = parserService;
            _dialogService = dialogService;

            SetFileCommand = new RelayCommand(SetFileExecute);
            StartProcessingCommand = new RelayCommand(StartProcessingExecute);
        }
        #endregion
    }
}
