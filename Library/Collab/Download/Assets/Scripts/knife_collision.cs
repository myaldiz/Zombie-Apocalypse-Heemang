using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knife_collision : MonoBehaviour {

	public GameObject knife;

	private GameObject zombie;
	private bool stucked = false;
	private Vector3 local_position;
	private Quaternion local_rotation;

	//sound
	private AudioSource audio_zombie;
	public AudioClip zombie_sound;


	void Start()
	{
		this.audio_zombie = this.gameObject.AddComponent<AudioSource> ();
		this.audio_zombie.clip = zombie_sound;
		this.audio_zombie.volume = 0.3f;
		this.audio_zombie.loop = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Zombie"))
		{
			Rigidbody rig = other.gameObject.GetComponent<Rigidbody> ();
			Rigidbody knife_rig = knife.GetComponent<Rigidbody> ();

			knife_rig.detectCollisions = false;
			knife_rig.velocity = Vector3.zero;
			knife_rig.constraints = RigidbodyConstraints.FreezeAll;
			knife_rig.isKinematic = true;

			zombie = other.gameObject;
			knife.transform.SetParent (zombie.transform);
			local_position = knife.transform.localPosition;
			local_rotation = knife.transform.localRotation;

			rig.constraints = RigidbodyConstraints.None;
			rig.AddForce ((other.gameObject.transform.position - knife.transform.position).normalized * 100.0f);


			audio_zombie.Play ();


			zombie.GetComponent<MovableCharacter> ().Kill_zombie ();

			stucked = true;
		}
	}

	void Update()
	{

		if(stucked)
		{
			knife.transform.localPosition = local_position;
			knife.transform.localRotation = local_rotation;
		}
	}
}
