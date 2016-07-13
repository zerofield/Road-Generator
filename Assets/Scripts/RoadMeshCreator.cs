
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
    public void GenerateRawMesh(int subdivision)
    {
        subdivision = Mathf.Clamp(subdivision, 2, int.MaxValue);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        GenerateMesh(subdivision, StartNode, vertices, triangles);


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
    void GenerateMesh(int subdivision, SegmentNode node, List<Vector3> vertices, List<int> triangles)
    {
        if (node == null)
        {
            return;
        }
        subdivision = Mathf.Clamp(subdivision, 2, int.MaxValue);

        MeshData meshData = node.GenerateMesh(subdivision, vertices.Count);
        vertices.AddRange(meshData.vertices);
        triangles.AddRange(meshData.triangle);

        if (node.children != null)
        {
            for (int i = 0; i < node.children.Length; ++i)
            {
                if (node.children[i] != null)
                {
                    GenerateMesh(subdivision, node.children[i], vertices, triangles);
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
        if (StartNode == null)
        {
            return;
        }

        smoothPercent = Mathf.Clamp(smoothPercent, 0, 0.5f);
        subdivision = Mathf.Clamp(subdivision, 2, int.MaxValue);

        SegmentNode newStartNode = CopyRoad(StartNode);
        GenerateSmoothRoad(newStartNode, smoothPercent);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        GenerateMesh(subdivision, newStartNode, vertices, triangles);

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

                    if (type == SegmentType.Straight)
                    {
                        if (childType == SegmentType.Straight || childType == SegmentType.Intersection)
                        {

                        }
                    }
                    else if (type == SegmentType.Intersection)
                    {
                        if (childType == SegmentType.Straight || childType == SegmentType.Intersection)
                        {

                        }
                        else if (childType == SegmentType.Straight)
                        {

                        }
                    }
                    else if (type == SegmentType.Smooth)
                    {
                        //如果是smooth类型，说明已经做过平滑处理，直接把child加入队列中
                        queue.Enqueue(childNode);
                        continue;
                    }
                    else if (type == SegmentType.Corner)
                    {

                    }
                }
            }
        }
    }

    bool IsSmoothNeeded(SegmentNode node1, SegmentNode node2)
    {
        float pitch1 = GetPitch(node1);
        float pitch2 = GetPitch(node2);

        float roll1 = GetRoll(node1);
        float roll2 = GetRoll(node2);

        return pitch1 != pitch2 || roll1 != roll2;
    }

    float GetPitch(SegmentNode node)
    {
        if (node.type == SegmentType.Straight || node.type == SegmentType.Intersection)
        {
            return ((StraightSegmentNode)node).pitch;
        }
        else if (node.type == SegmentType.Corner)
        {
            return ((CornerSegmentNode)node).pitch;
        }

        return 0;
    }

    float GetRoll(SegmentNode node)
    {
        if (node.type == SegmentType.Straight || node.type == SegmentType.Intersection)
        {
            return ((StraightSegmentNode)node).roll;
        }
        else if (node.type == SegmentType.Corner)
        {
            return ((CornerSegmentNode)node).roll;
        }
        return 0;
    }


}
