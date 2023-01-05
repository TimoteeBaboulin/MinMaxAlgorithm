﻿using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece{
    public Bishop(int team, Vector2Int coord) : base(team, coord){ }
    
    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates.x, Coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        for (int horizontal = -1; horizontal <= 1; horizontal+=2){
            for (int vertical = -1; vertical <= 1; vertical+=2){
                var actualCoordinates = Coordinates;
                actualCoordinates.x += vertical;
                actualCoordinates.y += horizontal;
                
                while (!IsOutOfBounds(currentBoard, actualCoordinates) ){
                    Piece pieceBlocking = GetPieceAt(currentBoard, actualCoordinates);
                    if (pieceBlocking == null){
                        moves.Add(new Move(Coordinates, actualCoordinates, this, null));
                        actualCoordinates.x += vertical;
                        actualCoordinates.y += horizontal;
                        continue;
                    }
                
                    if (pieceBlocking.Team != Team) 
                        moves.Add(new Move(Coordinates, actualCoordinates, this, pieceBlocking));
                
                    break;
                }
            }
        }

        return moves;
    }
    
    
    
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Bishop;
        return boardComponent.Sprites.Black.Bishop;
    }
    public override int GetValue(){
        return 350;
    }
}