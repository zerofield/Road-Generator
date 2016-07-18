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

    float GetRoll(float t);

    Vector3 GetNormal(float t);

    Vector3 GetTangent(float t);

    Quaternion GetRotation(float t);

    MeshData GenerateMesh(int subdivision, int baseIndex);

    void ShrinkStartPoint(float percent);

    void ShrinkEndPoint(float percent);
}

public abstract class SegmentNode : ICloneable, IRoadSegment
{
    #region Fields

    public const int DEFAULT_CHILD_INDEX = 0;

    public SegmentType type;

    public Vector3 startPoint;
    public Vector3 endPoint;

    public float width;

    public float leftWidth;

    public float rightWidth;

    public float miu;

    public float yaw;

    public SegmentNode parent;

    public SegmentNode[] children;

    #endregion

    #region Methods

    public SegmentNode(SegmentType type, float width, float miu, Vector3 startPoint)
    {
        this.type = type;
        this.miu = miu;
        this.width = width;
        this.leftWidth = 0.5f * width;
        this.rightWidth = 0.5f * width;
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

        if (newNode != null)
        {
            newNode.parent = this;
        }
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

        for (int i = 0; i <= subdivision; ++i)
        {
            float t = (float)i / subdivision;

            Vector3 left = GetPosition(t, -GetLeftWidth());
            Vector3 right = GetPosition(t, GetRightWidth());

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

    public float GetLeftWidth()
    {
        return leftWidth;
    }

    public float GetRightWidth()
    {
        return rightWidth;
    }

    public abstract Vector3 GetPosition(float t, float offset);
    public abstract Vector3 GetTangent(float t);
    public abstract void ShrinkStartPoint(float percent);
    public abstract void ShrinkEndPoint(float percent);
    public abstract float GetRoll(float t);
    public abstract object Clone();

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

    public StraightSegmentNode(float width, float miu, Vector3 startPoint, float length, float pitch, float roll, float yaw) : this(SegmentType.Straight, width, miu, startPoint, length, pitch, roll, yaw)
    {

    }

    public StraightSegmentNode(SegmentType type, float width, float miu, Vector3 startPoint, float length, float pitch, float roll, float yaw) : base(type, width, miu, startPoint)
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
        SegmentNode node = new StraightSegmentNode(type, width, miu, startPoint, length, pitch, roll, yaw);
        return node;
    }

    public override void ShrinkStartPoint(float percent)
    {
        percent = Mathf.Clamp01(percent);
        startPoint = startPoint + (endPoint - startPoint).normalized * percent * length;
        length *= (1 - percent);
    }

    public override void ShrinkEndPoint(float percent)
    {
        percent = Mathf.Clamp01(percent);
        endPoint = endPoint - (endPoint - startPoint).normalized * percent * length;
        length *= (1 - percent);
    }

    public override float GetRoll(float t)
    {
        return roll;
    }

    #endregion
}

public class IntersectionSegmentNode : StraightSegmentNode
{
    #region Fields

    /// <summary>
    /// 中心点
    /// </summary>
    private Vector3 center;

    public Vector3 centerLeft;
    public Vector3 centerRight;

    public Quaternion centerLeftRotation;
    public Quaternion centerRightRotation;

    public const int INDEX_CENTER = 0;
    public const int INDEX_CENTER_LEFT = 1;
    public const int INDEX_CENTER_RIGHT = 2;

    #endregion

    #region Methods
    public IntersectionSegmentNode(float width, float miu, Vector3 startPoint, float length, float pitch, float roll, float yaw) : base(SegmentType.Intersection, width, miu, startPoint, length, pitch, roll, yaw)
    {
        center = (startPoint + endPoint) / 2;

        centerLeft = center + rotation * Vector3.left * leftWidth;
        centerRight = center + rotation * Vector3.right * rightWidth;

        centerLeftRotation = rotation * Quaternion.Euler(0, -90, 0);
        centerRightRotation = rotation * Quaternion.Euler(0, 90, 0);

    }

    /// <summary>
    /// 减小左侧边缘
    /// </summary>
    /// <param name="percent">百分比</param>
    public void ShrinkLeftPoint(float percent)
    {
        leftWidth *= percent;
        centerLeft = center + rotation * Vector3.left * leftWidth;
        width = leftWidth + rightWidth;
    }

    /// <summary>
    /// 减小右侧边缘
    /// </summary>
    /// <param name="percent">百分比</param>
    public void ShrinkRightPoint(float percent)
    {
        rightWidth *= percent;
        centerRight = center + rotation * Vector3.right * rightWidth;
        width = leftWidth + rightWidth;
    }

    public override object Clone()
    {
        SegmentNode node = new IntersectionSegmentNode(width, miu, startPoint, length, pitch, roll, yaw);
        return node;
    }

    #endregion
}

public class SmoothSegmentNode : SegmentNode
{
    #region 

    public Vector3 controlPoint1;
    public Vector3 controlPoint2;
    public float startRoll;
    public float endRoll;

    private Vector3[] bezierPoints;

    #endregion

