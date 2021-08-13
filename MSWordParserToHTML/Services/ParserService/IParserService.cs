using MSWordParserToHTML.Models;
using System;

namespace MSWordParserToHTML.Services
{
    public interface IParserService
    {
        TextData InputTextData { get; set; }
        TextData OutputTextData { get; set; }

        void Start(Object path);
    }
}
