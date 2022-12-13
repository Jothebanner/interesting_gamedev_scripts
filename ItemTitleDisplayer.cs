using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemTitleDisplayer : MonoBehaviour
{
	public Vector3 titlePosition;
	public GameObject titleCanvas;
	private GameObject textCanvas;

	private GameObject playerUi;
	private TextMeshProUGUI uiText;

	GameObject lastObject = null;

	void Start()
    {
		CreateTitleObject();

		playerUi = GameObject.FindGameObjectWithTag("PlayerUi");
		uiText = playerUi.GetComponentInChildren<TextMeshProUGUI>();
    }

	private void DisplayTitleFromItem(RaycastHit hit)
	{
		if (hit.collider)
		{
			Debug.Log(hit.collider.gameObject.name);
			if (hit.collider.gameObject != lastObject)
			{
				if (lastObject && lastObject.GetComponent<ItemTitle>())
					lastObject.GetComponent<ItemTitle>().HideTitle();

				if (hit.collider.gameObject.GetComponent<ItemTitle>())
					hit.collider.gameObject.GetComponent<ItemTitle>().DisplayTitle();

				lastObject = hit.collider.gameObject;
			}
			if (hit.collider.transform.position != lastObject.transform.position)
			{
				hit.collider.gameObject.GetComponent<ItemTitle>().UpdateTitlePosition();
			}
		}
		else
		{
			if (lastObject && lastObject.GetComponent<ItemTitle>())
				lastObject.GetComponent<ItemTitle>().HideTitle();

			lastObject = null;
		}
	}

	private void DisplayTitleFromPlayersCanvas(RaycastHit hit)
	{
		if (hit.collider && hit.collider.GetComponent<ItemTitle>())
		{
			textCanvas.GetComponent<TextMeshProUGUI>().text = hit.collider.gameObject.name;
			titlePosition = hit.collider.gameObject.GetComponent<ItemTitle>().UpdateTitlePosition();
			titleCanvas.transform.position = titlePosition;

			if (titleCanvas.activeSelf == false)
				titleCanvas.SetActive(true);

			Debug.Log(hit.collider.gameObject.name);
			if (lastObject && hit.collider.transform.position != lastObject.transform.position)
			{
				titlePosition = hit.collider.gameObject.GetComponent<ItemTitle>().UpdateTitlePosition();
			}

			if (lastObject && hit.collider.gameObject != lastObject)
			{
				textCanvas.GetComponent<TextMeshProUGUI>().text = hit.collider.gameObject.name;
			}

			lastObject = hit.collider.gameObject;
		}
		else
		{
			if (titleCanvas.activeSelf == true)
				titleCanvas.SetActive(false);

			lastObject = null;
		}
	}

	public void DisplayTitleFromPlayersUI(RaycastHit hit)
	{
		if (hit.collider)
		{
			uiText.GetComponent<TextMeshProUGUI>().text = hit.collider.gameObject.name;
		}
		else
		{
			uiText.GetComponent<TextMeshProUGUI>().text = null;
		}
	}

	private void CreateTitleObject()
	{
		titleCanvas = new GameObject();
		textCanvas = new GameObject();

		textCanvas.gameObject.transform.parent = titleCanvas.transform;

		titleCanvas.AddComponent<Canvas>();
		titleCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
		titleCanvas.AddComponent<RotateWithPlayer>();

		textCanvas.AddComponent<TextMeshProUGUI>();
		textCanvas.GetComponent<TextMeshProUGUI>().text = transform.gameObject.name;
		textCanvas.GetComponent<TextMeshProUGUI>().rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		textCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

		Quaternion rotation = new Quaternion();
		rotation.x = 0;
		rotation.y = 180;
		rotation.z = 0;
		textCanvas.GetComponent<TextMeshProUGUI>().rectTransform.rotation = rotation;

		titleCanvas.name = "TitleCanvas";
		textCanvas.name = "TitleText";

		titleCanvas.SetActive(false);
	}
}
