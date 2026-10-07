using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace TurkGezer.Diagnostics
{
    public sealed class PerformanceRecorder : MonoBehaviour
    {
        [Min(1)] public float sampleSeconds = 30;
        public string scenarioId = "scenario";
        public bool startAutomatically;
        public bool IsRecording { get; private set; }
        public string LastFilePath { get; private set; }
        private float start;
        private readonly List<float> frames = new List<float>();
        private void Start() { if (startAutomatically) Begin(); }
        public void Begin() { frames.Clear(); start = Time.realtimeSinceStartup; IsRecording = true; }
        private void Update()
        {
            if (!IsRecording) return;
            if (Time.unscaledDeltaTime > 0) frames.Add(Time.unscaledDeltaTime);
            if (Time.realtimeSinceStartup - start >= sampleSeconds) Finish();
        }
        public void Finish()
        {
            if (!IsRecording) return;
            IsRecording = false;
            if (frames.Count == 0) return;
            float sum = 0, maximum = 0;
            foreach (float frame in frames) { sum += frame; maximum = Mathf.Max(maximum, frame); }
            string directory = Path.Combine(Application.persistentDataPath, "TurkGezer", "Measurements");
            try
            {
                Directory.CreateDirectory(directory);
                LastFilePath = Path.Combine(directory, "performance-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff") + ".csv");
                var culture = CultureInfo.InvariantCulture;
                string csv = "utc,scenario,platform,device,unity,frame_count,average_fps,minimum_fps,allocated_memory_mb\n" +
                    Escape(DateTime.UtcNow.ToString("o")) + "," + Escape(scenarioId) + "," + Escape(Application.platform.ToString()) + "," +
                    Escape(SystemInfo.deviceModel) + "," + Escape(Application.unityVersion) + "," + frames.Count + "," +
                    (frames.Count / sum).ToString("F2", culture) + "," + (1 / maximum).ToString("F2", culture) + "," +
                    (Profiler.GetTotalAllocatedMemoryLong() / 1048576d).ToString("F2", culture) + "\n";
                File.WriteAllText(LastFilePath, csv, Encoding.UTF8);
                Debug.Log("[TurkGezer] Performans kaydi: " + LastFilePath, this);
            }
            catch (Exception ex) { Debug.LogWarning("Performans kaydi yazilamadi: " + ex.Message, this); }
        }
        private static string Escape(string value) => "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
    }
}
