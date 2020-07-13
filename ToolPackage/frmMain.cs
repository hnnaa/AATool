using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UtilsLib.IO.Ini;

namespace ToolPackage
{
    public partial class FrmMain : Form
    {
        private const string ini_sector = "config";
        private const string ini_key = "last_open_form";
        private IniFiles ini;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Init()
        {
            try
            {
                MenuProject.DropDownItems.Clear();

                var types = this.GetType().Assembly.GetTypes();
                var pakageForms = types.ToList().FindAll(m => { return m.FullName.Contains("ToolPackage.PackageForm") && typeof(IPackageForm).IsAssignableFrom(m); });
                if (null == pakageForms || pakageForms.Count <= 0)
                {
                    return;
                }

                foreach (var frm in pakageForms)
                {
                    var frmInstance = Activator.CreateInstance(frm) as IPackageForm;
                    var tsmi = new ToolStripMenuItem
                    {
                        Text = frmInstance.PackageName,
                        Size = new System.Drawing.Size(44, 21),
                        Name = "toolItems" + frmInstance.PackageName
                    };
                    tsmi.Click += (s, e) => { ShowForm(frm); };
                    MenuProject.DropDownItems.Add(tsmi);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("初始化异常");
            }
        }

        private void ShowForm(Type t)
        {
            try
            {
                foreach (var c in this.panelMain.Controls)
                {
                    if (typeof(Form).IsAssignableFrom(c.GetType()))
                    {
                        (c as Form).Close();
                        (c as Form).Dispose();
                    }
                }
                this.panelMain.Controls.Clear();

                ini.set_value(ini_sector, ini_key, t.FullName);
                var frm = Activator.CreateInstance(t) as Form;
                frm.TopLevel = false;
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
                this.Height = frm.Height + 10;
                this.Width = frm.Width + 10;
                this.panelMain.Controls.Add(frm);
                frm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开失败" + ex.Message);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                Init();

                ini = new IniFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
                string type = ini.get_value(ini_sector, ini_key);

                //自动打开
                if (!string.IsNullOrEmpty(type))
                {
                    ShowForm(Type.GetType(type));
                }
            }
            catch
            {
            }
        }
    }
}
