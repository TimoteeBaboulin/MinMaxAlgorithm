using System.Collections.Generic;
using UnityEngine;

public class King : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public King(int team, int x, int y) : base(team, x, y){ }

    public override List<Vector2Int> PossibleMoves(Board board){
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var direction in Directions){
            var actualCoordinates = Coordinates + direction;
            if (!board.IsOutOfBounds(actualCoordinates) && board.GetPieceAt(actualCoordinates).Team != Team) moves.Add(actualCoordinates);
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.King;
        return board.Sprites.Black.King;
    }
}