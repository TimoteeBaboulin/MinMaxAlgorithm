using System;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up
    };
    
    public Rook(int team, int x, int y) : base(team, x, y){ }
    public Rook(Rook toCopy) : base(toCopy){}

    public override List<Vector2Int[]> PossibleMoves(Piece[,] board){
        List<Vector2Int[]> moves = new List<Vector2Int[]>();

        foreach (var direction in Directions){
            var range = 1;
            var actualCoordinates = Coordinates + direction * range;
            while (!IsOutOfBounds(board, actualCoordinates) && GetPieceAt(board, actualCoordinates) == null){
                moves.Add(new []{Coordinates, actualCoordinates});

                range++;
                actualCoordinates = Coordinates + direction * range;
            }
            if (!IsOutOfBounds(board, actualCoordinates) && GetPieceAt(board, actualCoordinates).Team != Team) moves.Add(new []{Coordinates, actualCoordinates});
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Rook;
        return board.Sprites.Black.Rook;
    }

    public override int GetValue(){
        return 5;
    }
    
    public override Piece Copy(){
        return new Rook(Team, Coordinates.x, Coordinates.y);
    }
}