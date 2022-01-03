# DGTranslator, Machine translation for doujin game file.

This application was made to help translating game file often use  in japanese indie game (Doujin Game) into other languages with machine translation. It was first created for Krkr (KAG) KS file format but is made to be easily extensible.

**Note** :  There is a desire to implement other file formats in the future.

## How to use
Use a console to run the application. 
Be sure to have a valid Configuration file (see : Config File)

Basic Usage : 
`Translator Krkr "Game/.../File.ks" -o "Translated"`

*Krkr* is a verb, it specify wich file format you want to translate.
**by default it is Krkr so you dont have to specify it**
One or more path to the file to be translate, you can use dir path, it will try to translate every file in the directory.

Some generic parameters : 
-o --output : specify output directory  
-l --lang : specify destination language

There is Verb specific parameters, you can see them using *Verb (-h / --help)*

## Config File

Config file (config.ini in root directory of the application) contain api related configuration. like the name of the api you prefer to use (ex : DeepLFree) or other field if needed like Api Key.

Basic config.ini file : 
```
ApiName=DeepLFree
ApiKey=YourKey
```

## Implemented File Format

| Game Engine  | File Format  | Verb |
| ------------ | ------------ | ------------ |
| Krkr (Kirikiri / KAG) | KS file format  | Krkr  |

## Implemented Translation Api

| Api  | Url  | Config Name  |
| ------------ | ------------ | ------------ |
| Free DeepL Api  | https://www.deepl.com/pro-api  | DeepLFree  |


