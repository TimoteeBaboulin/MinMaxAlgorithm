using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up
    };

    public Rook(int team) : base(team){ }

    public override List<Move> PossibleMoves(Piece[,] board, Vector2Int coordinates){
        if (board[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        var actualCoordinates = coordinates;
        
        for (int horizontal = -1; horizontal <= 1; horizontal+=2){
            actualCoordinates.y += horizontal;
            
            while (!IsOutOfBounds(board, actualCoordinates) ){
                Piece pieceBlocking = GetPieceAt(board, actualCoordinates);
                if (pieceBlocking == null){
                    moves.Add(new Move(coordinates, actualCoordinates, this, null));
                    actualCoordinates.y += horizontal;
                    continue;
                }
                
                if (pieceBlocking.Team != Team) 
                    moves.Add(new Move(coordinates, actualCoordinates, this, pieceBlocking));
                
                break;
            }

            actualCoordinates = coordinates;
        }
        
        for (int vertical = -1; vertical <= 1; vertical++){
            actualCoordinates.x += vertical;

            while (!IsOutOfBounds(board, actualCoordinates) ){
                Piece pieceBlocking = GetPieceAt(board, actualCoordinates);
                if (pieceBlocking == null){
                    moves.Add(new Move(coordinates, actualCoordinates, this, null));
                    actualCoordinates.x += vertical;
                    continue;
                }
                
                if (pieceBlocking.Team != Team) 
                    moves.Add(new Move(coordinates, actualCoordinates, this, pieceBlocking));
                
                break;
            }

            actualCoordinates = coordinates;
        }
        
        // foreach (var direction in Directions){
        //     var range = 1;
        //     var actualCoordinates = coordinates;
        //     Piece pieceBlocking = null;
        //     while (!IsOutOfBounds(board, actualCoordinates) && (pieceBlocking = GetPieceAt(board, actualCoordinates)) == null){
        //         moves.Add(new Move(coordinates, actualCoordinates, this, null));
        //
        //         range++;
        //         actualCoordinates = coordinates + direction * range;
        //     }
        //
        //     
        //     if (!IsOutOfBounds(board, actualCoordinates) && pieceBlocking != null && pieceBlocking.Team != Team){
        //         moves.Add(new Move(coordinates, actualCoordinates, this, pieceBlocking));
        //     }
        // }

        return moves;
    }
    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Rook;
        return board.Sprites.Black.Rook;
    }
    public override int GetValue(){
        return 5;
    }
}