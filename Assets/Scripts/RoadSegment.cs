using UnityEngine;
using System.Collections;

public class RoadSegment
{
    public Vector3 PointA
    {
        get;
        private set;
    }

    public Vector3 PointB
    {
        get;
        private set;
    }

    private float length;

    public float Length
    {
        get { return length; }
        set
        {
            length = value;
            if (length < 0)
            {
                length = 0;
                Debug.Log("Length should not be less than zero.");
            }
            UpdateValues();
        }
    }

    private float width;

    public float Width
    {
        get { return width; }
        set
        {
            width = value;
            if (width < 0)
            {
                width = 0;
                Debug.Log("Width must be greater than zero!");
            }
            UpdateValues();
        }
    }

    private float pitch;

    public float Pitch
    {
        get { return pitch; }
        set
        {
            pitch = value;
            UpdateValues();
        }
    }

    private float yaw;

    public float Yaw
    {
        get { return yaw; }
        set
        {
            yaw = value;
            UpdateValues();
        }
    }

    private float roll;

    public float Roll
    {
        get { return roll; }
        set
        {
            roll = value;
            UpdateValues();
        }
    }

    public Quaternion Rotation
    {
        get;
        private set;
    }

    public Vector3 Up
    {
        get;
        private set;
    }

    public Vector3 Down
    {
        get;
        private set;
    }

    public Vector3 Left
    {
        get;
        private set;
    }

    public Vector3 Right
    {
        get;
        private set;
    }

    public Vector3 Forward
    {
        get;
        private set;
    }

    public Vector3 Back
    {
        get;
        private set;
    }


    public Vector3 LowerLeftPoint
    {
        get;
        private set;
    }

    public Vector3 LowerRightPoint
    {
        get;
        private set;
    }

    public Vector3 UpperLeftPoint
    {
        get;
        private set;
    }

    public Vector3 UpperRightPoint
    {
        get;
        private set;
    }

    public RoadSegment(Vector3 pointA, float width, float length, float pitch, float yaw, float roll)
    {
        PointA = new Vector3(pointA.x, pointA.y, pointA.z);
        this.width = width;
        this.length = length;
        this.pitch = pitch;
        this.yaw = yaw;
        this.roll = roll;
        UpdateValues();
    }

    public void UpdateValues()
    {
        Rotation = Quaternion.Euler(pitch, yaw, roll);
        //direction vectors
        Forward = Rotation * Vector3.forward;
        Back = Rotation * Vector3.back;
        Left = Rotation * Vector3.left;
        Right = Rotation * Vector3.right;
        Up = Rotation * Vector3.up;
        Down = Rotation * Vector3.down;
        //pointB
        PointB = PointA + Forward * length;

        //4 points
        float halfWidth = width / 2;
        LowerLeftPoint = PointA + Left * halfWidth;
        LowerRightPoint = PointA + Right * halfWidth;
        UpperLeftPoint = PointB + Left * halfWidth;
        UpperRightPoint = PointB + Right * halfWidth;
    }

    public bool hasTheSameRotation(RoadSegment other)
    {
        if (other == null)
        {
            return false;
        }
        return Rotation.Equals(other.Rotation);
    }


    /// <summary>
    /// 绘制轮廓
    /// </summary>
    public void DrawSkeleton()
    {
        Debug.Log("DrawSkeleton()");
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        GL.Color(Color.blue);
        //left
        GL.Vertex(LowerLeftPoint);
        GL.Vertex(UpperLeftPoint);
        //top
        GL.Vertex(UpperLeftPoint);
        GL.Vertex(UpperRightPoint);

        //right
        GL.Vertex(UpperRightPoint);
        GL.Vertex(LowerRightPoint);
        //bottom
        GL.Vertex(LowerRightPoint);
        GL.Vertex(LowerLeftPoint);
        //a-b
        GL.Vertex(PointA);
        GL.Vertex(PointB);
        GL.End();
        GL.PopMatrix();
    }

}
