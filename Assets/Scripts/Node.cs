using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UI;

public class Node{
    public readonly Move Move;
    public readonly List<Node> Children = new();
    public bool IsTerminal => !_board.CanMove();
    
    private readonly Board _board;
    private Team _player;
    private int Mobility => _board.GetMobility();

    public Node(Board board, Move move, Team player){
        Move = move;
        _board = board;
        _player = player;
    }

    public int CalculateValue(){
        int value = 0;

        //Check for mate or pat
        if (IsTerminal){
            if (!_board.IsInCheck()) value = 0;
            else if (_board.CurrentPlayer == _player) value = int.MinValue;
            else value = int.MaxValue;
            _board.Undo();
            return value;
        }

        //Count material value
        foreach (var piece in _board.GetPieceFromTeam(_player)){
            value += piece.GetValue();
            value += PieceSquareTables.GetValue(piece);
        }

        foreach (var piece in _board.GetPieceFromTeam(_player == Team.White ? Team.Black : Team.White)){
            value -= piece.GetValue();
            value -= PieceSquareTables.GetValue(piece);
        }

        //Vérifies que toutes les pieces soient développées
        foreach (var piece in _board.GetPieceFromTeam(_player)){
            Type pieceType = piece.GetTypeOfPiece();
            if ((pieceType == typeof(Bishop) || pieceType == typeof(Knight)) && !piece.HasMoved){
                value -= 50;
            }
        }
        
        //Check for bishop pairs
        if (_board.HaveBishopPairAdvantage(_player)) value += 100;
        else if (_board.HaveBishopPairAdvantage(_player == Team.Black ? Team.White : Team.Black)) value -= 1;

        //Take mobility into account
        value += Mobility * 10;

        return value;
    }

    public void GenerateChildren(){
        foreach (var move in _board.GetLegalMoves()){
            Children.Add(new Node(_board, move, _player));
        }
    }

    public void UndoMove(){
        if (Move != null){
            _board.Undo();
        }
    }
    
    public void DoMove(){
        if (Move != null){
            _board.Do(Move);
        }
    }
}