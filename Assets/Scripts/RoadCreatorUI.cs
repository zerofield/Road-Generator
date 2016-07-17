using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    private List<RoadSelectionPoint> selectionPoints = new List<RoadSelectionPoint>();

    public RoadSelectionPoint selectionPointPrefab;

    public RoadMeshCreator creator;

    #region Fields
    public float minWidth = 1;
    public float minLength = 1;

    public float minMiu = 0;
    public float maxMiu = 1;

    public float minRoadWidth = 1;
    public int minSubdivition = 2;

    public int smoothMin = 10;
    public int smoothMax = 50;

    public InputField widthInputField;
    public InputField lengthInputField;
    public InputField miuInputField;
    public InputField pitchInputField;
    public InputField rollInputField;
    public InputField angleInputField;
    public InputField radiusInputField;
    public Button addSegmentButton;
    public Button addIntersectionButton;
    public Button removeSegmentButton;

    //generate panel
    public InputField subdivistionField;
    public Text smoothText;
    public Slider smoothSlider;
    public Button generateButton;
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

    public void OnMiuChanged(string newText)
    {
        float miu = tryGetFloat(newText, minMiu);
        miu = Mathf.Clamp(miu, minMiu, maxMiu);
        miuInputField.text = miu.ToString();
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
        miuInputField.onEndEdit.AddListener(OnMiuChanged);
        pitchInputField.onEndEdit.AddListener(OnPitchChanged);
        rollInputField.onEndEdit.AddListener(OnRollChanged);
        angleInputField.onEndEdit.AddListener(OnAngleChanged);
        radiusInputField.onEndEdit.AddListener(OnRadiusChanged);
        //buttons
        addSegmentButton.onClick.AddListener(OnAddSegmentClicked);
        addIntersectionButton.onClick.AddListener(OnAddIntersectionClicked);
        removeSegmentButton.onClick.AddListener(OnRemoveSegmentClicked);
        removeSegmentButton.interactable = false;
        generateButton.onClick.AddListener(OnGenerateClicked);
        //road
        subdivistionField.onEndEdit.AddListener(OnSubdivisionChanged);
        smoothSlider.onValueChanged.AddListener(OnSmoothChanged);
        //
        smoothSlider.minValue = smoothMin;
        smoothSlider.maxValue = smoothMax;
        smoothSlider.value = (smoothMin + smoothMax) / 2;

    }

    void Start()
    {
       // Test();
    }

    void Test()
    {
        float miu = 0.1f;

        StraightSegmentNode node0 = new StraightSegmentNode(10, miu, Vector3.zero, 30, -20, 20, 0);
        StraightSegmentNode node1 = new StraightSegmentNode(10, miu, node0.endPoint, 20, 0, 0, 0);
        CornerSegmentNode node2 = new CornerSegmentNode(10, miu, node1.endPoint, 0, node1.yaw, 30, 30, 60);
        CornerSegmentNode node3 = new CornerSegmentNode(10, miu, node2.endPoint, 20, node2.endYaw, 0, -30, 60);


        CornerSegmentNode node4 = new CornerSegmentNode(10, miu, node3.endPoint, 0, node3.endYaw, -20, 40, 60);
        CornerSegmentNode node5 = new CornerSegmentNode(20, miu, node4.endPoint, 0, node4.endYaw, 0, 60, 100);
        CornerSegmentNode node6 = new CornerSegmentNode(10, miu, node5.endPoint, 0, node5.endYaw, 0, -70, 100);



        node0.AddNode(node1);
        node1.AddNode(node2);
        node2.AddNode(node3);
        node3.AddNode(node4);
        node4.AddNode(node5);
        node5.AddNode(node6);

        creator.StartNode = node0;

        creator.GenerateSmoothRoadMesh(20, 0.1f);

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
        AddSegment(false);

    }

    /// <summary>
    /// 添加一个十字路
    /// </summary>
    public void OnAddIntersectionClicked()
    {
        AddSegment(true);
    }

    public void OnGenerateClicked()
    {
        float percent = smoothSlider.value / 100;
        int subdivision = (int)tryGetFloat(subdivistionField.text, minSubdivition);
        creator.GenerateSmoothRoadMesh(subdivision, percent);
    }

    /// <summary>
    /// 添加一个路段信息
    /// </summary>
    /// <param name="isAddIntersection">是否为添加分叉路段</param>
    void AddSegment(bool isAddIntersection)
    {
        if (currentSelectedNode == null && creator.StartNode != null)
        {
            return;
        }

        float width = tryGetFloat(widthInputField.text, minWidth);
        float length = tryGetFloat(lengthInputField.text, minLength);
        float miu = tryGetFloat(miuInputField.text, minMiu);
        float pitch = tryGetFloat(pitchInputField.text, 0);
        float roll = tryGetFloat(rollInputField.text, 0);
        float angle = tryGetFloat(angleInputField.text, 0);
        float radius = tryGetFloat(radiusInputField.text, 0);

        Vector3 startPoint = Vector3.zero;
        float yaw = 0;

        //TODO 处理没有任何路段的情况
        if (creator.StartNode == null)
        {
            startPoint = Vector3.zero;
            yaw = 0;
        }
        else
        {
            startPoint = currentSelectedNode.endPoint;

            if (currentSelectedNode is IntersectionSegmentNode)
            {
                IntersectionSegmentNode interNode = (IntersectionSegmentNode)currentSelectedNode;
                switch (currentChildIndex)
                {
                    case IntersectionSegmentNode.INDEX_CENTER:
                        startPoint = interNode.endPoint;
                        yaw = interNode.yaw;
                        break;
                    case IntersectionSegmentNode.INDEX_CENTER_LEFT:
                        startPoint = interNode.centerLeft;
                        yaw = interNode.centerLeftRotation.eulerAngles.y;
                        break;
                    case IntersectionSegmentNode.INDEX_CENTER_RIGHT:
                        startPoint = interNode.centerRight;
                        yaw = interNode.centerRightRotation.eulerAngles.y;
                        break;
                }

            }
            else if (currentSelectedNode is CornerSegmentNode)
            {
                yaw = ((CornerSegmentNode)currentSelectedNode).endYaw;
            }
            else
            {
                yaw = currentSelectedNode.yaw;
            }
        }

        SegmentNode newNode;

        if (isAddIntersection)//添加交叉路段
        {
            newNode = new IntersectionSegmentNode(width, miu, startPoint, length, pitch, roll, yaw);

        }
        else
        {//添加普通路段

            //分为转弯和平面两个情况来考虑
            if (angle != 0 && radius > 0)    //转弯路面
            {
                newNode = new CornerSegmentNode(width, miu, startPoint, pitch, yaw, roll, angle, radius);
            }
            else //普通路面
            {
                newNode = new StraightSegmentNode(width, miu, startPoint, length, pitch, roll, yaw);
            }
        }

        if (creator.StartNode == null)
        {
            creator.StartNode = newNode;
            currentChildIndex = SegmentNode.DEFAULT_CHILD_INDEX;
        }
        else
        {
            currentSelectedNode.AddNode(newNode, currentChildIndex);
        }

        if (isAddIntersection)
        {
            currentChildIndex = IntersectionSegmentNode.INDEX_CENTER;
        }
        else
        {
            currentChildIndex = SegmentNode.DEFAULT_CHILD_INDEX;
        }

        currentSelectedNode = newNode;
        GenerateRawMesh();
        CreateSelectionPoints();
        removeSegmentButton.interactable = true;
    }

    /// <summary>
    /// 创建原始网格模型
    /// </summary>
    void GenerateRawMesh()
    {
        creator.GenerateRawMesh(10);
    }

    /// <summary>
    /// 移除
    /// </summary>
    public void OnRemoveSegmentClicked()
    {
        if (currentSelectedNode == null)
        {
            removeSegmentButton.interactable = false;
            return;
        }
        else if (currentSelectedNode == creator.StartNode) //只有一个节点的情况
        {
            creator.StartNode = null;
            currentSelectedNode = null;
            removeSegmentButton.interactable = false;
            currentChildIndex = SegmentNode.DEFAULT_CHILD_INDEX; ;
        }
        else
        {
            int index = SegmentNode.DEFAULT_CHILD_INDEX;
            //将子节点从父节点移除，并且设置父节点为选中节点
            SegmentNode parent = currentSelectedNode.parent;

            if (parent != null)
            {
                index = parent.RemoveChild(currentSelectedNode);

            }

            currentSelectedNode = parent;
            currentChildIndex = index;
        }

        GenerateRawMesh();
        CreateSelectionPoints();
    }

    /// <summary>
    /// 创建路段最末端选择节点
    /// </summary>
    private void CreateSelectionPoints()
    {
        for (int i = 0; i < selectionPoints.Count; ++i)
        {
            Destroy(selectionPoints[i].gameObject);
            //TODO 改成Object Pool
        }

        selectionPoints.Clear();

        List<SegmentNode> nodeList = creator.GetEndPointSegments();

        if (nodeList != null)
        {
            for (int i = 0; i < nodeList.Count; ++i)
            {
                SegmentNode node = nodeList[i];

                if (node.type == SegmentType.Intersection) //交叉路口，会有三个选择点
                {
                    IntersectionSegmentNode intersectionNode = node as IntersectionSegmentNode;
                    RoadSelectionPoint point = makeIntersectionSelectionPoint(intersectionNode, IntersectionSegmentNode.INDEX_CENTER);
                    if (point != null)
                    {
                        selectionPoints.Add(point);
                    }
                    point = makeIntersectionSelectionPoint(intersectionNode, IntersectionSegmentNode.INDEX_CENTER_LEFT);
                    if (point != null)
                    {
                        selectionPoints.Add(point);

                    }
                    point = makeIntersectionSelectionPoint(intersectionNode, IntersectionSegmentNode.INDEX_CENTER_RIGHT);
                    if (point != null)
                    {
                        selectionPoints.Add(point);

                    }
                }
                else//普通路面只有一个点
                {
                    RoadSelectionPoint point = Instantiate<RoadSelectionPoint>(selectionPointPrefab);
                    point.Initialize(this, node, 0);
                    point.transform.position = node.endPoint;
                    SetSelectionPointColor(point);
                    selectionPoints.Add(point);
                }
            }
        }
    }

    /// <summary>
    /// 创建分叉路段选择点
    /// </summary>
    /// <param name="intersectionNode"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    RoadSelectionPoint makeIntersectionSelectionPoint(IntersectionSegmentNode intersectionNode, int index)
    {
        if (intersectionNode.children[index] != null)
        {
            return null;
        }

        RoadSelectionPoint point = Instantiate<RoadSelectionPoint>(selectionPointPrefab);
        point.Initialize(this, intersectionNode, index);
        Vector3 position = Vector3.zero;

        switch (index)
        {
            case IntersectionSegmentNode.INDEX_CENTER:
                position = intersectionNode.endPoint;
                break;
            case IntersectionSegmentNode.INDEX_CENTER_LEFT:
                position = intersectionNode.centerLeft;
                break;
            case IntersectionSegmentNode.INDEX_CENTER_RIGHT:
                position = intersectionNode.centerRight;
                break;
        }

        point.transform.position = position;
        SetSelectionPointColor(point);
        return point;
    }


    /// <summary>
    /// 选择点被选择回调方法
    /// </summary>
    /// <param name="point"></param>
    public void OnSelectionPointSelected(RoadSelectionPoint point)
    {
        currentSelectedNode = point.node;
        currentChildIndex = point.index;

        // 改变颜色
        for (int i = 0; i < selectionPoints.Count; ++i)
        {
            SetSelectionPointColor(selectionPoints[i]);
        }
    }


    /// <summary>
    /// 设置选择点的颜色
    /// </summary>
    /// <param name="point"></param>
    private void SetSelectionPointColor(RoadSelectionPoint point)
    {
        //设置颜色
        if (point.node == currentSelectedNode && point.index == currentChildIndex)
        {
            point.SetColor(Color.red);
        }
        else
        {
            point.SetColor(Color.gray);
        }
    }

}
