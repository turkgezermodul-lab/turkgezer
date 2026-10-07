using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TurkGezer.Editor
{
    public static class ModuleExporter
    {
        [Serializable] private sealed class Manifest { public string version; }
        [MenuItem("Tools/TurkGezer/Modulu UnityPackage Olarak Disari Aktar")]
        public static void ExportCore()
        {
            string output = Path.GetFullPath("../Paketler");
            Directory.CreateDirectory(output);
            string root = PackageRoot();
            if (Directory.Exists(root + "/Samples~/TasarimKutuphane") || Directory.Exists(root + "/Kutuphane"))
                throw new InvalidOperationException("Acik kaynak surumune lisansi dogrulanmamis tasarim kutuphanesi eklenemez.");
            CheckForSecrets(root);
            PackageIsolationAudit.Validate(root);
            var files = new System.Collections.Generic.Dictionary<string, string>();
            AddFiles(files, Directory.Exists(root + "/Runtime") ? root + "/Runtime" : root + "/Scripts/Runtime", "Assets/TurkGezerPaket/Scripts/Runtime");
            AddFiles(files, root + "/Editor", "Assets/TurkGezerPaket/Editor");
            AddFiles(files, Directory.Exists(root + "/Documentation~") ? root + "/Documentation~" : root + "/Dokumantasyon", "Assets/TurkGezerPaket/Dokumantasyon");
            // Never export a project Assets folder or auto-include its dependencies.
            string samples = Directory.Exists(root + "/Samples~/Baslangic") ? root + "/Samples~/Baslangic" : root + "/Ornekler";
            if (Directory.Exists(samples)) AddFiles(files, samples, "Assets/TurkGezerPaket/Ornekler");
            foreach (string name in new[] { "README.md", "CHANGELOG.md", "LICENSE.md", "Third Party Notices.md", "SECURITY.md" })
            {
                string source = root + "/" + name;
                if (name == "README.md")
                {
                    string temporaryDirectory = Path.GetFullPath("Temp/TurkGezerExport");
                    Directory.CreateDirectory(temporaryDirectory);
                    string adapted = temporaryDirectory + "/README.md";
                    File.WriteAllText(adapted, File.ReadAllText(source).Replace("Documentation~/", "Dokumantasyon/"));
                    if (File.Exists(source + ".meta")) File.Copy(source + ".meta", adapted + ".meta", true);
                    source = adapted;
                }
                files.Add(source, "Assets/TurkGezerPaket/" + name);
            }
            UnityPackageWriter.Write(Path.Combine(output, "TurkGezer-Modul-" + Version(root) + "-acik-kaynak.unitypackage"), files);
            Debug.Log("[TurkGezer] UnityPackage hazir: " + output);
        }
        public static string Version(string root)
        {
            string manifest = root + "/package.json";
            if (File.Exists(manifest)) return JsonUtility.FromJson<Manifest>(File.ReadAllText(manifest)).version;
            // UnityPackage intentionally contains no UPM manifest.
            return "1.2.0";
        }
        public static string PackageRoot()
        {
            var package = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().FirstOrDefault(item => item.name == "com.turkgezer.module");
            if (package != null) return package.resolvedPath.Replace('\\', '/');
            string script = AssetDatabase.FindAssets("ModuleExporter t:MonoScript").Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault();
            if (script == null) throw new InvalidOperationException("Modul paket koku bulunamadi.");
            return Path.GetDirectoryName(Path.GetDirectoryName(script)).Replace('\\', '/');
        }
        public static void AddFiles(System.Collections.Generic.Dictionary<string, string> files, string source, string destination)
        {
            foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".meta")) continue;
                string relative = file.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                files.Add(file.Replace('\\', '/'), (destination + "/" + relative).Replace('\\', '/'));
            }
        }
        public static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (string file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), false);
            foreach (string directory in Directory.GetDirectories(source))
                CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }
        public static void CheckForSecrets(string root)
        {
            foreach (string file in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(file).ToLowerInvariant();
                string name = Path.GetFileName(file);
                if ((name.StartsWith(".env", StringComparison.OrdinalIgnoreCase) && name != ".env.example") ||
                    new[] { ".pem", ".pfx", ".p12", ".jks", ".keystore" }.Contains(extension))
                    throw new InvalidOperationException("Ozel yapilandirma/anahtar dosyasi pakete giremez: " + name);
                if (!new[] { ".cs", ".asset", ".unity", ".prefab", ".json", ".txt", ".md", ".yaml", ".yml" }.Contains(extension)) continue;
                string text = File.ReadAllText(file);
                if (Regex.IsMatch(text, @"\bsk-[A-Za-z0-9_-]{20,}") ||
                    Regex.IsMatch(text, @"\b(?:gh[pousr]_[A-Za-z0-9]{25,}|github_pat_[A-Za-z0-9_]{40,})\b") ||
                    Regex.IsMatch(text, @"-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----") ||
                    Regex.IsMatch(text, @"(?im)^[ \t]*(?:deepSeekApiKey|apiKey|accessToken|password|clientSecret|licenseKey):[ \t]*[^\s\r\n]+"))
                    throw new InvalidOperationException("Pakette olasi API anahtari var. Icerik yazdirilmadi: " + file);
            }
        }
    }
}
