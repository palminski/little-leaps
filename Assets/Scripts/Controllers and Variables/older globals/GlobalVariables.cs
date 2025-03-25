
using System.Collections.Generic;
using UnityEngine;

public enum RoomColor {
    Purple,
    Green
}

[CreateAssetMenu(fileName = "GlobalVariables", menuName = "ScriptableObjects/ColorSettings", order = 1)]
public class GlobalVariables : ScriptableObject
{
    [Header("Game Colors")]
    public Color colorForPurple;
    
    public Color colorForGreen;

[Header("Physics settings")]
    public float globalSkinWidth = 0.001f;
}