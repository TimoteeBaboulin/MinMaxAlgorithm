using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece{
    public readonly Team Team;
    public bool HasMoved;
    public Vector2Int Coordinates;

    protected Piece(int team, Vector2Int coord){
        Team = (Team)team;
        Coordinates = coord;
    }

    public abstract List<Move> PossibleMoves(Board currentBoard);
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