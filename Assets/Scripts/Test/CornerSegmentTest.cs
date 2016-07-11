using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CornerSegmentTest : MonoBehaviour
{

    public float width = 5;
    public float angle = 30;
    public float radius = 50;
    public Vector3 startPoint = Vector3.zero;
    public float pitch = 0;
    public float yaw = 0;
    public float roll = 0;
    public int subdivision = 10;
    void Awake()
    {
    }

    void Update()
    {

        StartCoroutine(run());
    }

    IEnumerator run()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            GenerateMesh();
        }
    }


    void GenerateMesh()
    {

        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        CornerSegmentNode node = new CornerSegmentNode(width, startPoint, pitch, yaw, roll, angle, radius);
        MeshData data = node.GenerateMesh(subdivision, 0);

        CornerSegmentNode node2 = new CornerSegmentNode(width, node.endPoint, pitch, node.endYaw, roll, -angle, radius);
        MeshData data2 = node2.GenerateMesh(subdivision, data.vertices.Count);
        data.vertices.AddRange(data2.vertices);
        data.triangle.AddRange(data2.triangle);
        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.triangle.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        filter.mesh = mesh;
    }
}
