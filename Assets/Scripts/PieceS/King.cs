using System.Collections.Generic;
using UnityEngine;

public class King : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public King(int team) : base(team){ }

    public override List<Move> PossibleMoves(Piece[,] board, Vector2Int coordinates){
        if (board[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        foreach (var direction in Directions){
            var actualCoordinates = coordinates + direction;
            if (!IsOutOfBounds(board, actualCoordinates) && (GetPieceAt(board, actualCoordinates) == null || GetPieceAt(board, actualCoordinates).Team != Team))
                moves.Add(new Move(coordinates, actualCoordinates, this, GetPieceAt(board, actualCoordinates)));
        }

        return moves;
    }
    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.King;
        return board.Sprites.Black.King;
    }
    public override int GetValue(){
        return 0;
    }
}