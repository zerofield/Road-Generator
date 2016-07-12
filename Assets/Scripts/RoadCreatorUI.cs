using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoadCreatorUI : MonoBehaviour
{

    /// <summary>
    /// 当前被选中的节点
    /// </summary>
    private SegmentNode currentSelectedNode;

    /// <summary>
    /// 当前选中节点的子节点索引
    /// </summary>
    private int currentChildIndex;

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
        radiusInputField.text = radius.ToString();
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



    void Awake()
    {
        //road segment input fileds
        widthInputField.onEndEdit.AddListener(OnWidthChanged);
        lengthInputField.onEndEdit.AddListener(OnLengthChanged);
        pitchInputField.onEndEdit.AddListener(OnPitchChanged);
        rollInputField.onEndEdit.AddListener(OnRollChanged);
        angleInputField.onEndEdit.AddListener(OnAngleChanged);
        radiusInputField.onEndEdit.AddListener(OnRadiusChanged);
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



    /// <summary>
    /// 添加一个路段
    /// </summary>
    public void OnAddSegmentClicked()
    {

        if (currentSelectedNode == null && creator.StartNode != null)
        {
            return;
        }

        float width = tryGetFloat(widthInputField.text, minWidth);
        float length = tryGetFloat(lengthInputField.text, minLength);
        float pitch = tryGetFloat(pitchInputField.text, 0);
        float roll = tryGetFloat(rollInputField.text, 0);
        float angle = tryGetFloat(angleInputField.text, 0);
        float radius = tryGetFloat(radiusInputField.text, 0);

        Vector3 startPoint = Vector3.zero;
        float yaw = 0;


        if (creator.StartNode == null)
        {
            startPoint = Vector3.zero;
            yaw = 0;
        }
        else
        {
            startPoint = currentSelectedNode.endPoint;
            if (currentSelectedNode is CornerSegmentNode)
            {
                yaw = ((CornerSegmentNode)currentSelectedNode).endYaw;
            }
            else
            {
                yaw = currentSelectedNode.yaw;
            }
        }

        SegmentNode newNode;
        //分为转弯和平面两个情况来考虑
        if (angle != 0 && radius > 0)    //转弯路面
        {
            newNode = new CornerSegmentNode(width, startPoint, pitch, yaw, roll, angle, radius);
        }
        else //普通路面
        {
            newNode = new StraightSegmentNode(width, startPoint, length, pitch, roll, yaw);
        }

        if (creator.StartNode == null)
        {
            creator.StartNode = currentSelectedNode;
        }
        else
        {
            currentSelectedNode.AddNode(newNode, 0);
        }

        currentSelectedNode = newNode;

        Generate();

        removeSegmentButton.interactable = true;
    }

    public void Generate()
    {
        float smoothPercent = smoothSlider.value;
        int subdivision = (int)tryGetFloat(subdivistionField.text, minSubdivition);
        creator.GenerateMesh(smoothPercent, subdivision);
    }

    public void OnRemoveSegmentClicked()
    {
        if (currentSelectedNode == null)
        {
            removeSegmentButton.interactable = false;
            return;
        }
        else if (currentSelectedNode == creator.StartNode)
        {
            creator.StartNode = null;
            currentSelectedNode = null;
            removeSegmentButton.interactable = false;
        }
        else
        {
            int index = 0;
            //将子节点从父节点移除，并且设置父节点为选中节点
            if (currentSelectedNode.parent != null)
            {
                index = currentSelectedNode.parent.RemoveChild(currentSelectedNode);
            }

            currentSelectedNode = currentSelectedNode.parent;
        }
    }


    public void OnSelectionPointSelected(SegmentNode node, int index)
    {
        currentSelectedNode = node;
        currentChildIndex = index;
    }

}
