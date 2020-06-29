using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilsLib.FtpUtils;

namespace AATool.Tests
{
    public class TestSftp : ITest
    {
        public void Test()
        {
			try
			{
				SFTPHelper sFTP = new SFTPHelper("122.224.250.39","55535","sftp01","sftp01@1234");
				sFTP.Put(@"E:\home\hnn\test\t1.txt", "/root/geo/FrankWindowsServiceLogs/t1.txt");
			}
			catch (Exception ex)
			{

				
			}
        }


    }
}
