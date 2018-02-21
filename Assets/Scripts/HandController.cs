using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
	public Trap trapControl;

	public AudioClip AC;
	private int wave = 0;
	private float size = 1.5f;

	private GameObject sphere;

	public OVRInput.Controller controller;
	public string buttonName;
	public string buttonName_2;

	private GameObject grabbedObject;
	private GameObject pressedObject;

	GameControl game_control;

	private bool grabbing;
	private bool pressing;

	public float grabRadius;
	public float pressRadius;

	public LayerMask grabMask;
	public LayerMask pressMask;

	private Quaternion lastRotation, currentRotation;
	public float power;
	public float rotate_power;
	private Vector3 scale;

	private float i_force;
	private float force_ratio;

	public Material initial_button_material;
	public Material pressed_button_material;
	GameObject[] channels;
	int channel = 0;


	//sound

	private AudioSource audio_knife;
	public AudioClip knife_sound;
	private AudioSource audio_knife_grab;
	public AudioClip knife_grab_sound;
	private AudioSource audio_bbadda_grab;
	public AudioClip bbadda_grab_sound;

	public OculusHapticsController Haptic;


	private void Start ()
	{
		channels = new GameObject[10];
		channels [1] = GameObject.FindGameObjectWithTag ("channel_1");
		channels [2] = GameObject.FindGameObjectWithTag ("channel_2");
		channels [3] = GameObject.FindGameObjectWithTag ("channel_3");
		game_control = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameControl> ();

		this.audio_knife = this.gameObject.AddComponent<AudioSource> ();
		this.audio_knife.clip = knife_sound;
		this.audio_knife.volume = 0.3f;
		this.audio_knife.loop = false;

		this.audio_knife_grab = this.gameObject.AddComponent<AudioSource> ();
		this.audio_knife_grab.clip = knife_grab_sound;
		this.audio_knife_grab.volume = 0.3f;
		this.audio_knife_grab.loop = false;

		this.audio_bbadda_grab = this.gameObject.AddComponent<AudioSource> ();
		this.audio_bbadda_grab.clip = bbadda_grab_sound;
		this.audio_bbadda_grab.volume = 0.3f;
		this.audio_bbadda_grab.loop = false;


	}

	void OnTriggerEnter (Collider c)
	{
		//Debug.Log("trigger");
		PlayHaptics (c);
	}

	void PlayHaptics (Collider c)
	{
		Haptic.Vibrate (VibrationForce.Hard);
	}

	void GrabObject ()
	{

		//Debug.Log("grab");
		grabbing = true;
		RaycastHit[] hits;
		hits = Physics.SphereCastAll (transform.position, grabRadius, transform.forward, 0f, grabMask);
		if (hits.Length > 0)
		{
			int closestHit = 0;
			for (int i = 0; i < hits.Length; i++)
			{
				if (hits [i].distance > hits [closestHit].distance)
					closestHit = i;
			}
			grabbedObject = hits [closestHit].transform.gameObject;
			grabbedObject.GetComponent<Rigidbody> ().isKinematic = true;
			grabbedObject.transform.position = transform.position;
			grabbedObject.transform.rotation = transform.rotation;//position of pivot should be at hand grip
			if (grabbedObject.tag == "bbadda")
			{
				audio_bbadda_grab.Play ();
				grabbedObject.transform.Rotate (Vector3.right, -70);
			} else if (grabbedObject.tag == "knife")
			{
				audio_knife_grab.Play ();
				grabbedObject.transform.Rotate (Vector3.up, 180);
			}
			grabbedObject.transform.parent = transform;

		}

	}

	void DropObject ()
	{
		grabbing = false;
		if (grabbedObject != null)
		{
			
			grabbedObject.transform.parent = null;
			grabbedObject.GetComponent<Rigidbody> ().isKinematic = false;
			if (grabbedObject.tag == "knife")
			{
				
				Vector3 tmp = OVRInput.GetLocalControllerVelocity (controller);
				i_force = Mathf.Sqrt (Mathf.Pow (tmp.x, 2) + Mathf.Pow (tmp.z, 2));
				if (i_force < 2)
				{
					force_ratio = Mathf.Sqrt (2) / i_force;

				}
				//Debug.Log (i_force);
				//tmp.y = 0.1f;
				tmp.y = 0f;
				tmp.x = tmp.x * force_ratio;
				tmp.z = tmp.z * force_ratio;

				grabbedObject.GetComponent<Rigidbody> ().velocity = power * tmp;
				//Vector3 angular_v = GetAngularVelocity();
				Vector3 angular_v = new Vector3 (-2f, 0f, 0f);
				//Debug.Log(angular_v);
				//grabbedObject.GetComponent<Rigidbody>().angularVelocity = rotate_power * angular_v;
				grabbedObject.GetComponent<Rigidbody> ().AddRelativeTorque (rotate_power * angular_v, ForceMode.Impulse);

				audio_knife.Play ();
				grabbedObject = null;
			} else
			{
				
				grabbedObject = null;
			}

     
		}
	}

	Vector3 GetAngularVelocity ()
	{
		Quaternion deltaRotation = currentRotation * Quaternion.Inverse (lastRotation);
		return new Vector3 (Mathf.DeltaAngle (0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle (0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle (0, deltaRotation.eulerAngles.z));
	}

	void PressOn ()
	{
		//Debug.Log ("press");
		pressing = true;
		RaycastHit[] hits;
		hits = Physics.SphereCastAll (transform.position, pressRadius, transform.forward, 0f, pressMask);
		if (hits.Length > 0)
		{
			int closestHit = 0;
			for (int i = 0; i < hits.Length; i++)
			{
				if (hits [i].distance > hits [closestHit].distance)
					closestHit = i;
			}
			pressedObject = hits [closestHit].transform.gameObject;
			Vector3 scale = pressedObject.transform.localScale;
			pressedObject.transform.localScale = new Vector3 (scale.x, scale.y, scale.z * 0.5f);
			pressedObject.GetComponent<Renderer> ().material = pressed_button_material;

			game_control.button_event (pressedObject.tag);
		}
	}

	IEnumerator ChangeGlowEffect (Renderer in_render, string tag)
	{
		yield return new WaitForSeconds (10f);
		in_render.material = initial_button_material;
		if (tag == "button_3")
			trapControl.toggle_kill_advisor_room (false, null);
		if (tag == "button_2")
			trapControl.toggle_kill_corridor (false, null);
		if (tag == "button_1")
			trapControl.toggle_kill_student_room2 (false, null);
		if (tag == "button_0")
			trapControl.toggle_kill_student_room1 (false, null);
	}

	void PressOff ()
	{
		pressing = false;
		if (pressedObject != null)
		{
			if (pressedObject.tag == "channel_1")
			{
				Vector3 scale = pressedObject.transform.localScale;
				pressedObject.transform.localScale = new Vector3 (scale.x, scale.y, scale.z / 0.5f);
				channels [1].GetComponent<Renderer> ().material = initial_button_material;
				pressedObject.transform.parent = null;
				pressedObject = null;
			} else if (pressedObject.tag == "channel_2")
			{
				Vector3 scale = pressedObject.transform.localScale;
				pressedObject.transform.localScale = new Vector3 (scale.x, scale.y, scale.z / 0.5f);
				channels [2].GetComponent<Renderer> ().material = initial_button_material;
				pressedObject.transform.parent = null;
				pressedObject = null;
			} else if (pressedObject.tag == "channel_3")
			{
				Vector3 scale = pressedObject.transform.localScale;
				pressedObject.transform.localScale = new Vector3 (scale.x, scale.y, scale.z / 0.5f);
				channels [3].GetComponent<Renderer> ().material = initial_button_material;
				pressedObject.transform.parent = null;
				pressedObject = null;
			} else if (pressedObject.tag.StartsWith("button_"))
			{
				Vector3 scale = pressedObject.transform.localScale;
				pressedObject.transform.localScale = new Vector3 (scale.x, scale.y, scale.z / 0.5f);
				//StartCoroutine(ChangeGlowEffect(GameObject.FindGameObjectWithTag (pressedObject.tag).GetComponent<Renderer> (), pressedObject.tag));
				GameObject.FindGameObjectWithTag (pressedObject.tag).GetComponent<Renderer> ().material = initial_button_material;
				pressedObject.transform.parent = null;
				pressedObject = null;
			}
		}

	}

	void Update ()
	{
		//Haptic.Vibrate(VibrationForce.Hard);
		//https://unity3d.college/2017/02/15/oculus-haptic-feedback/
		if (grabbedObject != null)
		{
			lastRotation = currentRotation;
			currentRotation = grabbedObject.transform.rotation;
		}

		if (!grabbing && Input.GetAxis (buttonName) == 1)
			GrabObject ();
		if (grabbing && Input.GetAxis (buttonName) < 1)
			DropObject ();

		if (!pressing && Input.GetAxis (buttonName_2) == 1)
			PressOn ();
		if (pressing && Input.GetAxis (buttonName_2) < 1)
			PressOff ();


	}
}
