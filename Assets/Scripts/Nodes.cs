using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public enum ObjectType
{
    Obstacle,
    Walkable,
    Player,
    Zombie
}
public enum FilterType
{
    Plain,
    Gaussian3,
    Gaussian5
}

public class Node : IHeapItem<Node>
{
    public ObjectType obj_type;
    public int grid_x, grid_y;
    public Vector3 world_pos;
    public float cost_val;
    public Vector3 flow_field = Vector3.zero;
    public bool mem_closed_set;
    public bool is_path = false;
    public Node parent;
    public List<Node> neighbour;
    int heap_index;

    public int HeapIndex
    {
        get
        {
            return heap_index;
        }

        set
        {
            heap_index = value;
        }
    }
    int IComparable<Node>.CompareTo(Node other)
    {
        return other.cost_val.CompareTo(this.cost_val);
        //return cost_val.CompareTo(other.cost_val);

    }

    public Node(ObjectType obj, Vector3 _worldPos, int x, int y)
    {
        obj_type = obj;
        world_pos = _worldPos;
        grid_x = x;
        grid_y = y;
        neighbour = new List<Node>();
    }
    public bool IsWalkable()
    {
        return obj_type == ObjectType.Walkable;
    }
    public bool IsPlayer()
    {
        return obj_type == ObjectType.Player;   
    }

    public bool IsZombie()
    {
        return obj_type == ObjectType.Zombie;
    }

    /*int IComparable<Node>.CompareTo(Node other)
    {
        return (this.cost_val - other.cost_val == 0 ? 0 : (this.cost_val - other.cost_val > 0) ? 1 : -1);
    }*/
}

public class Nodes {

    Node[,] grid;
    public int size_x, size_y;
    public float node_length;
    public Vector2 grid_world_size;
    public Vector3 world_left_bottom;
    public GameObject player;
    public Vector3 player_pos;
    public FilterType filter_type = FilterType.Gaussian5;
    private bool is_adjacency_calculated = false;
    public bool is_flow_field_read = false;

    public Nodes(Vector3 world_left_bottom, Vector2 grid_world_size, float node_length, LayerMask mask)
    {
        this.grid_world_size = grid_world_size;
        this.world_left_bottom = world_left_bottom;
        this.node_length = node_length;

        Vector2 size = (grid_world_size / node_length);
        size_x = Mathf.RoundToInt(size.x);
        size_y = Mathf.RoundToInt(size.y);

        grid = new Node[size_x, size_y];

        bool is_walkable = false;
             
        for (int i = 0; i < size_x; i++)
            for (int j = 0; j < size_y; j++)
            {
                Vector3 world_point = world_left_bottom + Vector3.right * (i * node_length + node_length / 2) + Vector3.forward * (j * node_length + node_length / 2);
                Vector3 boxSize = new Vector3(node_length / 2f, 1f, node_length / 2);
                is_walkable = !Physics.CheckBox(world_point, boxSize, Quaternion.identity, mask);
                grid[i, j] = new Node(is_walkable ? ObjectType.Walkable : ObjectType.Obstacle, world_point, i, j);
            }
                                        
    }

    public bool IsWalkable(int i, int j)
    {
        return grid[i, j].IsWalkable();
    }

