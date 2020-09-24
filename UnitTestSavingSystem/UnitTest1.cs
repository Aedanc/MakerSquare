using System;
using System.IO;
using Engine;
using MakerSquare.SavingSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestSavingSystem
{
    [TestClass]
    public class UnitTest1
    {
        public void CleanupDirectory(string dir)
        {
            try
            {

                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
                else if (File.Exists(dir))
                    File.Delete(dir);
            }
            catch (Exception)
            {
            }
        }

        public bool CheckProjectHierarchy(string root_dir)
        {           
            return Directory.Exists(root_dir)
                && Directory.Exists(Path.Combine(root_dir, "Prefabs"))
                && Directory.Exists(Path.Combine(root_dir, "Scenes"))
                && File.Exists(Path.Combine(root_dir, ".mk2proj.txt"));
        }


        [TestMethod]
        public void TestSaveSysDirectoryExistsEmpty()
        {
            string dirname = Path.GetRandomFileName();
            try
            {
                Directory.CreateDirectory(dirname);
                MakerSquare.FileSystem.Manager.Initialize(dirname);
                var fsmanager = MakerSquare.FileSystem.Manager.Instance;
                SavedFilesManager savedFilesManager = new SavedFilesManager(fsmanager);
                Assert.IsTrue(CheckProjectHierarchy(dirname));
            }
            catch (MakerSquare.FileSystem.CannotCreateProjectDirException)
            {                
            }
            finally
            {
                CleanupDirectory(dirname);
            }
        }

        [TestMethod]
        public void TestSaveSysDirectoryExistsFull()
        {
            string dirname = Path.GetRandomFileName();
            try
            {
                Directory.CreateDirectory(dirname);
                Directory.CreateDirectory(Path.Combine(dirname, "Prefabs"));
                Directory.CreateDirectory(Path.Combine(dirname, "Scenes"));
                MakerSquare.FileSystem.Manager.Initialize(dirname);
                var fsmanager = MakerSquare.FileSystem.Manager.Instance;
                SavedFilesManager savedFilesManager = new SavedFilesManager(fsmanager);
                Assert.IsTrue(CheckProjectHierarchy(dirname));
            }
            catch (MakerSquare.FileSystem.CannotCreateProjectDirException)
            {

            }
            finally
            {
                CleanupDirectory(dirname);
            }
        }

        [TestMethod]
        public void TestSaveSysDirectoryIsFile()
        {
            string dirname = Path.GetRandomFileName();
            try
            {
                File.Create(dirname);
                MakerSquare.FileSystem.Manager.Initialize(dirname);
                var fsmanager = MakerSquare.FileSystem.Manager.Instance;
                SavedFilesManager savedFilesManager = new SavedFilesManager(fsmanager);
                Assert.IsTrue(CheckProjectHierarchy(dirname));
            }
            catch (MakerSquare.FileSystem.CannotCreateProjectDirException)
            {

            }
            finally
            {
                CleanupDirectory(dirname);
            }
        }

        [TestMethod]
        public void TestSaveSysDirectoryFree()
        {
            string dirname = Path.GetRandomFileName();
            try
            {
                MakerSquare.FileSystem.Manager.Initialize(dirname);
                var fsmanager = MakerSquare.FileSystem.Manager.Instance;
                SavedFilesManager savedFilesManager = new SavedFilesManager(fsmanager);
                Assert.IsTrue(CheckProjectHierarchy(dirname));
            }
            finally
            {
                CleanupDirectory(dirname);
            }
        }

        [TestMethod]
        public void TestSaveSysDirectoryNoRights()
        {
            string dirname = Path.Combine("C:", Path.GetRandomFileName());
            try
            {
                MakerSquare.FileSystem.Manager.Initialize(dirname);
                var fsmanager = MakerSquare.FileSystem.Manager.Instance;
                SavedFilesManager savedFilesManager = new SavedFilesManager(fsmanager);
                Assert.IsTrue(CheckProjectHierarchy(dirname));
            }
            catch (MakerSquare.FileSystem.CannotCreateProjectDirException)
            {

            }
            finally
            {
                CleanupDirectory(dirname);
            }
        }
       
        [TestMethod]
        public void TestPrefabSerializeDeserialize()
        {
            string dirname = Path.GetRandomFileName();
            try
            {
                MakerSquare.FileSystem.Manager.Initialize(dirname);
                var fsmanager = MakerSquare.FileSystem.Manager.Instance;
                SavedFilesManager savedFilesManager = new SavedFilesManager(fsmanager);
                var ram_entity = new Entity(new Position(12f, 14.3f));
                ram_entity.Name = "Potato!";
                savedFilesManager.SavePrefab(ram_entity, "rammy");
                savedFilesManager.LoadPrefab(out Entity file_entity, "rammy");
                Assert.IsTrue(file_entity.Name == ram_entity.Name);


            }
            catch (MakerSquare.SavingSystem.CannotLoadFileException)
            {

            }
            catch (MakerSquare.SavingSystem.CannotSaveFileException)
            {

            }
            finally
            {
                CleanupDirectory(dirname);
            }
        }

        [TestMethod]
        public void TestSceneSerializeDeserialize()
        {
            string dirname = Path.GetRandomFileName();
            try
            {
                MakerSquare.FileSystem.Manager.Initialize(dirname);
                var fsmanager = MakerSquare.FileSystem.Manager.Instance;
                SavedFilesManager savedFilesManager = new SavedFilesManager(fsmanager);
                var ram_entity = new Entity(new Position(12f, 14.3f));
                ram_entity.Name = "Potato!";
                var scene = new SavedScene();
                scene.entities.Add(ram_entity);
                savedFilesManager.SaveScene(scene, "rammy");
                savedFilesManager.LoadScene(out SavedScene file_scene, "rammy");
                Assert.IsTrue(file_scene.entities[0].Name == scene.entities[0].Name);
            }
            catch (MakerSquare.SavingSystem.CannotLoadFileException)
            {

            }
            catch (MakerSquare.SavingSystem.CannotSaveFileException)
            {

            }
            finally
            {
                CleanupDirectory(dirname);
            }
        }       
    }
}
