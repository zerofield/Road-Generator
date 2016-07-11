﻿using UnityEngine;
using System;
using System.Collections.Generic;

public enum SegmentType
{
    Straight,//直线
    Smooth,//过度
    Corner,//转弯
    Intersection//交叉路
};

public struct MeshData
{
    List<Vector3> vertices;
    List<int> triangle;
}

public interface IRoadSegment
{
    Vector3 GetPosition(float t, float offset);

    Vector3 GetNormal(float t);

    Vector3 GetTangent(float t);

    Quaternion GetRotation(float t);

    MeshData GenerateMesh(int subdivision, int baseIndex);

}

public abstract class SegmentNode : ICloneable, IRoadSegment
{
    #region Fields

    public SegmentType type;

    public Vector3 startPoint;
    public Vector3 endPoint;

    public float width;

    public SegmentNode parent;

    public SegmentNode[] children;

    #endregion

    #region Methods

    public SegmentNode(SegmentType type, float width, Vector3 startPoint)
    {
        this.type = type;
        this.width = width;
        this.startPoint = startPoint;

        if (type == SegmentType.Intersection)
        {
            children = new SegmentNode[3];
        }
        else
        {
            children = new SegmentNode[1];
        }
    }

    public virtual void AddNode(SegmentNode newNode)
    {
        AddNode(newNode, 0);
    }

    public virtual void AddNode(SegmentNode newNode, int index)
    {
        if (index < 0 || index >= children.Length)
        {
            throw new IndexOutOfRangeException();
        }

        children[index] = newNode;
        newNode.parent = this;
    }

    public virtual void InsertNode(SegmentNode newNode)
    {
        InsertNode(newNode, 0);
    }

    public virtual void InsertNode(SegmentNode newNode, int index)
    {
        if (index < 0 || index >= children.Length)
        {
            throw new IndexOutOfRangeException();
        }

        //new node
        SegmentNode otherNode = children[index];
        children[index] = newNode;
        newNode.parent = this;

        //other node
        newNode.AddNode(otherNode);
    }

    public virtual SegmentNode GetChild()
    {
        return GetChild(0);
    }

    public virtual SegmentNode GetChild(int index)
    {
        if (index < 0 || index >= children.Length)
        {
            throw new IndexOutOfRangeException();
        }
        return children[index];
    }

    public virtual Vector3 GetNormal(float t)
    {
        Vector3 tng = GetTangent(t);
        Vector3 binormal = Vector3.Cross(Vector3.up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    public virtual Quaternion GetRotation(float t)
    {
        t = Mathf.Clamp01(t);
        Vector3 tng = GetTangent(t);
        Vector3 nrm = GetNormal(t);
        return Quaternion.LookRotation(tng, nrm);
    }

    public virtual MeshData GenerateMesh(int subdivision, int baseIndex)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float halfWidth = width / 2;
        for (int i = 0; i <= subdivision; ++i)
        {
            float t = (float)i / subdivision;

            Vector3 left = GetPosition(t, -halfWidth);
            Vector3 right = GetPosition(t, halfWidth);

            vertices.Add(left);
            vertices.Add(right);
        }

        for (int i = 0; i < subdivision; ++i)
        {
            triangles.Add(baseIndex + 2 * (i + 1));
            triangles.Add(baseIndex + 2 * i + 1);
            triangles.Add(baseIndex + 2 * i);

            triangles.Add(baseIndex + 2 * (i + 1));
            triangles.Add(baseIndex + 2 * (i + 1) + 1);
            triangles.Add(baseIndex + 2 * i + 1);
        }
        MeshData meshData = new MeshData();

        return meshData;

    }

    public abstract object Clone();

    public abstract Vector3 GetPosition(float t, float offset);

    public abstract Vector3 GetTangent(float t);


    #endregion
}

public class StraightSegmentNode : SegmentNode
{
    #region Fields

    public float length;
    public float pitch;
    public float roll;
    public float yaw;

    private Quaternion rotation;

    #endregion

    #region Methods

