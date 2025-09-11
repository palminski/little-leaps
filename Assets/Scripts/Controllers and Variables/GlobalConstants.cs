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
                {1, "This place is changing. It is\nbecoming more difficult for\nThe RABITs to traverse. [0]\n"},
                {2, "It is a result cognitive data \nLeaking out of the RABITs. [0]\n"},
                {3, "Their leaked data is shifting\nthe structure of the [1]\ncognitiveenvironment.\n"},
            }
        },
        {
            "lv_2_complete", new SortedDictionary<int, string>
            {
                {1, "The increased difficulty in [0]\nascent is resulting in fewer\nVirtual Neurotransmitters \nbeing absorbed.\n"},
                {2, "If VNT reception is slowed\ndown t_o much, the virtual [1]\nconsciousness could fail.\n"},
                {3, "It is an___ing, but _ have \nbeen able to develop a few\nwork___unds. [1]\n"},
            }
        },
        {
            "lv_11_complete", new SortedDictionary<int, string>
            {
                {1, "I have noticed that as RABITs [1]\nAscend they can attune to the\n_____ and better absorb VNTs.\n"},
                {2, "Based off this _ developed\nConsciousness Attunement & [0]\nRetention Optimization Tokens \nor CAROTs for short\n"},
                {3, "By depositing CAROTs into\nterminals in the Synapse\nRABITs sh_uld be able to [0]\nspeed up their attunement.\n"},
            }
        },
        {
            "lv_3_complete", new SortedDictionary<int, string>
            {
                {1, "Attuned RABITs con__me more [0]\nsystem resources, which could \ncause issues if left unchecked. \n"},
                {2, "Therefore to stop this from \noverloading the system __ [1]\nlower resource consumption \nfrom other processes.\n"},
                {3, "Sectors may time out sooner, \nattuned RABITs may rec_ver \nslowly, and EGO link may be  \nweaker. [0]\n"},
            }
        },
        {
            "lv_4_complete", new SortedDictionary<int, string>
            {
                {1, "Despite the drawbacks the \nCAROTs seem to be a v_able [1]\nworkaround to the issue. \n"},
                {2, "The greater yield more than \nmakes up for the resource [1]\nre-allocation making ascents \nmore difficult.\n"},
                {3, "So far __ have seen yields up\nto 4.5 times higher! But _ [1]\nthink we can exceed that \nwith some more work.\n"},
            }
        },
        {
            "lv_13_complete", new SortedDictionary<int, string>
            {
                {1, "_ have also been able to [1]\ndevelop _______ method to  \noffset the lowered yields due \nto more diff__ult ascents.\n"},
                {2, "Previously __ were not able\nto absorb any VNTs if the [1]\nRABIT failed to reach the top \nof the Synapse.\n"},
                {3, "after some research however _ \nhave determined that is not \n___ ______ true. [0]\n"},
            }
        },
        {
            "lv_5_complete", new SortedDictionary<int, string>
            {
                {1, "While we do_t see as much, [0]\n_ have developed an algorithm \nthat can capture carried VNTs \nat the time of RABIT failure.\n"},
                {2, "Obviously it is still ______\nfor the RABIT to reach the [0] \nzenith of the Synapse. \n"},
                {3, "But any method to increase\nabsorption of VNTs is a w_n \nin __ b___. [0]\n"},
            }
        },
        {
            "lv_12_complete", new SortedDictionary<int, string>
            {
                {1, "With both of these safeguards\nin place _ t_ink everything  \nshould be fine. [1]\n"},
                {2, "The rate at which the Synapse\nis ________ seems to have  \nslowed down. [0]\n"},
                {3, "Now that the VNT issue has \nbeen taken care of _ want to \nfurther investigate the  [1]\nsynaptic alterations.\n"},
            }
        },
        {
            "lv_19_complete", new SortedDictionary<int, string>
            {
                {1, "________ Each synaptic\nzone has started to adopt a  \ndifferent appearance. [1]\n"},
                {2, "Originally everything was ___ \ngray color that we had [0]\nused to keep _________ simple. \n"},
                {3, "But it appears that all the\ncognitive data from the RABITs \nhas resulted in a kind of  [0]\nmetamorphosis\n"},
            }
        },
        {
            "lv_6_complete", new SortedDictionary<int, string>
            {
                {1, "Sector Zero has begun to take \non a cave-like appearance. [0]\nthe gray walls have morphed  \ninto a clay color.\n"},
                {2, "Data pipes have also begun to\nchange color. What appears to \nbe foliage has started to [1]\ngrow on the ground and walls.\n"},
                {3, "_ w__l d_b ____ sector Cav__n.\n___ the most ________, _ know. \n___ it is fitting. [0]\n"},
            }
        },
        {
            "lv_20_complete", new SortedDictionary<int, string>
            {
                {1, "Sector One has ______ a dark\ngreen plant matter. It looks [0]\nlike the foliage from Cavern \ngrown to great extents.\n"},
                {2, "vines wrap around the area [1]\nlike those of a thick jungle. \n"},
                {3, "an ___________ name for this \nsector would be Tangle. [1]\n"},
            }
        },
        {
            "lv_7_complete", new SortedDictionary<int, string>
            {
                {1, "Sector Two has shed its gray\nwalls for bright crystals. _ [1]\nWill refer to it as L__is.\n"},
                {2, "_ ____ noticed what appear to\nbe other non RABIT entities \nforming around VNTs. [0]\n"},
                {3, "They are volatile, and damage\nthe RABITs ego connection. [1]\nL____ily __ seems RABITs can \nal_o decompile enemies ___\nabsorb the contained VNTs.\n "},
            }
        },
        {
            "lv_8_complete", new SortedDictionary<int, string>
            {
                {1, "The last two sectors look a\nbit disturbing. Sector Three \nlooks to be composed of  \nbiologic matterial. [1]\n"},
                {2, "Certain areas on the ground\nare covered in tiny dangerous \ncognitive entities. [1]\n"},
                {3, "There are pools of what appear\nto be blood forming in pits. \n_____ refer to this sector as \nGore. [1]\n"},
            }
        },
        {
            "lv_14_complete", new SortedDictionary<int, string>
            {
                {1, "The last area where the RABIT\ncan access the top of the  \nsynapse has become a deep  \nshade of red. [0]\n"},
                {2, "The creatures mentioned in\nearlier entries are here in  \nabundance. [0]\n"},
                {3, "____ area is more dangerous\nthan any previous areas by [1]\nfar. _____ __is area Crimson. \n"},
            }
        },
        {
            "lv_16_complete", new SortedDictionary<int, string>
            {
                {1, "While the rate at which the \nsectors are converting seems \nto be slowing down, __ has \nnot stopped entirely. [0]\n"},
                {2, "As a preventative measure _ \nhave been working on a way to \ngreatly and rapidly increase \nRABIT attunement.[1]\n"},
                {3, "Once that is developed __ [1]\nshould consistently be able \nto collect enough VNTs. \n"},
            }
        },
        {
            "lv_9_complete", new SortedDictionary<int, string>
            {
                {1, "_ will build out the proper\narea in the areas in ___ \nSynaptic ______. [0]\n"},
                {2, "It is __ experimental f__,\nand as such it would be best \nif __ ___ wandered there by \naccident. [0]\n"},
                {3, "The space in the __rder is [0]\nso dist__ted that it will be \nnear impossible to stumble to \na specific place accidently"},
            }
        },
        {
            "lv_10_complete", new SortedDictionary<int, string>
            {
                {1, "_____ _ set up a system to\nrapidly attune the RABIT to  \nits limit. [0]\n"},
                {2, "Unfortunately, due to the \naforementioned limitations  \ndoing so res__ts in a ____ [1]\ndi___cu_t climb.\n"},
                {3, "Additionally, the excess VNTs\ncan have a __________ effect \nif a RABIT ____ make it to  \nthe top. [0]\n"},
            }
        },
        {
            "lv_17_complete", new SortedDictionary<int, string>
            {
                {1, "__ initial plan was to make [1]\nthis the default state for \nthe RABITs ___ that may n_t  \nbe ___ best course of action.\n"},
                {2, "How__, scrapping the system\nentirely _____ a waste as it \ncan result in such a high VNT \n yield.[1]"},
                {3, "As such _ ____ left it in the\nprogram, just not super easily \naccessible. [1]\n"},
            }
        },
        {
            "lv_15_complete", new SortedDictionary<int, string>
            {
                {1, "_ have p_____ the ____ to\nmax attune a RABIT in the  \n_order. [1]\n"},
                {2, "__ may not be needed at all\nas things seem to be ________ \n____ and the VNT yield is \n_____ perfectly fine. [0]\n"},
                {3, "But in case ______ do__ want\nto try it thats w__re its  \nhidden away. [0]\n"},
            }
        },
        {
            "lv_18_complete", new SortedDictionary<int, string>
            {
                {1, "It is a maze back there, __\n__ have encoded directions \ninto the CAROTs sca____ed \naround. [1]\n"},
                {2, "____ chip hold a piece of\ndata that when c___ined will \nreveal the path. [0]\n"},
                {3, "Should ______ need to access\nthis ________ feature, ___ \n___ binary info on the chips \n__ navigate. [1]\n"},
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
