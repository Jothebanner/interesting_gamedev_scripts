using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ZombieController : Mirror.NetworkBehaviour
{
	private Animator animator;
	private Rigidbody2D rb;
	private bool targetSighted;
	private bool attack;
	private GameObject closestEnemy;

	public float speed;
	public float health;

    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
		if (!isServer)
		{
			return;
		}

		closestEnemy = GetComponentInChildren<FOVDetection>().FindClosestEnemy();

		if (closestEnemy && GetComponentInChildren<FOVDetection>().inFOV(transform, closestEnemy.transform))
		{
			targetSighted = true;
		}
		else
		{
			targetSighted = false;
		}

		if (closestEnemy)
		{
			if ((transform.position - closestEnemy.transform.position).magnitude <= 2)
			{
				animator.SetBool("Attack", true);
				attack = true;
			}
			else
			{
				animator.SetBool("Attack", false);
				attack = false;
			}
		}
		else
		{
			animator.SetBool("Attack", false);
			attack = false;
		}
	}

    private void FixedUpdate()
    {
		if (!targetSighted && !attack)
		{
			rb.velocity = new Vector2(speed, 0) * Time.deltaTime;
			animator.SetFloat("horSpeed", rb.velocity.x);
		}
		else if (targetSighted && closestEnemy && !attack)
		{
			Vector2 direction = closestEnemy.transform.position - transform.position;
			float total = Mathf.Abs(direction.x) + Mathf.Abs(direction.y);
			float xPercent = direction.x / total;
			float yPercent = direction.y / total;
			Vector2 speedTowardsObject = new Vector2(xPercent, yPercent) * speed * 2 * Time.deltaTime;
			rb.velocity = speedTowardsObject;
			animator.SetFloat("horSpeed", rb.velocity.x);
		}
		else if (attack)
		{
			rb.velocity = new Vector2(0, 0);
			animator.SetFloat("horSpeed", rb.velocity.x);
		}
	}

	public void die()
	{
		Destroy(this.gameObject);
	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			die();
		}
	}
}
