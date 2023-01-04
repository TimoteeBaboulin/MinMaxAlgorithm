using System.Collections.Generic;
using UnityEngine;

public class Node{
    public readonly Move Move;
    public List<Node> Children;

    public bool IsTerminal => _possibleMoves.Count == 0;

    private readonly Board _board;

    private List<Move> _possibleMoves = new();
    private readonly int _height;
    private readonly int _length;
    

    public Node(Board board, Move move){
        Move = move;
        
        _board = board;
        _height = board.CurrentBoard.GetLength(0);
        _length = board.CurrentBoard.GetLength(1);
    }

    public float CalculateValue(Team player){
        int value = 0;

        if (IsTerminal){
            if (!_board.IsInCheck()) value = 0;
            else if (_board.CurrentPlayer == player) value = -100000;
            else value = 100000;
            _board.Undo();
            return value;
        }
        
        for (int x = 0; x < _height; x++){
            for (int y = 0; y < _length; y++){
                Piece piece = _board.CurrentBoard[x, y];
                if (piece == null)
                    continue;
                if (piece.Team == (Team)player){
                    value += piece.GetValue();
                    if ((x == 3 || x == 4) && (y == 3 || y == 4))
                        value += 50;
                }
                else{
                    value -= piece.GetValue();
                    if ((x == 3 || x == 4) && (y == 3 || y == 4))
                        value -= 50;
                    
                    foreach (var move in piece.PossibleMoves(_board, new Vector2Int(x,y))){
                        if (move.Defender != null && move.Defender.Team == (Team)player)
                            value -= move.Defender.GetValue();
                        if ((move.EndingPosition.x == 3 || move.EndingPosition.x == 4) &&
                            (move.EndingPosition.y == 3 || move.EndingPosition.y == 4))
                            value -= 50;
                    }
                }
            }
        }
        
        foreach (var move in _possibleMoves){
            if ((move.EndingPosition.x == 3 || move.EndingPosition.x == 4) && (move.EndingPosition.y == 3 || move.EndingPosition.y == 4))
                value += 50;
            if (move.Defender != null && move.Defender.Team != (Team)player && move.Defender.GetValue() >= move.Attacker.GetValue())
                value += move.Defender.GetValue() / 3;
        }

        if (_board.HaveBishopPairAdvantage(player)) value += 1;
        else if (_board.HaveBishopPairAdvantage(player == Team.Black ? Team.White : Team.Black)) value -= 1;

        value += _board.CalculateMobility(player);
        
        _board.Undo();
        return value;
    }
    
    public void GenerateChildren(){
        Children = new List<Node>();
        foreach (var move in _possibleMoves){
            Children.Add(new Node(_board, move));
        }
    }
    public void GenerateMoves(){
        if (Move != null){
            _board.Do(Move);
        }

        _possibleMoves.Clear();
        _possibleMoves = _board.GetLegalMoves();
    }

    public void UndoMove(){
        if (Move != null){
            _board.Undo();
        }
    }
}