using System.Collections.Generic;
using System.Linq;
using Core.Hint;

namespace Match3.Unity.Hint
{
    public static  class HintExplanationFormatter
    {
        public static string Format(HintExplanation hintExplanation)
        {
            if (hintExplanation == null)
            {
                return "Try swapping these pieces.";
            }

            var reasons = hintExplanation.Reasons;
            if (reasons.Count == 0)
            {
                return "Try swapping these pieces.";
            }
            
            var parts = new List<string>();
            if (reasons.Contains(HintReasonType.ActivatesSpecialDirectly))
            {
                parts.Add("activates a special piece");
            }
            else if (reasons.Contains(HintReasonType.CreatesSpecial))
            {
                parts.Add("creates a special piece");
            }
            else if (reasons.Contains(HintReasonType.CreatesMatch))
            {
                parts.Add("creates a match");
            }

            if (reasons.Contains(HintReasonType.ProgressesColorGoal))
            {
                parts.Add("helps collect goal pieces");
            }
            if (reasons.Contains(HintReasonType.DamagesObstacle))
            {
                parts.Add("damages obstacles");
            }

            if (parts.Count == 0)
            {
                return "Try swapping these pieces.";
            }

            var firstItem = parts[0];
            parts[0] = char.ToUpper(firstItem[0]) + firstItem.Substring(1);
            if (parts.Count == 1)
            {
                return parts[0] + ".";
            }
            
            var initialPart = string.Join(", ", parts.Take(parts.Count - 1));
            var lastItem = parts.Last();
            
            return $"{initialPart} and {lastItem}.";
        }
    }
}