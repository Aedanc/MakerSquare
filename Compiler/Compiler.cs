using Microsoft.CSharp;
using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Linq;
using System.Threading.Tasks;

namespace MakerSquare
{
    namespace Compiler
    {
        [Serializable]
        public class CompilingParameters
        {
            public CompilingParameters()
            {
                files = new List<string>();
                assemblyDeps = new List<string>();
                resourceFolder = null;
                fileName = "defaultExec.exe";
                filePath = ".";
            }

            public List<string> files;
            public List<string> assemblyDeps;
            public string resourceFolder;
            public string fileName;
            public string filePath;
        }

        public class Compiler
        {
            private CompilingParameters parameters = new CompilingParameters();

            public CompilingParameters getCopyCompilingParameters()
            {
                return parameters;
            }

            public void SetResourceFolder(string folder)
            {
                parameters.resourceFolder = folder;
            }

            public void AddFile(string filename)
            {
                parameters.files.Add(filename);
            }

            public void ResetFileList()
            {
                parameters.files.Clear();
            }

            public void AddDependency(string dep)
            {
                parameters.assemblyDeps.Add(dep);
            }

            public void ResetDependencies()
            {
                parameters.assemblyDeps.Clear();
            }

            public void SetExecutableName(string name)
            {
                if (!name.EndsWith(".exe"))
                    name += ".exe";
                int position = name.LastIndexOf('\\');
                if (position == -1)
                {
                    parameters.fileName = name;
                }
                else
                {
                    parameters.filePath = name.Substring(0, position + 1);
                    parameters.fileName = name.Substring(position + 1);
                }
            }

            public void SetExecPath(string path)
            {
                parameters.filePath = path;
            }

            private class IsolatedCompiler : MarshalByRefObject
            {
                private CompilingParameters parameters;       

                public IsolatedCompiler(CompilingParameters parameters_)
                {
                    parameters = parameters_;
                    CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                    CompilerParameters exeparams = new CompilerParameters();
                    List<CompilerError> resourceErrors = new List<CompilerError>();

                    exeparams.GenerateExecutable = true;
                    var dir = Directory.CreateDirectory(parameters.filePath);
                    exeparams.OutputAssembly = Path.Combine(parameters.filePath, parameters.fileName);
                                        
                    var processes = Process.GetProcessesByName(parameters.fileName.Substring(0, parameters.fileName.Length - 4));
                    if (processes.Length > 0)
                        processes.First().Kill();

                    foreach (var dep in parameters.assemblyDeps)
                    {
                        File.Copy(Path.Combine(".", dep), Path.Combine(parameters.filePath, dep), true);
                        exeparams.ReferencedAssemblies.Add(dep);
                    }

                    CompilerResults results = codeProvider.CompileAssemblyFromFile(exeparams, parameters.files.ToArray());
                    foreach (var error in results.Errors)
                    {
                        Debug.WriteLine(error);
                    }
                    if (results.Errors.Count > 0)
                    {
                        var errstr = "";
                        foreach (var error in results.Errors)
                            errstr += error + "\n";
                        throw new Exception(String.Format("Compiler errors : \n{0}", errstr));
                    }
                    CopyContentFiles();
                    CopyDLLs(exeparams, results);
                    Task.Run(() => ZippingUp(exeparams, results));                    
                }

                //Works around the file already being in use in order to copy it somewhere else
                private void CopyUsedFile(string file, string copy_destination)
                {
                    using (var inputFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var outputFile = new FileStream(copy_destination, FileMode.Create))
                        {
                            var buffer = new byte[0x10000];
                            int bytes;

                            while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                outputFile.Write(buffer, 0, bytes);
                            }
                        }
                    }
                }

                // Recurses down the folder structure
                private void CompressFolder(string path, ZipOutputStream zipStream, string zip_name)
                {
                    List<string> files = Directory.GetFiles(path).ToList();
                    files.Remove(".\\" + zip_name);

                    foreach (string filename in files)
                    {
                        FileInfo fi = new FileInfo(filename);

                        string entryName = filename.Substring(1); // Makes the name in zip based on the folder
                        entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                        ZipEntry newEntry = new ZipEntry(entryName);
                        newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity                    
                        zipStream.UseZip64 = UseZip64.Dynamic;
                        newEntry.Size = fi.Length;
                        zipStream.PutNextEntry(newEntry);

                        byte[] buffer = new byte[4096];
                        using (FileStream streamReader = File.OpenRead(filename))
                        {
                            StreamUtils.Copy(streamReader, zipStream, buffer);
                        }
                        zipStream.CloseEntry();
                    }
                    string[] folders = Directory.GetDirectories(path);

                    foreach (string folder in folders)
                    {
                        CompressFolder(folder, zipStream, zip_name);
                    }
                }

