using UnityEngine;
using System.Collections;
using Leap;

public class TestLeapLeap : MonoBehaviour {
    public Vector3 prevPos;
    public Vector3 nowPos;
    public Vector3 changePos;
    private float changeX = 0;
    private float changeY = 0;

    private Transform cubeTransform;

    private Controller controller;

    private GameObject grabObject;

    private bool triggered = false;

    private bool tapped = false;
    private int tapTimer = 0;

    // Use this for initialization
    void Start () {
        HandController handController = GameObject.Find("HandControllerSandBox").GetComponent<HandController>();
        controller = new Controller();
        controller.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP);
        controller.Config.SetFloat ("Gesture.ScreenTap.MinForwardVelocity", 15.0f);
        controller.Config.SetFloat ("Gesture.ScreenTap.HistorySeconds", 1.0f);
        controller.Config.SetFloat ("Gesture.ScreenTap.MinDistance", 1.0f);
        controller.Config.Save ();
        Frame frame = handController.GetFrame(); // controller is a Controller object
        HandList hands = frame.Hands;
        Hand firstHand = hands[0];
        prevPos = firstHand.PalmPosition.ToUnityScaled();
    }
	
	// Update is called once per frame
	void Update () {
        HandController handController = GameObject.Find("HandControllerSandBox").GetComponent<HandController>();
        Transform camera = GameObject.Find("Main Camera").transform;
        Frame frame = handController.GetFrame(); // controller is a Controller object
        HandList hands = frame.Hands;
        Hand firstHand = hands[0];
        float moveX = 0, moveY = 0;
        if (true) {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                moveX = -1f;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                moveX = 1f;
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                moveY = -1f;
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                moveY = 1f;
            }
        }
        handController.transform.Translate(moveX, moveY, 0);
        camera.Translate(moveX, moveY, 0);
        changeX = changeX + moveX;
        changeY = changeY + moveY;
        Vector3 changeVector = new Vector3(changeX, changeY, 0);
        nowPos = firstHand.PalmPosition.ToUnityScaled();
        nowPos = nowPos * 45;
        nowPos = nowPos + changeVector;

        Frame contFrame = controller.Frame();
        GestureList gestures = contFrame.Gestures();
        for (int i = 0; i < gestures.Count; i++) {
            Gesture gesture = gestures[i];
            if (gesture.Type == Gesture.GestureType.TYPE_SCREEN_TAP && tapTimer <= 0) {
                if (grabObject != null) {
                    if (!triggered) {
                        grabObject.transform.position = new Vector3(nowPos.x, nowPos.y - 6, -1);
                    }
                    grabObject = null;
                    triggered = false;
                    tapped = false;
                } else {
                    tapTimer = 30;
                    tapped = true;
                }
            }
        }

        if (tapTimer > 0) {
            tapTimer -= 1;
        } else {
            tapped = false;
        }
        
        if (grabObject != null)
        {
            if (!triggered) {
                grabObject.transform.position = new Vector3(nowPos.x, nowPos.y - 6, -1);
            }
            if (Input.anyKey)
            {
                if (!Input.GetKey(KeyCode.Mouse0))
                {
                    if ((Input.GetKey(KeyCode.LeftArrow)) || (Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.DownArrow)) || (Input.GetKey(KeyCode.UpArrow))) {
                        return;
                    }
                    GameObject textObj = GameObject.Find(grabObject.GetComponent<FlowElement>().textObjName);
                    if (textObj.GetComponent<TextMesh>().text == "<Insert Text Here>" || Input.GetKey(KeyCode.Backspace))
                    {
                        triggered = true;
                        textObj.GetComponent<TextMesh>().text = Input.inputString;
                    }
                    else
                    {
                        triggered = true;
                        textObj.GetComponent<TextMesh>().text = textObj.GetComponent<TextMesh>().text + Input.inputString;
                    }
                }
                if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey("enter"))
                {
                    if (!triggered) {
                        grabObject.transform.position = new Vector3(nowPos.x, nowPos.y - 6, -1);
                    }
                    grabObject = null;
                    triggered = false;
                    tapped = false;
                }
            }

        }
        //Debug.Log(grabObject.IsGrabbed());
        //if (grabObject.IsGrabbed())
        //{
        //    
        //}
    }

    public void hover(GameObject gameObj)
    {
        if (grabObject == null && !triggered && tapped)
        {
            tapped = false;
            grabObject = gameObj;
        }
    }
}
