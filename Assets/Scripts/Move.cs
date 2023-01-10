using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Move{
    public int StartingPosition;
    public int EndingPosition;
    public readonly Piece Attacker;
    public readonly Piece Defender;

    private bool _didAttackerMoveBefore;

    public Move(int startingPosition, int endingPosition, Piece attacker, Piece defender){
        StartingPosition = startingPosition;
        EndingPosition = endingPosition;
        Attacker = attacker;
        Defender = defender;
    }

    public void Do(Piece[] board){
        _didAttackerMoveBefore = Attacker.HasMoved;

        board[StartingPosition] = null;
        board[EndingPosition] = Attacker;
        Attacker.Coordinates = EndingPosition;

        Attacker.HasMoved = true;
    }

    public void Undo(Piece[] board){
        board[StartingPosition] = Attacker;
        board[EndingPosition] = Defender;

        Attacker.HasMoved = _didAttackerMoveBefore;
        Attacker.Coordinates = StartingPosition;
    }
}