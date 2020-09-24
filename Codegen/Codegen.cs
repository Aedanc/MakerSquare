using System.Collections.Generic;
using System.IO;
using System.Linq;
using MakerSquare.Compiler;
using MakerSquare.FileSystem;
using MakerSquare.FrontFacingECS;

namespace Codegen
{
    public class ReificationData
    {
        public ReificationData()
        {

        }

        public ReificationData(string varname, string reif_str)
        {
            this.varname = varname;
            this.reif_str = reif_str;
        }

        public string varname;
        public string reif_str;
    }

    public class Codegen
    {
        public static void CodegenScene(Manager manager, List<FFEntity> scene, Compiler compiler)
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(manager.ProjectDir, manager.PrefabsDirName)))
                File.Delete(file);
            foreach (var dir in Directory.EnumerateDirectories(Path.Combine(manager.ProjectDir, manager.PrefabsDirName)))
                Directory.Delete(dir, true);

            List<ReificationData> reificationDataList = new List<ReificationData>();
            int i = 0;
            foreach (var entity in scene)
            {
                GenerateEntity(manager, entity, compiler, reificationDataList, i);
                i++;
            }
            if (scene.Any(x => x.EntityTemplate != null))
                GeneratePrefabs(manager, scene, compiler, reificationDataList);

            GenerateSceneCode(reificationDataList, manager, compiler);            
        }

        private static void GenerateEntity(Manager manager, FFEntity entity, Compiler compiler,
            List<ReificationData> reificationDataList, int nthEntity)
        {
            if (entity.EntityTemplate == null)
            {
                string name = entity.Name + nthEntity.ToString();
                string path = Path.Combine(manager.ProjectDir, manager.PrefabsDirName, name + ".cs");
                var template = new EntityTemplate(entity, nthEntity);
                System.IO.File.WriteAllText(path, template.TransformText());
                reificationDataList.Add(template.ReificationData(entity, name));
                compiler.AddFile(path);
            }            
        }

        private static void GeneratePrefabs(Manager manager, List<FFEntity> scene, Compiler compiler, List<ReificationData> reificationDataList)
        {            
            string path = Path.Combine(manager.ProjectDir, manager.PrefabsDirName, "BreakoutPrefab.cs");
            var BreakoutEntities = scene.FindAll(x => x.EntityTemplate == "Breakout");  
            var template = new SpecificEntities.Breakout(BreakoutEntities);
            System.IO.File.WriteAllText(path, template.TransformText());
            reificationDataList.AddRange(template.ReificationData());
            compiler.AddFile(path);
        }

        private static void GenerateSceneCode(List<ReificationData> reificationDataList, Manager manager,
            Compiler compiler)
        {
            string path = Path.Combine(manager.ProjectDir, manager.PrefabsDirName, "Main.cs");
            var template = new Main(reificationDataList);
            System.IO.File.WriteAllText(path, template.TransformText());
            compiler.AddFile(path);
        }
    }
}