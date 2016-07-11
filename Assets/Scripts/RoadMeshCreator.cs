
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


    private SegmentNode currentNode;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void AddSegment(SegmentNode newNode)
    {
        if (currentNode != null)
        {
            currentNode.AddNode(newNode);
            currentNode = newNode;
        }
        else
        {
            currentNode = newNode;
        }
    }

    public void RemoveLastSegment()
    {
        if (currentNode != null)
        {
            currentNode = currentNode.parent;
            if (currentNode != null)
            {
            }
        }
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

       
    }




}
