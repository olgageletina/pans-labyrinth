using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        lines = GetComponent<LineRenderer>();

        maze = new Maze(); // instantiate new maze object
        maze.create(resolutionX, resolutionY, protos, protoBlock); // create it

        System.Random rnd = new System.Random(); 
        MCell firstCell = maze.cells[rnd.Next() % maze.resx, rnd.Next() % maze.resy]; // pick the starting cell

        maze.generateDistanceField(firstCell); // generate solution path from the first cell to the one with the highest cost
        MCell currentCell = firstCell; 
        MCell maxC = firstCell;


        foreach (MCell c in maze.cells)
        {
            if (c.cost > maxC.cost) maxC = c; // if the cell cost is greater than the max cost, update max cost to cell cost
        }

        double maxcost = maxC.cost;

        foreach (MCell c in maze.cells)
        {
            if (c.cost < 0.0)
            {
                c.setFloorColor(mooTwo); // if the cell is not accessible set it to a custom color
            }
            else
            {
                float ncost = (float)(c.cost / maxcost);
                c.setFloorColor(new Color(ncost, ncost, ncost, 1.0f)); // otherwise, set a gradient of grey
            }
        }


        MCell targetMoo = maxC;
   
        List<MCell> path = new List<MCell>();
        for (int s = 0; s < 200; ++s)
        {
            path.Add(targetMoo); 
            if (targetMoo == firstCell) break; // if the first and last cell are the same then break
            MCell nextCell = new MCell();

            for (int b = 0; b < 4; ++b)
            {
                if (targetMoo.isOpen(b) &&
                  targetMoo.others[b].cost == targetMoo.cost - 1.0
                  ) // if the last cell is open in the direction of the neighboring cell and the cost of the neighboring cell is 1 less, then make it the next cell
                {
                    nextCell = targetMoo.others[b];
                }
            }
            if (nextCell.others.Length > 0.0)
            {
                for (int d = 0; d < nextCell.others.Length; ++d)
                {

                    if (nextCell.isOpen(d) &&
                      nextCell.others[d].cost == targetMoo.cost - 2.0
                      ) // if the neighbors of the next cell have a cost of two less than the target cell, make it the next cell
                    {
                        targetMoo = nextCell;
                    }
                    else if (nextCell.cost == 0.0) targetMoo = firstCell; // end of solution path
                }
            }
        }
        
        lines.positionCount = path.Count;
        // draw the path

        for (int i=0;i < path.Count; ++i)
        {
            Vector3 lp = path[i].p;
            lp.y += 1.0f;
            lines.SetPosition(i, lp);
            Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
            lines.material = lineMaterial;
        }
    }

    public LineRenderer lines;
    public List<GameObject> protos = new List<GameObject>();
    public List<int> protoBlock = new List<int>();
    public Color moo;
    public Color mooTwo; // color asigned to cell floors whose value is less than 0.0 (they cannot be reached)
    
    public int resolutionX = 20;
    public int resolutionY = 20;

    // Update is called once per frame
    void Update () {
	    // game play experiments
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) 
            {
                //Debug.Log(hit.point); 

                if (hit.point.y > 2.0f) { // if the same cell was hit more than 4 times - create new maze to make it unsolvable
                    maze.create(resolutionX, resolutionY, protos, protoBlock);
                }
                

                int i = (int)(hit.point.x+0.5f);
                int j = (int)(hit.point.z+0.5f);

                if (i>=0 && j>=0 && i<maze.resx && j<maze.resy)
                {
                    MCell c = maze.cells[i,j];

                    c.setFloorColor(moo); // set the color to the one that indicates that the cell has been hit

                    Vector3 p = c.geometry.transform.position;
                     p.y += 0.5f; // raise the cell by 0.5 in the y axis
                     c.geometry.transform.position = p;

                    transform.position = c.p + new Vector3(0.0f, 5.0f, 0.0f);
                    transform.LookAt(c.p);
                } 
            }
        }
	}
    Maze maze = null;
}





//cell attributes
public class MCell
{

    public void setFloorColor(Color c) //set the floor color
    {
       
        Transform t = geometry.transform.Find("Quad");

        MeshRenderer r = t.GetComponent<MeshRenderer>();
        r.material.color = c;
    }


    public Vector3 p;
    public MCellType type;
    public bool isVisited = false; // whether or not the cell has been visited
    public MCell[] others = new MCell[4]; // the cell's neighbors
    public double cost; // the cell's cost
    public GameObject geometry; // the geomtery of the cell

