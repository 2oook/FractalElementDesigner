using RC_FE_Design___Analysis_and_synthesis.FEEditor;
using RC_FE_Design___Analysis_and_synthesis.IO.ProjectSaveModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.IO
{
    public class ProjectSaver
    {
        public static bool SaveProject(SavingProject project, string path) 
        {
            var result = false;

            var saveFileStream = File.Create(path);

            var serializer = new BinaryFormatter();
            serializer.Serialize(saveFileStream, project);

            return result;
        }

        public static SavingProject LoadProject(string path)
        {
            var result = new SavingProject();

            var openFileStream = File.OpenRead(path);

            var deserializer = new BinaryFormatter();

            result = (SavingProject)deserializer.Deserialize(openFileStream);

            return result;
        }
    }
}
