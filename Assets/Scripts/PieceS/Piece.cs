using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece{
    public readonly Team Team;
    public bool HasMoved;
    public int Coordinates;

    protected Piece(int team, int coord){
        Team = (Team)team;
        Coordinates = coord;
    }

    public abstract List<Move> PossibleMoves(Board currentBoard);
    public abstract Sprite GetSprite(BoardComponent boardComponent);
    public abstract int GetValue();
    public abstract int GetID();
    public abstract void SetToBitBoard();
    public abstract ref long GetBitBoardRef();
    
    public Type GetTypeOfPiece(){
        return this.GetType();
    }
    
    protected bool IsOutOfBounds(Piece[] board, int coord){
        return coord < 0 || coord >= 64;
    }
    protected Piece GetPieceAt(Piece[] board, int coord){
        return board[coord];
    }
}