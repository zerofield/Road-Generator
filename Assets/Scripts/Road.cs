using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road
{
    private List<BaseRoadSegment> segments = new List<BaseRoadSegment>();

    public List<BaseRoadSegment> Segments
    {
        get { return segments; }
    }
}
