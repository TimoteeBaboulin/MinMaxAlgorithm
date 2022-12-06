using System;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece{
    public Knight(int team, int x, int y) : base(team, x, y){ }    
    
    public override List<Vector2Int> PossibleMoves(Board board){
        List<Vector2Int> moves = new List<Vector2Int>();

        for (int x = -2; x <= 2; x++){
            for (int y = -2; y <= 2; y++){
                if (Math.Abs(x) + Math.Abs(y) != 3) continue;

                Piece goal = board.GetPieceAt(Coordinates + new Vector2Int(x, y));
                if (goal != null && goal.Team == Team) continue;
                moves.Add(Coordinates + new Vector2Int(x, y));
            }
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Knight;
        return board.Sprites.Black.Knight;
    }
}