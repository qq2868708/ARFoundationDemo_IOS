using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelState
{
    public string panelName;
    public GameObject panel;
    public bool openOrClose;
}

public class ARUIManager : MonoBehaviour
{
    public Canvas canvas;
    public Transform uiTag;
    public Raycast ray;

    public Dictionary<string, GameObject> uiPanel = new Dictionary<string, GameObject>();

    public GameObject optionsPanel;
    public GameObject controllerPanel;
    public GameObject mapPanel;

    //打开Options二级面板
    public Button openAndCloseForOptions;
    private bool openOrCloseOptions;

    public Dictionary<string, Button> Options = new Dictionary<string, Button>();

    //打开Controller二级面板
    public Button openAndCloseForController;
    private bool openOrCloseController;

    public Dictionary<string, Button> Controllers = new Dictionary<string, Button>();

    //打开Controller二级面板
    public Button openAndCloseForMap;
    private bool openOrCloseMap;

    public Dictionary<string, Button> Map = new Dictionary<string, Button>();

    //开放给其他脚本修改
    public Text debugText;
    //提供一个修改文本的委托
    public EventHandler updateTextHandler;

    //按钮委托
    public UnityAction changeMode;
    public UnityAction placePrefab;
    public UnityAction setRef;
    public UnityAction placeCoordiante;
    public UnityAction clearAllCoordinate;
    //微调委托
    public UnityAction moveForward;
    public UnityAction moveBackward;
    public UnityAction moveLeft;
    public UnityAction moveRight;
    public UnityAction turnLeft;
    public UnityAction turnRight;
    //public UnityAction turnDown;
    //public UnityAction turnUp;
    public UnityAction moveUp;
    public UnityAction moveDown;
    public UnityAction changeRotateMode;

    private void Awake()
    {
        ray = GetComponent<Raycast>();
        uiPanel.Add("OptionsPanel", optionsPanel);
        uiPanel.Add("ControllerPanel", controllerPanel);
        uiPanel.Add("MapPanel", mapPanel);
        openOrCloseOptions = true;
        openOrCloseController = true;
        openOrCloseMap = true;
    }

    private void OnEnable()
    {
        openAndCloseForOptions.onClick.AddListener(OpenOrCloseOptions);
        openAndCloseForController.onClick.AddListener(OpenOrCloseController);
        openAndCloseForMap.onClick.AddListener(OpenOrCloseMap);
        TransformHelper.FindChild(canvas.transform, "Mode").GetComponentInChildren<Button>().onClick.AddListener(changeMode);
        TransformHelper.FindChild(canvas.transform, "Place").GetComponentInChildren<Button>().onClick.AddListener(placePrefab);
        TransformHelper.FindChild(canvas.transform, "SetRef").GetComponentInChildren<Button>().onClick.AddListener(setRef);
        TransformHelper.FindChild(canvas.transform, "PlaceCoordinate").GetComponentInChildren<Button>().onClick.AddListener(placeCoordiante);
        TransformHelper.FindChild(canvas.transform, "ClearCoordinate").GetComponentInChildren<Button>().onClick.AddListener(clearAllCoordinate);

        TransformHelper.FindChild(canvas.transform, "前").GetComponent<Button>().onClick.AddListener(moveForward);
        TransformHelper.FindChild(canvas.transform, "后").GetComponent<Button>().onClick.AddListener(moveBackward);
        TransformHelper.FindChild(canvas.transform, "左").GetComponent<Button>().onClick.AddListener(moveLeft);
        TransformHelper.FindChild(canvas.transform, "右").GetComponent<Button>().onClick.AddListener(moveRight);

        TransformHelper.FindChild(canvas.transform, "左转").GetComponent<Button>().onClick.AddListener(turnLeft);
        TransformHelper.FindChild(canvas.transform, "右转").GetComponent<Button>().onClick.AddListener(turnRight);
        //TransformHelper.FindChild(canvas.transform, "俯").GetComponent<Button>().onClick.AddListener(turnDown);
        //TransformHelper.FindChild(canvas.transform, "仰").GetComponent<Button>().onClick.AddListener(turnUp);

        TransformHelper.FindChild(canvas.transform, "高").GetComponent<Button>().onClick.AddListener(moveUp);
        TransformHelper.FindChild(canvas.transform, "低").GetComponent<Button>().onClick.AddListener(moveDown);

        TransformHelper.FindChild(canvas.transform, "RotateSelf").GetComponent<Button>().onClick.AddListener(changeRotateMode);

    }

