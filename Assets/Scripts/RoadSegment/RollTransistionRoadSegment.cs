﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Roll过渡路段
/// </summary>
public class RollTransistionRoadSegment : BaseRoadSegment
{
    public RollTransistionRoadSegment(RoadPoint pointA, float roadWidth) : base(pointA, roadWidth)
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