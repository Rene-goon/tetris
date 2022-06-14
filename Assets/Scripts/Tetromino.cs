using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

[System.Serializable] //we want to see this data in the editor
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; } //will form our shapes w/ coordinates (storing our array of cells)
    public Vector2Int[,] wallkicks { get; private set; } //two dimentional array that will hold the data from Data for a tetromino

    public void Initialize()
    {
        this.cells = Data.Cells[this.tetromino];
        this.wallkicks = Data.WallKicks[this.tetromino];
    }
}