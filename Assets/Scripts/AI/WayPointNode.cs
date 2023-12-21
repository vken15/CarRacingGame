using UnityEngine;

public class WayPointNode : MonoBehaviour
{
    //[Header("Change speed when the AI reach the waypoint")]
    //public float maxSpeed = 0;

    [Header("Waypoint")]
    public float minDistanceToReachWayPoint = 5;
    public WayPointNode[] nextWayPointNode;
}
