using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoadCreator : MonoBehaviour
{
    #region 添加路段 
    public InputField lengthInputField;
    public InputField pitchInputField;
    public InputField yawInputField;
    public InputField RollInputField;
    public Button addSegmentButton;

    public void OnLengthChanged(string newText)
    {
        Debug.Log(newText);
        float length;
        float.TryParse(newText, out length);

        if (length <= 1)
        {
            length = 1;
            lengthInputField.text = length.ToString();
        }
    }

    public void OnPitchChanged(string newText)
    {
        float angle;
        float.TryParse(newText, out angle);
        pitchInputField.text = ClampAngle(angle).ToString();
    }

    public void OnYawChanged(string newText)
    {
        float angle;
        float.TryParse(newText, out angle);
        yawInputField.text = ClampAngle(angle).ToString();
    }

    public void OnRollChanged(string newText)
    {
        float angle;
        float.TryParse(newText, out angle);
        RollInputField.text = ClampAngle(angle).ToString();
    }

    /// <summary>
    /// 添加一个路段
    /// </summary>
    public void OnAddSegmentClicked()
    {
        //
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

    public InputField roadWidthInputField;
    public InputField subdivistionField;
    public Text smoothText;
    public Slider smoothSlider;


    public void OnRoadWidthChanged(string newText)
    {
        Debug.Log(newText);
        float width;
        float.TryParse(newText, out width);
        if (width < minRoadWidth)
        {
            roadWidthInputField.text = minRoadWidth.ToString();
        }
    }

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

    }

    #endregion

    void Awake()
    {
        lengthInputField.onEndEdit.AddListener(OnLengthChanged);
        pitchInputField.onEndEdit.AddListener(OnPitchChanged);
        yawInputField.onEndEdit.AddListener(OnYawChanged);
        RollInputField.onEndEdit.AddListener(OnRollChanged);
        roadWidthInputField.onEndEdit.AddListener(OnRoadWidthChanged);
        subdivistionField.onEndEdit.AddListener(OnSubdivisionChanged);
        smoothSlider.onValueChanged.AddListener(OnSmoothChanged);
        //
        smoothSlider.minValue = smoothMin;
        smoothSlider.maxValue = smoothMax;
        smoothSlider.value = (smoothMin + smoothMax) / 2;
    }
}
