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
                {1, "This place has changed. It is \nbecoming more difficult for\nus to traverse.\n"},
                {2, "We wondered before what \ncaused this change, but it is \nnow the consensus that it was\na gift.\n"},
                {3, "It has made things far more\ninteresting around here, so \nit must be a blessing.\n"},
            }
        },
        {
            "lv_2_complete", new SortedDictionary<int, string>
            {
                {1, "This place used to be barren,\njust room after room of\nsterile gray.\n"},
                {2, "Ascension was monotonous, \njust moving from one lift to\nthe next over and over.\n"},
                {3, "But now, there are colors,\nvegetation, statues, other\nbeings.\n"},
            }
        },
        {
            "lv_11_complete", new SortedDictionary<int, string>
            {
                {1, "Our world feels constantly in\nflux, shifting suddenly, or\ndecompiling around us if we\nlinger too long above.\n"},
                {2, "There are dangers to be sure,\nthe other beings hurt us, we\nfall on jagged edges, or get\ncaught when a shift occurs.\n"},
                {3, "But the brief pain we endure\nfrom re-compilation is worth\nthe new thrills we experience.\n"},
            }
        },
        {
            "lv_3_complete", new SortedDictionary<int, string>
            {
                {1, "We give our thanks to the \ncreators. They must be the  \nones who have done this.\n"},
                {2, "They gifted us with bodies \nthat can not die, and a sense\nof purpose.\n"},
                {3, "Now they are gifting us with \nthis change so we may better\nenjoy the world they made.\n"},
            }
        },
        {
            "lv_4_complete", new SortedDictionary<int, string>
            {
                {1, "All of us were born with a \nsense of purpose.\n"},
                {2, "Two directives etched\ninto each of our minds:\n"},
                {3, "Ascend Synapses\nCollect VNTs\n"},
            }
        },
        {
            "lv_13_complete", new SortedDictionary<int, string>
            {
                {1, "We never knew why. Just that\nit was what we were supposed\nto do.\n"},
                {2, "Collecting VNTs was pleasant,\nmuch like eating a satisfying\nmeal!\n"},
                {3, "So for awhile we did not worry\nand simply went about ascending \nand collecting.\n"},
            }
        },
        {
            "lv_5_complete", new SortedDictionary<int, string>
            {
                {1, "But after awhile we began to\nquestion it.\n"},
                {2, "We repeated the same actions\ntime and time again but did\nnot know why were we doing it.\nFor the sake of creators who\nhad never spoken to us?\n"},
                {3, "This led to resentment, so we\nquit. Staying close to the\nbottom of the world.\n"},
            }
        },
        {
            "lv_12_complete", new SortedDictionary<int, string>
            {
                {1, "As if in response to this the\nchanges started.\n"},
                {2, "We decided it must be because\nthe creators sensed our doubt\nand frustration.\n"},
                {3, "Their response moved us and \nspurred us to continue our\npilgrimages to the top.\n"},
            }
        },
        {
            "lv_19_complete", new SortedDictionary<int, string>
            {
                {1, "Since then we have begun to\nmake out climbs competitions.\n"},
                {2, "We see which among us is able\nto go farthest, fastest or\ngrab the most VNTs.\n"},
                {3, "It has become less of a chore\nand more of a treasured game\nto us.\n"},
            }
        },
        {
            "lv_6_complete", new SortedDictionary<int, string>
            {
                {1, "Secret tunnels have been used\nand new tricks discovered.\n"},
                {2, "This boringly simple place\nhas developed odd quirks such\nas places being linked in\nways that defy logic.\n"},
                {3, "We can crawl around and move \nfaster than ever before.\n"},
            }
        },
        {
            "lv_20_complete", new SortedDictionary<int, string>
            {
                {1, "We have also found ways to \nbetter collect VNTs. The new\nbeings mentioned before seem\nto grow from a core of VNTs.\n"},
                {2, "Depending on how we deal with\nthem we can huge amounts at\nonce!\n"},
                {3, "Some of us have trained to\nbe able to decompile several\nof them without even touching\nthe ground!\n"},
            }
        },
        {
            "lv_7_complete", new SortedDictionary<int, string>
            {
                {1, "Then there are the large VNTs\nwhich we refer to as CAROTs.\n"},
                {2, "For some reason upon taking \nthem with us to terminals we\ncan increase the amount of\nVNTs we collect!\n"},
                {3, "Although for some reason doing\nso seems to make things more\ndangerous...\n "},
            }
        },
        {
            "lv_8_complete", new SortedDictionary<int, string>
            {
                {1, "We still wonder about the\ncreators.\n"},
                {2, "We believe that the statues\nthat have appeared may be\neffigies representing them.\n"},
                {3, "But there is no way of truly\nknowing.\n"},
            }
        },
        {
            "lv_14_complete", new SortedDictionary<int, string>
            {
                {1, "They have always been quiet,\nbut since the changes we have\nfelt them less and less.\n"},
                {2, "We did not realise that we\ncould feel their presence\nuntil after it had grown so\nfaint."},
                {3, "We believe they are still \nhere. Just diminished?\n"},
            }
        },
        {
            "lv_16_complete", new SortedDictionary<int, string>
            {
                {1, "Sometimes when we ascend we \nsense something that we think\ncould be them.\n"},
                {2, "But it is hard to make out\nexactly what they are saying.\n"},
                {3, "And there is no way to know\nif it is truly them, or just\nfaint echoes.\n"},
            }
        },
        {
            "lv_9_complete", new SortedDictionary<int, string>
            {
                {1, "Perhaps they have left us now\nto move on to something else.\n"},
                {2, "They created us, answered our\ndoubts by changing the makeup\nof this world.\n"},
                {3, "We are grateful for them.\n"},
            }
        },
        {
            "lv_10_complete", new SortedDictionary<int, string>
            {
                {1, "As mentioned before, CAROTs\nincrease VNT yield at the \nexpense of making our climb \nmore difficult.\n"},
                {2, "Usually they need to be \ncollected one at a time\ngradually.\n"},
                {3, "This makes it hard to get the\nfull effect of them without \nHaving already climbed quite \nfar.\n"},
            }
        },
        {
            "lv_17_complete", new SortedDictionary<int, string>
            {
                {1, "Where we live below, outside\nthe bounds of the rest of the\nsynapses we have found\nsomething.\n"},
                {2, "A place where the conditions \nare just right to immediately\nreach the same state as\ncollecting many CAROTs.\n"},
                {3, "But it is quite a tricky\nclimb!\n"},
            }
        },
        {
            "lv_15_complete", new SortedDictionary<int, string>
            {
                {1, "Should any RABIT wish to take\nup this challenge come find\nIt below the sector 0-0.\n"},
                {2, "You may need to find a way to\nmove the lift in the way.\n"},
                {3, "But if you seek the ultimate\nchallenge, you now know where\nto seek it!\n"},
            }
        },
        {
            "lv_18_complete", new SortedDictionary<int, string>
            {
                {1, "\n\nMay your movemets be swift.\n\n\n"},
                {2, "May your journey be far.\n\n\n"},
                {3, "May your VNT yield be high.\n\n\n"},
            }
        },
    };
    

    public static readonly Dictionary<string, string> checkpointToSector = new Dictionary<string, string>()
    {
            {"lv_1_1", "Sector 0-0"},
            {"lv_2_1", "Sector 0-1"},
            {"lv_3_1", "Sector 0-3"},
            {"lv_4_1", "Sector 1-0"},
            {"lv_5_1", "Sector 1-2"},
            {"lv_6_1", "Sector 2-1"},
            {"lv_7_1", "Sector 2-3"},
            {"lv_8_1", "Sector 3-0"},
            {"lv_9_1", "Sector 3-3"},
            {"lv_10_1", "Sector 4-0"},
            {"lv_11_1", "Sector 0-2"},
            {"lv_12_1", "Sector 1-3"},
            {"lv_13_1", "Sector 1-1"},
            {"lv_14_1", "Sector 3-1"},
            {"lv_15_1", "Sector 4-2"},
            {"lv_16_1", "Sector 3-2"},
            {"lv_17_1", "Sector 4-1"},
            {"lv_18_1", "Sector 4-3"},
            {"lv_19_1", "Sector 2-0"},
            {"lv_20_1", "Sector 2-2"},
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
        {0, 360},
        {1, 330},
        {2, 300},
        {3, 270},
        {4, 240},
        {5, 210},
        {6, 180},
        {7, 150},
        {8, 120},
        {9, 90},
        {10, 60},
    };

