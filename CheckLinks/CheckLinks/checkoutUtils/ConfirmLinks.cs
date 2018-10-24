using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Media;
using CheckLinks;

namespace CheckLinks.checkoutUtils
{
    class ConfirmLinks
    {
        private HtmlWeb web;
        private HtmlDocument htmlDoc;
        private String[] asOtherTagsPath;
        private String sTagsPath;
        private String myResp;
        private HtmlNodeCollection hncAnkers;
        private HtmlNodeCollection hncDiv;
        private HtmlNodeCollection hncAnkersInDiv;
        private bool blResult;
        private bool blFindDiv;
        private int k;
        private Window mw;

        public void getNodesTagsA(String value)
        {//getNodesTagsA
            web = new HtmlWeb();
            asOtherTagsPath = new String[5];
            try
            {//try
                htmlDoc = web.Load(value);
            }//try
            catch (System.Exception ex)
            {//catch
                myResp = value +" /n"+" Некорректный адресс, возникла ошибка: " + ex.Message;
                MessageBox.Show(myResp);
                return;
            }//catch
            hncAnkers = new HtmlNodeCollection(htmlDoc.DocumentNode.SelectSingleNode(sTagsPath + "a"));
            hncDiv = null;
            k = 0;
            sTagsPath = "//body/";
            blFindDiv = false;
            Found:
            while (!blFindDiv)
            {//while
                hncDiv = null;
                hncAnkersInDiv = null;
                hncAnkersInDiv = htmlDoc.DocumentNode.SelectNodes(sTagsPath + "a");
                hncDiv = htmlDoc.DocumentNode.SelectNodes(sTagsPath + "div");
                if (htmlDoc.DocumentNode.SelectNodes(sTagsPath + "header") != null)
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
            if (asOtherTagsPath[k] != null)
                if (asOtherTagsPath[k].Length > 0)
                {
                    blFindDiv = false;
                    sTagsPath = asOtherTagsPath[k];
                    asOtherTagsPath[k] = "";
                    if (k < 4)
                        k++;
                    goto Found;
                }
                else
                {
                    k++;
                    if (k < 5)
                        goto ConfirmOtherTags;
                }
            else
            {
                k++;
                if (k < 5)
                    goto ConfirmOtherTags;
            }

            Score score = new Score();
            score.scoreLinks.Text = hncAnkers.LongCount().ToString();
            score.ShowDialog();


        }//getNodesTagsA
    }
}
