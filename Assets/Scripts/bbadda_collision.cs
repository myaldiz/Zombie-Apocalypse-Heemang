using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bbadda_collision : MonoBehaviour {
	private AudioSource audio_bbadda;
	public AudioClip bbadda_sound;

	void Start(){
		this.audio_bbadda = this.gameObject.AddComponent<AudioSource> ();
		this.audio_bbadda.clip = bbadda_sound;
		this.audio_bbadda.volume = 0.2f;
		this.audio_bbadda.loop = false;
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag ("Zombie"))
		{
			
			StartCoroutine(kill_zombie_after_strike(other.gameObject));
		}
	}

	IEnumerator kill_zombie_after_strike(GameObject other)
	{
		this.audio_bbadda.Play ();
		yield return new WaitForSeconds (0.7f);
		other.GetComponent<MovableCharacter> ().Kill_zombie ();
	}
}
