using UnityEngine;

public class Move{
    public Vector2Int StartingPosition;
    public Vector2Int EndingPosition;
    public Piece Attacker;
    public Piece Defender;

    private bool _didAttackerMoveBefore;

    public Move(Vector2Int startingPosition, Vector2Int endingPosition, Piece attacker, Piece defender){
        StartingPosition = startingPosition;
        EndingPosition = endingPosition;
        Attacker = attacker;
        Defender = defender;
    }

    public void Do(Piece[,] board){
        _didAttackerMoveBefore = board[StartingPosition.x, StartingPosition.y].HasMoved;

        board[StartingPosition.x, StartingPosition.y] = null;
        board[EndingPosition.x, EndingPosition.y] = Attacker;
        
        Attacker.HasMoved = true;
    }

    public void Undo(Piece[,] board){
        board[StartingPosition.x, StartingPosition.y] = Attacker;
        board[EndingPosition.x, EndingPosition.y] = Defender;

        board[StartingPosition.x, StartingPosition.y].HasMoved = _didAttackerMoveBefore;
    }
}