using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace AATool
{
    public static class bangzhu
    {
        private static string prikeyPath = @"D:\work\project\二次开发\073-贵阳平台项目20200602\cert\client01.key.p12";
        private static string pubkeyPath = @"D:\work\project\二次开发\073-贵阳平台项目20200602\cert\client01.key.p12";
        //private static string prikeyPath = @"E:\证书地址.der";
        //private static string pubkeyPath = @"E:\证书.crt";
        //用私钥生成签名
        public static string sign(string unsign)
        {
            string prikey = DecodeDERKey(prikeyPath);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.FromXmlString(prikey);
            }
            catch (Exception ex)
            {
                return "";
            }
            // 加密对象 
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsa);
            f.SetHashAlgorithm("SHA1");
            //把要签名的源串转化成字节数组
            byte[] source = System.Text.UnicodeEncoding.Default.GetBytes(unsign);
            SHA1Managed sha = new SHA1Managed();
            //对签名源串做哈昔算法,为sha1.
            byte[] result = sha.ComputeHash(source);
            string s = Convert.ToBase64String(result);
            Console.WriteLine(s);
            //对哈昔算法后的字符串进行签名.
            byte[] b = f.CreateSignature(result);
            //生长签名对象sign
            string sign = Convert.ToBase64String(b);
            return sign;
        }
        //用公钥进行签名的验证
        public static bool verify(string unsign, string sign)
        {
            string pubkey = pathToXMLKey(pubkeyPath);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubkey);
            //实例化验签对象.
            RSAPKCS1SignatureDeformatter f2 = new RSAPKCS1SignatureDeformatter(rsa);
            //设置哈昔算法为sha1
            f2.SetHashAlgorithm("SHA1");
            //把签名串转化为字节数组
            byte[] key = Convert.FromBase64String(sign);
            SHA1Managed sha2 = new SHA1Managed();
            //计算源串的哈昔值,生成字节数组.
            byte[] name = sha2.ComputeHash(System.Text.UnicodeEncoding.Default.GetBytes(unsign));
            //通过字节数组生成哈昔值
            string s2 = Convert.ToBase64String(name);
            Console.WriteLine(s2);
            return f2.VerifySignature(name, key);
        }
        //  获取der格式的私钥,转化成xml格式的私钥.
        private static string DecodeDERKey(String filename)
        {
            string xmlprivatekey = null;
            RSACryptoServiceProvider rsa = null;
            byte[] keyblob = GetFileBytes(filename);
            if (keyblob == null)
                xmlprivatekey = "";
            rsa = DecodeRSAPrivateKey(keyblob);
            if (rsa != null)
            {
                xmlprivatekey = rsa.ToXmlString(true);
            }
            return xmlprivatekey;
        }
        //------- Parses binary ans.1 RSA private key; returns RSACryptoServiceProvider  ---
        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;
            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            // BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            BinaryReader binr = new BinaryReader(mem, Encoding.ASCII);
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;
                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)	//version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;
                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);
                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();
            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }
            while (binr.PeekChar() == 0x00)
            {	//remove high order zeros in data
                binr.ReadByte();
                count -= 1;
            }
            return count;
        }
        private static byte[] GetFileBytes(String filename)
        {
            if (!File.Exists(filename))
                return null;
            Stream stream = new FileStream(filename, FileMode.Open);
            int datalen = (int)stream.Length;
            byte[] filebytes = new byte[datalen];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(filebytes, 0, datalen);
            string str = System.Text.Encoding.ASCII.GetString(filebytes);
            stream.Close();
            return filebytes;
        }
        //从x509格式的证书中,提取公钥,并转换成xml格式
        private static string pathToXMLKey(string pubkeyPath)
        {
            X509Certificate cert = null;
            try
            {	 // Try loading certificate as binary DER into an X509Certificate object.
                cert = X509Certificate.CreateFromCertFile(pubkeyPath);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {	//not binary DER; try BASE64 format
                StreamReader sr = File.OpenText(pubkeyPath);
                String filestr = sr.ReadToEnd();
                sr.Close();
                StringBuilder sb = new StringBuilder(filestr);
                sb.Replace("-----BEGIN CERTIFICATE-----", "");
                sb.Replace("-----END CERTIFICATE-----", "");
                try
                {        //see if the file is a valid Base64 encoded cert
                    byte[] certBytes = Convert.FromBase64String(sb.ToString());
                    cert = new X509Certificate(certBytes);
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Not valid binary DER or Base64 X509 certificate format");
                    return null;
                }
                catch (System.Security.Cryptography.CryptographicException)
                {
                    Console.WriteLine("Not valid binary DER or Base64 X509 certificate format");
                    return null;
                }
            }  // end outer catch
            string xmlpublickey = null;
            byte[] modulus, exponent;
            byte[] rsakey = cert.GetPublicKey();
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(rsakey);
            BinaryReader binr = new BinaryReader(mem);
            ushort twobytes = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;
                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;
                if (twobytes == 0x8102)	//data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();	// read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte();	//advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian
                int modsize = BitConverter.ToInt32(modint, 0);
                int firstbyte = binr.PeekChar();
                if (firstbyte == 0x00)
                {	//if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();	//skip this null byte
                    modsize -= 1;	//reduce modulus buffer size by 1
                }
                modulus = binr.ReadBytes(modsize);	//read the modulus bytes
                if (binr.ReadByte() != 0x02)			//expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();		// should only need one byte for actual exponent data
                exponent = binr.ReadBytes(expbytes);
                if (binr.PeekChar() != -1)	// if there is unexpected more data, then this is not a valid asn.1 RSAPublicKey
                    return null;
                // ------- create RSACryptoServiceProvider instance and initialize with public key   -----
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                rsa.ImportParameters(RSAKeyInfo);
                xmlpublickey = rsa.ToXmlString(false);
                return xmlpublickey;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }
    }

    public static class WinApi
    {

        // 函数定义部分
        /*
        DWORD GetLastError(VOID);
         */
        [DllImport("Kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)] //
        public static extern uint GetLastError();

        /*
        BOOL WINAPI CryptAcquireContext(
        HCRYPTPROV *phProv,
        LPCTSTR pszContainer,
        LPCTSTR pszProvider,
        DWORD dwProvType,
        DWORD dwFlags
        );
         */
        // [DllImport("advapi32.dll", ExactSpelling=true, EntryPoint="CryptAcquireContextW", CharSet=System.Runtime.InteropServices.CharSet.Unicode)]  
        // public static extern bool CryptAcquireContext(
        // ref IntPtr phProv,
        // String pszContainer, 
        // String pszProvider,
        // uint dwProvType,
        // uint dwFlags);

        [DllImport("advapi32.dll", EntryPoint = "CryptAcquireContextW", CharSet = CharSet.Unicode)]
        internal extern static bool CryptAcquireContext(out IntPtr hProv,
        [MarshalAs(UnmanagedType.LPWStr)] string container,
        [MarshalAs(UnmanagedType.LPWStr)] string provider,
        uint provType,
        uint flags);
        /*
        BOOL WINAPI CryptGetProvParam(
        HCRYPTPROV hProv,       
        DWORD dwParam,          
        BYTE *pbData,           
        DWORD *pdwDataLen,      
        DWORD dwFlags           
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptGetProvParam(
        uint hProv,
        uint dwParam,
        byte[] pbData,
        IntPtr pdwDataLen,
        uint dwFlags);

        /*
        BOOL WINAPI CryptGetUserKey(
        HCRYPTPROV hProv,      
        DWORD dwKeySpec,       
        HCRYPTKEY *phUserKey   
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptGetUserKey(
        IntPtr hProv,
        uint dwKeySpec,
        out IntPtr phUserKey);

        /*
        BOOL WINAPI CryptGenKey(
        HCRYPTPROV hProv,  
        ALG_ID Algid,      
        DWORD dwFlags,     
        HCRYPTKEY *phKey   
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptGenKey(
        uint hProv,
        uint Algid,
        uint dwFlags,
        IntPtr phKey);




        /*
        BOOL WINAPI CryptCreateHash(
        HCRYPTPROV hProv,    
        ALG_ID Algid,        
        HCRYPTKEY hKey,      
        DWORD dwFlags,       
        HCRYPTHASH *phHash  
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptCreateHash(
        uint hProv,
        uint Algid,
        uint hKey,
        uint dwFlags,
        IntPtr phHash);


        /*
        BOOL WINAPI CryptHashData(
        HCRYPTHASH hHash,  
        BYTE *pbData,      
        DWORD dwDataLen,   
        DWORD dwFlags      
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptHashData(
        uint hHash,
        byte[] pbData,
        uint dwDataLen,
        uint dwFlags);


        /*
        BOOL WINAPI CryptDeriveKey(
        HCRYPTPROV hProv,      
        ALG_ID Algid,          
        HCRYPTHASH hBaseData,  
        DWORD dwFlags,         
        HCRYPTKEY *phKey       
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptDeriveKey(
        uint hProv,
        uint Algid,
        uint hBaseData,
        uint dwFlags,
        IntPtr phKey);


        /*
        BOOL WINAPI CryptDestroyHash(
        HCRYPTHASH hHash       
        );

         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptDestroyHash(
        uint hHash);


        /*
        BOOL WINAPI CryptDestroyKey(
        HCRYPTKEY hKey     
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptDestroyKey(
        uint hKey);


        /*
        BOOL WINAPI CryptReleaseContext(
        HCRYPTPROV hProv,  
        DWORD dwFlags      
        );
         */
        [DllImport("advapi32.dll")]
        public static extern uint CryptReleaseContext(
        uint hProv,
        uint dwFlags);


        /*
        BOOL WINAPI CryptEncrypt(
        HCRYPTKEY hKey,      
        HCRYPTHASH hHash,    
        BOOL Final,          
        DWORD dwFlags,       
        BYTE *pbData,        
        DWORD *pdwDataLen,   
        DWORD dwBufLen       
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptEncrypt(
        uint hKey,
        uint hHash,
        bool Final,
        uint dwFlags,
        byte[] pbData,
        IntPtr pdwDataLen,
        uint dwBufLen
        );

        /*
        BOOL WINAPI CryptDecrypt(
        HCRYPTKEY hKey,    
        HCRYPTHASH hHash,  
        BOOL Final,        
        DWORD dwFlags,     
        BYTE *pbData,      
        DWORD *pdwDataLen  
        );
         */
        [DllImport("advapi32.dll")]
        public static extern bool CryptDecrypt(
        uint hKey,
        uint hHash,
        bool Final,
        uint dwFlags,
        byte[] pbData,
        IntPtr pdwDataLen
        );

        public const uint PROV_RSA_FULL = 1;

        public const uint AT_KEYEXCHANGE = 1;
        public const uint AT_SIGNATURE = 2;

        public static void Test()
        {
            IntPtr phProv = IntPtr.Zero;
            if (!CryptAcquireContext(out phProv, null, "FEITIAN ePassNG RSA Cryptographic Service Provider", PROV_RSA_FULL, 0))
            {
                //打开失败
                throw new Exception("Error during CryptAcquireContext(ersonnel_Remove)!\r\n");
            }

            IntPtr hKey = IntPtr.Zero;
            var uk = CryptGetUserKey(phProv, AT_SIGNATURE, out hKey);
        }

        //// 常量已经定义
        //// 其中一个函数
        //public static bool CreateKey(string sKet)
        //{
        //    uint hProv = 0;
        //    uint hKey = 0;
        //    char[] szUserName = new char[100];
        //    uint dwUserNameLen = 100;

        //    // 试图获取缺省的密钥容器，若失败，
        //    //则创建一个缺省容器

        //    IntPtr p = new IntPtr(hProv);
        //    if (!CryptAcquireContext(out p, sKet,
        //    MS_DEF_PROV, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT | CRYPT_MACHINE_KEYSET))
        //    {
        //        uint x1 = GetLastError();
        //        //若获取缺省密钥容器发生错误，
        //        //就创建一个缺省密钥容器, 使用参数CRYPT_NEWKEYSET
        //        IntPtr p1 = new IntPtr(hProv);
        //        if (!CryptAcquireContext(out p1, sKet, MS_DEF_PROV,
        //        PROV_RSA_FULL,
        //        CRYPT_NEWKEYSET | CRYPT_MACHINE_KEYSET))
        //        {
        //            uint x2 = GetLastError();
        //            Trace.WriteLine("创建缺省密钥容器发生错误!\n");
        //            return false;
        //        }

        //        // 取得缺省密钥容器名
        //        if (!CryptGetProvParam(hProv,
        //        PP_CONTAINER, Encoding.Default.GetBytes(szUserName), new IntPtr(dwUserNameLen), 0))

        //        {
        //            // 出错误时容器名置空
        //            szUserName[0] = '\0';
        //        }

        //        Trace.WriteLine("Create key container: " + szUserName);
        //    }

        //    // 试图获取签名密钥的名柄
        //    if (!CryptGetUserKey(hProv, AT_SIGNATURE, new IntPtr(hKey)))
        //    {
        //        if (GetLastError() == NTE_NO_KEY)
        //        {     // 创建数字签名密钥对
        //            Trace.WriteLine("Create signature key pair\n");
        //            if (!CryptGenKey(hProv, AT_SIGNATURE, 0, new IntPtr(hKey)))
        //            {
        //                Trace.WriteLine("错误代码： 创建数字签名密钥对发生错误!\n");
        //                return false;
        //            }
        //            else
        //            {
        //                CryptDestroyKey(hKey);
        //            }
        //        }
        //        else
        //        {
        //            Trace.WriteLine("错误代码：  during CryptGetUserKey!\n");
        //            return false;
        //        }
        //    }

        //    // 试图取得交换密钥的句柄，没有交换密钥时，创建交换密钥
        //    if (!CryptGetUserKey(hProv, AT_KEYEXCHANGE, new IntPtr(hKey)))
        //    {
        //        if (GetLastError() == NTE_NO_KEY)
        //        {//没有交换密钥时，创建交换密钥对
        //            Trace.WriteLine("创建交换密钥\n");
        //            if (!CryptGenKey(hProv, AT_KEYEXCHANGE, 0, new IntPtr(hKey)))
        //            {
        //                Trace.WriteLine("Error during CryptGenKey!\n");
        //                return false;
        //            }
        //            else
        //            {
        //                CryptDestroyKey(hKey);
        //            }
        //        }
        //        else
        //        {
        //            Trace.WriteLine("Error during CryptGetUserKey!\n");
        //            return false;
        //        }
        //    }
        //    CryptReleaseContext(hProv, 0); //释放句柄
        //    Trace.WriteLine("OK\n");
        //    return true;
        //}


    }
}
