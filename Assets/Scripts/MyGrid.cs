using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class MyGrid : MonoBehaviour{

    public GameObject prefab;
    public float cellSize;
    public int gridSizeX;
    public int gridSizeY;
    public GameObject[,] grid;

    public Node start;
    public Node finish;
    public Vector3 startingPosition;

    private Heap<Node> openSet;
    private HashSet<Node> closedSet;

    private Heap<Node> openSet_heatMap;
    private HashSet<Node> closedSet_heatMap;

    private List<Node> path;

    public static MyGrid Instance { set; get; }

    private void Start()
    {
        path = new List<Node>();
        CreateGrid();
        Instance = this;
        DoHeatMap();
        startingPosition = start.transform.position;
        startingPosition.x += 2f;
        startingPosition.z -= 2f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DoHeatMap();
    }

    public void DoHeatMap()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        if (closedSet_heatMap != null)
        {
            foreach(Node n in closedSet_heatMap)
            {
                n.gCost = 0;
                n.hCost = 0;
                n.parent = null;
            }
        }
        openSet_heatMap = new Heap<Node>(gridSizeX * gridSizeY);
        closedSet_heatMap = new HashSet<Node>();

        openSet_heatMap.Add(finish);

        while (openSet_heatMap.Count > 0)
        {
            Node currentNode = openSet_heatMap.RemoveFirst();
            closedSet_heatMap.Add(currentNode);

            currentNode.GetComponent<SpriteRenderer>().color = Color.white;

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.GetComponent<BasicTile>().walkable || closedSet_heatMap.Contains(neighbour))
                    continue;

                neighbour.GetComponent<SpriteRenderer>().color = Color.yellow;

                int newMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMoveCost < neighbour.gCost || !openSet_heatMap.Contains(neighbour))
                {
                    neighbour.gCost = newMoveCost;
                    neighbour.hCost = GetDistance(neighbour, finish);
                    neighbour.parent = currentNode;


                    if (!openSet_heatMap.Contains(neighbour))
                        openSet_heatMap.Add(neighbour);
                    openSet_heatMap.UpdateItem(neighbour);
                }
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log("Calculated vectors in " + sw.ElapsedMilliseconds + " ms");
        ShowCost();
    }


    public bool FindPath()
    {

        if (closedSet != null)
        {
            foreach(Node n in closedSet)
            {
                n.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        openSet = new Heap<Node>(gridSizeX * gridSizeY);
        closedSet = new HashSet<Node>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == finish)
            {
                RetracePath(start, finish);
                //ShowCost();
                return true;
            }

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.GetComponent<BasicTile>().walkable || closedSet.Contains(neighbour))
                    continue;

                int newMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMoveCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMoveCost;
                    neighbour.hCost = GetDistance(neighbour, finish);
                    neighbour.parent = currentNode;


                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    openSet.UpdateItem(neighbour);
                }
            }
        }
        //ShowCost();
        return false;
    }

    void ShowCost()
    {
        foreach(GameObject g in grid)
        {
            Node n = g.GetComponent<Node>();
            BasicTile t = g.GetComponent<BasicTile>();

            if (n == finish)
            {
                /*t.gCost.text = n.gCost.ToString();
                t.fCost.text = n.fCost.ToString();
                t.hCost.text = n.hCost.ToString();*/
                continue;
            }
                

            if (n.hCost == 0 || !t.walkable)
            {
                /*if (t.gCost.enabled)
                {
                    t.gCost.enabled = false;
                    t.hCost.enabled = false;
                    t.fCost.enabled = false;
                }                
                t.arrow.gameObject.SetActive(false);*/
                continue;
            } /*else if (n.hCost != 0 && !t.gCost.enabled)
            {
                t.gCost.enabled = true;
                t.hCost.enabled = true;
                t.fCost.enabled = true;
                t.arrow.gameObject.SetActive(true);
            }

            t.gCost.text = n.gCost.ToString();
            t.fCost.text = n.fCost.ToString();
            t.hCost.text = n.hCost.ToString();*/

            Node next;

            next = n.parent;

            Vector3 toMove = next.transform.position;
            toMove.x += 2f;
            toMove.z -= 2f;
            t.next = toMove;
            t.nextTile = next.GetComponent<BasicTile>();

            if(next == null)
                UnityEngine.Debug.Log("My position is: " + n.x + "  " + n.y);

            /*Vector3 direction = next.transform.position - n.transform.position;
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            t.arrow.localRotation = Quaternion.Euler(0, 0, angle);*/
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        path.Clear();
        Node currNode = endNode;

        while (currNode != startNode)
        {
            path.Add(currNode);
            currNode.parent.next = currNode;
            currNode = currNode.parent;
        }
        path.Reverse();
        start.next = path[0];

        foreach(Node n in path)
        {
            start.GetComponent<SpriteRenderer>().color = Color.cyan;

            if (n == finish)
                n.GetComponent<SpriteRenderer>().color = Color.magenta;
            else
                n.GetComponent<SpriteRenderer>().color = Color.grey;
        }
    }

    private int GetDistance(Node a, Node b)
    {
        int x = a.x > b.x ? a.x - b.x : b.x - a.x;
        int y = a.y > b.y ? a.y - b.y : b.y - a.y;
        return x + y;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> toReturn = new List<Node>();

        int x = node.x;
        int y = node.y;

        Node[] arr = new Node[4];
        arr[0] = CheckNode(x, y + 1);
        arr[1] = CheckNode(x + 1, y);
        arr[2] = CheckNode(x, y - 1);
        arr[3] = CheckNode(x - 1, y);

        foreach(Node n in arr)
        {
            if (n != null)
                toReturn.Add(n);
        }

        return toReturn;

    }

    private Node CheckNode(int x, int y)
    {
        if (x == gridSizeX || x == -1 || y == gridSizeY || y == -1)
        {
            return null;
        }
            
        else
            return grid[x, y].GetComponent<Node>();
    }

    public void CreateGrid()
    {
        ClearGrid();
        grid = new GameObject[gridSizeX, gridSizeY];
        for(int ix = 0; ix < gridSizeX; ix++)
        {
            for(int iy = 0; iy< gridSizeY; iy++)
            {
                GameObject node =  Instantiate(prefab, transform);
                node.transform.localPosition = new Vector3(ix * cellSize, iy * cellSize);
                grid[ix, iy] = node;
                Node nComp = node.AddComponent<Node>();
                nComp.FillInfo(ix, iy);

                if (ix == 0 && iy == (gridSizeY -1) / 2)
                {
                    start = nComp;
                    start.GetComponent<BasicTile>().placeable = false;
                }

                if (ix == (gridSizeX - 1) && iy == (gridSizeY - 1) / 2)
                {
                    finish = nComp;
                    finish.GetComponent<BasicTile>().placeable = false;
                    finish.GetComponent<BasicTile>().next = WaveControl.Instance.goal.position;
                }
            }
        }

        foreach (GameObject g in grid)
        {
            g.GetComponent<Node>().neighbours = GetNeighbours(g.GetComponent<Node>());
        }
    }

    public void ClearGrid()
    {
        if (grid == null)
            return;

        int x = grid.GetLength(0);
        int y = grid.GetLength(1);

        for (int ix = 0; ix < x; ix++)
        {
            for (int iy = 0; iy < y; iy++)
            {
                DestroyImmediate(grid[ix, iy]);
            }
        }

        grid = null;
    }
}

public class Node : MonoBehaviour , IHeapItem<Node>
{
    public int x;
    public int y;

    public int gCost;
    public int hCost;

    public int distance;

    public Node parent;
    public Node next;
    public List<Node> neighbours;
    int heapIndex;

    public void FillInfo(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        set
        {
            heapIndex = value;
        }
        get
        {
            return heapIndex;
        }
    }

    public int CompareTo(Node n)
    {
        int compare = fCost.CompareTo(n.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(n.hCost);
        }
        return -compare;
    }
}
