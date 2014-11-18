using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NHibernateGenLib.Base;
using NHibernateGenLib.Domain.ImgFlash;
using WindowsFormsFlashImg.db;
using WindowsFormsFlashImg.Method;
using System.IO;

namespace WindowsFormsFlashImg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private String mSavePath = "";
        private String mLastUploadPath = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataGridViewMain.AllowUserToAddRows = false;
            LoadData();
            mSavePath = ConfigDao.QSavePath();
            mLastUploadPath = ConfigDao.QLastUploadPath();
        }
        private void LoadData()
        {
            var list = TbImgDao.queryAll();
            DataGridView dv = this.dataGridViewMain;

            foreach (DataGridViewRow dgvr in dv.Rows)
            {
                dv.Rows.Remove(dgvr);
            }

            for (int i = 0; i < list.Count; i++)
            {
                var a = list.ElementAt(i);
                dv.Rows.Add(new object[] { a.Board, a.GitBranch, a.Uboot, a.Kernel,  a.System });
            }

            foreach (DataGridViewRow dgvr in dv.Rows)
            {
                dv.ReadOnly = true;
                dgvr.HeaderCell.Value = dgvr.Index + 1 + "";
            }
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            UtilMethod.ExtraZipFile(ConfigDao.QLastUploadPath(), ConfigDao.QSavePath());
        }

        private void setUploadPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = mSavePath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                mSavePath = fbd.SelectedPath;
                ConfigDao.UpdateSavePath(mSavePath);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (mSavePath.Length == 0)
            {
                MessageBox.Show("请先设置上传目录");
                return;
            }

            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = mLastUploadPath.Length == 0 ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Path.GetDirectoryName(mLastUploadPath);
                dialog.Filter = "zip|*.zip";
                if (dialog.ShowDialog() == DialogResult.OK)
                {                    
                    mLastUploadPath = dialog.FileName;
                    //Console.WriteLine(Path.GetFileName(mLastUploadPath)); return;
                    ConfigDao.UpdateLastUploadPath(mLastUploadPath);
                    if (UtilMethod.ExtraZipFile(mLastUploadPath, mSavePath))
                    {
                        MessageBox.Show("上传成功");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("上传失败");
                    }
                }
            }
        }
    }
}