    #region Methods
    public SmoothSegmentNode(float width, float miu, Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, float startRoll, float endRoll) : base(SegmentType.Smooth, width, miu, startPoint)
    {
        this.controlPoint1 = controlPoint1;
        this.controlPoint2 = controlPoint2;
        this.endPoint = endPoint;
        this.startRoll = startRoll;
        this.endRoll = endRoll;

        Quaternion rotation = Quaternion.LookRotation(endPoint - startPoint, Vector3.up);
        yaw = rotation.eulerAngles.y;

        bezierPoints = new Vector3[4];
        bezierPoints[0] = startPoint;
        bezierPoints[1] = this.controlPoint1;
        bezierPoints[2] = controlPoint2;
        bezierPoints[3] = endPoint;

        Debug.DrawLine(GetPosition(0, -width / 2), GetPosition(0, width / 2), Color.red, 30);
        Debug.DrawLine(GetPosition(1, -width / 2), GetPosition(1, width / 2), Color.red, 30);
    }

    public override Vector3 GetPosition(float t, float offset)
    {
        t = Mathf.Clamp01(t);
        Vector3 centerPoint = BezierCurve.GetPoint(bezierPoints, t);
        Quaternion rotation = GetRotation(t) * Quaternion.Euler(0, 0, GetRoll(t));
        return centerPoint + rotation * Vector3.right * offset;
    }

    public override float GetRoll(float t)
    {
        //cos 变化

        t = Mathf.Clamp01(t);
        float roll = 0f;

        float rad = t * Mathf.PI;

        float k = (startRoll - endRoll) / 2;
        float yOffset = (startRoll + endRoll) / 2;

        roll = k * Mathf.Cos(rad) + yOffset;

        return roll;
    }

    public override Vector3 GetTangent(float t)
    {
        return BezierCurve.GetTangent(bezierPoints, t);
    }

    public override object Clone()
    {
        SegmentNode node = new SmoothSegmentNode(width, miu, startPoint, controlPoint1, controlPoint2, endPoint, startRoll, endRoll);
        return node;
    }

    public override void ShrinkStartPoint(float percent)
    {
        //no-ops
    }

    public override void ShrinkEndPoint(float percent)
    {
        //no-ops
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

    public CornerSegmentNode(float width, float miu, Vector3 startPoint, float pitch, float yaw, float roll, float angle, float radius) : base(SegmentType.Corner, width, miu, startPoint)
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

        //debug 代码
        Debug.DrawLine(startPoint, startPoint + upwards * 5, Color.yellow, 30);
        Debug.DrawLine(endPoint, endPoint + upwards * 5, Color.yellow, 30);

        Debug.DrawLine(startPoint, circleCenterPoint, Color.black, 30);
        Debug.DrawLine(endPoint, circleCenterPoint, Color.black, 30);


        Debug.DrawLine(GetPosition(0, 0), GetPosition(0, 0) + GetNormal(0), Color.red, 30);
        Debug.DrawLine(GetPosition(1, 0), GetPosition(1, 0) + GetNormal(0), Color.magenta, 30);
    }

    public override Vector3 GetPosition(float t, float offset)
    {
        t = Mathf.Clamp01(t);
        Vector3 upwards = planeRotation * Vector3.up;
        Quaternion angleRotation = Quaternion.AngleAxis(t * angle, upwards);
        Vector3 pointPosition = circleCenterPoint + angleRotation * centerToStartPoint * radius;
        Vector3 position = pointPosition + GetRotation(t) * Vector3.right * offset;

        Debug.DrawLine(position, position + GetNormal(t), Color.magenta, 30);
        return position;
    }

    public override Vector3 GetTangent(float t)
    {
        Quaternion angleRotation = Quaternion.AngleAxis(t * angle, upwards);
        Vector3 pointUpwards = (planeRotation * angleRotation * Quaternion.Euler(0, 0, roll) * Vector3.up).normalized;
        Vector3 pointPosition = circleCenterPoint + angleRotation * centerToStartPoint * radius;
        Vector3 centerToPoint = pointPosition - circleCenterPoint;

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
        Vector3 pointUpwards = (planeRotation * angleRotation * Quaternion.Euler(0, 0, roll) * Vector3.up).normalized;

        return pointUpwards;
    }

    public override object Clone()
    {
        SegmentNode node = new CornerSegmentNode(width, miu, startPoint, pitch, startYaw, roll, angle, radius);
        return node;
    }

    public override void ShrinkStartPoint(float percent)
    {
        percent = Mathf.Clamp01(percent);
        Vector3 newStartPoint = GetPosition(percent, 0);
        Quaternion rotation = GetRotation(percent);

        startPoint = newStartPoint;
        //
        yaw = startYaw = rotation.eulerAngles.y;
        planeRotation = Quaternion.Euler(pitch, yaw, 0);
        circleCenterPoint = startPoint + planeRotation * direction * radius;
        centerToStartPoint = (startPoint - circleCenterPoint).normalized;
        upwards = planeRotation * Vector3.up;
        downwards = planeRotation * Vector3.down;
        //
        angle *= (1 - percent);

        Debug.DrawLine(startPoint, circleCenterPoint, Color.gray, 30);
    }

    public override void ShrinkEndPoint(float percent)
    {
        percent = Mathf.Clamp01(percent);
        Vector3 newEndPoint = GetPosition(1 - percent, 0);
        Quaternion rotation = GetRotation(1 - percent);

        //
        endPoint = newEndPoint;
        endYaw = rotation.eulerAngles.y;
        angle *= (1 - percent);

        Debug.DrawLine(endPoint, circleCenterPoint, Color.gray, 30);
    }

    public override float GetRoll(float t)
    {
        float r = GetRotation(t).eulerAngles.z;

        if (r > 180)
        {
            return r - 360;
        }
        else
        {
            return r;
        }
    }

    #endregion
}
