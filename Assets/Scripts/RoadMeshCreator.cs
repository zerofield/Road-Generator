
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 路面网格生成器
/// </summary>

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadMeshCreator : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public SegmentNode StartNode { get; set; }

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<SegmentNode> GetEndPointSegments()
    {
        List<SegmentNode> nodes = new List<SegmentNode>();

        if (StartNode != null)
        {
            GetEndPointSegments(StartNode, nodes);
        }

        return nodes;
    }

    /// <summary>
    /// 找出所有没有子节点的路段
    /// </summary>
    /// <param name="node"></param>
    /// <param name="outList"></param>
    void GetEndPointSegments(SegmentNode node, List<SegmentNode> outList)
    {
        if (node == null)
        {
            return;
        }

        if (node.children == null && !outList.Contains(node))
        {
            outList.Add(node);
        }

        for (int i = 0; i < node.children.Length; ++i)
        {
            SegmentNode child = node.children[i];

            if (child == null && !outList.Contains(node))
            {
                outList.Add(node);
            }
            else
            {
                GetEndPointSegments(node, outList);
            }
        }
    }



    /// <summary>
    /// 生成路面网格模型
    /// </summary>
    /// <param name="smoothPercent">路段平滑百分比，大于等于0小于0.5</param>
    /// <param name="subdivision">表面分段数，大于等于2</param>
    public void GenerateMesh(float smoothPercent, int subdivision)
    {
        smoothPercent = Mathf.Clamp(smoothPercent, 0, 0.5f);
        subdivision = Mathf.Clamp(subdivision, 2, int.MaxValue);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Generate(smoothPercent, subdivision, StartNode, vertices, triangles);


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }



    /// <summary>
    /// 递归生成
    /// </summary>
    /// <param name="smoothPercent"></param>
    /// <param name="subdivision"></param>
    /// <param name="node"></param>
    /// <param name="vertices"></param>
    /// <param name="triangles"></param>
    void Generate(float smoothPercent, int subdivision, SegmentNode node, List<Vector3> vertices, List<int> triangles)
    {
        if (node == null)
        {
            return;
        }

        MeshData meshData = node.GenerateMesh(subdivision, vertices.Count);
        vertices.AddRange(meshData.vertices);
        triangles.AddRange(meshData.triangle);

        if (node.children != null)
        {
            for (int i = 0; i < node.children.Length; ++i)
            {
                if (node.children[i] != null)
                {
                    Generate(smoothPercent, subdivision, node.children[i], vertices, triangles);
                }
            }

        }

    }



}
