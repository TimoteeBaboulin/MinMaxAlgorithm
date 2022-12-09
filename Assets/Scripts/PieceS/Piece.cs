using System.Collections.Generic;
using UnityEngine;

public abstract class Piece{
    public readonly int Team;
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

    // public void Move(Piece[,] board, Move move){
    //     if (board[move.StartingPosition.x, move.StartingPosition.y] != this) return;
    //     if (!HasMoved) HasMoved = true;
    //     
    //     board[move.StartingPosition.x, move.StartingPosition.y] = null;
    //     board[move.EndingPosition.x, move.EndingPosition.y] = this;
    // }
}

public class Move{
    public Vector2Int StartingPosition;
    public Vector2Int EndingPosition;
    public readonly Piece Attacker;
    public readonly Piece Defender;

    public Move(Vector2Int startingPosition, Vector2Int endingPosition, Piece attacker, Piece defender){
        StartingPosition = startingPosition;
        EndingPosition = endingPosition;
        Attacker = attacker;
        Defender = defender;
    }

    public void Do(Piece[,] board){
        board[StartingPosition.x, StartingPosition.y] = null;
        board[EndingPosition.x, EndingPosition.y] = Attacker;
    }

    public void Undo(Piece[,] board){
        board[StartingPosition.x, StartingPosition.y] = Attacker;
        board[EndingPosition.x, EndingPosition.y] = Defender;
    }
}