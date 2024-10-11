using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeChipsForPoints : MonoBehaviour
{
    [SerializeField] private DialogueEvent ActivateEvent;
    [SerializeField] private WorldDialogue worldDialogue;

    private void OnEnable()
    {
        if (ActivateEvent)
        {
            ActivateEvent.OnEventRaised.AddListener(OnEventRaised);
        }
    }
    private void OnDisable()
    {
        if (ActivateEvent)
        {
            ActivateEvent.OnEventRaised.RemoveListener(OnEventRaised);
        }
    }

    void OnEventRaised()
    {

        List<string> idsToRemove = new List<string>();
        Dictionary<string, FollowingObject> followingObjects = GameController.Instance.FollowingObjects;
        foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
        {
            if (entry.Value.Name == "chip")
            {
                idsToRemove.Add(entry.Key);
            }
        }
        int multiplier = 1;
        int totalPointsAdded = 0;
        int totalTimeAdded = 0;
        foreach (string key in idsToRemove)
        {
            GameController.Instance.TagObjectStringAsCollected(key);
            GameController.Instance.AddToTimer(30);
            GameController.Instance.AddToScore(5000 * multiplier);
            GameController.Instance.RemoveFollowingObject(key);
            multiplier++;

            totalPointsAdded += (5000 * multiplier);
            totalTimeAdded += 30;
        }
        if (worldDialogue)
        {
            worldDialogue.textElement.text = "";
            worldDialogue.textToType = $@"> SUCCESS! TIMER EXTENDED
> CHIPS DEPOSITED: {multiplier-1}
> POINTS ADDED: {totalPointsAdded}
> TIME ADDED TO RESET TIME: {totalTimeAdded}";
        }
    }
}
