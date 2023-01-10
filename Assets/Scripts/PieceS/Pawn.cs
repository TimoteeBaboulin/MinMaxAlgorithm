using System;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{
    //Top Right Bottom Left
    //TR BR BL TL
    private static readonly int[] Directions = {
        -7, 9, 7, -9
    };
    
    public Pawn(int team, int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        int forward = Team == Team.White ? -8 : 8;
        int actualCoordinates = Coordinates;
        actualCoordinates += forward;
        if (!IsOutOfBounds(currentBoard, actualCoordinates) && currentBoard[actualCoordinates] == null){
            moves.Add(new Move(Coordinates, actualCoordinates, this, null));

            actualCoordinates += forward;
            if (!HasMoved && !IsOutOfBounds(currentBoard, actualCoordinates) && currentBoard[actualCoordinates] == null) 
                moves.Add(new Move(Coordinates, actualCoordinates, this, null));
        }

        //Captures
        switch (Team){
            case Team.Black:
                actualCoordinates = Coordinates + 9;
                if (Board.NumSquaresToEdge[Coordinates][5] != 0 && board.IsEnemy(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                actualCoordinates = Coordinates + 7;
                if (Board.NumSquaresToEdge[Coordinates][6] != 0 && board.IsEnemy(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                break;
            case Team.White:
                actualCoordinates = Coordinates - 7;
                if (Board.NumSquaresToEdge[Coordinates][4] != 0 && board.IsEnemy(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                actualCoordinates = Coordinates - 9;
                if (Board.NumSquaresToEdge[Coordinates][7] != 0 && board.IsEnemy(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Pawn;
        return boardComponent.Sprites.Black.Pawn;
    }
    public override int GetValue(){
        return 100;
    }

    public override int GetID(){
        return Team == Team.White ? 0 : 6;
    }
}