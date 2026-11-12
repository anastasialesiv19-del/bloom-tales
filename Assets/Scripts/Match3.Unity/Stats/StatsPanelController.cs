using System.Text;
using Match3.Unity.Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Unity.Stats
{
    public sealed class StatsPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Dropdown levelDropdown;
        [SerializeField] private ScrollRect statsScrollRect;

        private LevelResultStore _store;

        private void Awake()
        {
            _store = new LevelResultStore();
            Hide();
        }

        public void Show()
        {
            if (root != null)
            {
                root.SetActive(true);
            }
            Render();
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        public void Refresh()
        {
            Render();
        }

        public void ClearResults()
        {
            _store.Clear();
            Render();
        }
        
        public void OnLevelChanged(int index)
        {
            Render();
            Canvas.ForceUpdateCanvases();
            statsScrollRect.verticalNormalizedPosition = 1f;
        }

        private void Render()
        {
            if (statsText == null)
            {
                return;
            }

            var data = _store.LoadAll();
            var summaries = LevelResultSummaryBuilder.Build(data.Results);
            var selectedLevelId = $"Level{levelDropdown.value + 1}";
            
            statsText.text = "No attempts yet.";
            var builder = new StringBuilder();

            foreach (var summary in summaries)
            {
                if (summary.LevelId != selectedLevelId)
                {
                    continue;
                }
                
                builder.AppendLine($"Attempts: {summary.Attempts}");
                builder.AppendLine($"Victories: {summary.Victories}");
                builder.AppendLine($"Defeats: {summary.Defeats}");
                builder.AppendLine(summary.BestTurnsToVictory >= 0 ? $"Best victory turns: {summary.BestTurnsToVictory}" : "Best victory turns: -");
                builder.AppendLine($"Hints applied: {summary.HintsApplied}");
                builder.AppendLine($"Last result: {summary.LastResult}");
                
                statsText.text = builder.ToString();
                break;
            }
        }
    }
}