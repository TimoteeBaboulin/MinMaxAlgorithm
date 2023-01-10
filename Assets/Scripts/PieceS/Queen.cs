using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece{
    //Top Right Bottom Left
    //TR BR BL TL
    private static readonly int[] Directions = {
        -8, +1, +8, -1, 
        -7, 9, 7, -9
    };
    
    public Queen(int team, int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard= board.CurrentBoard;
        if (currentBoard[Coordinates] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        for (int direction = 0; direction < 8; direction++){
            var actualCoordinates = Coordinates;
            for (int range = 0; range < Board.NumSquaresToEdge[Coordinates][direction]; range++){
                actualCoordinates += Directions[direction];
                if (board.IsFriendly(actualCoordinates, Team))
                    break;
                
                moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                
                if (board.CurrentBoard[actualCoordinates] != null)
                    break;
            }
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Queen;
        return boardComponent.Sprites.Black.Queen;
    }
    public override int GetValue(){
        return 1000;
    }
    
    public override int GetID(){
        return Team == Team.White ? 4 : 10;
    }
}