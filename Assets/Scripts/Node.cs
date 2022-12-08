using System;
using System.Collections.Generic;
using UnityEngine;

public class Node{
    public Piece[,] Board;
    private List<Node> _children;
    private int _player;
    private List<Move> _possibleMoves = new();
    private int _height;
    private int _length;

    public Node(Piece[,] board, int player){
        Board = (Piece[,])board.Clone();
        _player = player;

        _height = Board.GetLength(0);
        _length = Board.GetLength(1);
        
        _possibleMoves = CalculateMoves();
    }

    public Piece[,] GetMove(int depth, int color){
        if (depth == 0) throw new IndexOutOfRangeException("Can't have depth of 0");
        
        CalculateChildren();
        
        if (_children == null || _children.Count == 0){
            Debug.Log("Can't move");
            return Board;
        }
        
        float value = -1000;
        Node bestNode = null;
        foreach (var child in _children){
            float currentValue = child.GetValue(depth, color == 1 ? -1 : 1);
            if (currentValue > value){
                value = currentValue;
                bestNode = child;
            }
        }

        return bestNode.Board;
    }
    
    public float GetValue(int depth, int color){
        CalculateChildren();
        if (depth == 0){
            return color * CalculateValue();
        }
        
        float value = -1000;
        foreach (var child in _children){
            value = Math.Max(value, child.GetValue(depth - 1, color == 1 ? -1 : 1));
        }

        return value;
    }

    private float CalculateValue(){
        float value = 0;

        List<Move> possibleThreats = new List<Move>();
        
        for (int x = 0; x < _height; x++){
            for (int y = 0; y < _length; y++){
                Piece piece = Board[x, y];
                if (piece == null)
                    continue;
                if (piece.Team == _player){
                    value += piece.GetValue();
                    if ((x == 3 || x == 4) && (y == 3 || y == 4))
                        value += 0.5f;
                }
                else{
                    value -= piece.GetValue();
                    if ((x == 3 || x == 4) && (y == 3 || y == 4))
                        value += 0.5f;
                    
                    foreach (var move in piece.PossibleMoves(Board, new Vector2Int(x,y))){
                        if (move.Defender != null && move.Defender.Team == _player)
                            possibleThreats.Add(move);
                    }
                }
            }
        }

        foreach (var move in _possibleMoves){
            if ((move.EndingPosition.x == 3 || move.EndingPosition.x == 4) && (move.EndingPosition.y == 3 || move.EndingPosition.y == 4))
                value += 0.5f;
            if (move.Defender != null && move.Defender.Team != _player)
                value += (float)move.Defender.GetValue() / 3;
        }

        foreach (var threat in possibleThreats){
            value -= threat.Attacker.GetValue() - threat.Defender.GetValue();
        }

        return value;
    }

    private void CalculateChildren(){
        _children = new List<Node>();
        foreach (var move in _possibleMoves){
            Piece[,] newBoard = (Piece[,])Board.Clone();

            if (newBoard[move.StartingPosition.x, move.StartingPosition.y] == null) continue;
            move.Attacker.Move(newBoard, move);
            _children.Add(new Node(newBoard, _player == 0 ? 1 : 0));
        }
    }
    private List<Move> CalculateMoves(){
        List<Move> possibleMoves = new List<Move>();

        for (int x = 0; x < Board.GetLength(0); x++){
            for (int y = 0; y < Board.GetLength(1); y++){
                Piece piece = Board[x, y];
                if (piece == null || piece.Team != _player) continue;
                possibleMoves.AddRange(piece.PossibleMoves(Board, new Vector2Int(x,y)));
            }
        }

        return possibleMoves;
    }
}