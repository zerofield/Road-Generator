using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoadCreator : MonoBehaviour
{
    [HideInInspector]
    public Road road = new Road();

    #region 添加路段 

    public float minWidth = 1;
    public float minLength = 1;

    public InputField widthInputField;
    public InputField lengthInputField;
    public InputField pitchInputField;
    public InputField rollInputField;
    public InputField angleInputField;
    public InputField radiusInputField;
    public Button addSegmentButton;
    public Button removeSegmentButton;

    public void OnWidthChanged(string newText)
    {
        float width = tryGetFloat(newText, minWidth);

        if (width <= minWidth)
        {
            width = minWidth;
            widthInputField.text = width.ToString();
        }
    }

    public void OnLengthChanged(string newText)
    {

        float length = tryGetFloat(newText, minLength);

        if (length <= minLength)
        {
            length = minLength;
            lengthInputField.text = length.ToString();
        }
    }

    public void OnPitchChanged(string newText)
    {
        float angle = tryGetFloat(newText, 0);
        pitchInputField.text = ClampAngle(angle).ToString();
    }

    public void OnRollChanged(string newText)
    {
        float angle = tryGetFloat(newText, 0);
        rollInputField.text = ClampAngle(angle).ToString();
    }

    public void OnAngleChanged(string newText)
    {
        float angle = tryGetFloat(newText, 0);
        angleInputField.text = ClampAngle(angle).ToString();
    }

    public void OnRadiusChanged(string newText)
    {
        float radius = tryGetFloat(newText, 0);
        if (radius < 0)
        {
            radius = 0;
        }
        rollInputField.text = radius.ToString();
    }

    /// <summary>
    /// 添加一个路段
    /// </summary>
    public void OnAddSegmentClicked()
    {
        float width = tryGetFloat(widthInputField.text, minWidth);
        float length = tryGetFloat(lengthInputField.text, minLength);
        float pitch = tryGetFloat(pitchInputField.text, 0);
        float roll = tryGetFloat(rollInputField.text, 0);

        int count = road.Segments.Count;

        Vector3 pointA;

        if (count == 0)
        {
            //TODO 决定第一个点的位置
            pointA = Vector3.zero;
            removeSegmentButton.interactable = true;
        }
        else
        {
            pointA = road.Segments[road.Segments.Count - 1].PointB;
        }
        Debug.Log("Add a road segment");
        RoadSegment newSegment = new RoadSegment(pointA, width, length, pitch, yaw, roll);
        road.Segments.Add(newSegment);
    }

    public void OnRemoveSegmentClicked()
    {
        if (road.Segments.Count <= 0)
        {
            removeSegmentButton.interactable = false;
        }
    }



    /// <summary>
    /// 把角度限制在[0,360]之间
    /// </summary>
    /// <param name="angle">角度</param>
    /// <returns>转换后的角度</returns>
    private float ClampAngle(float angle)
    {
        while (angle >= 360)
        {
            angle -= 360;
        }
        while (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }
    #endregion




    #region  生成路面

    public float minRoadWidth = 1;
    public int minSubdivition = 2;
    public int smoothMin = 10;
    public int smoothMax = 50;

    public InputField subdivistionField;
    public Text smoothText;
    public Slider smoothSlider;

    public void OnSubdivisionChanged(string newText)
    {
        int division;
        int.TryParse(newText, out division);

        if (division < minSubdivition)
        {
            subdivistionField.text = minSubdivition.ToString();
        }
    }

    public void OnSmoothChanged(float value)
    {
        int ival = (int)value;
        smoothText.text = string.Format("Smooth\n({0}%)", ival);
    }

    public void Generate()
    {
        //TODO
    }

    #endregion

    void Awake()
    {
        //segment
        widthInputField.onEndEdit.AddListener(OnWidthChanged);
        lengthInputField.onEndEdit.AddListener(OnLengthChanged);
        pitchInputField.onEndEdit.AddListener(OnPitchChanged);
        rollInputField.onEndEdit.AddListener(OnRollChanged);
        angleInputField.onEndEdit.AddListener(OnAngleChanged);
        radiusInputField.onEndEdit.AddListener(OnAngleChanged);

        addSegmentButton.onClick.AddListener(OnAddSegmentClicked);
        removeSegmentButton.onClick.AddListener(OnRemoveSegmentClicked);
        removeSegmentButton.interactable = false;
        //road
        subdivistionField.onEndEdit.AddListener(OnSubdivisionChanged);
        smoothSlider.onValueChanged.AddListener(OnSmoothChanged);
        //
        smoothSlider.minValue = smoothMin;
        smoothSlider.maxValue = smoothMax;
        smoothSlider.value = (smoothMin + smoothMax) / 2;
    }

    private float tryGetFloat(string text, float defaultValue)
    {
        float value;
        return float.TryParse(text, out value) ? value : defaultValue;
    }



}
