//this class controles the general state of our game by rendering tiles

using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; } //reference to game piece
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int (10, 20); //our board is 10x20

    //our bounds
    public RectInt Bounds 
    {
        get
        {
            //offset the size of the rec by half to get the corner of the rectangle
            //we want the minimum direction (-) 
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    public void Awake() //built in Unity function that is called when component is first initalized
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>(); //assign

        for(int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
    }

    public void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece() //randomly picks a cell to spawn in
    {
        int random = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];

        this.activePiece.Initialize(this, this.spawnPosition, data); //initalize the random piece picked

        if(IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        } else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        this.tilemap.ClearAllTiles();
    }

    public void Set(Piece piece) //read the new copy of cells (in Piece) into our tilemap
    {
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position; //offset that will give the position (spawnposition's data) on the grid
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece) //unsets the tiles
    {
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position; //offset that will give the position (spawnposition's data) on the grid
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    //validates if position of each cell is valid on the game board
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        //go through each and every cell
        for(int i = 0; i < piece.cells.Length; i++)
        {
            //our tile position for each indivicue cell (not position of pieces)
            Vector3Int tilePosition = piece.cells[i] + position; 

            //checks if the position is within the RectInt range
            if(!bounds.Contains((Vector2Int)tilePosition))
                return false;

            //check if tile is already at that space
            if(this.tilemap.HasTile(tilePosition))
                return false;
        }

        return true;
    }

    //clearing lines
    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while(row < bounds.yMax)
        {
            if(IsLineFull(row))
            {
                LineClear(row);
            } else
            {
                row++;
            }
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;
        
        for(int column = bounds.xMin; column < bounds.xMax; column++)
        {
            Vector3Int position = new Vector3Int(column, row, 0); //position of column (vect3 bc that's what tilemaps use)

            if(!this.tilemap.HasTile(position)) //if there is no tile at any position, not full, tf break
                return false;
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0); //current position
            this.tilemap.SetTile(position, null);
        }

        // have every tile above fall down once cleared
        while(row < bounds.yMax)
        {
            for(int col = bounds.xMin; col < bounds.xMax; col++) // for every row, itterate through that column
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0); // setting our position to tile above row we are one
                TileBase above = this.tilemap.GetTile(position); //var holding position above

                position = new Vector3Int(col, row, 0); // back to current row position
                this.tilemap.SetTile(position, above); //set current position with tile above
            }

            row++;
        }
    }
}
