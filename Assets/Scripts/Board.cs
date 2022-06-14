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
        Set(this.activePiece);
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
}
