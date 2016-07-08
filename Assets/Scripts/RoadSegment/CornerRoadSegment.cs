using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 转角路段
/// </summary>
public class CornerRoadSegment : BaseRoadSegment
{
    public float yaw;

    public float pitch;

    public float roll;

    public Quaternion rotation;


    public float radius;

    public float angle;

    public CornerRoadSegment(RoadPoint pointA, float roadWidth, float pitch, float roll, float radius, float angle) : base(pointA, roadWidth)
    {

        this.pitch = pitch;
        this.yaw = pointA.yaw;
        this.roll = roll;
        this.radius = radius;
        this.angle = angle;

        //point A
        base.pointA.pitch = pitch;
        base.pointA.roll = roll;

        //rotation
        rotation = Quaternion.Euler(pitch, yaw, roll);

        //point B
        Vector3 direction = new Vector3(Mathf.Sign(angle), 0, 0);
        Vector3 circleCenter = pointA.position + rotation * direction * radius;

        //rotate pointA around circleCenter to pointB 
        Vector3 up = rotation * Vector3.up;
        Quaternion pointBRotation = Quaternion.AngleAxis(angle, up);
        Vector3 pointBPosition = circleCenter + pointBRotation * Vector3.forward * radius;

        //pointB = new RoadPoint(pointBPosition, )


        //length
    }

    protected override void UpdateValues()
    {
        throw new NotImplementedException();
    }

    public override float getY(float x, float z)
    {
        throw new NotImplementedException();
    }

    public override MeshData GenerateMesh(int subdivision, int baseIndex)
    {
        throw new NotImplementedException();
    }
}
