using System;
using System.Collections.Generic;
using UnityEngine;

public class Node{
    public Move Move;
    public List<Node> Children;

    public bool IsTerminal => _possibleMoves.Count == 0;

    private Piece[,] _board;
    private List<Move> _possibleMoves = new();
    private int _height;
    private int _length;
    

    public Node(Piece[,] board, Move move){
        Move = move;
        
        _board = (Piece[,])board.Clone();
        _height = _board.GetLength(0);
        _length = _board.GetLength(1);
    }

    public float CalculateValue(int player){
        float value = 0;
        Move.Do(_board);

        List<Move> possibleThreats = new List<Move>();
        
        for (int x = 0; x < _height; x++){
            for (int y = 0; y < _length; y++){
                Piece piece = _board[x, y];
                if (piece == null)
                    continue;
                if (piece.Team == player){
                    value += piece.GetValue();
                    // if ((x == 3 || x == 4) && (y == 3 || y == 4))
                    //     value += 0.5f;
                }
                else{
                    value -= piece.GetValue();
                    // if ((x == 3 || x == 4) && (y == 3 || y == 4))
                    //     value -= 0.5f;
                    
                    foreach (var move in piece.PossibleMoves(_board, new Vector2Int(x,y))){
                        if (move.Defender != null && move.Defender.Team == player)
                            possibleThreats.Add(move);
                    }
                }
            }
        }

        foreach (var move in _possibleMoves){
            if ((move.EndingPosition.x == 3 || move.EndingPosition.x == 4) && (move.EndingPosition.y == 3 || move.EndingPosition.y == 4))
                value += 0.5f;
            if (move.Defender != null && move.Defender.Team != player && move.Defender.GetValue() >= move.Attacker.GetValue())
                value += (float)move.Defender.GetValue() / 3;
        }
        
        foreach (var threat in possibleThreats){
            value -= threat.Defender.GetValue();
        }

        return value;
    }
    public void GenerateChildren(){
        Children = new List<Node>();
        foreach (var move in _possibleMoves){
            Piece[,] newBoard = (Piece[,])_board.Clone();

            if (newBoard[move.StartingPosition.x, move.StartingPosition.y] == null) continue;
            Children.Add(new Node(newBoard, move));
        }
    }
    public void GenerateMoves(int player){
        for (int x = 0; x < _board.GetLength(0); x++){
            for (int y = 0; y < _board.GetLength(1); y++){
                Piece piece = _board[x, y];
                if (piece == null || piece.Team != player) continue;
                _possibleMoves.AddRange(piece.PossibleMoves(_board, new Vector2Int(x,y)));
            }
        }
    }
}