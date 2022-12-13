using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDrag : MonoBehaviour
{
	float startPosX, startPosY;
	public bool isBeingHeld = false;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingHeld)
		{
			Vector3 mousePos;
			mousePos = Input.mousePosition;
			mousePos = Camera.main.ScreenToWorldPoint(mousePos);


			this.gameObject.transform.position = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY);
		}
    }

	private void OnMouseDown()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos;
			mousePos = Input.mousePosition;
			mousePos = Camera.main.ScreenToWorldPoint(mousePos);

			isBeingHeld = true;


			Vector3 offset = mousePos - this.gameObject.transform.position;

			startPosX = offset.x;
			startPosY = offset.y;

		}
	}

	private void OnMouseUp()
	{
		isBeingHeld = false;
	}
}
