using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 贝塞尔曲线路面
/// </summary>
public class BezierRoadSegment : BaseRoadSegment
{
    //控制点
    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    public BezierRoadSegment(RoadPoint pointA, RoadPoint pointB, float roadWidth) : base(pointA, roadWidth)
    {

    }

    public override MeshData GenerateMesh(int subdivision, int baseIndex)
    {
        throw new NotImplementedException();
    }

    public override float getY(float x, float z)
    {
        throw new NotImplementedException();
    }

    protected override void UpdateValues()
    {
        throw new NotImplementedException();
    }
}
