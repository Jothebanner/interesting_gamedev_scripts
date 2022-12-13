using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
	[SerializeField] public LineRenderer lineRenderer;

	[SerializeField] int amountOfPoints;

	public Vector2 origin;
	public Vector2 endPoint;

	List<Vector3> points;

	[SerializeField] GameObject midPointBoi;

	// Start is called before the first frame update
	void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
		points = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
	{
		//points = new List<Vector3> { origin, midPointBoi.transform.position , endPoint };
		CalculateCurve();
		lineRenderer.positionCount = points.Count;
		lineRenderer.SetPositions(points.ToArray());
    }

	void CalculateCurve()
	{
		points.Clear();

		for (int i = 1; i <= amountOfPoints; i++)
		{
			float currentTimeThing = (1 / (float)amountOfPoints) * i;

			// local position
			Vector3 distanceFromAnchor = midPointBoi.transform.position - (Vector3)((origin + endPoint) / 2);

			//Debug.Log(distanceFromAnchor);

			float lengthMod = currentTimeThing <= 0.5f ? (currentTimeThing) : (1.0f - currentTimeThing);

			Vector3 offset = (distanceFromAnchor * 2); //* lengthMod);

			// world position
			Vector3 currentPoint =
			Vector3.Lerp(Vector3.Lerp(origin, endPoint, currentTimeThing),
			Vector3.Lerp(midPointBoi.transform.position + (offset), endPoint, currentTimeThing),
			currentTimeThing);

			//currentPoint += offset;


			//Debug.Log("endPoint " + endPoint + " " + i);
			//Debug.Log(currentPoint);

			points.Add(currentPoint);
		}

		AnimationCurve curve = new AnimationCurve();

		Debug.Log(curve.Evaluate(.3f));

		curve.AddKey(.1f, 0.2f);
		curve.AddKey(.5f, 0.1f);
		curve.AddKey(1f, 0.2f);

		lineRenderer.widthCurve = curve;
	}
}
