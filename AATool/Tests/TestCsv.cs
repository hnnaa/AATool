using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AATool.Tests
{
    public class TestCsv : ITest
    {
        public void Test()
        {
            FileInfo f = new FileInfo("TestCsv.csv");
            using (var sw=new StreamWriter(f.OpenWrite()))
            {
                sw.WriteLine("`2020-10-10 00:00:00,yo");
                sw.Flush();
            }
        }
    }
}