    public bool isOpen(int dir) // determine whether the cell is open in any particular direction
     {
        if (others[dir] == null) return false; // false: if there are no neighboring cells in a particular direction
        if (!type.isOpen[dir]) return false; // false: if it's not open in a particular direction
        if (!others[dir].type.isOpen[Maze.anti[dir]]) return false; //false: if the neighboring cell isn't open in the anti direction

        return true;
    }
}

public class MCellType
{
    public GameObject geometry;
    public bool[] isOpen = new bool[4];
    public int index = 0;
}


//instatiate maze
public class Maze
{
    //path configuration
    public const int right = 0;
    public const int top = 1;
    public const int left = 2;
    public const int bottom = 3;

    //movement to neighboring cells
    public const int antiright = left;
    public const int antitop = bottom;
    public const int antileft = right;
    public const int antibottom = top;

    //movement to neighboring cells
    public static int[] anti = new int[] { antiright, antitop, antileft, antibottom };

    public Maze()
    {
    }

    public int resx = 0;
    public int resy = 0;

    public void rotateCell(MCell c)
    {
        c.type = cellTypes[(c.type.index + 1) % cellTypes.Count];
    }

    public void create(int rx, int ry, List<GameObject> m, List<int> y)
    {
        resx = rx;
        resy = ry;
        for (int i = 0; i < m.Count; ++i)
        {
            MCellType newtype = new MCellType();
            newtype.geometry = m[i];
            newtype.isOpen[0] = true;
            newtype.isOpen[1] = true;
            newtype.isOpen[2] = true;
            newtype.isOpen[3] = true;
            newtype.index = i;

            newtype.isOpen[y[i]] = false; //set a type to close

            cellTypes.Add(newtype);
        }

        cells = new MCell[rx, ry];
        float dx = 1.0f;
        float dy = 1.0f;

         System.Random rnd = new System.Random();
        //generate maze
        for (int j = 0; j < ry; ++j)
        {
            for (int i = 0; i < rx; ++i)
            {
                int cellindex = rnd.Next() % cellTypes.Count; // set cellindex to random [0, cellTypes.Count]

                MCell newcell = new MCell();
                newcell.type = cellTypes[cellindex]; // generate cell with the random index above

                newcell.p = new Vector3(i  * dx,  0.0f, j  * dy); // get center point of each cell (for solution path)

                //draw cell
                GameObject clone = GameObject.Instantiate<GameObject>(newcell.type.geometry);
                clone.transform.position =new Vector3 (i * dx, 0.0f, j * dy);

                newcell.geometry = clone;
                cells[i, j] = newcell;

            }
        }

        // determine neighboring cells and set restrictions for corner cells
        for (int j = 0; j < ry; ++j)
        {
            for (int i = 0; i < rx; ++i)
            {
                MCell c = cells[i, j];
                if (i > 0) c.others[left] = cells[i - 1, j]; 
                if (i < rx - 1) c.others[right] = cells[i + 1, j]; 

                if (j > 0) c.others[bottom] = cells[i, j - 1];
                if (j < ry - 1) c.others[top] = cells[i, j + 1];
            }
        }
    }


    //distance field
    public void generateDistanceField(MCell cell0)
    {
        foreach (MCell c in cells)
        {
            c.cost = -1.0; // set all cells to -1
        }

        List<MCell> front = new List<MCell>();
        front.Add(cell0); // add first cell to front cell list
        cell0.cost = 0.0; // set its cost to 0.0

        for (int i = 0; i < 100000; ++i)
        {
            if (front.Count == 0) break; // if there are no cells in front break
            MCell c = front[front.Count - 1]; // set current cell to first cell of front
            front.RemoveAt(front.Count - 1); // remove the cell

            for (int j = 0; j < c.others.Length; ++j)
            {
                if (!c.isOpen(j)) continue; // if cell is not open in direction of j continue
                MCell nc = c.others[j]; // new cell

                double transitionCost = (c.p-nc.p).magnitude; // calculate transition cost which is the distance between the two cell points
                double newOtherCost = c.cost + transitionCost; // cost is equal to current cell cost and transition cost
                if (nc.cost < 0.0 || newOtherCost < nc.cost)
                {
                    nc.cost = newOtherCost;
                    front.Add(nc); // add new cell to front
                }
            }
        }
    }

    public MCell[,] cells;
    public List<MCellType> cellTypes = new List<MCellType>();

}
