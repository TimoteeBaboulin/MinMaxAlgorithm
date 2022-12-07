using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public Queen(int team, int x, int y) : base(team, x, y){ }
    public Queen(Queen toCopy) : base(toCopy){}

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
            return board.Sprites.White.Queen;
        return board.Sprites.Black.Queen;
    }

    public override int GetValue(){
        return 9;
    }
    
    public override Piece Copy(){
        return new Queen(Team, Coordinates.x, Coordinates.y);
    }
}