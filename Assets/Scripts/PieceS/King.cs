using System.Collections.Generic;
using UnityEngine;

public class King : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public King(int team, Vector2Int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates.x, Coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        foreach (var direction in Directions){
            var actualCoordinates = Coordinates + direction;
            if (!IsOutOfBounds(currentBoard, actualCoordinates) && (GetPieceAt(currentBoard, actualCoordinates) == null || GetPieceAt(currentBoard, actualCoordinates).Team != Team))
                moves.Add(new Move(Coordinates, actualCoordinates, this, GetPieceAt(currentBoard, actualCoordinates)));
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