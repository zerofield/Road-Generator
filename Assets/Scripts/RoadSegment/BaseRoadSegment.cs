using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseRoadSegment
{
    /// <summary>
    /// 生成的模型数据格式
    /// </summary>
    public struct MeshData
    {
        public List<Vector3> vertices;
        public List<int> triangles;
    };

    public RoadPoint pointA;

    public RoadPoint pointB;

    public float roadWidth;

    public float roadLength;

    public BaseRoadSegment(RoadPoint pointA, float roadWidth)
    {
        this.pointA = pointA;
        this.roadWidth = roadWidth;
    }

    /// <summary>
    /// 更新状态变量
    /// </summary>
    protected abstract void UpdateValues();

    /// <summary>
    /// 生成网格
    /// </summary>
    /// <param name="subdivision">细分数量</param>
    /// <param name="baseIndex">顶点索引的基地址,用来偏移三角形的顶点索引</param>
    public abstract MeshData GenerateMesh(int subdivision, int baseIndex);

    /// <summary>
    /// 根据x,z坐标获得y
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="z">z坐标</param>
    /// <returns></returns>
    public abstract float getY(float x, float z);


}
