using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TurkGezer.Editor
{
    /// <summary>Checks the distribution's exact file set, without following project dependencies.</summary>
    public static class PackageIsolationAudit
    {
        [MenuItem("Tools/TurkGezer/Paket Bagimsizligini Denetle")]
        public static void Run() => Validate(ModuleExporter.PackageRoot());

        public static void Validate(string root)
        {
            string[] files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            var forbidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "LegacyInputBridge.cs", "LegacyMissionBridge.cs", "MilliUzayMisyonu.asset",
                "Uzay.unity", "Giris.unity", "Konuşma.unity", "SohbetYonetici.cs",
                "ISSTracker.cs", "OyunYoneticisiOtomatik.cs", "AstronotGorevYonetici.cs",
                "IcMekanGorevYonetici.cs"
            };
            var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string file in files)
            {
                string relative = file.Substring(root.Length).Replace('\\', '/').TrimStart('/');
                if (forbidden.Contains(Path.GetFileName(file)) || relative.Contains("/ThirdParty/") || relative.Contains("/Legacy/"))
                    throw new InvalidOperationException("Oyuna ozel icerik pakete giremez: " + relative);
                if (!file.EndsWith(".meta")) continue;
                var match = Regex.Match(File.ReadAllText(file), @"(?m)^guid:\s*([0-9a-f]{32})\s*$");
                if (match.Success && !known.Add(match.Groups[1].Value))
                    throw new InvalidOperationException("Pakette tekrarlanan GUID: " + relative);
            }
            // Unity UI is an explicitly declared dependency, not game content.
            var unityDataPackages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().Where(p => p.name == "com.unity.ugui" || p.name == "com.unity.textmeshpro");
            var dependencyGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var dataPackage in unityDataPackages)
            foreach (string meta in Directory.GetFiles(dataPackage.resolvedPath, "*.meta", SearchOption.AllDirectories))
            {
                var match = Regex.Match(File.ReadAllText(meta), @"(?m)^guid:\s*([0-9a-f]{32})\s*$");
                if (match.Success) dependencyGuids.Add(match.Groups[1].Value);
            }
            int references = 0;
            foreach (string file in files.Where(path => new[] { ".unity", ".prefab", ".asset", ".mat", ".anim", ".controller", ".overrideController", ".meta" }.Contains(Path.GetExtension(path))))
            foreach (Match match in Regex.Matches(File.ReadAllText(file), @"guid:\s*([0-9a-f]{32})"))
            {
                string guid = match.Groups[1].Value;
                if (guid.StartsWith("0000000000000000", StringComparison.Ordinal)) continue; // Unity built-ins.
                if (!known.Contains(guid) && !dependencyGuids.Contains(guid)) throw new InvalidOperationException("Paket disina referans: " + Path.GetFileName(file) + " -> " + guid);
                references++;
            }
            Debug.Log("TURKGEZER_ISOLATION_OK: " + files.Length + " files, " + references + " module/declared Unity data references; no old game-specific code/scene dependencies.");
        }
    }
}
