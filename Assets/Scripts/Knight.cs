using System;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece{
    public Knight(int team, int x, int y) : base(team, x, y){ }    
    public Knight(Knight toCopy) : base(toCopy){}
    
    public override List<Vector2Int[]> PossibleMoves(Piece[,] board){
        List<Vector2Int[]> moves = new List<Vector2Int[]>();

        Piece piece;
        Vector2Int actualCoordinates;
        
        for (int x = -2; x <= 2; x++){
            for (int y = -2; y <= 2; y++){
                if (Math.Abs(x) + Math.Abs(y) != 3 || IsOutOfBounds(board, actualCoordinates = Coordinates + new Vector2Int(x,y)) || (piece = GetPieceAt(board, actualCoordinates)) != null && piece.Team == Team) continue;

                Piece goal = GetPieceAt(board, actualCoordinates);
                if (goal != null && goal.Team == Team) continue;
                moves.Add(new []{Coordinates, actualCoordinates});
            }
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Knight;
        return board.Sprites.Black.Knight;
    }

    public override int GetValue(){
        return 3;
    }
    
    public override Piece Copy(){
        return new Knight(Team, Coordinates.x, Coordinates.y);
    }
}