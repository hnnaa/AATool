using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UtilsLib.EncryptUtil;

namespace AATool.Tests
{
    public class TestRSA : ITest
    {
        public void Test()
        {
            try
            {
                string prikey = "MIIEpAIBAAKCAQEAqOTxQbDUUkm/CS/eVTHgARhqCfs5mWe/FImEIDyPrsExswLUbc6pIXBulqtYOJ9Nl4fljNgXJbh4CyoUtiM5U9jf5yoPthJq667rPwtRVOIDnmNWKZNHponXjFBQs/juNkUgdX4kcWx695cWGkm+SRXCQQsKcSsqubFwLgXRP9HErfPwColtfmwVZVNXWA4ZtHTrSczbVDplMS8Fg73eprwIxYvpOFxFhAAwC4+Xw753Zd+u0P05YhXrQ/76g1YqNBkiHlOpWtwLVdL+zU9Nk5PxRGgjUqvJpDUxwjEHCEDxD4fxdgS+ml5pm0SsRKyXFm2+ZyPDlu7LQdx28kgAeQIDAQABAoIBAQCZvGQa/qIWEUjSTP9dn1uew6FqWoLwr64QPKjHSzkiwnrBzradCXuMk7ImdeuFBwTzBOGfledkD4k96F0X+fIp74TH/9JzUkp/JCbAqUWsNVtD5no8t/KFln1dHJKJb/Mt9Th5mowDrtIz4xRcc3nBPwwmdq1XnAp1Ix3Q1rzFEGX4Ndce6q2YkbpAMKgl3HeIbgKrKHOxmJJ9hJ3rUzc24D+e2dR9eYCym576RrxxuHISysa4N41MSdZq5jXaaWmwIGg68v5wEVJcEkmAc91webKAfy1SVdQiAl8I1xSI3JDUXloWrUAX3ZnRcknjEmhcs7by5QrOnbTd8ZR5Mkt9AoGBAPU4QLSj5QdNz+HjoRdfxda2lK1OPy3O/jReCE6NnqTbvVygQjFdlmYnGFhwnLa1SoWYW9M9mg9lRxsQoBt6Zdh4Pu08cG+3GY1i4tILWroLekPHImz8TcqYCeU9OlAt+Yo59NWTA0EboDfr/YPx8WYeJ5g1z+yz9qG8+iz04ZMDAoGBALBRtZ7ywEYGdHW47jhNpJwTfBygFCOZzoIIpYoEuHVrtlBiKtPgMEryReOEurpl6TKgdOVZ99F0G0lIMZtyqdRKkJh8t99S5z7Y7yWRA+1ImcmHNC/r+KZig1gcAW6z26xVNPsNtjiXwixrQ5cIsQGN53mgauGCVsrzyH3yMkfTAoGAfKiQHESFFWV1HHw/VEaXqENA6akxbPQhPjXfOy+7Skt6xC+j9ryAzIVrVupIgIlAzRFa/NQAEXuG3jdhbRaX16epNajX77LBBqMSc3zaLmHPUc1VrtnEIg2QWjz5+/CjPqWf1ULbEKoelM8fHYhNE4CY2EPi443I/we2lkwWsEkCgYEAhvck5XTARVnvj6VthF+6n1Yy7N6ES/QSJWtk+889O3Sl68YZLnIvxU7KvKv/G2ujJAO7N0y06/nKYUAH6QvOgBhLss3VGfel12/LL6BftySZw6/g/MXOYd6K18koN4spnjLjCUGnuI5CLio1ZMlcZOoww+NNrS1OLrCJSg84+c0CgYAkw6xcfaKLH39+jdc2DLVrip4JYv+p4v9GuQbn1C1aqjaxVR5ZjbYuIaJXhr8Z0qIFaYENBVAA0AM75lkZUcYwVNiIfSA2WSFlymoy90ovPIQaG5jaQXRv8hrjRJ1zQ/l5JzhngERVbi4BBcz2QfC9GxsEa4CYGciI/sMH44fUvg==";
                string pubkey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqOTxQbDUUkm/CS/eVTHgARhqCfs5mWe/FImEIDyPrsExswLUbc6pIXBulqtYOJ9Nl4fljNgXJbh4CyoUtiM5U9jf5yoPthJq667rPwtRVOIDnmNWKZNHponXjFBQs/juNkUgdX4kcWx695cWGkm+SRXCQQsKcSsqubFwLgXRP9HErfPwColtfmwVZVNXWA4ZtHTrSczbVDplMS8Fg73eprwIxYvpOFxFhAAwC4+Xw753Zd+u0P05YhXrQ/76g1YqNBkiHlOpWtwLVdL+zU9Nk5PxRGgjUqvJpDUxwjEHCEDxD4fxdgS+ml5pm0SsRKyXFm2+ZyPDlu7LQdx28kgAeQIDAQAB";

                string abc = "abc123";

                RSACryptoServiceProvider rsaCsp = AlipaySignature.LoadCertificateString(prikey, "RSA2");
                string r1 = Convert.ToBase64String(AlipaySignature.RSAEncrypt(rsaCsp, Encoding.UTF8.GetBytes(abc)));
                string r2 = Encoding.UTF8.GetString(AlipaySignature.RSADecrypt(rsaCsp, Convert.FromBase64String(r1)));

                var sPublicKeyPEM = "-----BEGIN PUBLIC KEY-----\r\n";
                sPublicKeyPEM += pubkey;
                sPublicKeyPEM += "-----END PUBLIC KEY-----\r\n\r\n";
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);
                string r11 = Convert.ToBase64String(AlipaySignature.RSAEncrypt(rsa, Encoding.UTF8.GetBytes(abc)));
                string r22 = Encoding.UTF8.GetString(AlipaySignature.RSADecrypt(rsaCsp, Convert.FromBase64String(r1)));
            }
            catch (Exception)
            {
            }
        }
    }
}
