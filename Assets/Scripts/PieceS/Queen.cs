using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public Queen(int team, Vector2Int coord) : base(team, coord){ }

    public override IEnumerable<Move> PossibleMoves(Board board){
        var currentBoard= board.CurrentBoard;
        if (currentBoard[Coordinates.x, Coordinates.y] != this) yield break;

        foreach (var direction in Directions){
            var range = 1;
            var actualCoordinates = Coordinates + direction * range;
            Piece pieceBlocking = null;
            while (!IsOutOfBounds(currentBoard, actualCoordinates) && (pieceBlocking = GetPieceAt(currentBoard, actualCoordinates)) == null){
                yield return new Move(Coordinates, actualCoordinates, this, null);

                range++;
                actualCoordinates = Coordinates + direction * range;
            }

            
            if (!IsOutOfBounds(currentBoard, actualCoordinates) && pieceBlocking != null && pieceBlocking.Team != Team){
                yield return new Move(Coordinates, actualCoordinates, this, pieceBlocking);
            }
        }
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Queen;
        return boardComponent.Sprites.Black.Queen;
    }
    public override int GetValue(){
        return 1000;
    }
    
    public override int GetID(){
        return Team == Team.White ? 4 : 10;
    }
}