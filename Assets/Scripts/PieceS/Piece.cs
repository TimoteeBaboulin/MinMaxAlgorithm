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

    public abstract IEnumerable<Move> PossibleMoves(Board currentBoard);
    public abstract Sprite GetSprite(BoardComponent boardComponent);
    public abstract int GetValue();
    public abstract int GetID();

    public Type GetTypeOfPiece(){
        return this.GetType();
    }
    
    protected bool IsOutOfBounds(Piece[,] board, Vector2Int coord){
        return coord.x >= 8 || coord.y >= 8 || coord.x < 0 || coord.y < 0;
    }
    protected Piece GetPieceAt(Piece[,] board, Vector2Int coord){
        return board[coord.x, coord.y];
    }
}