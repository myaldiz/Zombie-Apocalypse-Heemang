using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMovableObject : MonoBehaviour {

    private Transform object_pos;
    private Vector3 prev_pos;
    private Vector3 cur_pos;
    public GameObject PathFinding;
    public ObjectType my_type;
    private Nodes nodes;

    void UpdateGridPos()
    {
        cur_pos = object_pos.position;
        if (nodes != null && prev_pos != cur_pos)
        {
            ObjectType prev_type = nodes.World2Node(prev_pos).obj_type;
            nodes.World2Node(prev_pos).obj_type = prev_type == my_type ? ObjectType.Walkable : prev_type;
            nodes.World2Node(cur_pos).obj_type = my_type;
            prev_pos = cur_pos;
        }
        else
        {
            nodes = PathFinding.GetComponent<PathFindingGrid>().nodes;
        }
    }

    void Start () {
        object_pos = GetComponent<Transform>();
        prev_pos = object_pos.position;
        cur_pos = object_pos.position;
    }
	
    
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.P))
        {
            nodes.CalculatePath(nodes.World2Node(cur_pos).grid_x, nodes.World2Node(cur_pos).grid_y);
        }
        UpdateGridPos();   		
	}
}
