using System.IO;
using System.Reflection;
using SnoopB.Configuration;

namespace SnoopB.Resources
{
    public static class ResourceManager
    {
        private const string IonicZipFileName = "Ionic.Zip.dll";
        private const string ResourceBeginNameSpace = "SnoopB.Resources.";

        public static string ExtractIonicZipResource()
        {
            return ExtractResource(IonicZipFileName);
        }

        public static string ExtractResource(string resourceName)
        {
            var outputPath = Path.Combine(Configurator.Instance.CurrentExecutionPath, resourceName);

            if (File.Exists(outputPath))
                return outputPath;

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceBeginNameSpace + resourceName);
            if (stream == null)
                return null;

            var bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            File.WriteAllBytes(outputPath, bytes);

            //Ocultamos el archivo
            File.SetAttributes(outputPath, File.GetAttributes(outputPath) | FileAttributes.Hidden);

            return outputPath;
        }
    }
}