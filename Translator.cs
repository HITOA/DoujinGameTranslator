using TranslatorNS.TranslatorApi;
using TranslatorNS.Options;
using TranslatorNS.FileParser;
using IniParser;
using IniParser.Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TranslatorNS
{
    static class Translator
    {
        private const string configPath = "config.ini";

        private static IniData config;
        private static ITranslatorApi translationApi;
        private static IFileParser fileParser;
        private static string[] filesToTranslate;
        private static string outputDirectory;
        public static void Run(Object obj)
        {
            if (!GetConfig())
                throw new Exception("no config file found.");
            if (!CreateTranslationApi())
                throw new Exception("not able to create translation api. bad api name ?");

            switch (obj)
            {
                case KrkrTranslationOptions opt:
                    fileParser = new KrkrKsFileParser(opt.encoding);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (!ProcessPathes(obj as BaseTranslationOptions))
                throw new Exception("not able to process path(es).");

            EndSettings(obj as BaseTranslationOptions);

            StartTranslating();
        }
        public static void StartTranslating()
        {
            Console.WriteLine($"Starting translation. Output directory : \"{outputDirectory}\"");

            foreach (string file in filesToTranslate)
            {
                Console.WriteLine($"Processing file : \"{file}\"");

                string filename = Path.GetFileName(file);

                fileParser.Open(file);
                fileParser.Parse();

                string[] sentences = fileParser.sentences;
                int batchSize = translationApi.GetBatchSize();
                int currentBatch = 0;
                List<string> result = new List<string>();
                
                while (currentBatch * batchSize < sentences.Length)
                {
                    Console.WriteLine($"\tProcessing batch : {currentBatch}.");
                    string[] batch = sentences.Skip(currentBatch * batchSize).Take(batchSize).ToArray();

                    batch = translationApi.TranslateBatch(batch);

                    result.AddRange(batch);

                    currentBatch++;
                }

                fileParser.sentences = result.ToArray();

                fileParser.Save($"{outputDirectory}/{filename}");
                fileParser.Clean();
            }
        }
        public static void EndSettings(BaseTranslationOptions opt)
        {
            translationApi.SetLang(opt.lang);
        }
        public static bool ProcessPathes(BaseTranslationOptions opt)
        {
            if (opt.outputDir != null)
                outputDirectory = opt.outputDir;
            else
            {
                outputDirectory = "translated";
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);
            }

            List<string> paths = new List<string>();

            foreach (string path in opt.paths)
            {
                FileAttributes attributes = File.GetAttributes(path);
                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    string[] dirpaths = Directory.GetFiles(path);
                    foreach (string dirpath in dirpaths)
                        paths.Add(dirpath);
                }else
                    paths.Add(path);
            }

            filesToTranslate = paths.ToArray();

            return true;
        }
        public static bool GetConfig()
        {
            if (!File.Exists(configPath))
                return false;

            FileIniDataParser parser = new FileIniDataParser();
            config = parser.ReadFile(configPath);

            return true;
        }
        public static bool CreateTranslationApi()
        {
            string apiName;
            if (!config.TryGetKey("ApiName", out apiName))
                throw new Exception("\"ApiName\" field missing in config file.");

            string key;
            switch (apiName)
            {
                case "DeepLFree":
                    if (!config.TryGetKey("ApiKey", out key))
                        throw new Exception("DeepLFree need \"ApiKey\" field in config file.");
                    translationApi = new DeepLFreeTranslatorApi(key);
                    break;
                case "DeepLPro":
                    if (!config.TryGetKey("ApiKey", out key))
                        throw new Exception("DeepLPro need \"ApiKey\" field in config file.");
                    translationApi = new DeepLProTranslatorApi(key);
                    break;
                default: return false;
            }

            return true;
        }
    }
}
