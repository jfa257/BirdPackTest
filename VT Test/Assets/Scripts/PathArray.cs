using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PathArray")]
public class PathArray : ScriptableObject
{
    public List<Vector3> targetsOnPath;

    private void OnEnable()
    {
        targetsOnPath = targetsOnPath ?? new List<Vector3>();
    }
}
