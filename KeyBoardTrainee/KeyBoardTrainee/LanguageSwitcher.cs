using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeyBoardTrainee
{
    class LanguageSwitcher
    {
        private List<string> _langs;

        public List<string> Lang
        {
            get { return _langs; }
        }
        public LanguageSwitcher(string path, string mask) // path = @"..\..\lang\"; mask = "Strings";
        {
            _langs = new List<string>(); // создается пустой список для строк
            initLang(path, mask);//вызывается метод initLang(), который
        }
        private void initLang(string path, string mask) // path = @"..\..\lang\"; mask = "Strings";
        {
            //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            if (Directory.Exists(path))  //существование папки по указанному пути
            {
                //path += "\\"; или path += @"\";
                string[] fileEntries = Directory.GetFiles(path);//получить список имен файлов в папке lang в виде массива строк



                foreach (var item in fileEntries)//item - file name, а fileEntries - это массив имен файлов 
                {
                    if (File.Exists(item))// проверка существования файла
                    {
                        if (Path.GetExtension(item).Equals(".resx"))//GetExtension - берет расширение у имени файла
                        {
                            if (item.ToLower().Contains(mask.ToLower()))// делаем все буквы маленькие имени файла, аналогично с маской - "Strings", проверяем , что имя файла содержит слово String
                            {

                                Regex r = new Regex(@"\.(\w+)\.resx", RegexOptions.IgnoreCase);//\.(\w+)- один или более алфавитно-цифрофой символ
                                Match m = r.Match(item);//посик по регулярному выражения в item - имя файла, m =.ru.resx

                                Group g = m.Groups[1];//(\w+)=Groups, часть найденной строки, которая в круглых скобках, часть шаблона
                                _langs.Add(g.Value.Length > 0 ? g.Value : "en");

                            }
                        }
                    }
                }
            }
        }
    }
}
