using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarPathfinding : MonoBehaviour
{
    [SerializeField] private int detectionWidth = 10, detectionHeight = 10;
    [SerializeField] private float nodeSize = 1.0f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject gridVisualizer;
    private List<GameObject> visualizers = new List<GameObject>();
    private GameObject sirGluten;
    private Vector2 destPos;
    private List<Vector2> nextPosOnPath = new List<Vector2>();
    private Rigidbody2D rb;
    private Animator animator;

    private Dictionary<Vector2, Node> grid = new Dictionary<Vector2, Node>();

    void Start()
    {
        sirGluten = GameObject.Find("SirGluten");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        StartCoroutine(Pathfind());
        //CreatePath();
    }
    void Update() {
        animator.SetFloat("horizontal", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("vertical", rb.linearVelocity.y);
        animator.SetBool("isMoving", rb.linearVelocity.x != 0 || rb.linearVelocity.y != 0);

        if (rb.linearVelocity.x < 0) {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        } else if (rb.linearVelocity.x > 0) {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if (rb.linearVelocity.y != 0 && rb.linearVelocity.x == 0) {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    IEnumerator Pathfind(){
  

        if (nextPosOnPath.Count > 0) {
            Vector2 movementDirection = (nextPosOnPath[0] - rb.position);
            rb.linearVelocity = movementDirection.normalized * 4f;
            nextPosOnPath.RemoveAt(0);
        } else {
            CreatePath();
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Pathfind());
    }
    
    
    void CreatePath() {
        GenerateGrid();
        foreach(GameObject obj in visualizers) Destroy(obj);


        Vector2 destPos = new Vector2(Mathf.RoundToInt(sirGluten.transform.position.x), Mathf.RoundToInt(sirGluten.transform.position.y));
        Vector2 curPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        List<Node> openList = new List<Node>(){grid[curPos]};
        openList[0].HCost = Mathf.RoundToInt(Vector2.Distance(destPos, curPos) * 10);
        Dictionary<Vector2,Node> closedList = new Dictionary<Vector2, Node>();

        Node foundPath = null;
        int k = 0;
        while(true) {
            if (closedList.ContainsKey(destPos)) 
            {
                foundPath = closedList[destPos];
                break;
            }
            if (openList.Count == 0) {
                foundPath = null;
                break;
            }

            openList.Sort((a, b) => 
            {
                if (a.FCost == b.FCost)
                {
                    return a.HCost.CompareTo(b.HCost);
                }
                return b.FCost.CompareTo(a.FCost);
            });

            Node lowestF = openList[openList.Count - 1];
            openList.RemoveAt(openList.Count - 1);
            closedList[lowestF.Position] = lowestF;

            List<Node> adjancentTiles = new List<Node>();
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    if (i == 0 && j==0) continue;
                    Vector2 newPos = new Vector2(lowestF.Position.x + i, lowestF.Position.y + j);

                    if (!grid.ContainsKey(newPos) || closedList.ContainsKey(newPos)) continue;

                    GameObject newVisualizer = Instantiate(gridVisualizer,newPos,Quaternion.identity);
                    visualizers.Add(newVisualizer);

                    Node adjTile = new Node(newPos); 
                    adjTile.PrevNode = lowestF;
                    if (i != 0 && j != 0) {
                        adjTile.GCost = lowestF.GCost + 14;
                    } else {
                        adjTile.GCost = lowestF.GCost + 10;
                    }
                    adjTile.HCost = Mathf.RoundToInt(Vector2.Distance(destPos, newPos) * 10);
                    adjTile.FCost = adjTile.HCost + adjTile.GCost;

                    Node foundInOpen = null;
                    foreach (Node open in openList) if (open.Position == newPos) {
                        foundInOpen = open;
                    }

                    if (foundInOpen != null) {
                        if (adjTile.GCost > foundInOpen.GCost) {
                            foundInOpen.GCost = adjTile.GCost;
                            foundInOpen.FCost = adjTile.FCost; 
                            foundInOpen.PrevNode = lowestF;
                        }
                    } else {
                        openList.Add(adjTile);
                    }
                
                    //Debug.Log($"ADJ TILES COUNT: {adjancentTiles.Count}");
                    
                }
            }

            k++;
            if (k > 800) break;
        }

        Node curBacktrack = foundPath;
        List<Node> path = new List<Node>();
        while (curBacktrack != null) {
            GameObject visualizer = Instantiate(gridVisualizer, curBacktrack.Position, Quaternion.identity);
            visualizers.Add(visualizer);
            visualizer.GetComponent<SpriteRenderer>().color = Color.green;
            visualizer.GetComponent<SpriteRenderer>().sortingOrder = 1;

            path.Add(curBacktrack);
            curBacktrack = curBacktrack.PrevNode;

        }
        if (path.Count > 5) {
            nextPosOnPath.Add(path[path.Count - 2].Position);
            nextPosOnPath.Add(path[path.Count - 3].Position);
            nextPosOnPath.Add(path[path.Count - 4].Position);
        } else {
            nextPosOnPath.Add(destPos);
        }
    }

    void GenerateGrid()
    {
        grid = new Dictionary<Vector2, Node>();

        int startX = Mathf.RoundToInt(transform.position.x);
        int startY = Mathf.RoundToInt(transform.position.y);

        for (int y = startY - detectionHeight; y <= startY + detectionHeight; y++)
        {
            for (int x = startX - detectionWidth; x <= startX + detectionWidth; x++)
            {
                Vector2 nodePosition = new Vector2(x, y);

                if (Physics2D.OverlapCircle(nodePosition, nodeSize / 3, obstacleLayer) == null)
                {
                    grid[nodePosition] = new Node(nodePosition);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.cyan;

        foreach (var node in grid.Values)
        {
            Gizmos.DrawWireCube((Vector3)node.Position, Vector3.one * nodeSize);
        }
    }
}

public class Node
{
    private Vector2 position;
    private int fCost, gCost, hCost;
    private Node prevNode;

    public Node(Vector2 pos)
    {
        position = pos;

    }

    public Vector2 Position { get => position; set => position = value; }
    public global::System.Int32 FCost { get => fCost; set => fCost = value; }
    public global::System.Int32 GCost { get => gCost; set => gCost = value; }
    public global::System.Int32 HCost { get => hCost; set => hCost = value; }
    public Node PrevNode { get => prevNode; set => prevNode = value; }
}
