using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCheck : MonoBehaviour {

    GameControl control;
    BoxCollider box_col;
	bool instant_stat = false;
	bool knife_need = false;
    void Start () {
        control = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
        box_col = this.GetComponent <BoxCollider>();
    }
	
	IEnumerator Knife_Instantiate()
	{
		yield return new WaitForSeconds(0.75f);
		control.InstantiateKnife ();
		yield return new WaitForSeconds(1f);
		instant_stat = false;

	}
    private void OnTriggerExit(Collider other)
    {
		if (!instant_stat) {
			instant_stat = true;
			StartCoroutine (Knife_Instantiate ());
		}
		else
			knife_need = true;
    }
	void Update()
	{
		if (knife_need && !instant_stat) {
			knife_need = false;
			instant_stat = true;
			StartCoroutine (Knife_Instantiate ());
		}
			
	}


}
