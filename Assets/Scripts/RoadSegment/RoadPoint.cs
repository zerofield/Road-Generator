using UnityEngine;
using System.Collections;

public class RoadPoint
{
    public Vector3 Position
    {
        get;
        private set;
    }

    public float Pitch
    {
        get;
        private set;
    }

    public float Yaw
    {
        get;
        private set;
    }

    public float Roll
    {
        get;
        private set;
    }

    public RoadPoint(Vector3 position, float pitch, float yaw, float roll)
    {
        Position = position;
        Pitch = pitch;
        Yaw = yaw;
        Roll = roll;
    }

}
