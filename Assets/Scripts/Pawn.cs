using System;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{
    public Pawn(int team, int x, int y) : base(team, x, y){ }    
    
    public override List<Vector2Int> PossibleMoves(Board board){
        List<Vector2Int> moves = new List<Vector2Int>();

        Vector2Int forward = Team == 0 ? Vector2Int.down : Vector2Int.up;
        if (board.GetPieceAt(Coordinates + forward) == null){
            if (!HasMoved && board.GetPieceAt(Coordinates + 2 * forward) == null) moves.Add(Coordinates + 2 * forward);
            moves.Add(Coordinates + forward);
        }

        for (int x = -1; x <= 1; x += 2){
            if (board.GetPieceAt(Coordinates + Vector2Int.right * x).Team != Team) moves.Add(Coordinates + Vector2Int.right * x);
        }

        return moves;
    }

    public override Sprite GetSprite(Board board){
        if (Team == 0)
            return board.Sprites.White.Pawn;
        return board.Sprites.Black.Pawn;
    }
}