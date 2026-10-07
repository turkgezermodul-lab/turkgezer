using System;
using System.Collections.Generic;

namespace TurkGezer.Missions
{
    /// <summary>UI, sahne, klavye ve XR SDK'larindan bagimsiz gorev durumu.</summary>
    public sealed class MissionSession
    {
        private readonly Dictionary<string, MissionTask> tasks = new Dictionary<string, MissionTask>();
        private readonly Dictionary<string, int> progress = new Dictionary<string, int>();
        private readonly Dictionary<string, HashSet<string>> tokens = new Dictionary<string, HashSet<string>>();
        public MissionDefinition Definition { get; }
        public event Action<string, int, bool> ProgressChanged;
        public event Action Completed;
        public bool IsComplete
        {
            get
            {
                foreach (var pair in tasks) if (!IsTaskComplete(pair.Key)) return false;
                return tasks.Count > 0;
            }
        }

        public MissionSession(MissionDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            definition.Validate();
            Definition = definition;
            foreach (var task in definition.tasks)
            {
                // Copy definitions so Inspector changes cannot corrupt an active session.
                tasks.Add(task.id, new MissionTask { id = task.id, title = task.title,
                    targetCount = task.targetCount, prerequisites = (string[])(task.prerequisites ?? Array.Empty<string>()).Clone() });
                progress.Add(task.id, 0);
                tokens.Add(task.id, new HashSet<string>(StringComparer.Ordinal));
            }
        }

        public int GetProgress(string id) => id != null && progress.TryGetValue(id, out var count) ? count : 0;
        public bool IsTaskComplete(string id) => id != null && tasks.TryGetValue(id, out var task) && GetProgress(id) >= task.targetCount;
        public bool CanProgress(string id)
        {
            if (id == null || !tasks.TryGetValue(id, out var task) || IsTaskComplete(id)) return false;
            foreach (var prerequisite in task.prerequisites) if (!IsTaskComplete(prerequisite)) return false;
            return true;
        }

        public bool AddProgress(string id, int amount = 1, string uniqueToken = null)
        {
            if (amount <= 0 || !CanProgress(id)) return false;
            if (!string.IsNullOrEmpty(uniqueToken) && !tokens[id].Add(uniqueToken)) return false;
            progress[id] = (int)Math.Min((long)tasks[id].targetCount, (long)progress[id] + amount);
            ProgressChanged?.Invoke(id, progress[id], IsTaskComplete(id));
            if (IsComplete) Completed?.Invoke();
            return true;
        }

        public void Reset()
        {
            foreach (var id in tasks.Keys) { progress[id] = 0; tokens[id].Clear(); }
        }
    }
}
