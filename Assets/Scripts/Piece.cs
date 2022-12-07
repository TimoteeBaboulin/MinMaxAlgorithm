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

    public Piece(Piece toCopy){
        Team = toCopy.Team;
        Coordinates = toCopy.Coordinates;
    }

    public abstract List<Vector2Int[]> PossibleMoves(Piece[,] board);
    public abstract Sprite GetSprite(Board board);
    public abstract int GetValue();

    public abstract Piece Copy();

    // protected bool IsOutOfBounds(Piece[,] board, int x, int y){
    //     return x >= board.GetLength(0) || y >= board.GetLength(1) || x < 0 || y < 0;
    // }
    protected bool IsOutOfBounds(Piece[,] board, Vector2Int coord){
        return coord.x >= board.GetLength(0) || coord.y >= board.GetLength(1) || coord.x < 0 || coord.y < 0;
    }
    protected Piece GetPieceAt(Piece[,] board, Vector2Int coord){
        return board[coord.x, coord.y];
    }
    
    public void Move(Piece[,] board, Vector2Int[] move){
        //if (!PossibleMoves(board).Contains(move)) throw new Exception("Can't find move");

        if (!HasMoved) HasMoved = true;
            
        Piece goal = GetPieceAt(board, move[1]);
        // if (goal != null) board.PiecesEaten.Add(goal);
        board[Coordinates.x, Coordinates.y] = null;
        board[move[1].x, move[1].y] = this;
        
        Coordinates = new Vector2Int(move[1].x, move[1].y);
    }
}