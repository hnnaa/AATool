using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AATool.Tests
{
    public class TestCompiler : ITest
    {

        public void Test()
        {
            CSharpCodeProvider cs = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.ReferencedAssemblies.Add("UtilsLib.dll");//此处代码是添加对应dll文件的引用
            //cp.ReferencedAssemblies.Add("System.Core.dll");//System.Linq存在于System.Core.dll文件中
            //string strExpre = "using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;namespace DynamicCompileTest{public class TestClass1{public bool CheckBool(string source){ return source.Contains(\"SC\"); }}}";
            CompilerResults cr = cs.CompileAssemblyFromFile(cp, ".\\CalcClass.cs");
            if (cr.Errors.HasErrors)
            {
                Console.WriteLine(cr.Errors.ToString());
            }
            else
            {
                //5.创建一个Assembly对象
                Assembly ass = cr.CompiledAssembly;//动态编译程序集
                object obj = ass.CreateInstance("Pub.Files.CalcClass");
                MethodInfo mi = obj.GetType().GetMethod("Add");
                var result = mi.Invoke(obj, new object[] { 1.2m, 2.3m });
                //bool result = (bool)mi.Invoke(obj, new object[] { "LYF" });
                //bool result2 = (bool)mi.Invoke(obj, new object[] { "SCss" });

                MethodInfo mi2 = obj.GetType().GetMethod("GetNewList");
                var lst = new List<string>() { "a", "b", "c" };
                var result2 = mi2.Invoke(obj, new object[] { lst });

                MethodInfo mi3 = obj.GetType().GetMethod("ToHex");
                var result3 = mi3.Invoke(obj, new object[] { "1234567893" });

            }
        }
    }
}
