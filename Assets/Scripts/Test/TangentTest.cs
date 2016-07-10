using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TangentTest : MonoBehaviour
{

    public float width;
    public int division = 10;
    public Transform p0;
    public Transform p1;
    public Transform p2;


    public Transform p3;
    public Transform p4;
    public Transform p5;



    public float roll0;
    public float roll1;


  
    // Use this for initialization
    void Start()
    {

        /*
        Vector3 direction = new Vector3(1, 1, 1).normalized;

        Vector3 up = Vector3.Cross(Vector3.Cross(direction, Vector3.up), direction).normalized;

        //Debug.Log(up.x + ", " + up.y + ", " + up.z);
        Quaternion q = Quaternion.LookRotation(direction, up);
        //  Debug.Log(q.x + " " + q.y + " " + q.z);
        Debug.DrawLine(Vector3.zero, up * 10, Color.red, 100);
        Debug.DrawLine(Vector3.zero, Vector3.Cross(Vector3.up, direction) * 10, Color.green, 100);
        Debug.DrawLine(Vector3.zero, direction * 10, Color.black, 100);

        float angle = 0;
        Vector3 axis = Vector3.up;
        q.ToAngleAxis(out angle, out axis);

        Debug.Log(angle + " " + axis);


        //bezier

        printPoint(2);
        printPoint(3);
        printPoint(4);
        printPoint(6.5f);
        printPoint(10);

        printPoint(7.8f);
        */
        //test mesh
    }

    void Update()
    {

        StartCoroutine(WaitAndGenerateMesh());
    }

    IEnumerator WaitAndGenerateMesh()
    {
        while (true)
        {
            GenerateMesh();

            yield return new WaitForSeconds(2f);
        }

    }

    void GenerateMesh()
    {

        p1.position = (p2.position + p0.position) / 2;
        p2.position = p3.position;
        float dist = Vector3.Distance(p4.position, p3.position);
        p4.position = p3.position + (p3.position - p1.position).normalized * dist;


        RoadPoint rp0 = new RoadPoint(p0.position, 0, 0, roll0);
        RoadPoint rp1 = new RoadPoint(p1.position, 0, 0, 0);
        RoadPoint rp2 = new RoadPoint(p2.position, 0, 0, roll0);

        SmoothRoadSegment segment = new SmoothRoadSegment(rp0, rp1, rp2, width);


        MeshFilter filter = GetComponent<MeshFilter>();
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        segment.GenerateMesh(division, vertices, triangles, 0);


        RoadPoint rp3 = new RoadPoint(p3.position, 0, 0, roll0);
        RoadPoint rp4 = new RoadPoint(p4.position, 0, 0, 0);
        RoadPoint rp5 = new RoadPoint(p5.position, 0, 0, roll1);
        SmoothRoadSegment segment2 = new SmoothRoadSegment(rp3, rp4, rp5, width);
        segment2.GenerateMesh(division, vertices, triangles, vertices.Count);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        filter.mesh = mesh;
    }


    void printPoint(float x)
    {

        float t = x / 10;

        Vector3 p0 = Vector3.zero;
        Vector3 p1 = new Vector3(5, 2.5f, 0);
        Vector3 p2 = new Vector3(6, 2, 0);
        Vector3 p3 = new Vector3(10, 8, 0);


        Vector3[] pts =
        {
            p0, p1, p2, p3
        };
        //   Vector3 p = BezierCurve.GetPoint(pts, t);
        //    print(x + ", " + p.x);

        float newT = GetClosestT(pts, x, 0, 1, 0.0001f);
        Vector3 p = BezierCurve.GetPoint(pts, newT);
        print(x + ", " + p.x);
    }



    float GetClosestT(Vector3[] pts, float x, float startT, float endT, float eps)
    {

        // Debug.Log(startT + ", " + endT);
        Vector3 startPos = BezierCurve.GetPoint(pts, startT);

        Vector3 endPos = BezierCurve.GetPoint(pts, endT);

        Vector3 dist = endPos - startPos;

        if (Mathf.Abs(endPos.x - startPos.x) < eps)
        {
            return startT;
        }

        //   Debug.Log("dist: " + Mathf.Abs(x - startT) + ", " + Mathf.Abs(x - endT));

        if (Mathf.Abs(x - startPos.x) < Mathf.Abs(x - endPos.x))
        {
            return GetClosestT(pts, x, startT, (startT + endT) / 2, eps);
        }
        else
        {
            return GetClosestT(pts, x, (startT + endT) / 2, endT, eps);
        }

    }



}
