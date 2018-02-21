using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnBox
{
    public Vector3 center;
    public Vector3 size;
}

public class GameControl : MonoBehaviour {

    // Public variables for game
    public float spawn_rate = 1f;
    public bool draw_gizmo = false;
    public List<SpawnBox> spawn_boxes;

    //Prefabs
    public Object zombie_prefab;
    public Object knife_prefab;

    //References to other scripts
    public CameraChanger cam_control;
    public Bell_Control bell_control;
    private PlayerController player_controller;

    //private variables
    Transform manager_transform;
    bool is_perform_spawn = false;
    int num_zombies = 0;

	//sound

	private AudioSource audio_button;
	public AudioClip button_sound;

	// UI
	public UnityEngine.UI.Scrollbar Timer_UI;
	public float game_time = 0.0f;
	public float game_clear_time  = 1000.0f;
	public GameObject gameOverText;
	public GameObject gameClearText;

	private Trap trapControl;

    IEnumerator PerformSpawningZombies()
    {
        while (is_perform_spawn)
        {
            System.Random rnd = new System.Random();
			int spawn_index = rnd.Next(spawn_boxes.Count);
			/* Reduce student room zombie generation rate */
			if (spawn_index >= 3)
			{
				if (rnd.Next (10) >= 3)
					spawn_index = rnd.Next (3);
			}
            Vector3 position = spawn_boxes[spawn_index].center;
            float pos_x = Random.Range(-spawn_boxes[spawn_index].size.x / 2f, spawn_boxes[spawn_index].size.x / 2f);
            float pos_z = Random.Range(-spawn_boxes[spawn_index].size.z / 2f, spawn_boxes[spawn_index].size.z / 2f);
            position += new Vector3(pos_x, -1f, pos_z);
            Object zombie_instance = Instantiate(zombie_prefab, position, Quaternion.identity);

            ZombieBorn();
            yield return new WaitForSeconds(spawn_rate); 
        }
    }


    private void Start()
    {
        manager_transform = this.GetComponent<Transform>();
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        InstantiateKnife();

		//Sound configure
		this.audio_button = this.gameObject.AddComponent<AudioSource> ();
		this.audio_button.clip = button_sound;
		this.audio_button.volume = 0.3f;
		this.audio_button.loop = false;

		this.trapControl = this.gameObject.GetComponent<Trap> ();
    }

    public void ZombieDead()
    {
        num_zombies--;   
    }
    public void ZombieBorn()
    {
        num_zombies++;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown (KeyCode.A))
			GameOver ();
		
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!is_perform_spawn)
            {
                is_perform_spawn = true;
                StartCoroutine(PerformSpawningZombies());
                bell_control.StartRotateBells(15.0f);

            }
            else
            {
                is_perform_spawn = false;
            }
        }

		if (is_perform_spawn)
		{
			game_time += 0.1f;
			Timer_UI.image.fillAmount = Mathf.Min(1.0f, game_time / game_clear_time);

			if (game_time >= game_clear_time)
			{
				gameClearText.SetActive (true);
				is_perform_spawn = false;
			}
		}

		/* Test Code */
        if (Input.GetKey(KeyCode.Alpha1))
        {
			cam_control.turn_cam (0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
			cam_control.turn_cam (1);
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			cam_control.turn_cam (2);
		}
		if (Input.GetKey(KeyCode.Keypad1))
		{
			button_event ("button_2");
		}
		if (Input.GetKey(KeyCode.Keypad2))
		{
			button_event ("button_3");
		}
		if (Input.GetKey(KeyCode.Keypad4))
		{
			button_event ("button_0");
		}
		if (Input.GetKey(KeyCode.Keypad5))
		{
			button_event ("button_1");
		}
    }
    private void OnDrawGizmos()
    {
        if (draw_gizmo)
        {
            if (spawn_boxes != null)
            {
                foreach (SpawnBox box in spawn_boxes)
                {
                    Gizmos.DrawWireCube(box.center, box.size);
                }
            }

        }
    }
    public void button_event(string tag)
    {
		this.audio_button.Play ();
        if (tag == "channel_1")
        {

            cam_control.turn_cam(0);
		}
		else if (tag == "channel_2")
		{
			cam_control.turn_cam(1);

		}
		else if (tag == "channel_3")
		{
			cam_control.turn_cam(2);

		}
		else if (tag == "button_0" && player_controller.Mana >= 20.0f)
        { 
			trapControl.toggle_kill_student_room1 (true, player_controller);
        }
		else if (tag == "button_1" && player_controller.Mana >= 20.0f)
		{
			trapControl.toggle_kill_student_room2 (true, player_controller);
        }
		else if (tag == "button_2" && player_controller.Mana >= 30.0f)
		{
			trapControl.toggle_kill_corridor (true, player_controller);
		}
		else if (tag == "button_3" && player_controller.Mana >= 50.0f)
		{
			trapControl.toggle_kill_advisor_room (true, player_controller);
		}
    }
    public void InstantiateKnife()
    {
        Instantiate(knife_prefab, this.transform);
    }

	public void GameOver()
	{
		is_perform_spawn = false;
		gameOverText.SetActive (true);
	}
}
