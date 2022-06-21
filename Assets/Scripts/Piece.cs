//this class will contain the logic for our tetromino piece

using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; } //need to comunicate back to the game board when we need to reset the piece
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } //new copy of cells (in Tetrominos) so we can munipulate them
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; } //will contain the index of rotation we are currently on

    public float stepDelay = 1f; //timer delay for our step
    public float lockDelay = 0.5f; //lock delay

    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data) //using one piece that will be reinitalized each time with new data
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay; //one second later than what our current system time is
        this.lockTime = 0f; //once our piece has reached out lockDelay, it will lock into place


        if(this.cells == null) //if the array isn't already initalized 
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++) //populating the new array with the cells
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        this.board.Clear(this);

        this.lockTime += Time.deltaTime; //updates lockTime based on the amount of time that has passed since last frame was rendered

        if(Input.GetKeyDown(KeyCode.Q))
            Rotate(-1); //shift down an index to next rotation
        else if(Input.GetKeyDown(KeyCode.E))
            Rotate(1); //shift up an index

        if(Input.GetKeyDown(KeyCode.A))
            Move(Vector2Int.left);
        else if(Input.GetKeyDown(KeyCode.D))
            Move(Vector2Int.right);

        //soft drop
        if(Input.GetKeyDown(KeyCode.S))
            Move(Vector2Int.down);

        //hard drop
        if(Input.GetKeyDown(KeyCode.Space))
            HardDrop();

        if(Time.time >= this.stepTime)
            Step();

        this.board.Set(this);
    }

    //
    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        //check for a lock
        if(this.lockTime >= this.lockDelay)
            Lock();
    }

    public void HardDrop()
    {
        while(Move(Vector2Int.down))
            continue;

        Lock();
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.SpawnPiece();
    }

    private bool Move(Vector2Int translation)
    {
        //the current new position of our piece
        Vector3Int newPosition = this.position; 
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        //checks if position is valid
        bool valid = this.board.IsValidPosition(this, newPosition);

        //updates the new position if valid
        if(valid)
        {
            this.position = newPosition;
            this.lockTime = 0f; //resets
        }

        return valid;
    }

    //updates rotation index using rotation matrix on all our cells
    private void Rotate(int direct)
    {
        //storing our current rotation index
        int originalRotation = this.rotationIndex;
        
        //updating our rotation index
        this.rotationIndex = Wrap(this.rotationIndex + direct, 0, 4);

        ApplyRotationMatrix(direct); 

        //if wall kick fails, we need to revert everything we did
        if(!TestWallKicks(rotationIndex, direct))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direct);
        }
    }

    private void ApplyRotationMatrix(int direct)
    {
        for(int i = 0; i < this.cells.Length; i++)
        {
            //not int bc we offset the I & O cells by half a unit
            Vector3 cell = this.cells[i];

            int x, y; //new coordinates after it has been rotated

            switch(this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    //need to offset these points by half a unit
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    //rounds upward
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direct) + (cell.y * Data.RotationMatrix[1] * direct));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direct) + (cell.y * Data.RotationMatrix[3] * direct));
                    break;

                default:
                    //use values in array from Data and multiply them with our cells
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direct) + (cell.y * Data.RotationMatrix[1] * direct));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direct) + (cell.y * Data.RotationMatrix[3] * direct));
                    break;
            }

            //assigns new coordinates back to our cells
            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    //this function will test our wallkicks
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        //check which index we are dealing with (eg. 0>>1 or 1>>2 ...)
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        //five data set tests (dimension 1)
        for(int i = 0; i < this.data.wallkicks.GetLength(dimension: 1); i++)
        {
            Vector2Int translation = this.data.wallkicks[wallKickIndex, i];

            //exit if once we are in a valid position
            if(Move(translation))
                return true;
        }

        return false;
    }

    //returns which index we are dealing with when validating wallkicks 
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if(rotationDirection < 0)
            wallKickIndex--;

        return Wrap(wallKickIndex, 0, this.data.wallkicks.GetLength(0));
    }

    //wraps the index if it gets out of bounds (0, 1, 2, 3)
    private int Wrap(int input, int min, int max)
    {
        if(input < min) //if neg
            return max - (min - input) % (max - min);
        else //if > 3
            return max + (min - input) % (max - min);
    }
}
