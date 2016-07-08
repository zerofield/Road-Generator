using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        rotation = Quaternion.Euler(pitch, yaw, 0); //roll 不进行旋转，roll只是用于决定三角形表面角度

        //calculate point B
        Vector3 direction = new Vector3(Mathf.Sign(angle), 0, 0);
        Vector3 circleCenter = pointA.position + rotation * direction * radius;

        //rotate pointA around circleCenter to pointB 
        Vector3 upwards = rotation * Vector3.up;
        Vector3 downwards = rotation * Vector3.down;

        Quaternion pointBRotation = Quaternion.AngleAxis(angle, upwards);
        Vector3 bPosition = circleCenter + pointBRotation * ((pointA.position - circleCenter).normalized) * radius;

        //
        Vector3 centerToB = bPosition - circleCenter;
        Vector3 bForward;

        if (angle < 0)
        {
            bForward = Vector3.Cross(downwards, centerToB);
        }
        else
        {
            bForward = Vector3.Cross(upwards, centerToB);
        }

        Quaternion bRotation = Quaternion.LookRotation(bForward, upwards);

        float bPitch = bRotation.eulerAngles.x;
        float bYaw = bRotation.eulerAngles.y;
        float bRoll = bRotation.eulerAngles.z;

        pointB = new RoadPoint(bPosition, bPitch, bYaw, bRoll);
        //length
        roadLength = 2 * Mathf.PI * radius * Mathf.Abs(angle) / 360;

        Debug.DrawLine(pointA.position, circleCenter, Color.red, 100);
        Debug.DrawLine(pointB.position, circleCenter, Color.red, 100);
        Debug.Log("Corner segment PointB  pos: " + bPosition + ", pitch: " + bPitch + ", yaw: " + bYaw + ", roll: " + bRoll);
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
        //helper variables
        Vector3 direction = new Vector3(Mathf.Sign(angle), 0, 0);
        Vector3 circleCenter = pointA.position + rotation * direction * radius;
        Vector3 upwards = rotation * Vector3.up;
        Vector3 downwards = rotation * Vector3.down;
        //
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float halfWidth = roadWidth / 2;

        float angleIncrement = angle / subdivision;

        //add vertices
        for (int i = 0; i <= subdivision; ++i)
        {
            Quaternion angleRotation = Quaternion.AngleAxis(i * angleIncrement, upwards);
            Vector3 position = circleCenter + angleRotation * ((pointA.position - circleCenter).normalized) * radius;

            Vector3 pointUpwards = angleRotation * Quaternion.Euler(0, 0, roll) * Vector3.up;
            Vector3 pointDownwards = angleRotation * Quaternion.Euler(0, 0, roll) * Vector3.down;

            Vector3 pointForward;
            if (angle < 0)
            {
                pointForward = Vector3.Cross(pointDownwards, position - circleCenter);
            }
            else
            {
                pointForward = Vector3.Cross(pointUpwards, position - circleCenter);
            }

            Quaternion pointRotation = Quaternion.LookRotation(pointForward, pointUpwards);
            Vector3 left = position + pointRotation * Vector3.left * halfWidth;
            Vector3 right = position + pointRotation * Vector3.right * halfWidth;

            vertices.Add(left);
            vertices.Add(right);
        }

        //add triangles
        for (int i = 0; i < subdivision; ++i)
        {
            //triangle left
            triangles.Add(baseIndex + (2 * (i + 1)));
            triangles.Add(baseIndex + (2 * i + 1));
            triangles.Add(baseIndex + (2 * i));

            //triangles right
            triangles.Add(baseIndex + (2 * (i + 1)));
            triangles.Add(baseIndex + (2 * (i + 1) + 1));
            triangles.Add(baseIndex + (2 * i + 1));
        }

        MeshData data = new MeshData();
        data.vertices = vertices;
        data.triangles = triangles;

        return data;
    }
}


/*
version 1

    using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

        //calculate point B
        Vector3 direction = new Vector3(Mathf.Sign(angle), 0, 0);
        Vector3 circleCenter = pointA.position + rotation * direction * radius;

        //rotate pointA around circleCenter to pointB 
        Vector3 upwards = rotation * Vector3.up;
        Vector3 downwards = rotation * Vector3.down;



        Quaternion pointBRotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 bPosition = circleCenter + pointBRotation * ((pointA.position - circleCenter).normalized) * radius;

        //
        Vector3 centerToB = bPosition - circleCenter;
        Vector3 bForward;

        if (angle < 0)
        {
            bForward = Vector3.Cross(downwards, centerToB);
        }
        else
        {
            bForward = Vector3.Cross(upwards, centerToB);
        }

        Quaternion bRotation = Quaternion.LookRotation(bForward, upwards);

        float bPitch = bRotation.eulerAngles.x;
        float bYaw = bRotation.eulerAngles.y;
        float bRoll = bRotation.eulerAngles.z;

        pointB = new RoadPoint(bPosition, bPitch, bYaw, bRoll);
        //length
        roadLength = 2 * Mathf.PI * radius * Mathf.Abs(angle) / 360;

        Debug.DrawLine(pointA.position, circleCenter, Color.red, 100);
        Debug.DrawLine(pointB.position, circleCenter, Color.red, 100);
        Debug.Log("Corner segment PointB  pos: " + bPosition + ", pitch: " + bPitch + ", yaw: " + bYaw + ", roll: " + bRoll);
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
        //helper variables
        Vector3 direction = new Vector3(Mathf.Sign(angle), 0, 0);
        Vector3 circleCenter = pointA.position + rotation * direction * radius;
        Vector3 upwards = rotation * Vector3.up;
        Vector3 downwards = rotation * Vector3.down;
        //
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float halfWidth = roadWidth / 2;

        float angleIncrement = angle / subdivision;

        //add vertices
        for (int i = 0; i <= subdivision; ++i)
        {
            Quaternion angleRotation = Quaternion.AngleAxis(i * angleIncrement, upwards);
            Vector3 position = circleCenter + angleRotation * ((pointA.position - circleCenter).normalized) * radius;

            Vector3 pointForward;
            if (angle < 0)
            {
                pointForward = Vector3.Cross(downwards, position - circleCenter);
            }
            else
            {
                pointForward = Vector3.Cross(upwards, position - circleCenter);
            }

            Quaternion pointRotation = Quaternion.LookRotation(pointForward, upwards);
            Vector3 left = position + pointRotation * Vector3.left * halfWidth;
            Vector3 right = position + pointRotation * Vector3.right * halfWidth;

            vertices.Add(left);
            vertices.Add(right);
        }

        //add triangles
        for (int i = 0; i < subdivision; ++i)
        {
            //triangle left
            triangles.Add(baseIndex + (2 * (i + 1)));
            triangles.Add(baseIndex + (2 * i + 1));
            triangles.Add(baseIndex + (2 * i));

            //triangles right
            triangles.Add(baseIndex + (2 * (i + 1)));
            triangles.Add(baseIndex + (2 * (i + 1) + 1));
            triangles.Add(baseIndex + (2 * i + 1));
        }

        MeshData data = new MeshData();
        data.vertices = vertices;
        data.triangles = triangles;

        return data;
    }
}


    */
