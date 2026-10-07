using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TurkGezer.Editor
{
    /// <summary>Publishes the public code-only distribution and its lightweight starter kit.</summary>
    public static class ModuleReleaseBuilder
    {
        public static void Publish()
        {
            try
            {
                string root = ModuleExporter.PackageRoot();
                string samples = root + "/Samples~/Baslangic";
                if (!File.Exists(root + "/package.json")) throw new InvalidOperationException("Yayinlama UPM kaynak paketinden yapilmalidir.");
                if (Directory.Exists(root + "/Samples~/TasarimKutuphane"))
                    throw new InvalidOperationException("Acik kaynak dagitimina lisansi dogrulanmamis tasarim kutuphanesi eklenemez.");
                // The dedicated example root has its own GUIDs, separate from any older project.
                TurkGezerKitBuilder.Build(TurkGezerKitBuilder.Root);
                Copy(TurkGezerKitBuilder.Root, samples, true);
                AssetDatabase.Refresh();
                ModuleSelfTests.Run();
                // Art files are not regenerated, removed or replaced by the technical kit.
                ModuleExporter.ExportCore();
                Debug.Log("TURKGEZER_GENERAL_RELEASE_OK: " + ModuleExporter.Version(root));
            }
            catch (Exception ex) { Debug.LogException(ex); EditorApplication.Exit(1); }
        }
        private static void Copy(string source, string destination, bool overwrite)
        {
            Directory.CreateDirectory(destination);
            foreach (string file in Directory.GetFiles(source))
            {
                string target = Path.Combine(destination, Path.GetFileName(file));
                if (overwrite || !File.Exists(target)) File.Copy(file, target, overwrite);
            }
            foreach (string directory in Directory.GetDirectories(source)) Copy(directory, Path.Combine(destination, Path.GetFileName(directory)), overwrite);
        }
    }
}