    public Vector3 World_pos(int i, int j)
    {
        return grid[i, j].world_pos;
    }
    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }

    public Node World2Node(Vector3 world_point)
    {
        Vector3 dif = (world_point - world_left_bottom - Vector3.one * (node_length / 2.0f));
        Vector2 pos = new Vector2(dif.x / node_length, dif.z / node_length);
        int index_x = pos.x > 0 ? ( pos.x < size_x ? Mathf.RoundToInt(pos.x) : size_x - 1) : 0;
        int index_y = pos.y > 0 ? (pos.y < size_y ? Mathf.RoundToInt(pos.y) : size_y - 1) : 0;

        //Debug.Log(index_x.ToString() + " , " + index_y.ToString());
        return grid[index_x, index_y];
    }

    public IEnumerator GetEnumerator()
    {
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                yield return node;
            }
        }
    }

    private void InitGrid()
    {
        foreach (Node node in grid)
        {
            node.cost_val = 10000000f;
            node.mem_closed_set = false;
            node.is_path = false;
        }
        player_pos = player.transform.position;
        World2Node(player_pos).cost_val = 0f;
        //CalculateAdjacency();
    }

    public bool IsDirectPath(int x0, int y0, int x1, int y1)
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
        double delta_err;

        if (!grid[x1, y1].IsWalkable())
            return false;

        if (x0 == x1)
        {
            for (int i = System.Math.Min(y0, y1); i <= System.Math.Max(y0, y1); i++)
            {
                //Debug.Log(x0.ToString() + ", " + i.ToString());
                if (!grid[x0, i].IsWalkable())
                    return false;
            }
            return true;
        }
        else
            delta_err = System.Math.Abs(delta_y / delta_x);

        double error = 0;
        int y = y0;
        for (int x = x0; x < x1; x++)
        {
            //Debug.Log(x.ToString() + ", " + y.ToString());
            if (!grid[x, y].IsWalkable())
                return false;
            error = error + delta_err;
            while (error >= 0.5f)
            {
                if (!grid[x, y].IsWalkable())
                    return false;
                y += (int)(delta_y / System.Math.Abs(delta_y));
                error -= 1.0f;
                //Debug.Log("Error: "+ error + " x:" + x + " y:" + y); 
            }
        }
        
        return true;
    }
    private void CalculateAdjacency()
    {
        //int l = 20;
        //Optimized Method
        
        int total_size = size_x * size_y;
        int i, j, k, l;
        for (int x = 0; x < total_size; x++)
            for (int y = 0; y < x; y++)
            {
                i = x / size_y;
                j = x % size_y;
                k = y / size_y;
                l = y % size_y;
                //Debug.Log("1:" + i + " 2:" + j + " 3:" + k + " 4:" + l);
                if (IsDirectPath(i, j, k, l))
                {
                    grid[i, j].neighbour.Add(grid[k, l]);
                    grid[k, l].neighbour.Add(grid[i, j]);
                }
            }
         

        /*
        for (int x = 0; x < size_x; x++)
            for (int y = 0; y < size_y; y++)
                for (int z = 0; z < size_x; z++)
                    for (int t = 0; t < size_y; t++)
                    {
                        if ((x != z || y != t) && IsDirectPath(x, y, z, t))
                        {
                            grid[x, y].neighbour.Add(grid[z, t]);
                        }
                    }

          */

        //Debug.Log("Finished Neighbour");             
    }

    public void Test()
    {
        //CalculateAdjacency();
        //CalculateIntegrateField();
        /*for (int x = 10; x < 40; x++)
            for (int y = 20; y < 30; y++)
                for (int z = 10; z < 40; z++)
                    for (int t = 33; t < 45; t++)
                    {
                        string out_str = "x:" + x + " y:" + y + " z:" + z + " t:" + t + " Res->" + IsDirectPath(x, y, z, t);
                        Debug.Log(out_str);
                    }
        
        int x = 21, y = 35, z = 20, t = 50;
        string out_str = "x:" + x + " y:" + y + " z:" + z + " t:" + t + " Res->" + IsDirectPath(x, y, z, t);
        Debug.Log(out_str);*/
    }

    private List<Node> GetNeighbours(Node cur)
    {
        //Method 1
        /*List<Node> ret_nodes = new List<Node>();
        for (int i = System.Math.Max(0, cur.grid_x - 1); i <= cur.grid_x + 1 && i < size_x; i++)
            for (int j = System.Math.Max(0, cur.grid_y - 1); j <= cur.grid_y + 1 && j < size_y; j++)
            {
                if (i != cur.grid_x || j != cur.grid_y)
                    ret_nodes.Add(grid[i, j]);
            }
        return ret_nodes;
        */
        //Method 2
        return cur.neighbour;
    }

    private float CalculateCost(int x0, int y0, int x1, int y1)
    {
        float y_delta = y1 - y0;
        float x_delta = x1 - x0;
        y_delta *= y_delta;
        x_delta *= x_delta;
        float cost = Mathf.Sqrt(x_delta + y_delta);
        return cost;
    }
    private void CalculateIntegrateField()
    {
        InitGrid();
        MinHeap<Node> open_set = new MinHeap<Node>(size_x * size_y);

        open_set.Add(World2Node(player_pos));
        while (open_set.Count > 0)
        {
            Node cur_node = open_set.RemoveFirst();
            
            cur_node.mem_closed_set = true;

            foreach (Node neighbour in GetNeighbours(cur_node))
            {
                if (neighbour.IsWalkable() && !neighbour.mem_closed_set)
                {

                    float cost = cur_node.cost_val + CalculateCost(cur_node.grid_x, cur_node.grid_y, neighbour.grid_x, neighbour.grid_y);
                    if (cost < neighbour.cost_val)
                    {
                        neighbour.cost_val = cost;
                        neighbour.parent = cur_node;
                    }
                    if (!open_set.Contains(neighbour))
                        open_set.Add(neighbour);
                    else
                        open_set.UpdateItem(neighbour);
                }

            }
        }
        //Debug.Log()
    }

    public class Filter
    {
        private float[,] filter;
        public Vector2Int size;
        
        public Filter(FilterType filt)
        {
            switch (filt)
            {
                case FilterType.Gaussian3:
                    size = new Vector2Int(3, 3);
                    filter = new float[,] {{-0.112965690469401f, -0.164010679362316f, -0.112965690469401f },
                   { 0f, 0f, 0f }, {0.112965690469401f,  0.164010679362316f,  0.112965690469401f } };
                    break;
                case FilterType.Gaussian5:
                    size = new Vector2Int(5, 5);
                    filter = new float[,] {
                    { -0.021561678804552f,  -0.057723783028055f,  -0.078977313392513f,  -0.057723783028055f,  -0.021561678804552f },
                    { -0.030738137632340f,  -0.082290511952210f,  -0.112589355907622f,  -0.082290511952210f,  -0.030738137632340f },
                    { 0,                   0,                   0,                   0,                   0 },
                    { 0.021561678804552f,  0.057723783028055f,  0.078977313392513f,  0.057723783028055f,  0.021561678804552f },
                    { 0.030738137632340f,  0.082290511952210f,  0.112589355907622f,  0.082290511952210f,  0.030738137632340f }
                    };
                    break;
                case FilterType.Plain:
                    size = new Vector2Int(3, 3);
                    filter = new float[,] { { -0.25f, -0.5f, -0.25f }, { 0, 0, 0 }, { 0.25f, 0.5f, 0.25f } };
                    break;
                default:
                    break;
            }
            
        }
        public float this[int i, int j]
        {
            get
            {
                return filter[i,j];
            }
            set
            {
                filter[i, j] = value;
            }
        }
    }

    public void CalculateFlowField()
    {
        if (!is_adjacency_calculated)
        {
            CalculateAdjacency();
            is_adjacency_calculated = true;
        }
        CalculateIntegrateField();
        //Filter filter = new Filter(FilterType.Gaussian3);

        /*for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                for (int k = 0; k < filter.size.x; i++)
                {
                    for (int l = 0; l < filter.size.y; l++)
                    {
                    //Do the filtering!!

                    }
                }

            }
        }*/

    }
    public Vector3 GetFlowDirection(Vector3 pos)
    {
        GetInterpolatedFlowDir(pos);
        return World2Node(pos).flow_field;
    }

    public Vector3 QuadLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float u, float v)
    {
        Vector3 abu = Vector3.Lerp(a, b, u);
        Vector3 dcu = Vector3.Lerp(d, c, u);
        return Vector3.Lerp(abu, dcu, v);
    }

    public Vector3 GetInterpolatedFlowDir(Vector3 pos)
    {
        Node node1 = World2Node(pos);
        Node node2 = World2Node(pos + Vector3.right * 0.5f * node_length);
        Vector3 x_add = (node1 == node2 ? (Vector3.left * 0.5f * node_length) : (Vector3.right * 0.5f * node_length));
        Node node3 = World2Node(pos + Vector3.forward * 0.5f * node_length);
        Vector3 y_add = (node1 == node3 ? (Vector3.back * 0.5f * node_length) : (Vector3.forward * 0.5f * node_length));
        node2 = World2Node(pos + x_add);
        node3 = World2Node(pos + y_add);
        Node node4 = World2Node(pos + x_add + y_add);

        float u1 = Mathf.Abs(pos.x - node1.world_pos.x);
        float u2 = Mathf.Abs(pos.x - node2.world_pos.x);
        float u = u1 / (u1 + u2);

        float v1 = Mathf.Abs(pos.z - node1.world_pos.z);
        float v2 = Mathf.Abs(pos.z - node3.world_pos.z);
        float v = v1 / (v1 + v2);

        //Debug.Log("")
        return QuadLerp(node1.flow_field, node2.flow_field, node4.flow_field, node3.flow_field, u, v);
    }
    public void SaveIntegrateField()
    {
        StreamWriter file = new StreamWriter(".\\Assets\\PathFiles\\integrate.txt");
        string val;
        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y - 1; j++)
            {
                val = grid[i, j].cost_val.ToString() + ", ";
                file.Write(val.ToCharArray());
            }
            val = grid[i, size_y - 1].cost_val.ToString() + "\n";
            file.Write(val.ToCharArray());
        }
        file.Close();
    }
    public void ReadFlowField()
    {
        is_flow_field_read = true;
        StreamReader file_x = new StreamReader(".\\Assets\\PathFiles\\vector_field.x");
        StreamReader file_z = new StreamReader(".\\Assets\\PathFiles\\vector_field.y");
        string line_x, line_z;
        string[] token_x, token_z;

        for (int i = 0; i < size_x; i++)
        {
            line_x = file_x.ReadLine();
            line_z = file_z.ReadLine();
            token_x = line_x.Split(',');
            token_z = line_z.Split(',');
            for (int j = 0; j < size_y; j++)
            {
                grid[i, j].flow_field = new Vector3(float.Parse(token_x[j]), 0, float.Parse(token_z[j]));
            }
        }
        file_x.Close();
        file_z.Close();
    }

    public void CalculatePath(int x, int y)
    {
        grid[x, y].is_path = true;
        if (grid[x, y].parent != null)
            CalculatePath(grid[x, y].parent.grid_x, grid[x, y].parent.grid_y);
    }

   
}
