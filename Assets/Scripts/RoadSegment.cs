using UnityEngine;
using System.Collections;

public class RoadSegment
{
    private Vector3 pointA;

    public Vector3 PointA
    {
        get { return pointA; }
        set
        {
            pointA = value;
            UpdateValues();
        }
    }

    private Vector3 pointB;

    public Vector3 PointB
    {
        get { return pointB; }
    }

    private float length;

    public float Length
    {
        get { return length; }
        set
        {
            if (length < 0)
            {
                length = 0;
                Debug.Log("Length should not be less than zero.");
            }
            length = value;
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

    private Quaternion rotation;

    public Quaternion Rotation
    {
        get { return rotation; }
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

    public RoadSegment(Vector3 pointA, float length, float pitch, float yaw, float roll)
    {
        this.pointA = new Vector3(pointA.x, pointA.y, pointA.z);
        UpdateValues();
    }

    public void UpdateValues()
    {
        rotation = Quaternion.Euler(pitch, yaw, roll);
        //direction vectors
        Forward = rotation * Vector3.forward;
        Back = rotation * Vector3.back;
        Left = rotation * Vector3.left;
        Right = rotation * Vector3.right;
        Up = rotation * Vector3.up;
        Down = rotation * Vector3.down;
        //pointB
        pointB = pointA + Forward * length;
    }

}
