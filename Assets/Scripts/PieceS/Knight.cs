using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Knight : Piece{
    //Top Right Bottom Left
    //TR BR BL TL
    private static readonly int[] Directions = {
        -8, +1, +8, -1, 
        -7, 9, 7, -9
    };
    
    public Knight(int team, int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        int[] numSquaresToEdge = Board.NumSquaresToEdge[Coordinates];
        if (numSquaresToEdge[0] >= 2){
            //2 haut 1 droit
            if (numSquaresToEdge[1] >= 1)
                if (!board.IsFriendly(Coordinates - 15, Team))
                    moves.Add(new Move(Coordinates, Coordinates - 15, this, board.CurrentBoard[Coordinates - 15]));
            if (numSquaresToEdge[3] >= 1)
                if (!board.IsFriendly(Coordinates - 17, Team))
                    moves.Add(new Move(Coordinates, Coordinates - 17, this, board.CurrentBoard[Coordinates - 17]));
        }

        if (numSquaresToEdge[1] >= 2){
            if (numSquaresToEdge[0] >= 1)
                if (!board.IsFriendly(Coordinates - 6, Team))
                    moves.Add(new Move(Coordinates, Coordinates - 6, this, board.CurrentBoard[Coordinates - 6]));
            if (numSquaresToEdge[2] >= 1)
                if (!board.IsFriendly(Coordinates + 10, Team))
                    moves.Add(new Move(Coordinates, Coordinates + 10, this, board.CurrentBoard[Coordinates + 10]));
        }
        
        if (numSquaresToEdge[2] >= 2){
            if (numSquaresToEdge[1] >= 1)
                if (!board.IsFriendly(Coordinates + 17, Team))
                    moves.Add(new Move(Coordinates, Coordinates + 17, this, board.CurrentBoard[Coordinates + 17]));
            if (numSquaresToEdge[3] >= 1)
                if (!board.IsFriendly(Coordinates + 15, Team))
                    moves.Add(new Move(Coordinates, Coordinates + 15, this, board.CurrentBoard[Coordinates + 15]));
        }
        
        if (numSquaresToEdge[3] >= 2){
            if (numSquaresToEdge[0] >= 1)
                if (!board.IsFriendly(Coordinates - 10, Team))
                    moves.Add(new Move(Coordinates, Coordinates - 10, this, board.CurrentBoard[Coordinates - 10]));
            if (numSquaresToEdge[2] >= 1)
                if (!board.IsFriendly(Coordinates + 6, Team))
                    moves.Add(new Move(Coordinates, Coordinates + 6, this, board.CurrentBoard[Coordinates + 6]));
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Knight;
        return boardComponent.Sprites.Black.Knight;
    }
    public override int GetValue(){
        return 350;
    }
    
    public override int GetID(){
        return Team == Team.White ? 1 : 7;
    }
}