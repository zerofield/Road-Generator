using UnityEngine;
using System.Collections;
using System;

public class CurvyRoadSegment : BaseRoadSegment
{
    public CurvyRoadSegment(RoadPoint pointA, float roadWidth) : base(pointA, roadWidth)
    {

    }

    public override float getY(float x, float z)
    {
        throw new NotImplementedException();
    }

    protected override void GenerateMesh(int subdivision)
    {
        throw new NotImplementedException();
    }

    protected override void UpdateValues()
    {
        throw new NotImplementedException();
    }
}
