using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{
    public Pawn(int team, Vector2Int coord) : base(team, coord){ }

    public override IEnumerable<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates.x, Coordinates.y] != this) yield break;

        int forward = Team == 0 ? -1 : 1;
        Vector2Int actualCoordinates = Coordinates;
        actualCoordinates.x += forward;
        if (!IsOutOfBounds(currentBoard, actualCoordinates) && currentBoard[actualCoordinates.x, actualCoordinates.y] == null){
            yield return new Move(Coordinates, actualCoordinates, this, null);

            actualCoordinates.x += forward;
            if (!HasMoved && !IsOutOfBounds(currentBoard, actualCoordinates) && currentBoard[actualCoordinates.x, actualCoordinates.y] == null)
                yield return new Move(Coordinates, actualCoordinates, this, null);
        }

        actualCoordinates = Coordinates;
        actualCoordinates.x += forward;

        for (int horizontal = -1; horizontal <=1; horizontal+=2){
            actualCoordinates.y += horizontal;
            Piece possiblePiece = null;
            if (!IsOutOfBounds(currentBoard, actualCoordinates) &&
                (possiblePiece = currentBoard[actualCoordinates.x, actualCoordinates.y]) != null &&
                possiblePiece.Team != Team)
                yield return new Move(Coordinates, actualCoordinates, this, possiblePiece);
            actualCoordinates.y -= horizontal;
        }
    }
    
    
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Pawn;
        return boardComponent.Sprites.Black.Pawn;
    }
    public override int GetValue(){
        return 100;
    }

    public override int GetID(){
        return Team == Team.White ? 0 : 6;
    }
}