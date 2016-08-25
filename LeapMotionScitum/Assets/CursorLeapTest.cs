using UnityEngine;
using System.Collections;
using Leap;

public class CursorLeapTest : MonoBehaviour {

	public Sprite openHandSprite;
	public Sprite indexHandSprite;
	public Sprite closedHandSprite;

	private Vector3 prevPos;

	private int indexWait = 0;

	private int command = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		HandController handController = GameObject.Find("HandControllerSandBox").GetComponent<HandController>();
        //Transform camera = GameObject.Find("Main Camera").transform;
        Frame frame = handController.GetFrame(); // controller is a Controller object
        HandList hands = frame.Hands;
        Hand firstHand = hands[0];

		Vector3 nowPos = firstHand.StabilizedPalmPosition.ToUnityScaled() * 30;

		nowPos.y -= 10;

		Vector3 cursorPos = nowPos;

		cursorPos.y += 1.5f;
		cursorPos.x -= 0.5f;
    	//Vector3 pinchp = GameObject.Find("LeftHandPhysics").GetComponent<GrabbingHand>().GetIndexPos();

    	//Debug.Log(firstHand.Fingers[1].isExtended());

    	int extendedFingers = 0;

		for (int f = 0; f < firstHand.Fingers.Count; f++) {
		   		Finger digit = firstHand.Fingers [f];
				if (digit.IsExtended)
				        extendedFingers++; 
		}

		if (
			extendedFingers < 2 && firstHand.Fingers[1].IsExtended
			)
		{
			command = 1;
			GetComponent<SpriteRenderer>().sprite = indexHandSprite;
			GameObject.Find("IndexCursor").transform.position = cursorPos;
			if (
				(prevPos - transform.position).magnitude < 0.01f
				)
			{
				indexWait += 1;
			}
			else
			{
				indexWait = 0;
			}
		}
		else
		{
			indexWait = 0;
			if (extendedFingers < 2) {
				command = 2;
				GetComponent<SpriteRenderer>().sprite = closedHandSprite;
			}	
			else
			{
				command = 0;
				GetComponent<SpriteRenderer>().sprite = openHandSprite;
			}
		}

		if (indexWait >= 60) {
			Debug.Log(indexWait);
			RaycastHit hit;
			if (Physics.Raycast(cursorPos, Vector3.forward, out hit)) {
				Debug.Log("We have a hit.");
				if ((hit.collider.gameObject.tag == "Clickable" || hit.collider.gameObject.tag == "ClickDraggable") && command == 1) {
					hit.collider.gameObject.GetComponent<ClickDragHandler>().ActivateClick();
				}
			}

			if (Physics.Raycast(nowPos, Vector3.forward, out hit)) {
				if ((hit.collider.gameObject.tag == "Draggable" || hit.collider.gameObject.tag == "ClickDraggable") && command == 2) {
					hit.collider.gameObject.GetComponent<ClickDragHandler>().ActivateDrag();
				}
			}
		}

		prevPos = transform.position;

    	transform.position = nowPos;
	}
}
