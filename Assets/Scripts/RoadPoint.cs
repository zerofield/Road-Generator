using UnityEngine;
using System.Collections;

public class RoadPoint
{
    public Vector3 point;
    public RoadSegment segment;

    public RoadPoint(Vector3 point, RoadSegment segment)
    {
        this.point = point;
        this.segment = segment;
    }
}
