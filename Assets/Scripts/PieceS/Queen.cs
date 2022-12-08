using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Queen : Piece{
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public Queen(int team) : base(team){ }

    public override List<Move> PossibleMoves(Piece[,] board, Vector2Int coordinates){
        if (board[coordinates.x, coordinates.y] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        foreach (var direction in Directions){
            var range = 1;
            var actualCoordinates = coordinates + direction * range;
            Piece pieceBlocking = null;
            while (!IsOutOfBounds(board, actualCoordinates) && (pieceBlocking = GetPieceAt(board, actualCoordinates)) == null){
                moves.Add(new Move(coordinates, actualCoordinates, this, null));
        
                range++;
                actualCoordinates = coordinates + direction * range;
            }

            
            if (!IsOutOfBounds(board, actualCoordinates) && pieceBlocking != null && pieceBlocking.Team != Team){
                moves.Add(new Move(coordinates, actualCoordinates, this, pieceBlocking));
            }
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Queen;
        return board.Sprites.Black.Queen;
    }

    public override int GetValue(){
        return 9;
    }
    
    public override Piece Copy(){
        return new Queen(Team);
    }
}