using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public Queen(int team) : base(team){ }

    public override List<Move> PossibleMoves(Board board, Vector2Int coordinates){
        var currentBoard= board.CurrentBoard;
        if (currentBoard[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        foreach (var direction in Directions){
            var range = 1;
            var actualCoordinates = coordinates + direction * range;
            Piece pieceBlocking = null;
            while (!IsOutOfBounds(currentBoard, actualCoordinates) && (pieceBlocking = GetPieceAt(currentBoard, actualCoordinates)) == null){
                moves.Add(new Move(coordinates, actualCoordinates, this, null));
        
                range++;
                actualCoordinates = coordinates + direction * range;
            }

            
            if (!IsOutOfBounds(currentBoard, actualCoordinates) && pieceBlocking != null && pieceBlocking.Team != Team){
                moves.Add(new Move(coordinates, actualCoordinates, this, pieceBlocking));
            }
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Queen;
        return boardComponent.Sprites.Black.Queen;
    }
    public override int GetValue(){
        return 9;
    }
}