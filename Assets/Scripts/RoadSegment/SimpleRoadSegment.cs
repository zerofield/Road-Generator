using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 平面路段
/// </summary>
public class SimpleRoadSegment : BaseRoadSegment
{
    public float yaw;

    public float pitch;

    public float roll;

    public Quaternion rotation;

    public SimpleRoadSegment(RoadPoint pointA, float roadWidth, float roadLength, float pitch, float roll) : base(pointA, roadWidth)
    {

        this.pitch = pitch;
        this.yaw = pointA.yaw;
        this.roll = roll;

        base.roadLength = roadLength;
        base.pointA.pitch = this.pitch;
        base.pointA.roll = roll;

        rotation = Quaternion.Euler(this.pitch, yaw, this.roll);

        Vector3 pointBPos = pointA.position + rotation * Vector3.forward * roadLength;

        pointB = new RoadPoint(pointBPos, pitch, yaw, roll);
    }

    public override MeshData GenerateMesh(int subdivision, int baseIndex)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float halfWidth = roadWidth / 2;

        Vector3 ll = pointA.position + rotation * Vector3.left * halfWidth;
        Vector3 lr = pointA.position + rotation * Vector3.right * halfWidth;

        Vector3 ul = pointB.position + rotation * Vector3.left * halfWidth;
        Vector3 ur = pointB.position + rotation * Vector3.right * halfWidth;

        vertices.Add(ul);
        vertices.Add(ll);
        vertices.Add(lr);
        vertices.Add(ur);

        triangles.Add(baseIndex + 0);
        triangles.Add(baseIndex + 2);
        triangles.Add(baseIndex + 1);

        triangles.Add(baseIndex + 0);
        triangles.Add(baseIndex + 3);
        triangles.Add(baseIndex + 2);

        MeshData data = new MeshData();
        data.vertices = vertices;
        data.triangles = triangles;
        return data;
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
