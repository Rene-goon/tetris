using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece; // the main piece we will track
    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    // special update that gets called after all other updates
    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for(int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    // assigns main cell data to this cell's data (so if we rotate main piece, this piece will get updated too)
    private void Copy()
    {
        for(int i = 0; i < this.cells.Length; i++)
            this.cells[i] = this.trackingPiece.cells[i];
    }

    private void Drop()
    {
        // get current position of our tracking piece
        Vector3Int position = this.trackingPiece.position;

        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1; // offset by half bc positions are middle of board and neg directiong towards bottom

        this.board.Clear(this.trackingPiece); // to prevent IsValid from returning flase since that same piece is occupying that position

        // loop through every row in tilemap from bottom to top untill we find valid position to put ghost there
        for(int row = current; row >= bottom; row--)
        {
            position.y = row;

            if(this.board.IsValidPosition(this.trackingPiece, position))
                this.position = position;
            else
                break;
        }

        this.board.Set(this.trackingPiece);
    }

    private void Set()
    {
        for(int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position; 
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }

}
