using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

using NHibernateGenLib.Domain.ImgFlash;
using WindowsFormsFlashImg.db;

namespace WindowsFormsFlashImg.Method
{
    public class UtilMethod
    {
        private static String xmlGetByTag(XmlDocument xDoc, String tag)
        {
            return xDoc.GetElementsByTagName(tag)[0].InnerText;
        }
        public static bool ExtraZipFile(String zip, String saveDir)
        {
            String xml = "";
            using (ZipArchive archive = ZipFile.OpenRead(zip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        xml = Path.Combine(saveDir, entry.FullName);
                        if (File.Exists(xml)) File.Delete(xml);
                        entry.ExtractToFile(xml);
                        break;
                    }
                }
            }
            if (xml.Length == 0)
            {
                return false;
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xml);

            var board = xmlGetByTag(xDoc, "board");           
            var gitBranch = xmlGetByTag(xDoc, "gitBranch");            
            var lastestVer = xmlGetByTag(xDoc, "latestVer");
            var imgType = xmlGetByTag(xDoc, "imgType");
            var newFile = Path.Combine(saveDir, board + "_" + gitBranch, Path.GetFileName(zip));

            if (!Directory.Exists(Path.GetDirectoryName(newFile))) Directory.CreateDirectory(Path.GetDirectoryName(newFile));
            if (File.Exists(newFile)) File.Delete(newFile);

            File.Copy(zip, newFile);
            TbImgDao.updateOrInsert(board, gitBranch, imgType, lastestVer, newFile);

            return true;
        }
    }
}
