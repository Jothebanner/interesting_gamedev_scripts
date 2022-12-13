using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVDetection : MonoBehaviour
{
    public float maxAngle;
    public float maxRadius;
    public string enemyTag;

    private GameObject closestEnemy;

    void Update()
    {

    }

    public GameObject FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(enemyTag);

		if (allEnemies.Length > 0)
		{
			foreach (GameObject currentEnemy in allEnemies)
			{
				float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
				if (distanceToEnemy < distanceToClosestEnemy)
				{
					distanceToClosestEnemy = distanceToEnemy;
					closestEnemy = currentEnemy;
				}
			}
		}
        return closestEnemy;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        //Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.forward) * transform.up * maxRadius;
        //Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.forward) * transform.up * maxRadius;

        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, fovLine1);
        //Gizmos.DrawRay(transform.position, fovLine2);

        Gizmos.color = Color.red;
		if (closestEnemy)
			Gizmos.DrawRay(transform.position, (closestEnemy.transform.position - transform.position).normalized * maxRadius);
    }

    public bool inFOV(Transform checkingObject, Transform target)
    {
        if ((target.position - checkingObject.position).magnitude < maxRadius)
        {
            return true;
        }

        return false;
    }
}
