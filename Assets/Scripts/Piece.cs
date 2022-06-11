//this class will contain the logic for our tetromino piece

using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; } //need to comunicate back to the game board when we need to reset the piece
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } //new copy of cells (in Tetrominos) so we can munipulate them
    public Vector3Int position { get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data) //using one piece that will be reinitalized each time with new data
    {
        this.board = board;
        this.position = position;
        this.data = data;

        if(this.cells == null) //if the array isn't already initalized 
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++) //populating the new array with the cells
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
}
