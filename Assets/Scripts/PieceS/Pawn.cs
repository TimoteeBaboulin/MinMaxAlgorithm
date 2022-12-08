using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{
    public Pawn(int team) : base(team){ }

    public override List<Move> PossibleMoves(Piece[,] board, Vector2Int coordinates){
        if (board[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        int forward = Team == 0 ? -1 : 1;
        Vector2Int actualCoordinates = coordinates;
        actualCoordinates.x += forward;
        if (!IsOutOfBounds(board, actualCoordinates) && GetPieceAt(board, actualCoordinates) == null){
            moves.Add(new Move(coordinates, actualCoordinates, this, null));

            actualCoordinates.x += forward;
            if (!HasMoved && !IsOutOfBounds(board, actualCoordinates) && GetPieceAt(board, actualCoordinates) == null) 
                moves.Add(new Move(coordinates, actualCoordinates, this, null));
        }

        actualCoordinates = coordinates;
        actualCoordinates.x += forward;

        for (int horizontal = -1; horizontal <=1; horizontal+=2){
            actualCoordinates.y += horizontal;
            Piece possiblePiece = null;
            if (!IsOutOfBounds(board, actualCoordinates) && (possiblePiece = GetPieceAt(board, actualCoordinates)) != null && possiblePiece.Team != Team) 
                moves.Add(new Move(coordinates, actualCoordinates, this, possiblePiece)); 
            actualCoordinates.y -= horizontal;
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Pawn;
        return board.Sprites.Black.Pawn;
    }

    public override int GetValue(){
        return 1;
    }
    
    public override Piece Copy(){
        return new Pawn(Team);
    }
}