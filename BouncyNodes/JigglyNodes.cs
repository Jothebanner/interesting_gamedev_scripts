using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigglyNodes : MonoBehaviour
{
	[SerializeField] GameObject parent;

	[SerializeField] Vector2 offsetPostion;

	SpringJoint2D spring;

	[SerializeField] float snapRadius;

	[SerializeField] float breakDis;

	bool isBeingHeld;

	[SerializeField] public List<GameObject> children;

	[SerializeField] GameObject midPointBoi;


    // Start is called before the first frame update
    void Start()
    {
		spring = GetComponent<SpringJoint2D>();
		Physics2D.queriesStartInColliders = false;
		breakDis = 2.5f;
	}

    // Update is called once per frame
    void Update()
    {
		isBeingHeld = GetComponent<ClickAndDrag>().isBeingHeld;

		SetLine();

		if (!isBeingHeld)
		{
			this.gameObject.layer = LayerMask.NameToLayer("Default");
		}
		else
		{
			this.gameObject.layer = LayerMask.NameToLayer("Ignore");
			if (parent)
			{
				SetOffsetPosition();
			}
		}

		if (parent)
		{
			spring.connectedAnchor = (Vector2)parent.transform.position - offsetPostion;

			if (Vector2.Distance(transform.position, parent.transform.position) > breakDis && isBeingHeld)
			{
				Break();
			}
		}
		else if (isBeingHeld)
		{
			LookForParent();
		}
        
    }

	void SetOffsetPosition()
	{
		offsetPostion = parent.transform.position - this.transform.position;
	}

	void LookForParent()
	{
		Collider2D[] circleHit = Physics2D.OverlapCircleAll(transform.position, snapRadius, ~(LayerMask.GetMask("Ignore")));

		GameObject closestCircle = null;

		float closestDistance = Mathf.Infinity;

		for (int i = 0; i < circleHit.Length; i++)
		{
			if (circleHit.Length > 0 && circleHit[i].gameObject.GetComponent<JigglyNodes>().parent != this.gameObject && !children.Contains(circleHit[i].gameObject))
			{
				float currentDistance = Vector3.Distance(transform.position, circleHit[i].gameObject.transform.position);
				if (currentDistance < closestDistance)
				{
					closestCircle = circleHit[i].gameObject;
					closestDistance = currentDistance;
				}
			}
		}

		if (closestCircle)
		{
			parent = closestCircle;
			spring.enabled = true;
			SetOffsetPosition();
			Camera.main.GetComponent<Rigidbody2D>().AddForce(offsetPostion * 0.3f, ForceMode2D.Impulse);

			List<GameObject> childrenToAdd = new List<GameObject>(children);
			childrenToAdd.Add(this.gameObject);
			AddChildNodes(childrenToAdd);
		}

	}

	public void AddChildNodes(List<GameObject> childrenToAdd)
	{
		if (parent)
		{
			foreach (GameObject child in childrenToAdd)
			{
				parent.GetComponent<JigglyNodes>().children.Add(child);
			}
			parent.GetComponent<JigglyNodes>().AddChildNodes(childrenToAdd);
		}
	}

	public void RemoveChildrenNodes(List<GameObject> childrenToRemove)
	{
		foreach (GameObject child in childrenToRemove)
		{
			children.Remove(child);
		}
		if (parent != null)
		parent.GetComponent<JigglyNodes>().RemoveChildrenNodes(childrenToRemove);
	}

	void Break()
	{
		if (parent != null)
		{
			List<GameObject> childrenToRemove = new List<GameObject>(children);
			childrenToRemove.Add(this.gameObject);
			parent.GetComponent<JigglyNodes>().RemoveChildrenNodes(childrenToRemove);
		}
		parent = null;
		spring.enabled = false;
	}

	// if it has a parent keep the anchor on the parent minus offset
	// if not then allow it to be parented

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		if (parent)
			Gizmos.DrawLine(transform.position, parent.transform.position);
	}

	void SetLine()
	{
		if (parent)
		{
			GetComponent<DrawLine>().origin = transform.position;
			GetComponent<DrawLine>().endPoint = parent.transform.position;
			midPointBoi.GetComponent<SpringJoint2D>().connectedAnchor = ((transform.position + parent.transform.position) / 2);
		}
		else
		{
			GetComponent<DrawLine>().origin = transform.position;
			GetComponent<DrawLine>().endPoint = transform.position;
			midPointBoi.GetComponent<SpringJoint2D>().connectedAnchor = transform.position;
			midPointBoi.GetComponent<SpringJoint2D>().frequency = 5;
		}
	}
}
