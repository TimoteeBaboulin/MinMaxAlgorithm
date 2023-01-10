using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece{
    //Top Right Bot Left
    private static readonly int[] Directions = {
        -8, +1, +8, -1
    };

    public Rook(int team, int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();
        
        for (int direction = 0; direction < 4; direction++){
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
            return boardComponent.Sprites.White.Rook;
        return boardComponent.Sprites.Black.Rook;
    }
    public override int GetValue(){
        return 525;
    }
    
    public override int GetID(){
        return Team == Team.White ? 3 : 9;
    }
}