using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilsLib.Doc
{
    public class DocHelper
    {
        public static bool SaveToPDF(string srcFileName, string dstFileName)
        {
            File.Delete(dstFileName);
            using (Spire.Doc.Document doc = new Spire.Doc.Document(srcFileName))
            {
                doc.SaveToFile(dstFileName, Spire.Doc.FileFormat.PDF);
            }
            return File.Exists(dstFileName);
        }
    }
}
