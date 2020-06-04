using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace UtilsLib
{
    /// <summary>
    /// 临时文件创建类，存放本地数据（lyh）
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// DataSet To xml
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool XmlSaveDataSet(DataSet ds, string path, string filename)
        {
            try
            {
                if (ds != null)
                {
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    if (System.IO.File.Exists(path + "\\" + filename))
                        System.IO.File.Delete(path + "\\" + filename);
                    ds.WriteXml(path + "\\" + filename);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        /// <summary>
        /// DataTable 保存xml
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool XmlSaveDataTable(DataTable dt, string path, string filename)
        {
            try
            {
                if (dt != null)
                {
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    dt.WriteXml(path + "\\" + filename);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        /// <summary>
        /// 读取xml到DataSet
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DataTable XmlToDataTable(string path, string filename)
        {
            try
            {
                DataTable dt = new DataTable();
                if (System.IO.File.Exists(path + "\\" + filename))
                {
                    dt.ReadXml(path + "\\" + filename);
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                if (System.IO.File.Exists(path + "\\" + filename))
                    System.IO.File.Delete(path + "\\" + filename);
                throw;
            }


        }
        /// <summary>
        /// 读取xml到DataSet
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DataSet XmlToDataSet(string path, string filename)
        {
            try
            {
                DataSet ds = new DataSet();
                if (System.IO.File.Exists(path + "\\" + filename))
                {
                    ds.ReadXml(path + "\\" + filename);
                    return ds;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                if (System.IO.File.Exists(path + "\\" + filename))
                    System.IO.File.Delete(path + "\\" + filename);
                throw;
            }


        }
        /// <summary>
        /// 删除目录内的所有文件和子目录
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool DeleteChildsFilesAndDirs(string dirPath)
        {
            bool isOk = false;
            try
            {
                if (Directory.Exists(dirPath))
                {
                    string[] files = Directory.GetFiles(dirPath);
                    for (int i = 0; i < files.Length; i++)
                    {
                        File.Delete(files[i]);
                    }
                    string[] dirs = Directory.GetDirectories(dirPath);
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        Directory.Delete(dirs[i], true);
                    }
                }
                else
                {
                    CreateDirtory(dirPath);
                }

                isOk = true;
            }
            catch
            {
                isOk = false;
            }
            return isOk;
        }

        /// <summary>
        /// 检测某个文件夹是否存在
        /// </summary>
        /// <param name="path">绝对路径，比如：D:\MyFolder</param>
        /// <returns>True：存在；False：不存在</returns>
        public static bool ExistsDictionary(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 删除文件夹和下属子文件夹
        /// </summary>
        /// <param name="path">指定创建的文件夹名称，比如D:\\MyFolder</param>
        /// <returns>True：删除成功；False：删除失败</returns>
        public static bool DeletedDirectory(string path)
        {
            bool flag = false;

            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception)
            {

                flag = false;
            }

            return flag;

        }

        /// <summary>
        /// 创建文件夹和子文件夹
        /// </summary>
        /// <param name="path">指定创建的文件夹名称，比如D:\\MyFolder，如果D下面没有，会自动创建</param>
        /// <returns>True：创建成功；False：创建失败</returns>
        public static void CreateDirtory(string path)
        {
            string tmpPath = path.Replace('/', '\\');
            if (!File.Exists(tmpPath))
            {
                string[] dirArray = tmpPath.Split('\\');
                string temp = string.Empty;
                for (int i = 0; i < dirArray.Length; i++)
                {
                    temp += dirArray[i].Trim() + "\\";
                    if (!Directory.Exists(temp))
                        Directory.CreateDirectory(temp);
                }
            }

        }

        /// <summary>
        /// 复制目录及子目录的所有文件到新的目录
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="objPath"></param>
        public static void CopyFiles(string sourseFileDir, string destFileDir)
        {
            CopyFiles(sourseFileDir, destFileDir, new Func<string, bool>((m) => { return true; }), new Func<string, bool>((m) => { return true; }));
        }
        /// <summary>
        /// 复制目录及子目录的所有文件到新的目录
        /// </summary>
        /// <param name="sourseFileDir">源</param>
        /// <param name="destFileDir">目标</param>
        /// <param name="checkFileFunc">true：复制文件</param>
        /// <param name="checkDirFunc">true：复制目录</param>
        public static void CopyFiles(string sourseFileDir, string destFileDir, Func<string, bool> checkFileFunc, Func<string, bool> checkDirFunc)
        {
            if (!Directory.Exists(destFileDir))
            {
                Directory.CreateDirectory(destFileDir);
            }

            string[] files = Directory.GetFiles(sourseFileDir);
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = Path.Combine(destFileDir, Path.GetFileName(files[i]));
                if (checkFileFunc(filePath))
                    File.Copy(files[i], filePath, true);
            }

            string[] dirs = Directory.GetDirectories(sourseFileDir);
            for (int i = 0; i < dirs.Length; i++)
            {
                string dirPath = Path.Combine(destFileDir, Path.GetFileName(dirs[i]));
                if (checkDirFunc(dirPath))
                    CopyFiles(dirs[i], dirPath, checkFileFunc, checkDirFunc);
            }
        }
        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetFileMD5Hash(string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名(包括后缀名)</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static bool WriteFile(string path, string name, string content)
        {
            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = path + "/" + name;
                if (!File.Exists(filename))
                {
                    FileStream fs = File.Create(filename);
                    fs.Close();
                }
                StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.GetEncoding("gb2312"));
                sw.WriteLine(content);
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
                string content = sr.ReadToEnd().ToString();
                sr.Close();
                return content;
            }
            catch
            {
                return "";
            }
        }
    }
}
