using UnityEngine;
using System.Collections;

public abstract class BaseRoadSegment
{
    public RoadPoint PointA
    {
        get;
        private set;
    }

    public RoadPoint PointB
    {
        get;
        private set;
    }

    public float RoadWidth
    {
        get;
        private set;
    }

    public float RoadLength
    {
        get;
        private set;
    }

    public BaseRoadSegment(RoadPoint pointA, float roadWidth)
    {
        PointA = pointA;
    }

    /// <summary>
    /// 
    /// </summary>
    protected abstract void UpdateValues();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="subdivision"></param>
    protected abstract void GenerateMesh(int subdivision);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public abstract float getY(float x, float z);


}
