using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoadCreatorUI : MonoBehaviour
{

    public RoadMeshCreator creator;

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
    #endregion

    #region

    public float minRoadWidth = 1;
    public int minSubdivition = 2;
    public int smoothMin = 10;
    public int smoothMax = 50;

    public InputField subdivistionField;
    public Text smoothText;
    public Slider smoothSlider;
    #endregion

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
        pitchInputField.text = Mathf.Clamp(angle, -360, 360).ToString();
    }

    public void OnRollChanged(string newText)
    {
        float angle = tryGetFloat(newText, 0);
        rollInputField.text = Mathf.Clamp(angle, -360, 360).ToString();
    }

    public void OnAngleChanged(string newText)
    {
        float angle = tryGetFloat(newText, 0);
        angleInputField.text = Mathf.Clamp(angle, -360, 360).ToString();
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
        float angle = tryGetFloat(angleInputField.text, 0);
        float radius = tryGetFloat(radiusInputField.text, 0);

        int subdivision = (int)tryGetFloat(subdivistionField.text, minSubdivition);

        RoadPoint pointA;

        int count = creator.RawSegmentsCount;
        BaseRoadSegment segment = null;

        if (count == 0)
        {
            float tempPitch = 0;
            float tempYaw = 0;
            float tempRoll = 0;
            pointA = new RoadPoint(Vector3.zero, tempPitch, tempYaw, tempRoll);
        }
        else
        {
            pointA = creator.LastRoadSegment().pointB;
        }

        //分为转弯和平面两个情况来考虑
        if (angle != 0 && radius > 0)    //转弯路面
        {
            segment = new CornerRoadSegment(pointA, width, pitch, roll, radius, angle);
        }
        else //普通路面
        {
            segment = new SimpleRoadSegment(pointA, width, length, pitch, roll);
        }

        creator.AddSegment(segment);
        creator.GenerateMesh(0, subdivision, false);

        removeSegmentButton.interactable = true;
    }

    public void OnRemoveSegmentClicked()
    {
        creator.RemoveLastSegment();
        if (creator.RawSegmentsCount <= 0)
        {
            removeSegmentButton.interactable = false;
        }
        int subdivision = (int)tryGetFloat(subdivistionField.text, minSubdivition);
        creator.GenerateMesh(0, subdivision, false);
    }

    public void OnSubdivisionChanged(string newText)
    {
        int division = (int)tryGetFloat(newText, minSubdivition);
        if (division < minSubdivition)
        {
            subdivistionField.text = division.ToString();
        }
    }

    public void OnSmoothChanged(float value)
    {
        int ival = (int)value;
        smoothText.text = string.Format("Smooth\n({0}%)", ival);
    }

    public void Generate()
    {
        float smoothPercent = smoothSlider.value;
        int subdivision = (int)tryGetFloat(subdivistionField.text, minSubdivition);
        creator.GenerateMesh(smoothPercent, subdivision, true);
    }

    void Awake()
    {
        //road segment input fileds
        widthInputField.onEndEdit.AddListener(OnWidthChanged);
        lengthInputField.onEndEdit.AddListener(OnLengthChanged);
        pitchInputField.onEndEdit.AddListener(OnPitchChanged);
        rollInputField.onEndEdit.AddListener(OnRollChanged);
        angleInputField.onEndEdit.AddListener(OnAngleChanged);
        radiusInputField.onEndEdit.AddListener(OnAngleChanged);
        //buttons
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
