using UnityEngine;
using System.Collections;

public class CameraRenderer : MonoBehaviour
{

    public RoadCreator creator;

    public void OnPostRender()
    {
        creator.road.DrawSkeleton();
    }
}
