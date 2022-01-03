using CommandLine;

namespace TranslatorNS.Options
{
    [Verb("Krkr", isDefault: true, HelpText = "Translate Krkr game file.")]
    class KrkrTranslationOptions : BaseTranslationOptions
    {
        public enum KrkrEncodingType
        {
            sjis,
            utf16le
        }

        [Option('e', "encoding", Default = KrkrEncodingType.utf16le, HelpText = "Specify encoding for all the file.")]
        public KrkrEncodingType encoding { get; set; }
    }
}
