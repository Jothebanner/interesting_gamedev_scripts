using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MyOwnSpecialSoundController : MonoBehaviour
{
	[SerializeField] bool createSoundColliderOnImpact;
	private AudioSource audioSource;
	[SerializeField] float velocity = 0;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void CreateCollider(float loudness)
	{
		velocity = loudness;
		//Debug.Log(velocity);

		foreach (Collider collider in Physics.OverlapSphere(transform.position, loudness))
		{
			if (collider.gameObject.tag == "Ghost" && collider.gameObject.GetComponent<Hearing>() == null)
			{
				collider.gameObject.GetComponent<Hearing>().HearSound();
			}
		}


	}

	private void OnCollisionEnter(Collision collision)
	{
		float velocity = 0;

		velocity = collision.relativeVelocity.magnitude;

		//Debug.Log(collision.gameObject.name);
		audioSource.PlayOneShot(audioSource.clip);

		if (createSoundColliderOnImpact)
		{
			CreateCollider(velocity);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, velocity);
	}
}
