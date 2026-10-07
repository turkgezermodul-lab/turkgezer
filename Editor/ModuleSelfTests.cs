using System;
using System.Collections.Generic;
using System.IO;
using TurkGezer.Api;
using TurkGezer.Missions;
using UnityEditor;
using UnityEngine;

namespace TurkGezer.Editor
{
    public static class ModuleSelfTests
    {
        [Serializable] private sealed class Results { public string utc; public int passed; public List<string> tests = new List<string>(); }
        [MenuItem("Tools/TurkGezer/Cekirdek Testlerini Calistir")]
        public static void Run()
        {
            var results = new Results { utc = DateTime.UtcNow.ToString("o") };
            var definition = ScriptableObject.CreateInstance<MissionDefinition>();
            try
            {
                definition.missionId = "test";
                definition.tasks = new List<MissionTask>
                {
                    new MissionTask { id = "a", targetCount = 2 },
                    new MissionTask { id = "b", prerequisites = new[] { "a" } }
                };
                var session = new MissionSession(definition);
                int completed = 0;
                session.Completed += () => completed++;
                Check(!session.AddProgress("b", 1, "blocked"), "Onkosul tamamlanmadan ilerleme reddedilir", results);
                Check(session.AddProgress("a", 1, "one"), "Ilk ilerleme kabul edilir", results);
                Check(!session.AddProgress("a", 1, "one"), "Ayni token tekrar sayilmaz", results);
                Check(session.GetProgress("a") == 1, "Tekrardan sonra sayac korunur", results);
                Check(!session.AddProgress("a", -1), "Negatif ilerleme reddedilir", results);
                Check(!session.AddProgress("missing"), "Bilinmeyen gorev reddedilir", results);
                Check(session.AddProgress("a", int.MaxValue, "two") && session.GetProgress("a") == 2,
                    "Buyuk ilerleme hedefte sinirlanir", results);
                Check(session.AddProgress("b", 1, "blocked"), "Reddedilen token onkosuldan sonra kullanilabilir", results);
                Check(session.IsComplete && completed == 1, "Misyon tamamlanma eventi bir kez uretilir", results);
                Check(!session.AddProgress("b") && completed == 1, "Tamamlanan gorev tekrar tamamlanamaz", results);
                session.Reset();
                Check(!session.IsComplete && session.GetProgress("a") == 0 && session.AddProgress("a", 1, "one"),
                    "Reset ilerlemeyi ve tokenlari temizler", results);
                definition.tasks[1].id = "a";
                Check(Rejects(definition), "Tekrarlanan gorev kimligi reddedilir", results);
                definition.tasks[1].id = "b";
                definition.tasks[0].prerequisites = new[] { "b" };
                Check(Rejects(definition), "Dongusel bagimlilik reddedilir", results);
                definition.tasks[0].prerequisites = new[] { "missing" };
                Check(Rejects(definition), "Eksik onkosul reddedilir", results);
                Check(IssPositionParser.TryParse("{\"name\":\"iss\",\"id\":25544,\"latitude\":20,\"longitude\":30,\"altitude\":420,\"velocity\":27600,\"timestamp\":1700000000}",
                    IssProvider.WhereTheIssAt, out var wtia) && wtia.hasAltitudeAndVelocity, "WTIA JSON ayrisma", results);
                Check(IssPositionParser.TryParse("{\"message\":\"success\",\"timestamp\":1700000000,\"iss_position\":{\"latitude\":\"20.5\",\"longitude\":\"30.5\"}}",
                    IssProvider.OpenNotify, out var notify) && !notify.hasAltitudeAndVelocity, "Open Notify JSON ayrisma", results);
                Check(!IssPositionParser.TryParse("{}", IssProvider.WhereTheIssAt, out _), "Bos ISS yaniti reddedilir", results);
                Check(!IssPositionParser.TryParse("{\"name\":\"iss\",\"id\":25544,\"latitude\":120,\"longitude\":0,\"altitude\":420,\"timestamp\":1700000000}",
                    IssProvider.WhereTheIssAt, out _), "Gecersiz koordinat reddedilir", results);
                Check(!IssPositionParser.TryParse("not-json", IssProvider.OpenNotify, out _), "Bozuk JSON reddedilir", results);
                Check(ChatService.ParseResponse("{\"choices\":[{\"message\":{\"role\":\"assistant\",\"content\":\"Merhaba\"}}]}") == "Merhaba",
                    "Soru-cevap JSON ayrisma", results);
                Check(ChatService.ParseResponse("{}") == null, "Eksik sohbet yaniti reddedilir", results);
                string directory = Path.GetFullPath("../Dogrulama");
                Directory.CreateDirectory(directory);
                File.WriteAllText(Path.Combine(directory, "cekirdek-testleri.json"), JsonUtility.ToJson(results, true));
                Debug.Log("[TurkGezer] " + results.passed + " cekirdek testi basarili.");
            }
            finally { UnityEngine.Object.DestroyImmediate(definition); }
        }
        private static bool Rejects(MissionDefinition definition)
        {
            try { definition.Validate(); return false; } catch (ArgumentException) { return true; }
        }
        private static void Check(bool condition, string name, Results results)
        {
            if (!condition) throw new Exception("TurkGezer test basarisiz: " + name);
            results.passed++; results.tests.Add(name);
        }
    }
}
