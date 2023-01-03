using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{
    public Pawn(int team) : base(team){ }

    public override List<Move> PossibleMoves(Board board, Vector2Int coordinates){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        int forward = Team == 0 ? -1 : 1;
        Vector2Int actualCoordinates = coordinates;
        actualCoordinates.x += forward;
        if (!IsOutOfBounds(currentBoard, actualCoordinates) && GetPieceAt(currentBoard, actualCoordinates) == null){
            moves.Add(new Move(coordinates, actualCoordinates, this, null));

            actualCoordinates.x += forward;
            if (!HasMoved && !IsOutOfBounds(currentBoard, actualCoordinates) && GetPieceAt(currentBoard, actualCoordinates) == null) 
                moves.Add(new Move(coordinates, actualCoordinates, this, null));
        }

        actualCoordinates = coordinates;
        actualCoordinates.x += forward;

        for (int horizontal = -1; horizontal <=1; horizontal+=2){
            actualCoordinates.y += horizontal;
            Piece possiblePiece = null;
            if (!IsOutOfBounds(currentBoard, actualCoordinates) && (possiblePiece = GetPieceAt(currentBoard, actualCoordinates)) != null && possiblePiece.Team != Team) 
                moves.Add(new Move(coordinates, actualCoordinates, this, possiblePiece)); 
            actualCoordinates.y -= horizontal;
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Pawn;
        return boardComponent.Sprites.Black.Pawn;
    }
    public override int GetValue(){
        return 1;
    }
}