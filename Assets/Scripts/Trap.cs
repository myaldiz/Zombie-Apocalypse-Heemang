using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

	// See Trap collider
	public GameObject advisor_room;
	private Vector3 advisor_room_center = new Vector3(0.0f, 1.2f, 0.5f);
	private Vector3 advisor_room_size = new Vector3(3.0f, 1.5f, 5.0f);
	public GameObject advisor_room_effect;

	public GameObject corridor;
	private Vector3 corridor_center = new Vector3(0.0f, 1.2f, 12f);
	private Vector3 corridor_size = new Vector3(6.0f, 1.5f, 6.5f);
	public GameObject corridor_effect;

	public GameObject student_room1;
	private Vector3 student_room1_center = new Vector3(-26.62f, 1.89f, -65.07f);
	private Vector3 student_room1_size = new Vector3(5.5f, 2.0f, 2.5f);
	public GameObject student_room1_effect;

	public GameObject student_room2;
	private Vector3 student_room2_center = new Vector3(-26.62f, 1.89f, -50.17f);
	private Vector3 student_room2_size = new Vector3(5.5f, 2.0f, 2.5f);
	public GameObject student_room2_effect;

	public bool kill_advisor_room = false;
	public bool kill_corridor = false;
	public bool kill_student_room1 = false;
	public bool kill_student_room2 = false;

	private AudioSource audio_flame;
	public AudioClip flame_sound;
	private AudioSource audio_elec;
	public AudioClip elec_sound;



	void Start(){
		this.audio_flame = this.gameObject.AddComponent<AudioSource> ();
		this.audio_flame.clip = flame_sound;
		this.audio_flame.volume = 0.2f;
		this.audio_flame.loop = false;

		this.audio_elec = this.gameObject.AddComponent<AudioSource> ();
		this.audio_elec.clip = elec_sound;
		this.audio_elec.volume = 0.2f;
		this.audio_elec.loop = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (kill_advisor_room)
		{
			foreach (Collider c in Physics.OverlapBox (advisor_room_center, advisor_room_size))
			{
				if (c.gameObject.CompareTag ("Zombie"))
				{
					c.gameObject.GetComponent<MovableCharacter> ().Kill_zombie ();
				}
			}
		}
		if (kill_corridor)
		{
			foreach (Collider c in Physics.OverlapBox (corridor_center, corridor_size))
			{
				if (c.gameObject.CompareTag ("Zombie"))
				{
					c.gameObject.GetComponent<MovableCharacter> ().Kill_zombie ();
				}
			}
		}
		if (kill_student_room1)
		{
			foreach (Collider c in Physics.OverlapBox (student_room1_center, student_room1_size))
			{
				if (c.gameObject.CompareTag ("Zombie"))
				{
					c.gameObject.GetComponent<MovableCharacter> ().Kill_zombie ();
				}
			}
		}
		if (kill_student_room2)
		{
			foreach (Collider c in Physics.OverlapBox (student_room2_center, student_room2_size))
			{
				if (c.gameObject.CompareTag ("Zombie"))
				{
					c.gameObject.GetComponent<MovableCharacter> ().Kill_zombie ();
				}
			}
		}
	}

	public void toggle_kill_advisor_room(bool start, PlayerController player_controller)
	{
		/* Detect overlap function calling */
		if (kill_advisor_room == start)
			return;
		
		kill_advisor_room = start;
		// start/stop effect
		if (start)
		{
			this.audio_flame.Play ();
			this.audio_flame.Play ();
			GameObject effect = Instantiate (advisor_room_effect);
			effect.transform.SetParent (advisor_room.transform, false);
			player_controller.Mana -= 50.0f;
			StartCoroutine (EndEffect_advisor ());
		}
	}
	public void toggle_kill_corridor(bool start, PlayerController player_controller)
	{
		/* Detect overlap function calling */
		if (kill_corridor == start)
			return;

		kill_corridor = start;
		// start/stop effect
		if (start)
		{
			this.audio_elec.Play ();
			GameObject effect = Instantiate (corridor_effect);
			effect.transform.SetParent (corridor.transform, false);
			player_controller.Mana -= 30.0f;
			StartCoroutine (EndEffect_corridor ());
		}
	}
	public void toggle_kill_student_room1(bool start, PlayerController player_controller)
	{
		/* Detect overlap function calling */
		if (kill_student_room1 == start)
			return;

		kill_student_room1 = start;
		// start/stop effect
		if (start)
		{
			this.audio_elec.Play ();
			GameObject effect = Instantiate (student_room1_effect);
			effect.transform.SetParent (student_room1.transform, false);
			player_controller.Mana -= 20.0f;
			StartCoroutine (EndEffect_student1 ());
		}
	}
	public void toggle_kill_student_room2(bool start, PlayerController player_controller)
	{
		/* Detect overlap function calling */
		if (kill_student_room2 == start)
			return;

		kill_student_room2 = start;
		// start/stop effect
		if (start)
		{
			this.audio_elec.Play ();
			GameObject effect = Instantiate (student_room2_effect);
			effect.transform.SetParent (student_room2.transform, false);
			player_controller.Mana -= 20.0f;
			StartCoroutine (EndEffect_student2 ());
		}
	}

	IEnumerator EndEffect_advisor ()
	{
		yield return new WaitForSeconds (5f);
		kill_advisor_room = false;
	}
	IEnumerator EndEffect_corridor ()
	{
		yield return new WaitForSeconds (5f);
		kill_corridor = false;
	}
	IEnumerator EndEffect_student1 ()
	{
		yield return new WaitForSeconds (5f);
		kill_student_room1 = false;
	}
	IEnumerator EndEffect_student2 ()
	{
		yield return new WaitForSeconds (5f);
		kill_student_room2 = false;
	}
}
