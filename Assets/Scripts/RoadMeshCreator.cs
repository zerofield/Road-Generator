
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 路面网格生成器
/// </summary>
public class RoadMeshCreator : MonoBehaviour
{

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private List<BaseRoadSegment> rawSegments = new List<BaseRoadSegment>();

    public int RawSegmentsCount
    {
        get
        {
            return rawSegments.Count;
        }
    }

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void AddSegment(BaseRoadSegment segment)
    {
        if (segment != null)
        {
            rawSegments.Add(segment);
        }
    }

    public void RemoveLastSegment()
    {
        if (rawSegments.Count > 0)
        {
            rawSegments.RemoveAt(rawSegments.Count - 1);
        }
    }

    /// <summary>
    /// 获得最后一个路段
    /// </summary>
    /// <returns></returns>
    public BaseRoadSegment LastRoadSegment()
    {
        if (rawSegments.Count == 0)
        {
            return null;
        }
        return rawSegments[rawSegments.Count - 1];
    }


    /// <summary>
    /// 生成路面网格模型
    /// </summary>
    /// <param name="smoothPercent">路段平滑百分比，大于等于0小于0.5</param>
    /// <param name="subdivision">表面分段数，大于等于2</param>
    /// <param name="smooth">是否平滑</param>
    public void GenerateMesh(float smoothPercent, int subdivision, bool smooth)
    {
        smoothPercent = Mathf.Clamp(smoothPercent, 0, 0.5f);
        subdivision = Mathf.Clamp(subdivision, 2, int.MaxValue);

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        List<BaseRoadSegment> segments = rawSegments;
        if (smooth)
        {
            //平滑路段，使得路面连续
            segments = smoothSegments(rawSegments);
        }

        if (segments != null)
        {
            foreach (BaseRoadSegment segment in segments)
            {
                int baseIndex = vertices.Count;
                BaseRoadSegment.MeshData meshData = segment.GenerateMesh(subdivision, baseIndex);
                vertices.AddRange(meshData.vertices);
                triangles.AddRange(meshData.triangles);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        meshFilter.mesh = mesh;
    }

    /// <summary>
    /// 平滑路段
    /// </summary>
    /// <returns></returns>
    private List<BaseRoadSegment> smoothSegments(List<BaseRoadSegment> rawSegments)
    {
        //TODO 

        List<BaseRoadSegment> newSegments = new List<BaseRoadSegment>();

        for (int i = 0; i < rawSegments.Count - 1; ++i)
        {
            BaseRoadSegment segment1 = rawSegments[i];
            BaseRoadSegment segment2 = rawSegments[i + 1];

           


        }


        return newSegments;
    }


}
