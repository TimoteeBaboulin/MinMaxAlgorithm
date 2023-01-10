﻿using System.Collections.Generic;
using UnityEngine;

public class King : Piece{
    //Top Right Bottom Left
    //TR BR BL TL
    private static readonly int[] Directions = {
        -8, +1, +8, -1, 
        -7, 9, 7, -9
    };
    
    public King(int team, int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        for (int x = 0; x < 8; x++){
            if (Board.NumSquaresToEdge[Coordinates][x] >= 1 && board.IsEnemy(Coordinates + Directions[x], Team)){
                moves.Add(new Move(Coordinates, Coordinates + Directions[x], this,
                    board.CurrentBoard[Coordinates + Directions[x]]));
            }
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.King;
        return boardComponent.Sprites.Black.King;
    }
    public override int GetValue(){
        return 0;
    }
    
    public override int GetID(){
        return Team == Team.White ? 5 : 11;
    }
}