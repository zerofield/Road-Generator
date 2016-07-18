
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

        SegmentNode newNode = (SegmentNode)node.Clone();

        if (node.children != null)
        {
            for (int i = 0; i < node.children.Length; ++i)
            {
                SegmentNode childNode = node.children[i];
                newNode.AddNode(CopyRoad(childNode), i);
            }
        }
        return newNode;
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

        if (startNode != null)
        {
            queue.Enqueue(startNode);
        }

        //Bredth First 
        while (queue.Count > 0)
        {
            SegmentNode parentNode = queue.Dequeue();

            SegmentType type = parentNode.type;

            if (parentNode.children == null)
            {
                continue;
            }

            for (int index = 0; index < parentNode.children.Length; ++index)
            {
                SegmentNode childNode = parentNode.children[index];

                if (childNode == null)
                {
                    continue;
                }

                if (IsSmoothNeeded(parentNode, index))
                {
                    //拆分路段
                    Vector3 oldPrentEndPoint = Vector3.zero;
                    Vector3 oldChildStartPoint = childNode.startPoint;

                    childNode.ShrinkStartPoint(smooth);

                    //目前使用两个路段之间最小值作为过渡
                    float roadWidth;

                    Vector3 startPoint = Vector3.zero;
                    Vector3 controlPoint1;
                    Vector3 controlPoint2;
                    Vector3 endPoint = childNode.startPoint;

                    float roll1 = 0f;
                    float roll2 = childNode.GetRoll(0f);

                    //使用parent作为过渡段的miu
                    float miu = parentNode.miu;

                    //分交叉路段和普通路段来处理
                    if (parentNode.type == SegmentType.Intersection)
                    {
                        float intersectionWidth = 0;

                        IntersectionSegmentNode interSection = (IntersectionSegmentNode)parentNode;

                        //重新计算宽度
                        switch (index)
                        {
                            case IntersectionSegmentNode.INDEX_CENTER:
                                intersectionWidth = interSection.width;
                                oldPrentEndPoint = interSection.endPoint;
                                interSection.ShrinkEndPoint(smooth);
                                startPoint = interSection.endPoint;
                                roll1 = interSection.GetRoll(1f);
                                break;

                            case IntersectionSegmentNode.INDEX_CENTER_LEFT:
                                intersectionWidth = interSection.length;
                                oldPrentEndPoint = interSection.centerLeft;
                                interSection.ShrinkLeftPoint(smooth);
                                startPoint = interSection.centerLeft;
                                roll1 = interSection.centerLeftRotation.eulerAngles.z;
                                break;
                            case IntersectionSegmentNode.INDEX_CENTER_RIGHT:
                                intersectionWidth = interSection.length;
                                oldPrentEndPoint = interSection.centerRight;
                                interSection.ShrinkRightPoint(smooth);
                                startPoint = interSection.centerRight;
                                roll1 = interSection.centerRightRotation.eulerAngles.z;
                                break;
                        }

                        roadWidth = Mathf.Min(intersectionWidth, childNode.width);
                        controlPoint1 = oldPrentEndPoint;
                        controlPoint2 = oldChildStartPoint;
                    }
                    else
                    {
                        oldPrentEndPoint = parentNode.endPoint;
                        parentNode.ShrinkEndPoint(smooth);
                        roll1 = parentNode.GetRoll(1f);

                        roadWidth = Mathf.Min(parentNode.width, childNode.width);
                        startPoint = parentNode.endPoint;
                        controlPoint1 = oldPrentEndPoint;
                        controlPoint2 = oldChildStartPoint;


                        if (parentNode.type == SegmentType.Corner)  //处理corner的控制点问题
                        {
                            //重新计算controlPoint1
                            CornerSegmentNode cornerNode = parentNode as CornerSegmentNode;
                            Quaternion rotation = cornerNode.GetRotation(1);

                            float length = Mathf.Abs(cornerNode.radius * Mathf.Tan(Mathf.Deg2Rad * Mathf.Abs(cornerNode.angle * smooth)));
                            controlPoint1 = cornerNode.endPoint + rotation * Vector3.forward * length;
                        }
                    }

                    //处理转弯child节点的控制点
                    if (childNode.type == SegmentType.Corner)
                    {
                        //重新计算controlPoint2
                        CornerSegmentNode cornerNode = childNode as CornerSegmentNode;
                        Quaternion rotation = cornerNode.GetRotation(0);

                        float length = Mathf.Abs(cornerNode.radius * Mathf.Tan(Mathf.Deg2Rad * Mathf.Abs(cornerNode.angle * smooth)));
                        controlPoint2 = cornerNode.startPoint + rotation * Vector3.back * length;
                    }

                    Debug.DrawLine(startPoint + Vector3.up * 0.1f, controlPoint1 + Vector3.up * 0.1f, Color.cyan, 30);
                    Debug.DrawLine(endPoint + Vector3.up * 0.1f, controlPoint2 + Vector3.up * 0.1f, Color.green, 30);

                    SmoothSegmentNode smoothNode = new SmoothSegmentNode(roadWidth, miu, startPoint, controlPoint1, controlPoint2, endPoint, roll1, roll2);

                    //插入节点，重建树的层级关系
                    smoothNode.AddNode(childNode, 0);
                    parentNode.AddNode(smoothNode, index);
                }

                //跳过平滑段，直接添加实际处理段
                queue.Enqueue(childNode);
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


        //目前全部切分
        return true;
    }

    float GetPitch(SegmentNode node, int index)
    {

        //TODO 
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
        //TODO 
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
