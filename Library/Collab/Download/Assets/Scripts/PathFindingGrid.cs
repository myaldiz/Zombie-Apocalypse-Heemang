using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingGrid : MonoBehaviour {

    public LayerMask unwalkable_mask;
    public Vector2 grid_world_size;
    public Nodes nodes;
    public GameObject player;
    public bool draw_gizmo = false;
    

    public float node_length;

    private void Start()
    {
        Vector3 world_left_bottom = transform.position - Vector3.right * grid_world_size.x / 2.0f - Vector3.forward * grid_world_size.y / 2.0f;
        nodes = new Nodes(world_left_bottom, grid_world_size, node_length, unwalkable_mask);
        nodes.player = player;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            nodes.CalculateFlowField();  
        }
        if (Input.GetKey(KeyCode.S))
        {
            nodes.SaveIntegrateField();
        }
        if (Input.GetKey(KeyCode.T))
        {
            nodes.Test();
        }
        if (Input.GetKey(KeyCode.G))
        {
            draw_vector_gizmo = true;
        }
        if (nodes != null && !nodes.is_flow_field_read)
        {
            nodes.ReadFlowField();
        }

    }

    bool draw_vector_gizmo = false;
    private void OnDrawGizmos()
    {

        if (draw_gizmo)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(grid_world_size.x, 1, grid_world_size.y));

            if (nodes != null)
            {
                //nodes.player = player;

                foreach (Node node in nodes)
                {
                    if (!draw_vector_gizmo)
                    {

                        Gizmos.color = node.IsWalkable() ? Color.white : (node.IsPlayer() ? Color.blue : (node.IsZombie() ? Color.green : Color.red));
                        if (node.is_path)
                            Gizmos.color = Color.black;
                        Gizmos.DrawCube(node.world_pos, Vector3.one * node_length / 1.8f);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        DrawArrow.ForGizmo(node.world_pos, node.flow_field);

                    }

                }

                /*if (randon)
                {
                    System.Random rnd = new System.Random();
                    randon = false;
                    r1 = rnd.Next(0, 99);
                    r2 = rnd.Next(0, 99);
                    r3 = rnd.Next(0, 99);
                    r4 = rnd.Next(0, 99);
                }
                    Node node1 = nodes.GetNode(r1, r2);
                    Node node2 = nodes.GetNode(r3, r4);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(node1.world_pos, Vector3.one * node_length / 1.8f);
                    Gizmos.DrawCube(node2.world_pos, Vector3.one * node_length / 1.8f);

                    Gizmos.color = Color.magenta;
                    DrawPath(r1, r2, r3, r4);

                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(node1.world_pos, Vector3.one * node_length / 1.8f);
                    Gizmos.DrawCube(node2.world_pos, Vector3.one * node_length / 1.8f);

                 */
                //Debug.Log(nodes.IsDirectPath(21, 20, 20, 35));
            }
        }
    }

    //int r1 = 0, r2 = 0, r3 = 0, r4 = 0;
    public bool randon = false;
    private void DrawPath(int x0, int y0, int x1, int y1)
    {
        if (x1 < x0)
        {
            int temp = x1;
            x1 = x0;
            x0 = temp;
            temp = y1;
            y1 = y0;
            y0 = temp;
        }

        double delta_x = x1 - x0;
        double delta_y = y1 - y0;
        double delta_err = 0;

        if (x0 == x1)
        {
            for (int i = System.Math.Min(y0, y1); i <= System.Math.Max(y0, y1); i++)
            {
                //Debug.Log(x0.ToString() + ", " + i.ToString());
                Gizmos.DrawCube(nodes.World_pos(x0, i), Vector3.one * node_length / 1.8f);
            }
        }
        else
            delta_err = System.Math.Abs(delta_y / delta_x);

        double error = 0;
        int y = y0;
        for (int x = x0; x < x1; x++)
        {
            //Debug.Log(x.ToString() + ", " + y.ToString());
            Gizmos.DrawCube(nodes.World_pos(x, y), Vector3.one * node_length / 1.8f);

            error = error + delta_err;
            while (error >= 0.5f)
            {
                Gizmos.DrawCube(nodes.World_pos(x, y), Vector3.one * node_length / 1.8f);
                y += (int)(delta_y / System.Math.Abs(delta_y));
                error -= 1.0f;
                //Debug.Log("Error: " + error + " x:" + x + " y:" + y);

            }
        }
        Gizmos.DrawCube(nodes.World_pos(x1, y1), Vector3.one * node_length / 1.8f);
    }

    
}
                                                      