                private void ZippingUp(CompilerParameters exeparams, CompilerResults results)
                {
                    string zip_name = parameters.fileName.Substring(0, parameters.fileName.LastIndexOf(".")) + "_portable.zip";
                    if (File.Exists(zip_name))
                        File.Delete(zip_name);
                    FileStream fsOut = File.Create(zip_name);
                    ZipOutputStream zipStream = new ZipOutputStream(fsOut);
                    zipStream.SetLevel(3);
                    CompressFolder(parameters.filePath, zipStream, zip_name);
                    zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                    zipStream.Close();
                }

                private void CopyContentFiles()
                {
                    if (parameters.resourceFolder == null)
                    {
                        Debug.WriteLine("No resource folder set, assuming you do not need it / already have it .");
                        return;
                    }
                    if (!Directory.Exists(parameters.resourceFolder))
                        throw new Exception("Directory \"" + parameters.resourceFolder + "\" could not be found !");
                    try
                    {
                        Directory.Delete(parameters.filePath + @"\Content", true);
                    }
                    catch (Exception) { }

                    Directory.CreateDirectory(parameters.filePath + @"\Content");
                    foreach (string dirPath in Directory.GetDirectories(parameters.resourceFolder, "*",
                        SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(parameters.resourceFolder, Path.Combine(parameters.filePath, "Content")));

                    foreach (string newPath in Directory.GetFiles(parameters.resourceFolder, "*.*",
                        SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(parameters.resourceFolder, Path.Combine(parameters.filePath, "Content")), true);

                }

                private void CopyXDir(string x_name)
                {
                    if (Directory.Exists(Path.Combine(parameters.filePath, x_name)))
                        Directory.Delete(Path.Combine(parameters.filePath, x_name), true);
                    Directory.CreateDirectory(Path.Combine(parameters.filePath, x_name));
                    var CurrDir = Directory.GetCurrentDirectory();
                    foreach (string dirPath in Directory.GetDirectories(Path.Combine(CurrDir, x_name), "*",
                       SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(Directory.GetCurrentDirectory(), parameters.filePath));

                    foreach (string newPath in Directory.GetFiles(Path.Combine(CurrDir, x_name), "*.*",
                        SearchOption.AllDirectories))
                        CopyUsedFile(newPath, newPath.Replace(Directory.GetCurrentDirectory(), parameters.filePath));
                }

                private void CopyDLLs(CompilerParameters exeparams, CompilerResults results)
                {
                    void __CopyDLLs(List<string> names, Assembly CurrentAssembly)
                    {
                        foreach (var assemblyName in CurrentAssembly.GetReferencedAssemblies())
                        {
                            if (!names.Contains(assemblyName.Name))
                            {
                                names.Add(assemblyName.Name);
                                __CopyDLLs(names, Assembly.Load(assemblyName));
                            }
                        }
                    }

                    var assemblyNames = new List<string>();
                    __CopyDLLs(assemblyNames, results.CompiledAssembly);

                    foreach (var dll in assemblyNames)
                    { 
                        if (File.Exists(dll + ".dll"))
                            File.Copy(dll + ".dll", Path.Combine(parameters.filePath, dll + ".dll"), true);
                    }

                    CopyXDir("x86");
                    CopyXDir("x64");
                }
            }

            public void CompileToPortableExec()
            {
                AppDomain domain;
                domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(),
                        null, AppDomain.CurrentDomain.SetupInformation);
                var args = new object[] { parameters };
                domain.CreateInstanceAndUnwrap(typeof(IsolatedCompiler).Assembly.FullName, typeof(IsolatedCompiler).FullName, true,
                    BindingFlags.CreateInstance, null, args, null, null);
                AppDomain.Unload(domain);
            }
        }        
    }
}

