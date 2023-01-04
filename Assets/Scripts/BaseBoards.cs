﻿public static class BaseBoards{
    public static int[,] Standard ={
        {-4, -2, -3, -5, -6, -3, -2, -4 },
        {-1, -1, -1, -1, -1, -1, -1, -1 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 1,  1,  1,  1,  1,  1,  1,  1 },
        { 4,  2,  3,  5,  6,  3,  2,  4 },
    };

    public static int[,] QueenMoveTest ={
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0, -5,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  5 },
    };
    
    public static int[,] PawnMoveTest ={
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0, -1, -1,  0,  0,  0,  0,  0 },
        { 0,  0,  1,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  1 },
    };
    
    public static int[,] RookMoveTest ={
        { 0,  0,  0,  0,  0,  0,  4,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0, -4,  0,  0,  0,  0, -4,  0 },
        { 0,  0,  4,  0,  0,  0,  0,  0 },
        { 0,  0,  1,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  4 },
    };
    
    public static int[,] KnightMoveTest ={
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  2,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0, -2,  0,  0, -2,  0,  0,  0 },
        { 0,  0,  2,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  2,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  2 },
    };
    
    public static int[,] BishopMoveTest ={
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0, -3,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  1,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0, -3,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  3,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  3 },
    };
    
    public static int[,] KingMoveTest ={
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0, -6,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  6,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  6 },
        { 0,  0,  0,  0,  0,  0,  0,  6 },
    };
    
    public static int[,] Test ={
        {-0, -2, -3, -5, -6, -3, -2, -0 },
        {-1, -1, -1, -1, -1, -1, -1, -1 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 1,  1,  1,  1,  1,  1,  1,  1 },
        { 4,  2,  3,  5,  6,  3,  2,  4 },
    };
    
    public static int[,] CheckTest ={
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0, -4, -6,  0,  0,  0 },
        { 6,  5,  0, -4,  0,  0,  0,  0 },
    };

    public static int[,] CheckMateTest ={
        { 0,  4,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  4 },
        {-1,  0,  0,  0,  0,  0,  0,  0 },
        {-6,  0,  0,  0,  0,  0,  0,  0 },
    };
    
    public static int[,] BishopPairTest ={
        { 0,  0,  0,  0,  0,  0,  0, -6 },
        { 0,  0,  0,  0,  3,  3,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0, -2,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0, -3, -3,  0,  0 },
        { 6,  0,  0,  0,  0,  0,  0,  0 },
    };
}