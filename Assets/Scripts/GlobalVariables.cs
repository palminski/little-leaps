
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalVariables", menuName = "ScriptableObjects/ColorSettings", order = 1)]
public class GlobalVariables : ScriptableObject
{
    [Header("Game Colors")]
    public Color colorForRoomState0;
    
    public Color colorForRoomState1;

[Header("Physics settings")]
    public float globalSkinWidth = 0.001f;
}