    private void OnDisable()
    {
        openAndCloseForOptions.onClick.RemoveListener(OpenOrCloseOptions);
        openAndCloseForController.onClick.RemoveListener(OpenOrCloseController);
        TransformHelper.FindChild(canvas.transform, "Mode").GetComponentInChildren<Button>().onClick.RemoveListener(changeMode);
        TransformHelper.FindChild(canvas.transform, "Place").GetComponentInChildren<Button>().onClick.RemoveListener(placePrefab);
        TransformHelper.FindChild(canvas.transform, "SetRef").GetComponentInChildren<Button>().onClick.RemoveListener(setRef);
        TransformHelper.FindChild(canvas.transform, "PlaceCoordinate").GetComponentInChildren<Button>().onClick.RemoveListener(placeCoordiante);
        TransformHelper.FindChild(canvas.transform, "ClearCoordinate").GetComponentInChildren<Button>().onClick.RemoveListener(clearAllCoordinate);

        TransformHelper.FindChild(canvas.transform, "前").GetComponent<Button>().onClick.RemoveListener(moveForward);
        TransformHelper.FindChild(canvas.transform, "后").GetComponent<Button>().onClick.RemoveListener(moveBackward);
        TransformHelper.FindChild(canvas.transform, "左").GetComponent<Button>().onClick.RemoveListener(moveLeft);
        TransformHelper.FindChild(canvas.transform, "右").GetComponent<Button>().onClick.RemoveListener(moveRight);

        TransformHelper.FindChild(canvas.transform, "左转").GetComponent<Button>().onClick.RemoveListener(turnLeft);
        TransformHelper.FindChild(canvas.transform, "右转").GetComponent<Button>().onClick.RemoveListener(turnRight);
        //TransformHelper.FindChild(canvas.transform, "俯").GetComponent<Button>().onClick.RemoveListener(turnDown);
        //TransformHelper.FindChild(canvas.transform, "仰").GetComponent<Button>().onClick.RemoveListener(turnUp);

        TransformHelper.FindChild(canvas.transform, "高").GetComponent<Button>().onClick.RemoveListener(moveUp);
        TransformHelper.FindChild(canvas.transform, "低").GetComponent<Button>().onClick.RemoveListener(moveDown);

        TransformHelper.FindChild(canvas.transform, "RotateSelf").GetComponent<Button>().onClick.RemoveListener(changeRotateMode);
    }

    void Update()
    {
        if (updateTextHandler != null)
        {
            updateTextHandler.Invoke(this, new EventArgs());
        }

        if (ray.placementIndicator.activeSelf==false)
        {
            if(uiTag.gameObject.activeSelf==true)
            {
                uiTag.gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            uiTag.gameObject.SetActive(true);
        }

        var pos = Camera.main.WorldToScreenPoint(ray.placementIndicator.transform.position);
        uiTag.transform.position = pos;
        pos = ray.placementIndicator.transform.position;
        pos.y -= ray.horRef;
        uiTag.GetComponentInChildren<Text>().text = pos.ToString();

        
    }

    class MoveArgs
    {
        public GameObject target;
        public Vector3 targetPos;
    }

    private IEnumerator MoveInAndOut(MoveArgs args)
    {
        while (Vector3.Distance( args.target.transform.position , args.targetPos)>0.01)
        {
            Vector3 pos = Vector3.Lerp(args.target.transform.position, args.targetPos, 0.5f);
            args.target.transform.position = pos;
            yield return null;
        }
        TransformHelper.FindChild(args.target.transform, "Open").Rotate(new Vector3(0, 0, 180));
    }


    private void OpenOrCloseOptions()
    {
        var target = uiPanel["OptionsPanel"];
        if (openOrCloseOptions)
        {
            var targetPos = target.transform.position + new Vector3(-500, 0, 0);
            StartCoroutine("MoveInAndOut", new MoveArgs{target=target,targetPos=targetPos });
        }
        else
        {
            var targetPos = target.transform.position + new Vector3(500, 0, 0);
            StartCoroutine("MoveInAndOut", new MoveArgs { target = target, targetPos = targetPos });
        }
        openOrCloseOptions = !openOrCloseOptions;
    }

    private void OpenOrCloseController()
    {
        var target = uiPanel["ControllerPanel"];
        if (openOrCloseController)
        {
            var targetPos = target.transform.position + new Vector3(0, -600, 0);
            StartCoroutine("MoveInAndOut", new MoveArgs { target = target, targetPos = targetPos });
        }
        else
        {
            var targetPos = target.transform.position + new Vector3(0, 600, 0);
            StartCoroutine("MoveInAndOut", new MoveArgs { target = target, targetPos = targetPos });
        }
        openOrCloseController = !openOrCloseController;
    }

    private void OpenOrCloseMap()
    {
        var target = uiPanel["MapPanel"];
        if (openOrCloseMap)
        {
            var targetPos = target.transform.position + new Vector3(0, -600, 0);
            StartCoroutine("MoveInAndOut", new MoveArgs { target = target, targetPos = targetPos });
        }
        else
        {
            var targetPos = target.transform.position + new Vector3(0, 600, 0);
            StartCoroutine("MoveInAndOut", new MoveArgs { target = target, targetPos = targetPos });
        }
        openOrCloseMap = !openOrCloseMap;
    }

    public void UpdateDebugText(string str)
    {
#if UNITY_EDITOR
        Debug.Log("UpdateText");
#endif
        debugText.text = str;
    }
}
