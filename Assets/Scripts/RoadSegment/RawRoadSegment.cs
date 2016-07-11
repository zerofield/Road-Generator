using UnityEngine;
using System.Collections;

public struct RawRoadSegment
{
    public enum Type
    {
        Straight,
        Corner,
        Intersection
    };

    public float length;
    public float width;

    public float pitch;
    public float yaw;
    public float roll;

    public float angle;
    public float radius;

    public Vector3 startPoint;

    public Type type;

    public RawRoadSegment[] next;
}
