using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [SerializeField] public float radius;
    [Range(0,360)]
    [SerializeField] public float angle;
    public bool canSeePlayer;
    public List<GameObject> targets;
    Vector3 targetDirection;

    void Start()
    {
        var FOVCoroutine = FOVRoutine();
        StartCoroutine(FOVCoroutine);
    }

    private IEnumerator FOVRoutine()
	{
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while(true)
		{
            yield return wait;
            FOVCheck();
		}
	}

    private void FOVCheck()
	{

        // TODO: refactor lol
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("player"));

        if (!canSeePlayer)
            targets.Clear();

        //Debug.Log("hits: " + hits.Length);
        if (hits.Length > 0)
		{
            foreach(Collider hit in hits)
			{
                if (hit.gameObject.tag == "player")
                {
                    Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;

                    targetDirection = directionToTarget;

                    // if it's in the ai's view range
                    if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, hit.transform.position);
                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, LayerMask.GetMask("blockboi")))
                        {
                            if (targets.Contains(hit.gameObject))
                            {
                                int i = targets.FindIndex(o => o == hit.gameObject);
                                targets[i] = hit.gameObject;
                                Debug.Log("player updated");
                                canSeePlayer = true;
                            }
                            else
                            {
                                canSeePlayer = true;
                                targets.Add(hit.gameObject);
                                Debug.Log("player set");
                            }
                        }
                        else
                        {
                            canSeePlayer = false;
                            targets.Clear();
                        }

                    }
                    else
                    {
                        canSeePlayer = false;
                        targets.Clear();
                    }
                }

			}
		}
        else
		{
            targets.Clear();
		}
	}

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.blue; 
        if (targets.Count > 0)
            Gizmos.DrawLine(transform.position, targets[0].transform.position);
	}
}
