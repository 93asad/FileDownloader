using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;

namespace FileDownloader
{
    public class Program
    {
        public void Main(string[] httpLinks)
        {
            XmlFileDownloader downloader = new XmlFileDownloader(httpLinks);
        }
    }

    internal class XmlFileDownloader
    {
        private const string HTML_START = "http://www";
        private const string FTP_START = "ftp://ftp"; 
        private const string ARCHIVES = "/Archives";
        private const string LINK_END_REGEX = @"(/[0-9a-zA-Z-]+.[a-z]+)$";
        private const string EMPTY_STRING = "";
        private const string XML_REGEX = @"([0-9a-zA-Z-_]+.xml)$";

        private string ftpDirLink;
        private string httpLink;
        

        public XmlFileDownloader(string[] httpLinks)
        {
            DownloadXMLFiles(httpLinks);
        }

        private void DownloadXMLFiles(string[] httpLinks)
        {
            foreach (string httpLink in httpLinks)
            {
                this.httpLink = httpLink; 
                this.ftpDirLink = GetFtpDirLink(httpLink);
                List<string> xmlFileNames = GetXmlFileNames(ftpDirLink);

                DownloadXMLFiles(xmlFileNames);
            }
        }

        private List<string> GetXmlFileNames(string ftpDirLink)
        {
            List<string> xmlFileNames = new List<string>();

            try
            {

                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpDirLink);

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    Match match = Regex.Match(reader.ReadLine(), XML_REGEX);
                    if (!match.Success)
                        continue;

                    xmlFileNames.Add(match.Groups[0].Value);
                }
                
                //Clean-up
                reader.Close();
                responseStream.Close(); //redundant
                response.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error connecting to the FTP Server");
            }

            return xmlFileNames;
        }

        private string GetFtpDirLink(string httpLink)
        {
          /*
            change :http://www" to "ftp://ftp"
            -remove "Archives/"
           - remove the last part of the link, in this case, it is "0001144204-15-055309-index.htm"

            final result is: ftp://ftp.sec.gov/edgar/data/1086745/000114420415055309
          */

            // get Ftp directory link
            string ftpDirLink = httpLink.Replace(HTML_START, FTP_START).Replace(ARCHIVES, EMPTY_STRING);
            ftpDirLink = Regex.Replace(ftpDirLink, LINK_END_REGEX, EMPTY_STRING);
            Console.WriteLine(ftpDirLink);
           
            return ftpDirLink;
        }
    
        private void DownloadXMLFiles(List<string> xmlFileNames)
        {
            string zipFileName = getZipFileName();

            foreach (string xmlFileName in xmlFileNames)
            {
                string ftpLink = ftpDirLink + "/" + xmlFileName;
                Console.WriteLine(ftpLink);
                Console.WriteLine(xmlFileName);

                

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpLink);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                
                setDirectory("AlternativeDownload");

                





                    using (var fileStream = new FileStream(@"AlternativeDownload/" + zipFileName, FileMode.OpenOrCreate))
                    {
                        using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Update, true))
                        {
                            var demoFile = archive.CreateEntry(xmlFileName);

                            using (var entryStream = demoFile.Open())
                            {
                                int bufferSize = 2048;
                                int readCount;
                                byte[] buffer = new byte[2048];

                                readCount = responseStream.Read(buffer, 0, bufferSize);
                                while (readCount > 0)
                                {
                                    entryStream.Write(buffer, 0, bufferSize);
                                    readCount = responseStream.Read(buffer, 0, bufferSize);
                                }
                        }
                    }
                }    
            }
        }

        private string getZipFileName()
        {
            Console.WriteLine("" + "sadasdsadsad");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(httpLink);
            /*String s = htmlDoc.DocumentNode.SelectSingleNode("/div[@class='infoHead'").FirstChild.InnerText;
            Console.WriteLine(s + "sadasdsadsad");
            Console.ReadKey();*/
            return " ";
        }

        private void setDirectory(string dirName)
        {
            if (!Directory.Exists(dirName))
            {
                DirectoryInfo directory = Directory.CreateDirectory(dirName);
            }
        }
    }
}

    

