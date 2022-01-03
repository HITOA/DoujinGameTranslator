using CommandLine;
using System.Collections.Generic;
using TranslatorNS.TranslatorApi;

namespace TranslatorNS.Options
{
    class BaseTranslationOptions
    {
        [Value(0, HelpText = "Path to File or Directory to be translated.", Required = true)]
        public IEnumerable<string> paths { get; set; }

        [Option('o', "output", Default = null, HelpText = "Specify output directory.")]
        public string? outputDir { get; set; }

        [Option('l', "lang", Default = Lang.English, HelpText = "Destination language for translation.")]
        public Lang lang { get; set; }
    }
}
