using System;
using System.Collections.Generic;
using UnityEngine;

public class Node{
    public Piece[,] Board;
    private List<Node> _children;
    private int _player;

    public Node(Piece[,] board, int player){
        Board = (Piece[,])board.Clone();
        _player = player;
    }

    public Piece[,] GetMove(int depth, int player){
        if (depth == 0) throw new IndexOutOfRangeException("Can't have depth of 0");
        
        CalculateChildren();
        
        if (_children == null || _children.Count == 0){
            Debug.Log("Can't move");
            return Board;
        }
        
        float value = -1000;
        Node bestNode = null;
        foreach (var child in _children){
            float currentValue = child.GetValue(depth - 1, player == 1 ? -1 : 1);
            if (currentValue > value){
                value = currentValue;
                bestNode = child;
            }
        }

        return bestNode.Board;
    }
    
    public float GetValue(int depth, int player){
        if (depth == 0 || _children == null || _children.Count == 0){
            return player * CalculateValue();
        }

        CalculateChildren();
        
        float value = -1000;
        foreach (var child in _children){
            value = Math.Max(value, child.GetValue(depth - 1, player == 1 ? -1 : 1));
        }

        return value;
    }

    private float CalculateValue(){
        float value = 0;
        foreach (var piece in Board){
            if (piece == null)
                continue;
            if (piece.Team == _player)
                value += piece.GetValue();
            else
                value -= piece.GetValue();
        }

        return value;
    }

    private void CalculateChildren(){
        List<Vector2Int[]> possibleMoves = new List<Vector2Int[]>();
        foreach (var move in Board){
            if (move == null) continue;
            possibleMoves.AddRange(move.PossibleMoves(Board));
        }

        _children = new List<Node>();
        foreach (var move in possibleMoves){
            Piece[,] newBoard = new Piece[Board.GetLength(0),Board.GetLength(1)];

            for (int x = 0; x < newBoard.GetLength(0); x++){
                for (int y = 0; y < newBoard.GetLength(1); y++){
                    if (Board[x,y] != null) newBoard[x, y] = Board[x, y].Copy();
                }
            }
            
            newBoard = (Piece[,])Board.Clone();
            if (newBoard[move[0].x, move[0].y] == null) continue;
            newBoard[move[0].x, move[0].y].Move(newBoard, move);
            _children.Add(new Node(newBoard, _player == 0 ? 1 : 0));
        }
    }
}