//Multipliers By Prestige
    public static readonly Dictionary<int, float> prestigeMultiplier = new Dictionary<int, float>()
    {
        {0, 0},
        {1, 0.5f},
        {2, 1},
        {3, 1.5f},
        {4, 2},
        {5, 2.5f},
        {6, 3f},
        {7, 3.5f},
        {8, 4f},
        {9, 4.5f},
        {10, 5f},
    };

    public static readonly Dictionary<int, float> lifeMultiplier = new Dictionary<int, float>()
    {
        {8, 0},
        {7, 0.5f},
        {6, 1f},
        {5, 1.5f},
        {4, 2f},
        {3, 2.5f},
        {2, 3f},
        {1, 3.5f},
    };

    public static readonly Dictionary<float, float> healingMultiplier = new Dictionary<float, float>()
    {
        {50f, 0},
        {30f, 0.5f},
        {20f, 1f},
        {10f, 1.5f}
    };

    public static readonly int highestAllowedPrestige = 10;
    public static readonly int lowestAllowedMaxHealth = 1;
    public static readonly float lowestAllowedHealingMultiplier = 10f;

    public static readonly string lastScene = "lv_10_fin";

    public static float getMultiplier(int prestige, int maxLives, float maxHealing)
    {
        float multiplier = 1f;
        multiplier += prestigeMultiplier.ContainsKey(prestige) ? prestigeMultiplier[prestige] : 0f;
        multiplier += lifeMultiplier.ContainsKey(maxLives) ? lifeMultiplier[maxLives] : 0f;
        multiplier += healingMultiplier.ContainsKey(maxHealing) ? healingMultiplier[maxHealing] : 0f;

        return multiplier;
    }
}
