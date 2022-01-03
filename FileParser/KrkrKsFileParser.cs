using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslatorNS.Options;

namespace TranslatorNS.FileParser
{
    internal class KrkrKsFileParser : IFileParser
    {
        public enum TokenType
        {
            TEXT,
            OTHER
        }

        private string? krkrKsFileStr = null;
        private List<KeyValuePair<string, TokenType>>? krkrKsFileTokens = null;

        public KrkrTranslationOptions.KrkrEncodingType encoding;

        public string[] sentences
        {
            get;
            set;
        }

        public KrkrKsFileParser(KrkrTranslationOptions.KrkrEncodingType encoding)
        {
            this.encoding = encoding;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public Encoding GetEncoding()
        {
            switch (encoding)
            {
                case KrkrTranslationOptions.KrkrEncodingType.sjis: return Encoding.GetEncoding(932);
                case KrkrTranslationOptions.KrkrEncodingType.utf16le: return Encoding.Unicode;
                default: throw new Exception("bad encoding.");
            }
        }

        public void Open(string path)
        {
            byte[] fileBuffer = File.ReadAllBytes(path);
            krkrKsFileStr = Encoding.Unicode.GetString(Encoding.Convert(GetEncoding(), Encoding.Unicode, fileBuffer));
        }

        public void Parse()
        {
            if (krkrKsFileStr == null)
                return;

            List<KeyValuePair<string, TokenType>> tokens = new List<KeyValuePair<string, TokenType>>();
            List<string> vs = new List<string>();

            HelperTokenizer<TokenType>.Tokenize(krkrKsFileStr,
                Tokenizer,
                (string str, TokenType type) =>
                {
                    tokens.Add(new KeyValuePair<string, TokenType>(str, type));
                    if (type == TokenType.TEXT)
                        vs.Add(str);
                });

            krkrKsFileTokens = tokens;
            sentences = vs.ToArray();
        }

        public KeyValuePair<int, TokenType> Tokenizer(string str, int pos)
        {
            int size = 1;
            TokenType type = TokenType.OTHER;

            switch(str[pos])
            {
                case ';':
                    size = HelperTokenizer<TokenType>.TokenizeSequence(str, pos, '\n');
                    break;
                case '@':
                    size = HelperTokenizer<TokenType>.TokenizeSequence(str, pos, '\n');
                    break;
                case '*':
                    size = HelperTokenizer<TokenType>.TokenizeSequence(str, pos, '\n');
                    break;
                case '[':
                    size = HelperTokenizer<TokenType>.TokenizeSequence(str, pos, ']');
                    break;
                case '\n':
                    size = 1;
                    break;
                default:
                    size = HelperTokenizer<TokenType>.TokenizeSequence(str, pos, new char[] { '\n', ';', '[', ']' }, false, false);
                    if (size > 2)
                        type = TokenType.TEXT;
                    break;
            }

            return new KeyValuePair<int, TokenType>(size, type);
        }
        public void Save(string path)
        {
            if (krkrKsFileStr == null)
                return;

            string filetosave = string.Empty;
            int sentencesIndex = 0;

            foreach (var token in krkrKsFileTokens)
            {
                if (token.Value == TokenType.TEXT)
                {
                    filetosave += sentences[sentencesIndex];
                    sentencesIndex++;
                }
                else
                    filetosave += token.Key;
            }

            byte[] fileBuffer = Encoding.Convert(Encoding.Unicode, GetEncoding(), 
                Encoding.Unicode.GetBytes(filetosave));
            File.WriteAllBytes(path, fileBuffer);
        }

        public void Clean()
        {
            krkrKsFileStr = null;
            krkrKsFileTokens = null;
        }
    }
}
