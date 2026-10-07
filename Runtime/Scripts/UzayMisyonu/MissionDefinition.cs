using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurkGezer.Missions
{
    [Serializable]
    public sealed class MissionTask
    {
        public string id;
        public string title;
        [TextArea] public string description;
        [Min(1)] public int targetCount = 1;
        public string[] prerequisites = Array.Empty<string>();
    }

    [CreateAssetMenu(menuName = "TurkGezer/Uzay Misyonu", fileName = "YeniMisyon")]
    public sealed class MissionDefinition : ScriptableObject
    {
        public string missionId = "yeni-misyon";
        public string title = "Uzay Misyonu";
        [TextArea] public string description;
        public List<MissionTask> tasks = new List<MissionTask>();

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(missionId)) throw new ArgumentException("Misyon kimligi bos olamaz.");
            if (tasks == null || tasks.Count == 0) throw new ArgumentException("Misyona en az bir gorev ekleyin.");
            var byId = new Dictionary<string, MissionTask>(StringComparer.Ordinal);
            foreach (var task in tasks)
            {
                if (task == null || string.IsNullOrWhiteSpace(task.id) || task.targetCount < 1)
                    throw new ArgumentException("Gorev kimligi ve hedef sayisi gecersiz.");
                if (byId.ContainsKey(task.id)) throw new ArgumentException("Tekrarlanan gorev: " + task.id);
                byId.Add(task.id, task);
            }
            var visiting = new HashSet<string>();
            var visited = new HashSet<string>();
            foreach (var task in tasks) Visit(task, byId, visiting, visited);
        }

        private static void Visit(MissionTask task, Dictionary<string, MissionTask> byId,
            HashSet<string> visiting, HashSet<string> visited)
        {
            if (visited.Contains(task.id)) return;
            if (!visiting.Add(task.id)) throw new ArgumentException("Dongusel gorev bagimliligi: " + task.id);
            foreach (var id in task.prerequisites ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(id) || !byId.TryGetValue(id, out var prerequisite))
                    throw new ArgumentException("Onkosul gorevi bulunamadi: " + id);
                Visit(prerequisite, byId, visiting, visited);
            }
            visiting.Remove(task.id);
            visited.Add(task.id);
        }
    }
}
