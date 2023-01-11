using System;

public static class BitBoards{
    public static long WhitePawnsOccupiedSquares;
    public static long WhiteKnightOccupiedSquares;
    public static long WhiteBishopOccupiedSquares;
    public static long WhiteRookOccupiedSquares;
    public static long WhiteQueenOccupiedSquares;
    public static long WhiteKingOccupiedSquares;
    
    public static long BlackPawnsOccupiedSquares;
    public static long BlackKnightOccupiedSquares;
    public static long BlackBishopOccupiedSquares;
    public static long BlackRookOccupiedSquares;
    public static long BlackQueenOccupiedSquares;
    public static long BlackKingOccupiedSquares;

    public static long WhiteOccupiedSquares => WhitePawnsOccupiedSquares | WhiteKnightOccupiedSquares | WhiteBishopOccupiedSquares |
                                           WhiteRookOccupiedSquares | WhiteQueenOccupiedSquares | WhiteKingOccupiedSquares;
    public static long BlackOccupiedSquares => BlackPawnsOccupiedSquares | BlackKnightOccupiedSquares | BlackBishopOccupiedSquares |
                                           BlackRookOccupiedSquares | BlackQueenOccupiedSquares | BlackKingOccupiedSquares;

    public static long OccupiedSquares => WhiteOccupiedSquares | BlackOccupiedSquares;
    public static long EmptySquares => ~OccupiedSquares;

    public static bool GetBit(long bitBoard, int coordinates){
        return (bitBoard & ((long)1 << coordinates)) != 0;
    }

    public static long GetTwoComplement(long bitBoard){
        return -(long)bitBoard;
    }
}