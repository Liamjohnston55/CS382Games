using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public TMP_Text levelText;  // drag in your “LevelText” text component
    public Slider   xpBar;      // drag in your XP Slider

    private PlayerStats stats;

    void Start() {
        // grab the current "PlayerStats:
        stats = GameObject.FindFirstObjectByType<PlayerStats>();
        if (stats == null) {
            return;
        }

        // show current level & XP
        UpdateLevelText(stats.level);
        UpdateXPBar(stats.currentXP, stats.xpToNextLevel);

        // subscribe to get active updates on your level and xp
        stats.OnLevelUp   += UpdateLevelText;
        stats.OnXPChanged += UpdateXPBar;
    }

    private void UpdateLevelText(int newLevel)
    {
        levelText.text = $"Level {newLevel}";
    }

    private void UpdateXPBar(int currentXP, int xpToNext)
    {
        xpBar.maxValue = xpToNext;
        xpBar.value    = currentXP;
    }

    void OnDestroy()
    {
        if (stats != null)
        {
            stats.OnLevelUp   -= UpdateLevelText;
            stats.OnXPChanged -= UpdateXPBar;
        }
    }
}
