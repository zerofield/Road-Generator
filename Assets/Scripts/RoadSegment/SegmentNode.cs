using UnityEngine;
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
    public List<Vector3> vertices;
    public List<int> triangle;
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

    public const int DEFAULT_CHILD_INDEX = 0;

    public SegmentType type;

    public Vector3 startPoint;
    public Vector3 endPoint;

    public float width;
    public float yaw;

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

    /// <summary>
    /// 移除节点并返回移除的结点索引
    /// </summary>
    /// <param name="node">节点</param>
    /// <returns>该节点对应的索引</returns>
    public int RemoveChild(SegmentNode node)
    {

        int index = -1;
        for (int i = 0; i < children.Length; ++i)
        {
            if (children[i] == node)
            {
                children[i].parent = null;
                children[i] = null;
                index = i;
                break;
            }
        }
        return index;
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
        meshData.vertices = vertices;
        meshData.triangle = triangles;
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

    protected Quaternion rotation;

    #endregion

    #region Methods

    public StraightSegmentNode(float width, Vector3 startPoint, float length, float pitch, float roll, float yaw) : this(SegmentType.Straight, width, startPoint, length, pitch, roll, yaw)
    {

    }

    public StraightSegmentNode(SegmentType type, float width, Vector3 startPoint, float length, float pitch, float roll, float yaw) : base(type, width, startPoint)
    {
        this.length = length;
        this.pitch = pitch;
        this.roll = roll;
        this.yaw = yaw;
        this.rotation = Quaternion.Euler(pitch, yaw, roll);

        endPoint = startPoint + rotation * Vector3.forward * length;
    }


    public override MeshData GenerateMesh(int subdivision, int baseIndex)
    {
        return base.GenerateMesh(1, baseIndex);
    }

    public override Vector3 GetPosition(float t, float offset)
    {
        return Vector3.Lerp(startPoint, endPoint, t) + GetRotation(t) * Vector3.right * offset;
    }

    public override Quaternion GetRotation(float t)
    {
        return rotation;
    }

    public override Vector3 GetTangent(float t)
    {
        return (endPoint - startPoint).normalized;
    }

    public override object Clone()
    {
        SegmentNode node = new StraightSegmentNode(type, width, startPoint, length, pitch, roll, yaw);
        //复制父子节点
        node.parent = parent;
        children.CopyTo(node.children, 0);
        return node;
    }

    public virtual void ShrinkStartPoint(float percent)
    {
        percent = Mathf.Clamp01(percent);
        startPoint = startPoint + (endPoint - startPoint).normalized * (1 - percent) * length;
        length *= (1 - percent);
    }

    public virtual void ShrinkEndPoint(float percent)
    {
        percent = Mathf.Clamp01(percent);
        endPoint = endPoint - (endPoint - startPoint).normalized * (1 - percent) * length;
        length *= (1 - percent);
    }

    #endregion
}

public class IntersectionSegmentNode : StraightSegmentNode
{
    #region Fields

    public Vector3 centerLeft;

    public Vector3 centerRight;

    public Quaternion centerLeftRotation;
    public Quaternion centerRightRotation;

    public const int INDEX_CENTER = 0;
    public const int INDEX_CENTER_LEFT = 1;
    public const int INDEX_CENTER_RIGHT = 2;

    #endregion

    #region Methods
    public IntersectionSegmentNode(float width, Vector3 startPoint, float length, float pitch, float roll, float yaw) : base(SegmentType.Intersection, width, startPoint, length, pitch, roll, yaw)
    {
        //TODO 增加控制点等信息
        Vector3 center = (startPoint + endPoint) / 2;
        centerLeft = center + rotation * Vector3.left * width / 2;
        centerRight = center + rotation * Vector3.right * width / 2;

        centerLeftRotation = rotation * Quaternion.Euler(0, -90, 0);
        centerRightRotation = rotation * Quaternion.Euler(0, 90, 0);

    }

