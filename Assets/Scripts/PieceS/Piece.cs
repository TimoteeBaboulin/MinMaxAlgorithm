using System.Collections.Generic;
using UnityEngine;

public abstract class Piece{
    public int Team;
    public bool HasMoved;

    public Piece(int team){
        Team = team;
    }

    public abstract List<Move> PossibleMoves(Piece[,] board, Vector2Int coordinates);
    public abstract Sprite GetSprite(Board board);
    public abstract int GetValue();

    public abstract Piece Copy();
    
    protected bool IsOutOfBounds(Piece[,] board, Vector2Int coord){
        return coord.x >= board.GetLength(0) || coord.y >= board.GetLength(1) || coord.x < 0 || coord.y < 0;
    }
    protected Piece GetPieceAt(Piece[,] board, Vector2Int coord){
        return board[coord.x, coord.y];
    }

    // protected bool GetCoordinates(Piece[,] board, out Vector2Int coordinates){
    //     for (int x = 0; x < board.GetLength(0); x++){
    //         for (int y = 0; y < board.GetLength(1); y++){
    //             if (board[x,y] != this) continue;
    //             coordinates = new Vector2Int(x, y);
    //             return true;
    //         }
    //     }
    //     coordinates = Vector2Int.zero;
    //     return false;
    // }
    public void Move(Piece[,] board, Move move){
        if (board[move.StartingPosition.x, move.StartingPosition.y] != this) return;
        if (!HasMoved) HasMoved = true;
        
        board[move.StartingPosition.x, move.StartingPosition.y] = null;
        board[move.EndingPosition.x, move.EndingPosition.y] = this;
    }
}

public struct Move{
    public Vector2Int StartingPosition;
    public Vector2Int EndingPosition;
    public Piece Attacker;
    public Piece Defender;

    public Move(Vector2Int startingPosition, Vector2Int endingPosition, Piece attacker, Piece defender){
        StartingPosition = startingPosition;
        EndingPosition = endingPosition;
        Attacker = attacker;
        Defender = defender;
    }
}