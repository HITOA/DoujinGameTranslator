using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorNS.FileParser
{
    static class HelperTokenizer<T>
    {
        public delegate KeyValuePair<int, T> tokenizerDel(string str, int index);
        public delegate void callbackDel(string data, T translatable);

        public static int TokenizeSequence(string str, int pos, char stopchar, bool ignoreFirst = true, bool takeLast = true)
        {
            int size = ignoreFirst ? 1 : 0;

            while (pos + size < str.Length) {
                size++;
                if (str[pos + size] == stopchar)
                    break;
            }

            return takeLast ? size + 1 : size;
        }

        public static int TokenizeSequence(string str, int pos, char[] stopchar, bool ignoreFirst = true, bool takeLast = true)
        {
            int size = ignoreFirst ? 1 : 0;

            while (pos + size < str.Length)
            {
                size++;
                if (stopchar.Contains(str[pos + size]))
                    break;
            }

            return takeLast ? size + 1 : size;
        }

        public static void Tokenize(string str, tokenizerDel tokenizer, callbackDel callback)
        {
            int c = 0;
            while (c < str.Length)
            {
                KeyValuePair<int, T> r = tokenizer(str, c);
                if (r.Key == 0)
                    throw new Exception("tokenizer is not allowed to return r<=0.");
                if (r.Key + c > str.Length)
                    throw new Exception("tokenizer return value outside bound of the string.");

                callback(str.Substring(c, r.Key), r.Value);

                c += r.Key;
            }
        }
    }
}
