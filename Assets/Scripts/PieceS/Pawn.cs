using System;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{
    //Top Right Bottom Left
    //TR BR BL TL
    private static readonly int[] Directions = {
        +9, -7, -9, +7
    };
    
    public Pawn(int team, int coord) : base(team, coord){ }

    public override List<Move> PossibleMoves(Board board){
        var currentBoard = board.CurrentBoard;
        if (currentBoard[Coordinates] != this) return new List<Move>();
        
        List<Move> moves = new List<Move>();

        int forward = Team == Team.White ? 8 : -8;
        int actualCoordinates = Coordinates;
        actualCoordinates += forward;
        if (IsOutOfBounds(currentBoard, actualCoordinates)) return new List<Move>();
        if (!BitBoards.GetBit(BitBoards.OccupiedSquares, actualCoordinates)){
            moves.Add(new Move(Coordinates, actualCoordinates, this, null));

            actualCoordinates += forward;
            if (!HasMoved && !IsOutOfBounds(currentBoard, actualCoordinates) && !BitBoards.GetBit(BitBoards.OccupiedSquares, actualCoordinates)) 
                moves.Add(new Move(Coordinates, actualCoordinates, this, null));
        }
        
        if (currentBoard[actualCoordinates] == null){
            
        }

        //Captures
        switch (Team){
            case Team.Black:
                actualCoordinates = Coordinates - 9;
                if (Board.NumSquaresToEdge[Coordinates][5] != 0 && BitBoards.GetBit(BitBoards.WhiteOccupiedSquares, actualCoordinates))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                actualCoordinates = Coordinates - 7;
                if (Board.NumSquaresToEdge[Coordinates][6] != 0 && BitBoards.GetBit(BitBoards.WhiteOccupiedSquares, actualCoordinates))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                break;
            case Team.White:
                actualCoordinates = Coordinates + 7;
                if (Board.NumSquaresToEdge[Coordinates][4] != 0 && BitBoards.GetBit(BitBoards.BlackOccupiedSquares, actualCoordinates))
                    moves.Add(new Move(Coordinates, actualCoordinates, this, board.CurrentBoard[actualCoordinates]));
                actualCoordinates = Coordinates + 9;
                if (Board.NumSquaresToEdge[Coordinates][7] != 0 && BitBoards.GetBit(BitBoards.BlackOccupiedSquares, actualCoordinates))
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

    public override void SetToBitBoard(){
        var bit = (long)1 << Coordinates;
        switch (Team){
            case Team.Black:
                BitBoards.BlackPawnsOccupiedSquares |= bit;
                break;
            case Team.White:
                BitBoards.WhitePawnsOccupiedSquares |= bit;
                break;
        }
    }

    public override ref long GetBitBoardRef(){
        switch (Team){
            case Team.Black:
                return ref BitBoards.BlackPawnsOccupiedSquares;
                break;
            case Team.White:
                return ref BitBoards.WhitePawnsOccupiedSquares;
                break;
        }

        throw new NullReferenceException();
    }
}