﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 贝塞尔曲线路面
/// </summary>
public class BezierRoadSegment : BaseRoadSegment
{
    public BezierRoadSegment(RoadPoint pointA, float roadWidth) : base(pointA, roadWidth)
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