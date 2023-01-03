﻿using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Knight : Piece{
    public Knight(int team) : base(team){ }

    public override List<Move> PossibleMoves(Board board, Vector2Int coordinates){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        Piece piece;
        Vector2Int actualCoordinates;
        
        for (int x = -2; x <= 2; x++){
            for (int y = -2; y <= 2; y++){
                if (Math.Abs(x) + Math.Abs(y) != 3 || IsOutOfBounds(currentBoard, actualCoordinates = coordinates + new Vector2Int(x,y)) || (piece = GetPieceAt(currentBoard, actualCoordinates)) != null && piece.Team == Team) continue;

                Piece goal = GetPieceAt(currentBoard, actualCoordinates);
                if (goal != null && goal.Team == Team) continue;
                moves.Add(new Move(coordinates, actualCoordinates, this, GetPieceAt(currentBoard, actualCoordinates)));
            }
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Knight;
        return boardComponent.Sprites.Black.Knight;
    }
    public override int GetValue(){
        return 3;
    }
}