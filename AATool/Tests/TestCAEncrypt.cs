using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilsLib.EncryptUtil;

namespace AATool.Tests
{
    public class TestCAEncrypt : ITest
    {
        public void Test()
        {
            var dd = CAEncrypt.CADecryption("AEkD0iG423Q1/spUa1sNHc7zB8ZvmsMY0UO71pT9+pwNs+Q/27eUj50pxMv1UAQuRQwkAHNH560vbHD / LGhzP6CEDRSoHhQ / 2QBwl7SbOC2wxcMGkG5IRFMHrmEqC4nXwtgQY4l7f5Flq + uW3p8hCjHAQ6 / bgQrKZbQLBjWEa7LUQfauvRYwnQ5gP + 2L4VJ0Hkgot + APanBbkqy3YJ9mmOrpiNCMCQdfX5yEgz4ussrcASZdutejfyHCVGJr41NeizHj93PgS2lTVIsBxp4M2eaee6kCmvdjRqaXd2LVvfUD4i7Gnts + seE + u + geOTcu1RvupFw6OgE7J / g9j0T7ew ==", @"D:\work\project\二次开发\073-贵阳平台项目20200602\cert\client01.key.p12", "123456789");


        }


    }
}
