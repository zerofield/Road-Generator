
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
        if (node == null)
        {
            return null;
        }

        SegmentNode parent = (SegmentNode)node.Clone();

        if (node.children != null)
        {
            for (int i = 0; i < node.children.Length; ++i)
            {
                SegmentNode child = node.children[i];
                parent.AddNode(CopyRoad(child), i);
            }
        }
        return parent;
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
        GenerateSmoothRoadSegmentNode(newStartNode, smoothPercent);

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

    /// <summary>
    /// 生成光滑路面节点
    /// </summary>
    /// <param name="startNode">初始节点</param>
    /// <param name="smooth">光滑程度</param>
    public void GenerateSmoothRoadSegmentNode(SegmentNode startNode, float smooth)
    {
        Queue<SegmentNode> queue = new Queue<SegmentNode>();

        if (StartNode != null)
        {
            queue.Enqueue(StartNode);
        }

        //Bredth First 
        while (queue.Count > 0)
        {
            SegmentNode node = queue.Dequeue();

            SegmentType type = node.type;

            if (node.children == null)
            {
                continue;
            }

            for (int index = 0; index < node.children.Length; ++index)
            {
                SegmentNode child = node.children[index];

                if (child == null)
                {
                    continue;
                }

                if (IsSmoothNeeded(node, index))
                {
                    //拆分路段

                    //分交叉路段和普通路段来处理
                    if (node.type == SegmentType.Intersection)
                    {

                    }
                    else
                    {
                        Vector3 oldEndPoint = node.endPoint;
                        Vector3 oldChildStartPoint = child.startPoint;

                        node.ShrinkEndPoint(smooth);
                        child.ShrinkStartPoint(smooth);

                        //TODO 处理corner 的控制点问题

                        //目前使用两个路段之间最大值作为过渡
                        float width = Mathf.Max(node.width, child.width);

                        SmoothSegmentNode smoothNode = new SmoothSegmentNode(width, node.endPoint, oldEndPoint, child.startPoint, oldChildStartPoint, node.GetRoll(1f), child.GetRoll(0f));

                        //插入节点，重建树的层级关系
                        smoothNode.AddNode(child, 0);
                        node.AddNode(smoothNode, index);
                    }
                }

                //跳过平滑段，直接添加实际处理段
                queue.Enqueue(child);
            }
        }
    }

    bool IsSmoothNeeded(SegmentNode parent, int index)
    {

        if (parent == null || parent.children == null || parent.type == SegmentType.Smooth)//smooth不可再分
        {
            return false;
        }

        SegmentNode child = parent.children[index];

        if (child == null)
        {
            return false;
        }

        //TODO


        return true;
    }

    float GetPitch(SegmentNode node, int index)
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

    float GetRoll(SegmentNode node, int index)
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
