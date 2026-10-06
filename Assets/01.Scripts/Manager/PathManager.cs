using System;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : Singleton<PathManager>
{
    public List<Path> Paths;

    public Transform GetPoint(int routeIndex, int waypointIndex)
    {
        if(waypointIndex >= Paths[routeIndex].Waypoints.Count)
        {
            return null;
        }
        return Paths[routeIndex].Waypoints[waypointIndex];
    }
}