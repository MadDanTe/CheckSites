using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;
//using Formatto.Core;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
//using Core.Standard;

namespace ConfirmSitemap
{
    class LinksSitemap
    {//LinksSitemap
        private HtmlWeb web;
        private HtmlDocument htmlDoc;
        private HtmlNodeCollection hncAnkers;
        private HtmlNodeCollection hncDiv;
        private HtmlNodeCollection hncAnkersInDiv;
        private HttpWebRequest hwrRequest;
        private WebRequest wrRequest;
        private WebResponse wrResponse;
        private HttpWebResponse hwrRespons;
        private String sTagsPath;
        private String myResp;
        private String[] asOtherTagsPath;
        private bool blResult;
        private bool blFindDiv;
        private int k;
        private ChromeDriver cdChrome;
        private ICollection icLogs;

        /*
         * 
         * Получаем элемент в виде тега <a> находящегося в разметке у нас по пути //body/div/main/div/a, а после возвращаем колекцию данных тегов.
         * Исключения добавленно для отлавливания некорректного адресса.
         *
         */
        public HtmlNodeCollection getNodesTagsA(String value)
        {//getNodesTagsA
            Console.WriteLine("Анализ " + value + ", подождите...");
            web = new HtmlWeb();
            asOtherTagsPath = new String[5];
            try
            {//try
                htmlDoc = web.Load(value);
            }//try
            catch (System.Exception ex)
            {//catch
               // FileLog.AddLogWithConsole(value+"\n "+"Некорректный адресс, возникла ошибка: " + ex.Message);
                myResp = Console.ReadLine();
                System.Environment.Exit(0);
            }//catch
            hncAnkers = new HtmlNodeCollection(htmlDoc.DocumentNode.SelectSingleNode(sTagsPath+"a"));
            hncDiv = null;
            k = 0;
            sTagsPath = "//body/";
            blFindDiv = false;
            Found:
            while (!blFindDiv)
            {//while
                hncDiv = null;
                hncAnkersInDiv = null;
                hncAnkersInDiv = htmlDoc.DocumentNode.SelectNodes(sTagsPath+"a");
                hncDiv = htmlDoc.DocumentNode.SelectNodes(sTagsPath + "div");
                if(htmlDoc.DocumentNode.SelectNodes(sTagsPath + "header")!=null)
                    asOtherTagsPath[0] = sTagsPath + "header/";
                if (htmlDoc.DocumentNode.SelectNodes(sTagsPath + "main") != null)
                    asOtherTagsPath[1] = sTagsPath + "main/";
                if (htmlDoc.DocumentNode.SelectNodes(sTagsPath + "footer") != null)
                    asOtherTagsPath[2] = sTagsPath + "footer/";
                if (htmlDoc.DocumentNode.SelectNodes(sTagsPath + "aside") != null)
                    asOtherTagsPath[3] = sTagsPath + "aside/";
                if (htmlDoc.DocumentNode.SelectNodes(sTagsPath + "nav") != null)
                    asOtherTagsPath[4] = sTagsPath + "nav/";
                if (hncDiv != null)
                    sTagsPath = sTagsPath + "div/";
                else
                    blFindDiv = true;
                if (hncAnkersInDiv != null)
                    foreach (HtmlNode a in hncAnkersInDiv)
                    {//foreach
                        hncAnkers.Add(a);
                    }//foreach
            }//while
            ConfirmOtherTags:
           if(asOtherTagsPath[k]!=null)
                if (asOtherTagsPath[k].Length > 0)
                {
                     blFindDiv = false;
                     sTagsPath = asOtherTagsPath[k];
                     asOtherTagsPath[k] = "";
                     if(k<4)
                            k++;
                     goto Found;
                }
                else
                {
                     k++;
                     if(k<5)
                     goto ConfirmOtherTags;
                }
            else
            {
                k++;
                if (k < 5)
                    goto ConfirmOtherTags;
            }

            return hncAnkers;
        }//getNodesTagsA

        /*
         *
         * Разбираем тег <a> и получаем из него ссылки на странице. после чего запросом Get обращаемся к ним и проверяем статус ответа от сервера.
         * при обращении к сайту интервал ожидания слишком велик и выпадает исключения первышения интервала ожидания.
         * 
         */

        public bool confirmLinks(HtmlNodeCollection hncCollection, String sHost)
        {//confirmLinks
            Console.WriteLine("Идёт проверка ссылок, подождите...");
            int i = 1;
            ChromeOptions coChrome = new ChromeOptions();
            ChromePerformanceLoggingPreferences cplpChrome = new ChromePerformanceLoggingPreferences();
            cplpChrome.AddTracingCategories(new string[] { "devtools.timeline" });
            coChrome.PerformanceLoggingPreferences = cplpChrome;
            coChrome.AddArgument("--also-emit-success-logs");
            coChrome.SetLoggingPreference("performance", LogLevel.All);
            cdChrome = new ChromeDriver(coChrome);
            String sHref = "";
            foreach (HtmlNode node in hncCollection)
            {//foreach
                bool console = false;
                sHref = Regex.Split(node.Attributes["href"].Value, @"\?\w*")[0];
                sHost = Regex.Replace(sHost, @"/sitemap", "");
                if (!Regex.IsMatch(sHref, @"http\w*")&&!Regex.IsMatch(sHref, @"//\w*"))
                    sHref = sHost + sHref;
                cdChrome.Navigate().GoToUrl(sHref);
                var l2 = cdChrome.Manage().Logs.GetLog("browser");
                //if (l2.Count == 0)
                    //FileLog.AddLog(i + ". " + sHref + " - 200 OK" + "\n");
                //else
                    foreach (LogEntry log in l2)
                    {//foreach
                        if (Regex.IsMatch(log.Message, "\\w*" + sHref + " - \\w*"))
                        {//then
                           // FileLog.AddLogWithConsole(i + ". " + Regex.Replace(log.Message, "\\w*" + sHref, sHref) + "\n");
                            blResult = true;
                            console = true;
                            break;
                        }//then

                    }//foreach
                if (!console && l2.Count!=0)
                {//then
                    //FileLog.AddLogWithConsole(i + ". " + sHref + " Console Error:"+"\n");
                    foreach (LogEntry log in l2)
                       // FileLog.AddLogWithConsole("     "+log.Message + "\n");
                   blResult = true;
                }//then
                i++;
            }//foreach
            cdChrome.Close();
            return blResult;
        }//confirmLinks
    }//LinksSitemap
}
