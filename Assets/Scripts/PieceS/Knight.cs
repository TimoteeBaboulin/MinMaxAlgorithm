using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Knight : Piece{
    //Top Right Bottom Left
    //TR BR BL TL
    private static readonly int[] Directions = {
        -8, +1, +8, -1, 
        -7, 9, 7, -9
    };
    
    public Knight(int team, int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        int[] numSquaresToEdge = Board.NumSquaresToEdge[Coordinates];
        if (numSquaresToEdge[0] >= 2){
            //2 haut 1 droit
            if (numSquaresToEdge[1] >= 1){
                var actualCoordinates = Coordinates + 17;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
            if (numSquaresToEdge[3] >= 1){
                var actualCoordinates = Coordinates + 15;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
        }

        if (numSquaresToEdge[1] >= 2){
            if (numSquaresToEdge[0] >= 1){
                var actualCoordinates = Coordinates + 10;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
            if (numSquaresToEdge[2] >= 1){
                var actualCoordinates = Coordinates - 6;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
        }
        
        if (numSquaresToEdge[2] >= 2){
            if (numSquaresToEdge[1] >= 1){
                var actualCoordinates = Coordinates - 15;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
            if (numSquaresToEdge[3] >= 1){
                var actualCoordinates = Coordinates - 17;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
        }
        
        if (numSquaresToEdge[3] >= 2){
            if (numSquaresToEdge[0] >= 1){
                var actualCoordinates = Coordinates + 6;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
            if (numSquaresToEdge[2] >= 1){
                var actualCoordinates = Coordinates - 10;
                if (!board.IsFriendly(actualCoordinates, Team))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
            }
        }

        return moves;
    }
    public override Sprite GetSprite(BoardComponent boardComponent){
        if (Team == 0)
            return boardComponent.Sprites.White.Knight;
        return boardComponent.Sprites.Black.Knight;
    }
    public override int GetValue(){
        return 350;
    }
    
    public override int GetID(){
        return Team == Team.White ? 1 : 7;
    }

    public override void SetToBitBoard(){
        var bit = (long)1 << Coordinates;
        switch (Team){
            case Team.Black:
                BitBoards.BlackKnightOccupiedSquares |= bit;
                break;
            case Team.White:
                BitBoards.WhiteKnightOccupiedSquares |= bit;
                break;
        }
    }
    
    public override ref long GetBitBoardRef(){
        switch (Team){
            case Team.Black:
                return ref BitBoards.BlackKnightOccupiedSquares;
                break;
            case Team.White:
                return ref BitBoards.WhiteKnightOccupiedSquares;
                break;
        }

        throw new NullReferenceException();
    }
}