using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create GameParameter", fileName = "GameParameter")]
public class GameParameter : ScriptableObject {
    public float ChainConnectTime = 0.5f;
}
