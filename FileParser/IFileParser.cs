using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorNS.FileParser
{
    interface IFileParser
    {
        string[] sentences { get; set; }
        void Open(string path);
        void Save(string path);
        void Parse();
        void Clean();
    }
}
