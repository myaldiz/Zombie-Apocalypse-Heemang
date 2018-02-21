using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombie_detection : MonoBehaviour {

	public GameControl gameControl;
	public Bell_Control bellControl;

	public Vector3 room_center;
	public Vector3 room_size;

	private float timer = 0.0f;

	void Update()
	{
		if (has_zombie())
		{
			timer += 0.1f;
		} else
		{
			timer = 0.0f;
		}

		if (timer >= 50.0f)
		{
			bellControl.StartRotateBells (5.0f);
		}

	}
	void LateUpdate()
	{
		if (timer >= 100.0f)
		{
			gameControl.GameOver ();
		}
	}

	private bool has_zombie()
	{
		foreach (Collider c in Physics.OverlapBox (room_center, room_size))
		{
			if (c.gameObject.CompareTag ("Zombie"))
			{
				return true;
			}
		}

		return false;
	}
}
