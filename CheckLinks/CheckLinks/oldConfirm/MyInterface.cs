//using Core.Standard;
//using Formatto.Core;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfirmSitemap
{
    class MyInterface
    {
        private String myResp;
        private LinksSitemap lsCheck;
        private HtmlNodeCollection hncAnkers;
        private bool blResult;

        public MyInterface()
        {//MyInterface()
            myResp = "";
            hncAnkers = null;
            lsCheck = new LinksSitemap();
            blResult = true;

            Console.WriteLine("Введите адрес к sitemap (Если оставить поле пустым, по умолчанию будет проверен http://localhost:54753/sitemap):");
            myResp = Console.ReadLine();
            if (myResp.Length == 0)
                myResp = @"http://localhost:54753/sitemap";
            else
            {//else
                if (!Regex.IsMatch(myResp, @"http\w*"))
                    myResp = "http://" + myResp;
                if(!Regex.IsMatch(myResp, @"\w*/sitemap"))
                    myResp = myResp+"/sitemap";
            }//else
            hncAnkers = lsCheck.getNodesTagsA(myResp);
            //if (hncAnkers != null && hncAnkers.LongCount()!=0)
               // FileLog.AddLogWithConsole("Найденно " + hncAnkers.LongCount() + " ссылок" + "\n");
            //else
            //{//else
                //FileLog.AddLogWithConsole("Ссылки не найдены..." + "\n" + "Нажмите Enter для выхода.");
                myResp = Console.ReadLine();
                System.Environment.Exit(0);
            //}//else
            Console.WriteLine("Нажмите Enter для проверки ссылок");
            Console.ReadLine();
            blResult = lsCheck.confirmLinks(hncAnkers, myResp);
            if (blResult)
                Console.WriteLine("Проверка завершена. Имеються ошибки. Полный отчёт смотрите в logs." + "\n" + "Для выхода нажмите Enter.");
            else
                Console.WriteLine("Проверка завершенна без ошибок." + "\n" + "Для выхода нажмите Enter.");
            myResp = Console.ReadLine();
            System.Environment.Exit(0);
        }//MyInterface()
    }
}
