using UnityEngine;
using System.Collections;

public struct RoadPoint
{
    public Vector3 position;

    public float pitch;

    public float yaw;

    public float roll;

    public RoadPoint(Vector3 position, float pitch, float yaw, float roll)
    {
        this.position = position;
        this.pitch = pitch;
        this.yaw = yaw;
        this.roll = roll;
    }

}