    public StraightSegmentNode(SegmentType type, float width, Vector3 startPoint, float length, float pitch, float roll, float yaw) : base(type, width, startPoint)
    {
        this.length = length;
        this.pitch = pitch;
        this.roll = roll;
        this.yaw = yaw;
        this.rotation = Quaternion.Euler(pitch, yaw, roll);

        endPoint = startPoint + rotation * Vector3.forward * length;
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }

    public override MeshData GenerateMesh(int subdivision, int baseIndex)
    {
        return base.GenerateMesh(1, baseIndex);
    }

    public override Vector3 GetPosition(float t, float offset)
    {
        return Vector3.Lerp(startPoint, endPoint, t);
    }

    public override Quaternion GetRotation(float t)
    {
        return rotation;
    }

    public override Vector3 GetTangent(float t)
    {
        return (endPoint - startPoint).normalized;
    }

    #endregion
}

public class IntersectionSegmentNode : StraightSegmentNode
{
    #region Fields

    #endregion

    #region Methods
    public IntersectionSegmentNode(float width, Vector3 startPoint, float length, float pitch, float roll, float yaw) : base(SegmentType.Intersection, width, startPoint, length, pitch, roll, yaw)
    {
        //TODO 增加控制点等信息
    }
    #endregion
}

public class SmoothSegmentNode : SegmentNode
{
    #region 

    public Vector3 midPoint;
    public float startRoll;
    public float endRoll;

    private Vector3[] bezierPoints;

    #endregion

    #region Methods
    public SmoothSegmentNode(float width, Vector3 startPoint, Vector3 midPoint, Vector3 endPoint) : base(SegmentType.Smooth, width, startPoint)
    {
        this.midPoint = midPoint;
        this.endPoint = endPoint;

        bezierPoints = new Vector3[4];
        bezierPoints[0] = startPoint;
        bezierPoints[1] = midPoint;
        bezierPoints[2] = midPoint;
        bezierPoints[3] = endPoint;
    }

    public override Vector3 GetPosition(float t, float offset)
    {
        t = Mathf.Clamp01(t);
        Vector3 centerPoint = BezierCurve.GetPoint(bezierPoints, t);
        Quaternion rotation = GetRotation(t) * Quaternion.Euler(0, 0, getRoll(t));
        return centerPoint + rotation * Vector3.right * offset;
    }

    public float getRoll(float t)
    {
        t = Mathf.Clamp01(t);
        return Mathf.Lerp(startRoll, endRoll, t);
    }

    public override Vector3 GetTangent(float t)
    {
        return BezierCurve.GetTangent(bezierPoints, t);
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }
    #endregion
}

public class CornerSegmentNode : SegmentNode
{
    #region Fields

    public float pitch;
    public float roll;

    public float startYaw;
    public float endYaw;

    public float angle;
    public float radius;

    #endregion

    #region Methods

    public CornerSegmentNode(float width, Vector3 startPoint, float pitch, float yaw, float roll, float angle, float radius) : base(SegmentType.Corner, width, startPoint)
    {
        this.pitch = pitch;
        this.startYaw = yaw;
        this.roll = roll;
        this.angle = angle;
        this.radius = radius;

        //rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0); //roll 不进行旋转，roll只是用于决定三角形面的角度

        //calculate endPoint
        Vector3 direction = new Vector3(Mathf.Sign(angle), 0, 0);
        Vector3 circleCenter = startPoint + rotation * direction * radius;

        Vector3 upwards = rotation * Vector3.up;
        Vector3 downwards = rotation * Vector3.down;

        Quaternion endPointRotation = Quaternion.AngleAxis(angle, upwards);
        endPoint = circleCenter + endPointRotation * ((startPoint - circleCenter).normalized) * radius;


        Debug.DrawLine(startPoint, circleCenter, Color.red, 100);
        Debug.DrawLine(endPoint, circleCenter, Color.red, 100);

    }

    public override Vector3 GetPosition(float t, float offset)
    {
        throw new NotImplementedException();
    }

    public override Vector3 GetTangent(float t)
    {
        throw new NotImplementedException();
    }

    public override Quaternion GetRotation(float t)
    {
        return base.GetRotation(t) * Quaternion.Euler(0, 0, roll);
    }

    public override Vector3 GetNormal(float t)
    {
        return base.GetNormal(t);
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }

    #endregion
}