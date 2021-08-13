using Microsoft.Office.Interop.Word;
using MSWordParserToHTML.Models;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MSWordParserToHTML.Services
{
    public class ParserService : IParserService
    {

        #region Поля
        private _Application application;
        private _Document _document;

        private Object _missingObj = System.Reflection.Missing.Value;
        private Object trueObj = true;
        private Object falseObj = false;
        #endregion

        private string InputText { get; set; }
        private string OutputText { get; set; }

        public TextData InputTextData { get; set; }
        public TextData OutputTextData { get; set; }
       

        public void Start(Object path)
        {
            try
            {
                application = new Application();

                //path = @$"C:\Users\ivans\source\repos\MSWordParserToHTML\20210404 Заземление сайт.docx";
                _document = application.Documents.Add(ref path, ref _missingObj, ref _missingObj, ref _missingObj);

                Object pathToSaveObj = "D:\\tempDoc.html";
                _document.SaveAs(ref pathToSaveObj, WdSaveFormat.wdFormatHTML, ref _missingObj,
                    ref _missingObj, ref _missingObj, ref _missingObj,
                    ref _missingObj, ref _missingObj, ref _missingObj,
                    ref _missingObj, ref _missingObj, ref _missingObj,
                    ref _missingObj, ref _missingObj, ref _missingObj,
                    ref _missingObj);

                _document.Close(ref falseObj, ref _missingObj, ref _missingObj);
                application.Quit(ref _missingObj, ref _missingObj, ref _missingObj);
                _document = null;
                application = null;


                using (FileStream fs = new FileStream((string)pathToSaveObj, FileMode.Open, FileAccess.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(1251)))
                {
                    InputText = OutputText = sr.ReadToEnd();
                }

                this.RemoveCommentsTags();
                this.RemoveClassAndLangTags();
                this.RemoveStyles();
                this.RemoveMainTags();
                this.RemoveExtraSpaces();
                this.ReplaceListToHtmlFormat();
                this.EditImages();

                InputTextData = new TextData() { Text = InputText, Length = InputText.Length };
                OutputTextData = new TextData() { Text = OutputText, Length = OutputText.Length };
            }
            catch (Exception ex)
            {
                
            }
        }

        public void RemoveCommentsTags()
        {
            try
            {
                Regex regex = new Regex(@"(<!--)(.*?)(-->)", RegexOptions.Singleline);
                Match match;
                while ((match = regex.Match(OutputText)) != null && match.Success)
                {
                    int startPoint = match.Groups[0].Index;
                    int delLength = match.Groups[3].Index + 3 - startPoint;

                    OutputText = OutputText.Remove(startPoint, delLength);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void RemoveClassAndLangTags()
        {
            try
            {
                Regex regex = new Regex(@"\<\w*\s*((lang|class)=\w*\s*)", RegexOptions.Singleline);
                Match match;
                while ((match = regex.Match(OutputText)) != null && match.Success)
                {
                    int startPoint = match.Groups[1].Index;
                    int delLength = match.Groups[1].Length;
                   
                    OutputText = OutputText.Remove(startPoint, delLength);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void RemoveStyles()
        {
            try
            {
                Regex regex = new Regex(@"(style='.*?')", RegexOptions.Singleline);
                OutputText = regex.Replace(OutputText, " ");
            }
            catch (Exception ex)
            {

            }
        }

        public void RemoveMainTags()
        {
            try
            {
                Regex regex1 = new Regex(@"(<html\s*(.*?)>)(.*?)(<\/html>)", RegexOptions.Singleline);
                Match match = regex1.Match(OutputText);

                int startPoint = 0;
                int delLength = 0;

                if (match.Success)
                {
                    startPoint = match.Groups[4].Index;
                    delLength = match.Groups[4].Length;

                    OutputText = OutputText.Remove(startPoint, delLength);

                    startPoint = match.Groups[1].Index;
                    delLength = match.Groups[1].Length;

                    OutputText = OutputText.Remove(startPoint, delLength);
                }

                
                Regex regex2 = new Regex(@"(<head\s*(.*?)>)(.*?)(<\/head>)", RegexOptions.Singleline);
                match = regex2.Match(OutputText);
                if (match.Success)
                {
                    startPoint = match.Groups[0].Index;
                    delLength = match.Groups[0].Length;

                    OutputText = OutputText.Remove(startPoint, delLength);
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void RemoveExtraSpaces()
        {
            try
            {
                Regex regex = new Regex(@"(<\w*)(.*?)(\s+?)(>)", RegexOptions.Singleline);
                Match match;

                int startPoint = 0;
                int delLength =0;

                while ((match = regex.Match(OutputText)) != null && match.Success)
                {
                    startPoint = match.Groups[3].Index;
                    delLength = match.Groups[3].Length;

                    OutputText = OutputText.Remove(startPoint, delLength);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void ReplaceListToHtmlFormat()
        {
            try
            {
                Regex regex = new Regex(@"(<\w*>)(<!\[if !supportLists\]>)(.*?)([0-9]*)(\))(.*?)(<!\[endif\]>)<span>(.*?)<\/span>(<\/\w*>)", RegexOptions.Singleline);
                Match match = null;

                string resultTempStr = "<ul>\r\n";

                int startPoint = regex.Match(OutputText).Index;
                int delLength = 0;

                
                match = regex.Match(OutputText);
                bool flag = false;
                do
                {
                    flag = false;

                    int strPos = match.Groups[0].Index;
                    int currentListPos = Convert.ToInt32(match.Groups[4].Value);
                    string data = match.Groups[8].Value;
                   
                    resultTempStr += $"<li><span>{data};</span></li>\r\n";

                    int nextListPos = 0;
                    int currentMatchEndPos = 0;
                    int nextMatchStartPos = 0;
                    Match nextMatch = match.NextMatch();
                    if (nextMatch.Success)
                    {
                        currentMatchEndPos = match.Index + match.Length;
                        nextMatchStartPos = nextMatch.Index;
                        nextListPos = Convert.ToInt32(nextMatch.Groups[4].Value);                     
                    }
                    else
                    {
                        nextListPos = 1;
                        flag = true;
                    }
                    
                    if (nextListPos < currentListPos)
                    {
                        resultTempStr += "</ul>\r\n";

                        delLength = (match.Index + match.Length) - startPoint;

                        string temp = OutputText.Substring(startPoint, delLength);

                        OutputText = OutputText.Remove(startPoint, delLength);
                        OutputText = OutputText.Insert(startPoint, resultTempStr);
                        resultTempStr = "<ul>\r\n";
                        if (flag)
                            break;

                        match = regex.Match(OutputText);
                        startPoint = match.Index;
                        flag = true;

                        continue;
                    }

                    resultTempStr += OutputText.Substring(currentMatchEndPos, nextMatchStartPos - currentMatchEndPos);

                } while (flag ? match.Success : (match = match.NextMatch()).Success);

               
            }
            catch (Exception ex)
            {

            }
        }
    
        public void EditImages()
        {
            try
            {
                Regex regex = new Regex(@"(<!\[if !vml\]>)(.*)(<!\[endif\]>)", RegexOptions.Singleline);
                Match match;

                int startPoint = 0;
                int delLength = 0;

                while ((match = regex.Match(OutputText)) != null && match.Success)
                {
                    startPoint = match.Groups[3].Index;
                    delLength = match.Groups[3].Length;
                    OutputText = OutputText.Remove(startPoint, delLength);

                    startPoint = match.Groups[1].Index;
                    delLength = match.Groups[1].Length;
                    OutputText = OutputText.Remove(startPoint, delLength);
                    OutputText = OutputText.Insert(startPoint, "</v:shape>");
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
//while (((match = regex.Match(InputText)) != null ? (match = match.NextMatch()) != null : match != null) && match.Success)
