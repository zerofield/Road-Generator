using UnityEngine;
using System.Collections;
using System;

public class CornerRoadSegment : BaseRoadSegment
{

    public float Radius
    {
        get;
        private set;
    }

    public float Angle
    {
        get;
        private set;
    }

    public CornerRoadSegment(RoadPoint pointA, float roadWidth, float radius, float angle) : base(pointA, roadWidth)
    {

    }

    protected override void UpdateValues()
    {
        throw new NotImplementedException();
    }

    protected override void GenerateMesh(int subdivision)
    {
        throw new NotImplementedException();
    }

    public override float getY(float x, float z)
    {
        throw new NotImplementedException();
    }
}
