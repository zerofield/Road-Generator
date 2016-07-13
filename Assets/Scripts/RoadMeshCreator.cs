
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
                GetEndPointSegments(child, outList);
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

    /// <summary>
    /// 从node开始复制一条路出来
    /// </summary>
    /// <param name="node">起始节点</param>
    /// <returns></returns>
    SegmentNode CopyRoad(SegmentNode node)
    {

        Queue<SegmentNode> queue = new Queue<SegmentNode>();
        SegmentNode newStartNode = (SegmentNode)node.Clone();
        queue.Enqueue(newStartNode);

        while (queue.Count > 0)
        {
            SegmentNode currentNode = queue.Dequeue();

            if (currentNode.children != null)
            {
                for (int i = 0; i < currentNode.children.Length; ++i)
                {
                    if (currentNode.children[i] != null)
                    {
                        SegmentNode childNode = (SegmentNode)currentNode.children[i].Clone();
                        currentNode.AddNode(childNode, i);
                        queue.Enqueue(childNode);
                    }
                }
            }
        }
        return newStartNode;
    }


    /// <summary>
    /// 生成光滑路面模型
    /// </summary>
    /// <param name="subdivision">细分数量</param>
    /// <param name="smoothPercent">光滑比例</param>
    public void GenerateSmoothRoadMesh(int subdivision, float smoothPercent)
    {
        if(StartNode == null)
        {
            return;
        }

        SegmentNode newStartNode =  CopyRoad(StartNode);
        GenerateSmoothRoad(newStartNode, smoothPercent);

        smoothPercent = Mathf.Clamp(smoothPercent, 0, 0.5f);
        subdivision = Mathf.Clamp(subdivision, 2, int.MaxValue);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Generate(smoothPercent, subdivision, newStartNode, vertices, triangles);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    public void GenerateSmoothRoad(SegmentNode startNode, float smooth)
    {
        Queue<SegmentNode> queue = new Queue<SegmentNode>();

        if (StartNode != null)
        {
            queue.Enqueue(StartNode);
        }

        while (queue.Count > 0)
        {
            SegmentNode node = queue.Dequeue();

            SegmentType type = node.type;

            if (node.children != null)
            {
                for (int i = 0; i < node.children.Length; ++i)
                {
                    SegmentNode childNode = node.children[i];
                    if (childNode == null)
                    {
                        continue;
                    }

                    SegmentType childType = childNode.type;



                }
            }
        }
    }



}
