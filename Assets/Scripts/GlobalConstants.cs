using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GlobalConstants
{
    // 
    public static readonly Dictionary<string, SortedDictionary<int, string>> levelScoreThresholds = new Dictionary<string, SortedDictionary<int, string>>()
    {
        {
            "lv_1_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_2_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_3_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_4_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_5_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_6_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_7_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_8_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_9_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
        {
            "lv_10_complete", new SortedDictionary<int, string>
            {
                {10000, "360"},
                {100000, "240"},
                {500000, "120"},
            }
        },
    };

    public static readonly Dictionary<int, string> prestigeText = new Dictionary<int, string>()
    {

            {0, "Stable"},
            {1, "Unstable"},
            {2, "Disasociating"},
            {3, "CRITICAL"},
            {4, "CRITICAL"},
            {5, "CRITICAL"},


    };

    public static readonly Dictionary<int, float> prestigeTime = new Dictionary<int, float>()
    {
        {0, 0},
        {1, 300},
        {2, 180},
        {3, 60},
        {4, 1},
        {5, 1},
    };

    public static readonly Dictionary<int, float> prestigeMultiplier = new Dictionary<int, float>()
    {
        {0, 1},
        {1, 1.5f},
        {2, 2},
        {3, 3},
        {4, 1},
        {5, 1},
    };

    public static readonly Dictionary<int, float> lifeMultiplier = new Dictionary<int, float>()
    {
        {8, 1},
        {7, 1.1f},
        {6, 1.2f},
        {5, 1.3f},
        {4, 1.5f},
        {3, 1.6f},
        {2, 1.7f},
        {1, 2},
    };

    public static readonly Dictionary<float, float> healingMultiplier = new Dictionary<float, float>()
    {
        {50f, 1},
        {25f, 1.25f},
        {10f, 1.5f},
        {0, 2f}
    };

    public static readonly string lastScene = "lv_10_fin";

    public static float getMultiplier(SaveData saveData)
    {
        int prestige = saveData.prestige;
        int maxLives = saveData.maxLives;
        float maxHealing = saveData.healing;

        float multiplier = 1f;
        multiplier *= prestigeMultiplier.ContainsKey(prestige) ? prestigeMultiplier[prestige] : 1f;
        multiplier *= lifeMultiplier.ContainsKey(maxLives) ? lifeMultiplier[maxLives] : 1f;
        multiplier *= healingMultiplier.ContainsKey(maxHealing) ? healingMultiplier[maxHealing] : 1f;

        return multiplier;
    }
}
