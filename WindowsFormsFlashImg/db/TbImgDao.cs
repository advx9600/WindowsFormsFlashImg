using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate;

using NHibernateGenLib.Base;
using NHibernateGenLib.Domain.ImgFlash;

namespace WindowsFormsFlashImg.db
{
    public class TbImgDao : Dao
    {
        public static IList<TbImage> queryAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.QueryOver<TbImage>().List();
        }

        public static void updateOrInsert(String board, String gitBranch, String imgType, String lastestVer, String newFile)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                Boolean isUpdate = true;
                var list = session.QueryOver<TbImage>().Where(c => c.Board == board).And(c => c.GitBranch == gitBranch).List();
                if (list.Count == 0) isUpdate = false;
                TbImage img = isUpdate ? list.ElementAt(0) : new TbImage();
                img.Board = board;
                img.GitBranch = gitBranch;
                switch (imgType)
                {
                    case "uboot":
                    case "u-boot": img.Uboot = lastestVer; img.UbootPath = newFile; break;
                    case "kernel": img.Kernel = lastestVer; img.KernelPath = newFile; break;
                    case "system": img.System = lastestVer; img.SystemPath = newFile; break;
                }
                if (isUpdate) Update(img); else Add(img);
            }
        }
    }


    public class ConfigDao : Dao
    {
        private static String config_SavePath = "save_path";
        private static String config_LastUploadPath = "last_upload_path";

        private static String QueryConfig(String name)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var list = session.QueryOver<TbConfig>().Where(c => c.Name == name).List();
                if (list.ElementAt(0).Val == null) return "";
                return list.ElementAt(0).Val.ToString();
            }
        }
        public static String QSavePath()
        {
            return QueryConfig(config_SavePath);
        }
        public static void UpdateSavePath(String val)
        {
            updateConfig(config_SavePath, val);
        }
        public static String QLastUploadPath()
        {
            return QueryConfig(config_LastUploadPath);
        }
        public static void UpdateLastUploadPath(String val)
        {
            updateConfig(config_LastUploadPath, val);
        }

    }
}
