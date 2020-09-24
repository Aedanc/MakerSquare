using Microsoft.VisualStudio.TestTools.UnitTesting;
using MakerSquare.Compiler;

namespace UnitTestCompiler
{
    [TestClass]
    public class UnitTestCompiler
    {

        private Compiler prop_comp = new Compiler();

        [TestMethod]
        public void TestCopyParameters()
        {
            var param_before = prop_comp.getCopyCompilingParameters();
            prop_comp.SetResourceFolder("RasourceFalder");
            Assert.IsFalse(param_before.resourceFolder != "RasourceFalder");
        }

        [TestMethod]
        public void TestExecNameAutoFilePath()
        {
            prop_comp.SetExecPath("");
            prop_comp.SetExecutableName("mdr.exe");
            var filename = prop_comp.getCopyCompilingParameters().fileName;
            var filepath = prop_comp.getCopyCompilingParameters().filePath;
            Assert.IsTrue(filepath == "");
            Assert.IsTrue(filename == "mdr.exe");
        }

        [TestMethod]
        public void TestExecNameAutoFilePath2()
        {
            prop_comp.SetExecutableName("exec\\mdr.exe");
            var filename = prop_comp.getCopyCompilingParameters().fileName;
            var filepath = prop_comp.getCopyCompilingParameters().filePath;
            Assert.IsTrue(filepath == "exec\\");
            Assert.IsTrue(filename == "mdr.exe");
        }
        
        [TestMethod]
        public void TestExecNameAutoFilePath3()
        {
            prop_comp.SetExecutableName("lul");
            Assert.IsTrue(prop_comp.getCopyCompilingParameters().fileName == "lul.exe");
        }
    }
}
