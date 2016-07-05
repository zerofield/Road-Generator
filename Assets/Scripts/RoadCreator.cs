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

    void Awake()
    {

    }
}
