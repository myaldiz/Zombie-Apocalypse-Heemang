using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell_Control : MonoBehaviour {

	private float curve = 0.0f;
	public float t = 15.0f;
	public bool first_bell_end = false;
	public GameObject spot_lights;
	public Light[] light_spot;
	private Material mat;
	public Material mat_no_emission;
    GameObject game_manager;
	private bool ringing = false;

	private AudioSource audio_bgm;
	public AudioClip bgm;

	private AudioSource audio_siren;
	public AudioClip siren;

    void Start() {
		Renderer renderer = GetComponent<Renderer> ();
		mat = renderer.materials [1];
        game_manager = GameObject.FindGameObjectWithTag("GameManager");
        game_manager.GetComponent<GameControl>().bell_control = this;

		this.audio_siren = this.gameObject.AddComponent<AudioSource> ();
		this.audio_siren.clip = siren;
		this.audio_siren.volume = 0.1f;
		this.audio_siren.loop = false;

		this.audio_bgm = this.gameObject.AddComponent<AudioSource> ();
		this.audio_bgm.clip = bgm;
		this.audio_bgm.volume = 0.3f;
		this.audio_bgm.loop = false;

    }

	IEnumerator RotateBells(bool restart)
    {
        while (true)
        {
            spot_lights.transform.Rotate(new Vector3(0, 0, 1), 4.0f);
            curve += 0.03f;
            t -= 0.02f;
            t = Mathf.Max(0.0f, t);
            foreach (Light l in light_spot)
            {
                l.intensity = t * (Mathf.Sin(curve) + 1);
            }

            if (t <= 0.01f)
            {
                Material[] new_mats = GetComponent<Renderer>().materials;
                new_mats[1] = mat_no_emission;
                GetComponent<Renderer>().materials = new_mats;
				if (!restart) {
					this.audio_siren.Stop ();
					this.audio_bgm.Play ();
				}
				ringing = false;
				first_bell_end = true;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

	public void StartRotateBells(float time)
	{
		if (ringing)
			return;

		ringing = true;
		t = time;

		/* Set initial emission material */
		Material[] new_mats = GetComponent<Renderer>().materials;
		new_mats[1] = mat;
		GetComponent<Renderer>().materials = new_mats;

		bool restart = false;

		if(time != 5.0f)
		{
			this.audio_bgm.Pause ();
			this.audio_siren.Play ();
			restart = true;
		}
		StartCoroutine(RotateBells(restart));
    }
   
}
