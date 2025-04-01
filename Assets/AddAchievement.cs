using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AddAchievement : MonoBehaviour
{

    [SerializeField] private TriggerEvent triggerOn;
    [SerializeField] private string achievementName;
    // Start is called before the first frame update
    void Start()
    {
        if (triggerOn == null)
        {
            AddAchievementName();
        }
    }

    private void OnEnable()
    {
        if (triggerOn)
        {
            triggerOn.OnEventRaised.AddListener(AddAchievementName);
        }
    }
    private void OnDisable()
    {
        if (triggerOn)
        {
            triggerOn.OnEventRaised.RemoveListener(AddAchievementName);
        }
    }

    // Update is called once per frame
    private void AddAchievementName()
    {
        if (achievementName.Length > 0 && SteamManager.Initialized)
        {
            SteamUserStats.SetAchievement(achievementName);
            SteamUserStats.StoreStats();
        }
    }
}
