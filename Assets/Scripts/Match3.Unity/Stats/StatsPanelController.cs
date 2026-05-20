using System.Text;
using Match3.Unity.Save;
using TMPro;
using UnityEngine;

namespace Match3.Unity.Stats
{
    public sealed class StatsPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text filePathText;

        private LevelResultStore store;

        private void Awake()
        {
            store = new LevelResultStore();
            Hide();
        }

        public void Show()
        {
            if (root != null)
                root.SetActive(true);

            Render();
        }

        public void Hide()
        {
            if (root != null)
                root.SetActive(false);
        }

        public void Refresh()
        {
            Render();
        }

        public void ClearResults()
        {
            store.Clear();
            Render();
        }

        private void Render()
        {
            if (statsText == null)
                return;

            var data = store.LoadAll();
            var summaries = LevelResultSummaryBuilder.Build(data.Results);

            if (filePathText != null)
                filePathText.text = store.FilePath;

            if (summaries.Count == 0)
            {
                statsText.text = "No saved level results yet.";
                return;
            }

            var builder = new StringBuilder();

            builder.AppendLine("LEVEL RESULTS");
            builder.AppendLine();

            foreach (var summary in summaries)
            {
                builder.AppendLine(summary.LevelId);
                builder.AppendLine($"Attempts: {summary.Attempts}");
                builder.AppendLine($"Victories: {summary.Victories}");
                builder.AppendLine($"Defeats: {summary.Defeats}");

                if (summary.BestTurnsToVictory >= 0)
                    builder.AppendLine($"Best victory turns: {summary.BestTurnsToVictory}");
                else
                    builder.AppendLine("Best victory turns: -");

                builder.AppendLine($"Manual hints shown: {summary.ManualHintsShown}");
                builder.AppendLine($"Idle hints shown: {summary.IdleHintsShown}");
                builder.AppendLine($"Hints applied: {summary.HintsApplied}");
                builder.AppendLine($"Total cleared cells: {summary.TotalClearedCells}");
                builder.AppendLine($"Destroyed obstacles: {summary.DestroyedObstacles}");
                builder.AppendLine($"Last result: {summary.LastResult}");
                builder.AppendLine();
            }

            statsText.text = builder.ToString();
        }
    }
}