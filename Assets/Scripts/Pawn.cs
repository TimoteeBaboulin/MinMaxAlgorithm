using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{
    public Pawn(int team, int x, int y) : base(team, x, y){ }    
    public Pawn(Pawn toCopy) : base(toCopy){}
    
    public override List<Vector2Int[]> PossibleMoves(Piece[,] board){
        List<Vector2Int[]> moves = new List<Vector2Int[]>();

        Vector2Int forward = Team == 0 ? Vector2Int.left : Vector2Int.right;
        Vector2Int actualCoordinates;
        if (!IsOutOfBounds(board, actualCoordinates = Coordinates + forward) && GetPieceAt(board, actualCoordinates) == null){
            moves.Add(new []{Coordinates, actualCoordinates});
            
            if (!HasMoved && !IsOutOfBounds(board, actualCoordinates = Coordinates + 2 * forward) && GetPieceAt(board, actualCoordinates) == null) moves.Add(new []{Coordinates, actualCoordinates});
        }

        for (int x = -1; x <= 1; x += 2){
            if (!IsOutOfBounds(board, actualCoordinates = Coordinates + Vector2Int.right * x) && GetPieceAt(board, actualCoordinates) != null && GetPieceAt(board, actualCoordinates).Team != Team) moves.Add(new []{Coordinates, actualCoordinates});
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
        return new Pawn(Team, Coordinates.x, Coordinates.y);
    }
}