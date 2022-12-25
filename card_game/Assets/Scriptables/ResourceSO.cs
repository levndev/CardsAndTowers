using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSO : ScriptableObject
{
    public string UID;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(UID))
        {
            AssignNewUID();
        }
    }

    private void Reset()
    {
        AssignNewUID();
    }

    public void AssignNewUID()
    {
        Debug.Log($"sUSSY : {name}");
        UID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
