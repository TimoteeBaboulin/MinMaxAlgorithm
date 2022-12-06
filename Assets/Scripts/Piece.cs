using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece{
    public int Team;
    public bool HasMoved;
    public Vector2Int Coordinates;

    public Piece(int team, int x, int y){
        Team = team;
        Coordinates = new Vector2Int(x, y);
    }

    public abstract List<Vector2Int> PossibleMoves(Board board);

    public abstract Sprite GetSprite(Board board);

    protected void Move(Board board, Vector2Int coord){
        if (!PossibleMoves(board).Contains(coord)) throw new Exception("Can't find move");

        if (!HasMoved) HasMoved = true;
            
        Piece goal = board.GetPieceAt(coord);
        if (goal != null) board.PiecesEaten.Add(goal);
        board.CurrentBoard[Coordinates.x, Coordinates.y] = null;
        board.CurrentBoard[coord.x, coord.y] = this;
    }
}