    public override object Clone()
    {
        SegmentNode node = new IntersectionSegmentNode(width, startPoint, length, pitch, roll, yaw);
        //复制父子节点
        node.parent = parent;
        children.CopyTo(node.children, 0);
        return node;
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

        Quaternion rotation = Quaternion.LookRotation(endPoint - startPoint, Vector3.up);
        yaw = rotation.eulerAngles.y;

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
        SegmentNode node = new SmoothSegmentNode(width, startPoint, midPoint, endPoint);
        //复制父子节点
        node.parent = parent;
        children.CopyTo(node.children, 0);
        return node;
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


    //计算相关变量
    private Quaternion planeRotation;
    private Vector3 upwards;
    private Vector3 downwards;

    private Vector3 direction;

    /// <summary>
    /// 圆心
    /// </summary>
    private Vector3 circleCenterPoint;

    /// <summary>
    /// 圆心到起始点的矢量 
    /// </summary>
    private Vector3 centerToStartPoint;
    #endregion

    #region Methods

    public CornerSegmentNode(float width, Vector3 startPoint, float pitch, float yaw, float roll, float angle, float radius) : base(SegmentType.Corner, width, startPoint)
    {
        this.pitch = pitch;
        this.yaw = yaw;
        this.startYaw = yaw;
        this.roll = roll;
        this.angle = angle;
        this.radius = radius;

        //计算常用变量
        planeRotation = Quaternion.Euler(pitch, yaw, 0); //roll 不进行旋转，roll只是用于决定三角形面的角度
        upwards = planeRotation * Vector3.up;
        downwards = planeRotation * Vector3.down;
        direction = new Vector3(Mathf.Sign(angle), 0, 0);
        circleCenterPoint = startPoint + planeRotation * direction * radius;
        centerToStartPoint = (startPoint - circleCenterPoint).normalized;
        //
        endPoint = GetPosition(1.0f, 0);
        Quaternion endRotation = GetRotation(1.0f);
        endYaw = endRotation.eulerAngles.y;

        Debug.DrawLine(startPoint, circleCenterPoint, Color.red, 30);
        Debug.DrawLine(endPoint, circleCenterPoint, Color.red, 30);

    }

    public override Vector3 GetPosition(float t, float offset)
    {
        t = Mathf.Clamp01(t);
        Vector3 upwards = planeRotation * Vector3.up;
        Quaternion angleRotation = Quaternion.AngleAxis(t * angle, upwards);
        Vector3 pointPosition = circleCenterPoint + angleRotation * centerToStartPoint * radius;
        Vector3 position = pointPosition + GetRotation(t) * Vector3.right * offset;
        return position;
    }

    public override Vector3 GetTangent(float t)
    {
        Quaternion angleRotation = Quaternion.AngleAxis(t * angle, upwards);
        Vector3 pointPosition = circleCenterPoint + angleRotation * centerToStartPoint * radius;
        Vector3 centerToPoint = pointPosition - circleCenterPoint;
        Vector3 pointUpwards = angleRotation * Quaternion.Euler(0, 0, roll) * Vector3.up;
        if (angle < 0)
        {
            return Vector3.Cross(centerToPoint, pointUpwards).normalized;
        }
        else
        {
            return Vector3.Cross(pointUpwards, centerToPoint).normalized;
        }
    }

    public override Vector3 GetNormal(float t)
    {
        t = Mathf.Clamp01(t);
        Quaternion angleRotation = Quaternion.AngleAxis(t * angle, upwards);
        Vector3 pointUpwards = (angleRotation * Quaternion.Euler(0, 0, roll) * Vector3.up).normalized;
        return pointUpwards;
    }

    public override object Clone()
    {
        SegmentNode node = new CornerSegmentNode(width, startPoint, pitch, startYaw, roll, angle, radius);
        //复制父子节点
        node.parent = parent;
        children.CopyTo(node.children, 0);
        return node;
    }

    #endregion
}
