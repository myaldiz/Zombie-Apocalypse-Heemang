using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SerializeField]
public class MovableCharacter : MonoBehaviour {

    
    public ObjectType my_type;
    public bool is_Zombie_active = true;

    private Nodes nodes;
    private Transform char_pos;
    GameObject game_manager;
    GameControl game_control;
    Animator zombie_animator;
    Vector2 input;
    private bool attack;

    private float alpha = 1.0f;
    public Material zombie_transparent;
	public Renderer zombie_renderer;

    public int method_move = 0;
    public float walking_speed = 1.0f;
    
	GameObject player;
	public float damage_radius;
	Vector3 dist_zombie_human;

    NavMeshAgent agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    bool dest_set = false;
    public float m_Damping = 0.15f;
	bool is_alive = true;
	bool is_attack = false;

    private GameObject zombie;

    void Start()
    {
        char_pos = GetComponent<Transform>();
        game_manager = GameObject.FindGameObjectWithTag("GameManager");
        game_control = game_manager.GetComponent<GameControl>();
        zombie_animator = GetComponent<Animator>();
        zombie_animator.speed = walking_speed;
        agent = GetComponent<NavMeshAgent>();
        attack = false;

        player = GameObject.FindGameObjectWithTag("Player");
        //Other technique
        agent.updatePosition = false;

    }

    IEnumerator StartKillingZombie()
    {
        while (true)
        {
            alpha -= 0.003f;
            Color current_color = zombie_renderer.material.color;
            Color new_color = new Color(current_color.r, current_color.g, current_color.b, alpha);
            zombie_renderer.material.color = new_color;
            if (alpha <= 0.001f)
                break;
            yield return new WaitForFixedUpdate();
        }
        Destroy(this.gameObject);
    }

    public void Kill_zombie()
    {
		is_alive = false;
		agent.isStopped = true;
		zombie_animator.SetBool("Die", true);
		Material cloned_material = new Material(zombie_transparent);
        zombie_renderer.material = cloned_material;

        this.gameObject.layer = LayerMask.NameToLayer("Zombie_dead");

        game_control.ZombieDead();
        StartCoroutine(StartKillingZombie());
    }

    private bool Init_Node()
    {
        if (nodes == null)
            nodes = game_manager.GetComponent<PathFindingGrid>().nodes;
        if (nodes != null)
        {
            if (!nodes.is_flow_field_read)
            {
                nodes.ReadFlowField();
            }
            return true;
        }
        return false;
    }


    private void FixedUpdate()
    {
        if (method_move == 0)
        {
			if (is_Zombie_active && Init_Node() && is_alive && !is_attack)
            {
                zombie_animator.SetBool("Move", true);
                //Vector3 direction = nodes.GetFlowDirection(char_pos.position); //Not Interpolated
                Vector3 direction = nodes.GetInterpolatedFlowDir(char_pos.position);
                zombie_animator.SetFloat("Horizontal", direction.x);
                zombie_animator.SetFloat("Vertical", direction.z);
                //Debug.Log(zombie_animator.deltaPosition);
                Vector3 pos_dif = direction * Time.fixedDeltaTime * walking_speed * 0.307f;
                //Debug.Log(zombie_animator.pivotPosition);
                gameObject.transform.position += pos_dif;
            }   
        }
        else
        {
            if (!dest_set)
            {
                dest_set = true;
                
            }
			agent.SetDestination(player.transform.position);
        }
    }
    private void Update()
    {
        
		if (is_alive && method_move == 1) {
			Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

			// Map 'worldDeltaPosition' to local space
			float dx = Vector3.Dot (transform.right, worldDeltaPosition);
			float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
			Vector2 deltaPosition = new Vector2 (dx, dy);

			// Low-pass filter the deltaMove
			float smooth = Mathf.Min (1.0f, Time.deltaTime / 0.15f);
			smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

			// Update velocity if time advances
			if (Time.deltaTime > 1e-5f) {
				velocity = smoothDeltaPosition / Time.deltaTime;
				//velocity = deltaPosition / Time.deltaTime;
				//Debug.Log(" Vel: " + velocity.magnitude + " Delta Pos: " + worldDeltaPosition);
			}
			bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;
			//Debug.Log("Move:" + shouldMove + " Vel: " + velocity.magnitude);

			// Update animation parameters
			zombie_animator.SetBool ("Move", shouldMove);
			zombie_animator.SetFloat ("Horizontal", velocity.x, m_Damping, Time.deltaTime);
			zombie_animator.SetFloat ("Vertical", velocity.y, m_Damping, Time.deltaTime);

		}
		if (is_alive) {
			
			dist_zombie_human = transform.position - player.transform.position;
			if (!is_attack && dist_zombie_human.magnitude < damage_radius) {
				is_attack = true;
				StartCoroutine (check_distance ());


				Vector3 rotate_vec = new Vector3( player.transform.position.x, transform.position.y, player.transform.position.z ) ;

				transform.LookAt (rotate_vec);
			}
		}

        //GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;
    }

		IEnumerator check_distance()
		{
			while (true) {
				
				player.GetComponent<PlayerController> ().DamagePlayer ();
				zombie_animator.SetBool("Attack", true);//Animation for attack

				yield return new WaitForSeconds (1.0f);
				
				dist_zombie_human = transform.position - player.transform.position;
				if (dist_zombie_human.magnitude > damage_radius) {
				zombie_animator.SetBool ("Attack", false);

				break;
				}
			}
		is_attack = false;
		}
    private void OnAnimatorMove()
    {
        // Update position to agent position
        //Debug.Log(transform.position + " , " + agent.nextPosition);
        if (method_move == 1)
        {
            Vector3 position = zombie_animator.rootPosition;
            position.y = agent.nextPosition.y;
            transform.position = position;
        }
    }
}
