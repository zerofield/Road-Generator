using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road
{
    private List<RoadSegment> segments = new List<RoadSegment>();

    public List<RoadSegment> Segments
    {
        get { return segments; }
    }

    public void DrawSkeleton()
    {
        for (int i = 0; i < segments.Count; ++i)
        {
            segments[i].DrawSkeleton();
        }

    }
}
