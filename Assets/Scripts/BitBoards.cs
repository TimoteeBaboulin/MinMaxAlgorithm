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

    public static long[] Files = new long[8];
    public static long[] Diagonals = new long[15];

    public static void PreComputeBitBoards(){
        for (int x = 0; x < 8; x++){
            Files[x] = 0;
            Files[x] |= 1;
            for (int y = 0; y < 7; y++){
                Files[x] <<= 8;
                Files[x] |= 1;
            }
            Files[x] <<= x;
        }
    }

    public static long CalculateDiagonal(int coord){
        const ulong mainDiagonal = 0x8040201008040201;
        int diagonal = 8 * (coord & 7) - (coord & 56);
        int north = -diagonal & (diagonal >> 31);
        int south = diagonal & (-diagonal >> 31);
        return (long)(mainDiagonal >> south) << north;
    }

    public static long CalculateAntiDiagonal(int coord){
        const ulong mainDiagonal = 0x0102040810204080;
        int diagonal = 56 - 8 * (coord & 7) - (coord & 56);
        int north = -diagonal & (diagonal >> 31);
        int south = diagonal & (-diagonal >> 31);
        return (long) (mainDiagonal >> south) << north;
    }
    
    public static bool GetBit(long bitBoard, int coordinates){
        return (bitBoard & ((long)1 << coordinates)) != 0;
    }

    public static long GetTwoComplement(long bitBoard){
        return -(long)bitBoard;
    }

    public static long GenerateLong(ulong baseBits){
        return (long)baseBits;
    }
}