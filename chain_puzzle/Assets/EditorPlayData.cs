using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create EditorPlayData", fileName = "EditorPlayData")]
public class EditorPlayData : ScriptableObject {

    public string DebugLoadSequanceName;

    public bool IsInvalidDebugLoad()
    {
        if (DebugLoadSequanceName == null)
        {
            return false;
        }
        if (DebugLoadSequanceName == "")
        {
            return false;
        }
        return true;
    }

    public void Reset()
    {
        DebugLoadSequanceName = null;
    }
}
