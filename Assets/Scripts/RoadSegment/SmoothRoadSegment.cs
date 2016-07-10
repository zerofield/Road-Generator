using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 平滑过渡路段，可以对直线段进行Pitch和Roll平滑过渡
/// </summary>
public class SmoothRoadSegment
{
    public enum SmoothType
    {
        None,//无过渡
        Bezier,//贝塞尔过渡
        Roll//Roll过渡
    };

    public SmoothType type;

    /// <summary>
    /// 路面宽度
    /// </summary>
    public float roadWidth;

    /// <summary>
    /// 起始点
    /// </summary>
    public RoadPoint startPoint;

    /// <summary>
    /// 中间点
    /// </summary>
    public RoadPoint midPoint;

    /// <summary>
    /// 结束点
    /// </summary>
    public RoadPoint endPoint;

    /// <summary>
    /// 贝塞尔曲线需要的点数组
    /// </summary>
    private Vector3[] bezierPoints;


    public SmoothRoadSegment(RoadPoint startPoint, RoadPoint midPoint, RoadPoint endPoint, float roadWidth)
    {

        //startPoint, midPoint, endPoint 可以组成一个垂直于XZ平面的平面
        //startPoint到midPiont以及midPoint到endPoint是在这条平面上的

        this.startPoint = startPoint;
        this.midPoint = midPoint;
        this.endPoint = endPoint;
        this.roadWidth = roadWidth;



        //yaw必须相同, 否则抛出异常
        if (startPoint.yaw != endPoint.yaw || startPoint.yaw != midPoint.yaw)
        {
            throw new System.Exception();
        }

        //方向不同说明需要用贝塞尔过渡
        if ((midPoint.position - startPoint.position).normalized != (midPoint.position - endPoint.position))
        {
            type = SmoothType.Bezier;
        }
        else
        {
            if (startPoint.roll != endPoint.roll)
            {
                type = SmoothType.Roll;
            }
            else
            {
                type = SmoothType.None;
            }
        }

        //贝塞尔曲线点
        bezierPoints = new Vector3[4];
        bezierPoints[0] = startPoint.position;
        bezierPoints[1] = midPoint.position;
        bezierPoints[2] = midPoint.position;
        bezierPoints[3] = endPoint.position;

    }

    public float getRoll(float t)
    {
        t = Mathf.Clamp01(t);
        return Mathf.Lerp(startPoint.roll, endPoint.roll, t);
    }

    public Vector3 GetPosition(float t)
    {
        t = Mathf.Clamp01(t);
        if (type == SmoothType.None || type == SmoothType.Roll)
        {
            return Vector3.Lerp(startPoint.position, endPoint.position, t);
        }
        else
        {
            return BezierCurve.GetPoint(bezierPoints, t);
        }
    }

    /// <summary>
    /// 计算斜率
    /// </summary>
    /// <param name="t">t</param>
    /// <returns></returns>
    public Vector3 GetTangent(float t)
    {
        t = Mathf.Clamp01(t);
        if (type == SmoothType.None || type == SmoothType.Roll)
        {
            return (endPoint.position - startPoint.position).normalized;
        }
        else
        {
            return BezierCurve.GetTangent(bezierPoints, t);
        }
    }

    public Vector3 GetNormal(float t)
    {
        Vector3 tng = GetTangent(t);
        Vector3 binormal = Vector3.Cross(Vector3.up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    public Quaternion GetRotation(float t)
    {
        t = Mathf.Clamp01(t);
        Vector3 tng = GetTangent(t);
        Vector3 nrm = GetNormal(t);
        return Quaternion.LookRotation(tng, nrm);
    }

    public Vector3 GetLeftPoint(float t, float offset)
    {
        t = Mathf.Clamp01(t);
        Vector3 center = GetPosition(t);
        Quaternion rotation = GetRotation(t) * Quaternion.Euler(0, 0, getRoll(t));
        return center + rotation * Vector3.left * roadWidth * offset;
    }

    public Vector3 GetRightPoint(float t, float offset)
    {
        t = Mathf.Clamp01(t);
        Vector3 center = GetPosition(t);
        Quaternion rotation = GetRotation(t) * Quaternion.Euler(0, 0, getRoll(t));
        return center + rotation * Vector3.right * roadWidth * offset;
    }

    public void GenerateMesh(int subdivision, List<Vector3> vertices, List<int> triangles, int baseIndex)
    {

        float halfWidth = roadWidth / 2;
        for (int i = 0; i <= subdivision; ++i)
        {
            float t = (float)i / subdivision;

            Vector3 left = GetLeftPoint(t, halfWidth);
            Vector3 right = GetRightPoint(t, halfWidth);

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

    }

}
