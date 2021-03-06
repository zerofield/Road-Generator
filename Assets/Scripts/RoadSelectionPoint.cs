﻿using UnityEngine;
using System.Collections;

public class RoadSelectionPoint : MonoBehaviour
{


    [System.NonSerialized]
    public RoadCreatorUI creatorUI;

    [System.NonSerialized]
    public int index;

    [System.NonSerialized]
    public SegmentNode node;

    public void Initialize(RoadCreatorUI creatorUI, SegmentNode node, int index)
    {
        this.creatorUI = creatorUI;
        this.node = node;
        this.index = index;
    }

    public void SetColor(Color color)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.color = color;
    }

    void OnMouseUp()
    {
        if (creatorUI != null)
        {
            creatorUI.OnSelectionPointSelected(this);
        }
    }
}
