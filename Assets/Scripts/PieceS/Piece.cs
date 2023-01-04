using System;
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

    public Type GetTypeOfPiece(){
        return this.GetType();
    }
    
    protected bool IsOutOfBounds(Piece[,] board, Vector2Int coord){
        return coord.x >= board.GetLength(0) || coord.y >= board.GetLength(1) || coord.x < 0 || coord.y < 0;
    }
    protected Piece GetPieceAt(Piece[,] board, Vector2Int coord){
        return board[coord.x, coord.y];
    }
}