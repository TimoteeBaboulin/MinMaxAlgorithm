﻿using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece{
    private static readonly Vector2Int[] Directions = 
        { new(1, 1), new(1, -1), new(-1, -1), new(-1, 1) };
    
    public Bishop(int team, int x, int y) : base(team, x, y){ }

    public override List<Vector2Int> PossibleMoves(Board board){
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var direction in Directions){
            var range = 1;
            var actualCoordinates = Coordinates + direction * range;
            while (!board.IsOutOfBounds(actualCoordinates) && board.GetPieceAt(actualCoordinates) == null){
                moves.Add(actualCoordinates);
                
                range++;
                actualCoordinates = Coordinates + direction * range;
            }
            if (!board.IsOutOfBounds(actualCoordinates) && board.GetPieceAt(actualCoordinates).Team != Team) moves.Add(actualCoordinates);
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Bishop;
        return board.Sprites.Black.Bishop;
    }
}