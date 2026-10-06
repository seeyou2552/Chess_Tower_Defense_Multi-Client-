using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathData : MonoBehaviour
{
    public List<Path> Paths = new List<Path>();

    void Start()
    {
        PathManager.Instance.Paths = Paths;
    }


}

[System.Serializable]
public class Path
{
    public List<Transform> Waypoints;
}