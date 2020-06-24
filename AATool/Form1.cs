using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace AATool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var types = this.GetType().Assembly.GetTypes();
            if (types == null || types.Length <= 0)
            {
                return;
            }
            var g_types = types.ToList().FindAll(m => { return m.FullName.Contains("AATool.Tests")&&  typeof(ITest).IsAssignableFrom(m) ; });
            if (g_types == null || g_types.Count <= 0)
            {
                return;
            }
            foreach (var g_type in g_types)
            {
                ITest test = Activator.CreateInstance(g_type) as ITest;
                test.Test();
            }
        }
    }
}
