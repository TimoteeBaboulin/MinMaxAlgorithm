using System.Collections.Generic;
using UnityEngine;

public abstract class Piece{
    public readonly Team Team;
    public bool HasMoved;

    protected Piece(int team){
        Team = (Team)team;
    }

    public abstract List<Move> PossibleMoves(Board currentBoard, Vector2Int coordinates);
    public abstract Sprite GetSprite(BoardComponent boardComponent);
    public abstract int GetValue();

    protected bool IsOutOfBounds(Piece[,] board, Vector2Int coord){
        return coord.x >= board.GetLength(0) || coord.y >= board.GetLength(1) || coord.x < 0 || coord.y < 0;
    }
    protected Piece GetPieceAt(Piece[,] board, Vector2Int coord){
        return board[coord.x, coord.y];
    }
}

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

        // Attacker = board[StartingPosition.x, StartingPosition.y];
        // Defender = board[EndingPosition.x, EndingPosition.y];
        
        
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