using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchGrid : MonoBehaviour
{
    public int width = 9;
    public int height = 9;
    public int winCondition = 9;
    public bool pieceSelect = false;
    public int pieceAx = 0;
    public int pieceBx = 0;
    public int pieceAy = 0;
    public int pieceBy = 0;
    

    private int[,] grid; // this is the grid for my match 3 game - 5 different types of pieces [0 = empty] [1-5 type]
    public List<int> startingPieces = new List<int>();
    private List<GameObject> currentPieces = new List<GameObject>(); //list that holds the current pieces in play
   // public List<GameObject> nextPieces = new List<GameObject>(); // the pieces that are coming next

     public GameObject circlePrefab, squarePrefab, trianglePrefab, diamondPrefab, hexPrefab;
    //changing this to a dictionary instead of just a list of prefabs
    public Dictionary<int, GameObject> piecePrefabs = new Dictionary<int, GameObject>();
    
    private void Start() //setting up my grid
    {
        //populating my dictionary
        piecePrefabs.Add(1, circlePrefab);
        piecePrefabs.Add(2, squarePrefab);
        piecePrefabs.Add(3, trianglePrefab);
        piecePrefabs.Add(4, diamondPrefab);
        piecePrefabs.Add(5, hexPrefab);

        //init and instan my grid
        grid = new int[width, height];

        var thisPiece = 0;
        for ( var x= 0; x < width; x++)
        {
            
            for (var y = 0; y < height; y++)
            {
                
                grid[x, y] = startingPieces[thisPiece];
                thisPiece++;
            }
            
        }

        //updating it once it is built
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //check if a space is empty
    public bool IsEmpty (int x, int y)
    {
        if (y >= height)
        {
            return true;
        }
        
        else if (grid[x, y] == 0)
        {
            return  true;
        }

        return false;
        
    }

   

    public void DropColumn (int column)
    {
        
        for (var y = 0; y < height; y++)
        {
            if (IsEmpty(column, y))
            {
                if (!IsEmpty(column, y + 1))
                {
                    for (var yy = 1; yy < height - y; yy++)
                    {
                        grid[column, y + (yy-1)] = grid[column, y + yy];
                        
                    }                                     
                }
               else if (IsEmpty(column, y + 1))
                {
                    grid[column, y] = 0;
                }
                
                
            }
        }
        UpdateDisplay();
        MatchThree();
        return;
    }
    
    private int CheckPiece(int x, int y)
    {
        return grid[x, y];
    }

    private void UpdateDisplay()
    {
        //destroying all the pieces that were spawned
        foreach (var piece in currentPieces)
        {
            Destroy(piece);
        }

        currentPieces.Clear();
        //then for everything in the grid, spawn the correct piece

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
              int thisPiece =  CheckPiece(x, y);
                if (thisPiece > 0)
                {
                    var spawnPiece = Instantiate(piecePrefabs[thisPiece]);
                    spawnPiece.transform.position = new Vector3(x, y);
                    var gameObjectsPosition = spawnPiece.GetComponent<GridPositionScript>();
                    gameObjectsPosition.SetPostion(x, y);

                    currentPieces.Add(spawnPiece);
                }
              

            }
        }

        if (PlayerWin())
        {
            Debug.Log("The player has won");
        }
    }

   public bool PlayerWin()
    {
        return GridIsEmpty() == true;
        
    }

    
    public bool MatchThree()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (y <= height - 3)
                    if (grid[x, y] != 0 && grid[x, y] == grid[x, y + 1] && grid[x, y] == grid[x, y + 2]) 
                    {
                        grid[x, y] = 0;
                        grid[x, y + 1] = 0;
                        grid[x, y + 2] = 0;
                        DropColumn(x);
                        DropColumn(x);
                        DropColumn(x);
                        return true;

                    }
                        

                if (x <= width - 3)
                    if (grid[x, y] != 0 && grid[x, y] == grid[x + 1, y] && grid[x, y] == grid[x + 2, y]) 
                    {
                        grid[x, y] = 0;
                        grid[x + 1, y] = 0;
                        grid[x + 2, y] = 0;
                        DropColumn(x);
                        DropColumn(x + 1);
                        DropColumn(x + 2);
                        return true;
                    }

                
                
            }
        }

        return false;
    }

   public void SelectPiece(GameObject parent)
    {
      if (!pieceSelect)
        {
            //if this is the first piece selected store the positions
            pieceSelect = true;
            var parentGridValue = parent.GetComponent<GridPositionScript>();
            pieceAx = parentGridValue.myX;
            pieceAy = parentGridValue.myY;
            
        }
      else if (pieceSelect)
        {
            //if this is the second piece grab their positions
            var parentGridValue = parent.GetComponent<GridPositionScript>();
            pieceBx = parentGridValue.myX;
            pieceBy = parentGridValue.myY;

            if (pieceAx == pieceBx)
            { 
                if (pieceBy == pieceAy + 1 || pieceBy == pieceAy - 1)
                {
                    Debug.Log("swapping them along the y");
                    //swap em on y
                    SwapPieces(pieceAx, pieceAy, pieceBx, pieceBy);
                }
            }
            
            else if(pieceAy == pieceBy)
            {
                if (pieceBx == pieceAx + 1 || pieceBx == pieceAx -1)
                {
                    Debug.Log("swapping them along the x");
                    //swap em on x
                    pieceSelect = false;
                    SwapPieces(pieceAx, pieceAy, pieceBx, pieceBy);
                }
            }
            else
            {
                pieceSelect = false;
            }
        }
    
    }
    private void SwapPieces(int x, int y, int xi, int yi)
    {
        var pieceA = CheckPiece(x, y);
        var pieceB = CheckPiece(xi, yi);

        grid[x, y] = pieceB;
        grid[xi, yi] = pieceA;
        
        MatchThree();
        UpdateDisplay();

        // UpdateDisplay();
        // CheckMatch();

    }

  
    public bool GridIsEmpty()
    {
        int minReq = 0;
        for (var x = 0; x < width; x++)
        {
            for(var y = 0; y < height; y++)
            {
                if (grid[x, y] == 0)
                    minReq++;
            }
            
        }
        if (minReq >= winCondition)
        {
            return true;
        }
        return false;
    }
}
