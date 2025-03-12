using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Randomizer : MonoBehaviour
{
    [SerializeField]private GameObject sirGluten;
    [SerializeField]private GameObject foundue;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField]private GameObject barrier;
    private List<Vector2> possiblePos = new List<Vector2>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();
        sirGluten.transform.position = possiblePos[Random.Range(0,possiblePos.Count)];

        Vector2 candidatePos = sirGluten.transform.position;
        while(Vector2.Distance(sirGluten.transform.position, candidatePos) <= 10) {
            candidatePos = possiblePos[Random.Range(0,possiblePos.Count)];
        }
        foundue.transform.position = candidatePos;

        for (int i = 0; i < 10; i++) {
            Vector2 cPos = possiblePos[Random.Range(0,possiblePos.Count)];

            while(cPos == (Vector2)sirGluten.transform.position || cPos == (Vector2)foundue.transform.position) {
                cPos = possiblePos[Random.Range(0,possiblePos.Count)];
            }
            Instantiate(barrier,cPos,Quaternion.identity);
        }
        
    }

    void GenerateGrid()
    {
        possiblePos = new List<Vector2>();

        int startX = Mathf.RoundToInt(transform.position.x);
        int startY = Mathf.RoundToInt(transform.position.y);

        for (int y = startY - 20; y <= startY + 20; y++)
        {
            for (int x = startX - 20; x <= startX + 20; x++)
            {
                Vector2 nodePosition = new Vector2(x, y);

                if (Physics2D.OverlapCircle(nodePosition, 1f / 3, obstacleLayer) != null)
                {
                    possiblePos.Add(nodePosition);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
