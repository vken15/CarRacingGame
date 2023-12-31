﻿using UnityEngine;

#if UNITY_EDITOR
#endif

public class DrawPathHandler : MonoBehaviour
{
    [SerializeField] private Transform transformRootObject;
    private WayPointNode[] waypointNodes;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (transformRootObject == null)
            return;
        //Get all Waypoints
        waypointNodes = transformRootObject.GetComponentsInChildren<WayPointNode>();
        //Iterate the list
        foreach (WayPointNode waypoint in waypointNodes)
        {
            foreach (WayPointNode nextWayPoint in waypoint.nextWayPointNode)
            {
                if (nextWayPoint != null)
                    Gizmos.DrawLine(waypoint.transform.position, nextWayPoint.transform.position);
            }
        }
    }

}
