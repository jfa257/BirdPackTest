using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PathArray")]
public class PathArray : ScriptableObject
{
    public List<Vector3> targetsOnPath;
    public bool updatePositionData = false;

    private void OnEnable()
    {
        targetsOnPath = targetsOnPath ?? new List<Vector3>();
        if (updatePositionData)
        {

        }

    }
}
