using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoadCreator : MonoBehaviour
{
    #region 添加路段相关变量
    public InputField lengthInputField;
    public InputField pitchInputField;
    public InputField yawInputField;
    public InputField RollInputField;
    public Button addSegmentButton;
    #endregion

    #region 添加路段相关事件

    public void onLengthChanged(string newText)
    {
        Debug.Log(newText);
        float value = float.Parse(newText);

        if (value <= 1)
        {
            value = 1;
            lengthInputField.text = value.ToString();
        }


    }

    public void onPitchChanged(string newText)
    {
        float angle = float.Parse(newText);
        pitchInputField.text = ClampAngle(angle).ToString();
    }

    public void onYawChanged(string newText)
    {
        float angle = float.Parse(newText);
        yawInputField.text = ClampAngle(angle).ToString();
    }

    public void onRollChanged(string newText)
    {
        float angle = float.Parse(newText);
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
    public Slider smoothSlider;


    public void OnRoadWidthChanged(string newText)
    {
        Debug.Log(newText);
        float width = float.Parse(newText);
        if(width< minRoadWidth)
        {
            roadWidthInputField.text = minRoadWidth.ToString();
        }
    }

    public void OnSubdivisionChanged(string newText)
    {
        int division = int.Parse(newText);
        if(division< minSubdivition)
        {
            subdivistionField.text = minSubdivition.ToString();
        }
    }

    public void Generate()
    {

    }

    #endregion

    void Awake()
    {
        smoothSlider.minValue = smoothMin;
        smoothSlider.maxValue = smoothMax;
        smoothSlider.value = (smoothMin + smoothMax) / 2;
    }
}
