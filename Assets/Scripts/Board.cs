//this class controles the general state of our game by rendering tiles

using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; } //reference to game piece
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;

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

        this.activePiece.Initialize(this, spawnPosition, data); //initalize the random piece picked
        Set(this.activePiece);
    }

    public void Set(Piece piece) //read the new copy of cells (in Piece) into our tilemap
    {
        for(int i = 0; i > piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position; //offset that will give the position (spawnposition's data) on the grid
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
}
