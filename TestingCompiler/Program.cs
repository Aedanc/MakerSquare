using MakerSquare.Compiler;
using System;
using System.Reflection;
using System.Diagnostics;

namespace TestingCompiler
{
    class Program
    {
        public static class Tests
        {
            public static void SimpleTest()
            {
                Compiler compiler = new Compiler();

                try
                {
                    compiler.AddFile("Testfile.cs");
                    compiler.AddDependency("System.Windows.Forms.dll");
                    compiler.SetExecPath(".");
                    compiler.SetExecutableName("compilation_testing.exe");
                    compiler.CompileToPortableExec();
                    Debug.WriteLine("Simple test successfully compiled.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Caught exception during SIMPLETEST : " + ex.Message);
                }
            }

            public static void MonogameTest()
            {
                Compiler compiler = new Compiler();             
                try
                {
                    compiler.AddFile(@"..\..\..\TestMonoGame\Game1.cs");
                    compiler.AddFile(@"..\..\..\TestMonoGame\Program.cs");                    
                    compiler.AddDependency("MonoGame.Framework.dll");
                    compiler.SetExecPath(".");
                    compiler.SetResourceFolder(@"..\..\..\TestMonoGame\Content");
                    compiler.SetExecutableName("PortableMonogame.exe");
                    compiler.CompileToPortableExec();
                    Debug.WriteLine("MonoGame Test successfully compiled.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Caught exception during MONOGAMETEST : " + ex.Message);
                }

            }
        }

        static void Main(string[] args)
        {
            Tests.MonogameTest();
        }
    }
}
