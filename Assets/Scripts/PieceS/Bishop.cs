using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece{
    private static readonly Vector2Int[] Directions = 
        { new(1, 1), new(1, -1), new(-1, -1), new(-1, 1) };
    
    public Bishop(int team) : base(team){ }

    public override List<Move> PossibleMoves(Piece[,] board, Vector2Int coordinates){
        if (board[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        var actualCoordinates = coordinates;
        for (int horizontal = -1; horizontal <= 1; horizontal+=2){
            for (int vertical = -1; vertical <= 1; vertical+=2){
                actualCoordinates = coordinates;
                actualCoordinates.x += vertical;
                actualCoordinates.y += horizontal;
                
                while (!IsOutOfBounds(board, actualCoordinates) ){
                    Piece pieceBlocking = GetPieceAt(board, actualCoordinates);
                    if (pieceBlocking == null){
                        moves.Add(new Move(coordinates, actualCoordinates, this, null));
                        actualCoordinates.x += vertical;
                        actualCoordinates.y += horizontal;
                        continue;
                    }
                
                    if (pieceBlocking.Team != Team) 
                        moves.Add(new Move(coordinates, actualCoordinates, this, pieceBlocking));
                
                    break;
                }
            }
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Bishop;
        return board.Sprites.Black.Bishop;
    }

    public override int GetValue(){
        return 3;
    }
    
    public override Piece Copy(){
        return new Bishop(Team);
    }
}