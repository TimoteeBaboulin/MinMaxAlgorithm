using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up
    };

    public Rook(int team, Vector2Int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates.x, Coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        var actualCoordinates = Coordinates;
        
        for (int horizontal = -1; horizontal <= 1; horizontal+=2){
            actualCoordinates.y += horizontal;
            
            while (!IsOutOfBounds(currentBoard, actualCoordinates) ){
                Piece pieceBlocking = GetPieceAt(currentBoard, actualCoordinates);
                if (pieceBlocking == null){
                    moves.Add(new Move(Coordinates, actualCoordinates, this, null));
                    actualCoordinates.y += horizontal;
                    continue;
                }
                
                if (pieceBlocking.Team != Team) 
                    moves.Add(new Move(Coordinates, actualCoordinates, this, pieceBlocking));
                
                break;
            }

            actualCoordinates = Coordinates;
        }
        
        for (int vertical = -1; vertical <= 1; vertical++){
            actualCoordinates.x += vertical;

            while (!IsOutOfBounds(currentBoard, actualCoordinates) ){
                Piece pieceBlocking = GetPieceAt(currentBoard, actualCoordinates);
                if (pieceBlocking == null){
                    moves.Add(new Move(Coordinates, actualCoordinates, this, null));
                    actualCoordinates.x += vertical;
                    continue;
                }
                
                if (pieceBlocking.Team != Team) 
                    moves.Add(new Move(Coordinates, actualCoordinates, this, pieceBlocking));
                
                break;
            }

            actualCoordinates = Coordinates;
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Rook;
        return boardComponent.Sprites.Black.Rook;
    }
    public override int GetValue(){
        return 525;
    }
}