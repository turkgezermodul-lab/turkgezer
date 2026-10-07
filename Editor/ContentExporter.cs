using System;
using System.Globalization;
using System.IO;
using System.Text;
using TurkGezer.Experiments;
using UnityEditor;
using UnityEngine;

namespace TurkGezer.Editor
{
    public static class ContentExporter
    {
        [MenuItem("Tools/TurkGezer/Secili Modeli STL Olarak Aktar")]
        public static void ExportStl()
        {
            var selected = Selection.activeGameObject;
            if (selected == null) { Debug.LogWarning("STL icin bir model secin."); return; }
            string path = EditorUtility.SaveFilePanel("STL aktar (1 Unity birimi = 1 metre)", "", selected.name + ".stl", "stl");
            if (string.IsNullOrEmpty(path)) return;
            var text = new StringBuilder("solid TurkGezer\n");
            int faces = 0;
            foreach (var filter in selected.GetComponentsInChildren<MeshFilter>(true))
            {
                var mesh = filter.sharedMesh;
                if (mesh == null) continue;
                if (!mesh.isReadable) throw new InvalidOperationException("Mesh Read/Write kapali. Importer'dan acin: " + filter.name);
                var vertices = mesh.vertices; var indices = mesh.triangles;
                for (int i = 0; i < indices.Length; i += 3)
                {
                    Vector3 a = filter.transform.TransformPoint(vertices[indices[i]]) - selected.transform.position;
                    Vector3 b = filter.transform.TransformPoint(vertices[indices[i + 1]]) - selected.transform.position;
                    Vector3 c = filter.transform.TransformPoint(vertices[indices[i + 2]]) - selected.transform.position;
                    a *= 1000; b *= 1000; c *= 1000;
                    Vector3 normal = Vector3.Cross(b - a, c - a).normalized;
                    text.Append("facet normal ").Append(Vector(normal)).Append("\nouter loop\nvertex ").Append(Vector(a))
                        .Append("\nvertex ").Append(Vector(b)).Append("\nvertex ").Append(Vector(c)).Append("\nendloop\nendfacet\n");
                    faces++;
                }
            }
            if (faces == 0) { Debug.LogWarning("Secimde aktarilabilir ucgen yok."); return; }
            text.Append("endsolid TurkGezer\n");
            File.WriteAllText(path, text.ToString());
            Debug.Log("STL aktarildi; boyut, manifold/kapali geometri ve baskiya uygunluk dilimleyicide ayrica denetlenmeli: " + path);
        }
        private static string Vector(Vector3 value) => value.x.ToString("R", CultureInfo.InvariantCulture) + " " +
            value.y.ToString("R", CultureInfo.InvariantCulture) + " " + value.z.ToString("R", CultureInfo.InvariantCulture);
        [MenuItem("Tools/TurkGezer/Secili Deney Kartini HTML Olarak Aktar")]
        public static void ExportCard()
        {
            var experiment = Selection.activeObject as ExperimentDefinition;
            if (experiment == null) { Debug.LogWarning("Bir ExperimentDefinition asseti secin."); return; }
            string path = EditorUtility.SaveFilePanel("Deney karti", "", experiment.name + ".html", "html");
            if (string.IsNullOrEmpty(path)) return;
            string html = "<!doctype html><html lang='tr'><meta charset='utf-8'><title>TurkGezer Deney Karti</title>" +
                "<style>body{font:18px Arial;margin:40px;line-height:1.6}article{border:2px solid #18375b;padding:24px}h1{color:#18375b}@media print{body{margin:10mm}}</style><article><p>TURKGEZER DENEY KARTI</p><h1>" +
                Escape(experiment.title) + "</h1><p>" + Escape(experiment.explanation).Replace("\n", "<br>") + "</p><p>Kaynak: " +
                Escape(experiment.sourceUrl) + "</p><p>Dusun: Deneyin uzayda yapilmasi hangi kosullari degistirir?</p></article></html>";
            File.WriteAllText(path, html, Encoding.UTF8);
            Debug.Log("Deney karti aktarildi; tarayicida yazdirabilirsiniz: " + path);
        }
        private static string Escape(string value) => (value ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
    }
}
