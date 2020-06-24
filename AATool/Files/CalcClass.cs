using System;
using System.Collections.Generic;
using UtilsLib;

namespace Pub.Files
{
    public class CalcClass
    {
        public decimal Add(decimal a, decimal b)
        {
            return a + b;
        }

        public List<string> GetNewList(List<string> lst)
        {
            return lst.FindAll(m => { return m.CompareTo("b") > 0; });
        }

        public byte[] ToHex(string input)
        {
            return StringToByte.strToToHexByte(input);
        }
    }
}
