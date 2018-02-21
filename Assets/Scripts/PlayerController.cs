using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

	public float Health;
	public float Mana;
	public float ManaRegenRatio = 0.03f;
	public UnityEngine.UI.Slider health_bar;
	public UnityEngine.UI.Slider mana_bar;

	public float playerRadius;
	public LayerMask zombieMask;
	private GameObject zombie;

	public GameControl gameControl;

	NavMeshAgent agent;

	public void DamagePlayer ()
	{
		Health -= 1f;
		//Debug.Log ("Health : " + " " + Health);
		if (Health <= 0)
		{
			gameControl.GameOver ();
		}
	}

	void Update ()
	{
		health_bar.value = Health;
		mana_bar.value = Mana;
        
		// Mana auto generation
		Mana = Mathf.Min (100.0f, Mana + ManaRegenRatio);
	}
}
