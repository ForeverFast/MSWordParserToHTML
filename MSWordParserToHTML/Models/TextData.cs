using MSWordParserToHTML.Extentions;

namespace MSWordParserToHTML.Models
{
    public class TextData : OnPropertyChangedClass
    {
        private string _text;
        private int _length;

        public string Text { get => _text; set => SetProperty(ref _text, value); }
        public int Length { get => _length; set => SetProperty(ref _length, value); }
    }
}
