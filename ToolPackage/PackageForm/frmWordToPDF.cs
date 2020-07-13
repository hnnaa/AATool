using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilsLib.Doc;

namespace ToolPackage.PackageForm
{
    public partial class FrmWordToPDF : Form, IPackageForm
    {
        public FrmWordToPDF()
        {
            InitializeComponent();
        }

        public string PackageName { get { return "Word转PDF"; } }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.OK == openFileDialog1.ShowDialog())
                {
                    txtFile.Text = openFileDialog1.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常");
            }
        }

        private void btnTransform_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "PDF文件|*.pdf";
                if (DialogResult.OK == saveFileDialog1.ShowDialog())
                {
                    Application.DoEvents();
                    if (DocHelper.SaveToPDF(txtFile.Text, saveFileDialog1.FileName))
                    {
                        MessageBox.Show("成功");
                    }
                    else
                    {
                        MessageBox.Show("失败");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常");
            }
        }
    }
}
