using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilsLib.IO.Ini
{
    public class IniFiles
    {
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, System.Text.StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        private string _filename;
        public IniFiles(string filename)
        {
            _filename = filename;
        }

        public string get_value(string sector, string key)
        {
            StringBuilder sp = new StringBuilder(512);
            long returnValue = GetPrivateProfileString(sector, key, "", sp, 512, _filename);
            return sp.ToString().Trim();
        }

        public void set_value(string sector, string key, string value)
        {
            WritePrivateProfileString(sector, key, value, _filename);
        }
    }
}
