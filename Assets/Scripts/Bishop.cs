using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece{
    private static readonly Vector2Int[] Directions = 
        { new(1, 1), new(1, -1), new(-1, -1), new(-1, 1) };
    
    public Bishop(int team, int x, int y) : base(team, x, y){ }
    public Bishop(Bishop toCopy) : base(toCopy){}

    public override List<Vector2Int[]> PossibleMoves(Piece[,] board){
        List<Vector2Int[]> moves = new List<Vector2Int[]>();

        foreach (var direction in Directions){
            var range = 1;
            var actualCoordinates = Coordinates + direction * range;
            while (!IsOutOfBounds(board, actualCoordinates) && GetPieceAt(board, actualCoordinates) == null){
                moves.Add(new []{Coordinates, actualCoordinates});
                
                range++;
                actualCoordinates = Coordinates + direction * range;
            }
            if (!IsOutOfBounds(board, actualCoordinates) && GetPieceAt(board, actualCoordinates).Team != Team) moves.Add(new []{Coordinates, actualCoordinates});
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
        return new Bishop(Team, Coordinates.x, Coordinates.y);
    }
}