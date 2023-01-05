using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board{
    private readonly Piece[,] _board;
    public Piece[,] CurrentBoard => _board;
    private Team _player;
    public Team CurrentPlayer => _player;

    private List<Piece>[] _teamPieces;
    private Piece[] _kings;
    private readonly Stack<Move> _moves;
    private readonly Bishop[,] _bishops;
    
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public Board(Piece[,] baseBoard, int currentPlayer){
        _board = baseBoard;
        _player = (Team)currentPlayer;
        _moves = new Stack<Move>();

        _kings = new Piece[2];
        _teamPieces = new List<Piece>[2];
        _teamPieces[0] = new List<Piece>();
        _teamPieces[1] = new List<Piece>();
        foreach (var piece in _board){
            if (piece != null){
                _teamPieces[(int)piece.Team].Add(piece);
                if (piece.GetType() == typeof(King)) _kings[(int)piece.Team] = piece;
            }
        }
    }

    //Methodes liees au calcul des moves
    public List<Move> GetLegalMoves(){
        List<Move> moves = new List<Move>();
        foreach (var piece in _teamPieces[(int) _player]){
            moves.AddRange(piece.PossibleMoves(this));
        }

        for (int x = moves.Count - 1; x >= 0; x--){
            var currentMove = moves[x];
            currentMove.Do(_board);

            if (IsInCheck()) moves.Remove(moves[x]);
            currentMove.Undo(_board);
        }

        return moves;
    }

    public List<Move> GetLegalMoves(Team team){
        List<Move> moves = new List<Move>();
        foreach (var piece in _teamPieces[(int) team]){
            moves.AddRange(piece.PossibleMoves(this));
        }

        for (int x = moves.Count - 1; x >= 0; x--){
            var currentMove = moves[x];
            currentMove.Do(_board);

            if (IsInCheck()) moves.Remove(moves[x]);
            currentMove.Undo(_board);
        }

        return moves;
    }
    public bool IsInCheck(){
        Piece king = _kings[(int)_player];
        Vector2Int actualCoordinates;
        foreach (var direction in Directions){
            int range = 1;
            actualCoordinates = king.Coordinates + direction * range;
            
            while (IsInBounds(actualCoordinates) && _board[actualCoordinates.x, actualCoordinates.y] == null){
                range++;
                actualCoordinates.x += direction.x;
                actualCoordinates.y += direction.y;
            }
            bool isInBounds = IsInBounds(actualCoordinates);
            if (!isInBounds) continue;
            Piece piece = _board[actualCoordinates.x, actualCoordinates.y];
            bool diagonal;
            if (direction.x == 0 || direction.y == 0) diagonal = false;
            else
                diagonal = true;
            if (piece.Team == _player) continue;
            if (diagonal){
                if (range == 1 && piece.GetType() == typeof(Pawn)) return true;
                if (piece.GetType() == typeof(Bishop) || piece.GetType() == typeof(Queen)) return true;
            }
            else{
                if (piece.GetType() == typeof(Rook) || piece.GetType() == typeof(Queen)) return true;
            }
        }
        
        for (int x = -2; x <= 2; x++){
            for (int y = -2; y <= 2; y++){
                if (Math.Abs(x) + Math.Abs(y) != 3 || !IsInBounds(actualCoordinates = king.Coordinates + new Vector2Int(x,y))) continue;

                Piece piece = _board[actualCoordinates.x, actualCoordinates.y];
                if (piece != null && piece.Team != _player && piece.GetType() == typeof(Knight)) return true;
            }
        }

        return false;
        // List<Move> moves = new List<Move>();
        // foreach (var piece in _teamPieces[(int) (_player == Team.White ? Team.Black : Team.White)]){
        //     moves.AddRange(piece.PossibleMoves(this));
        // }
        //
        // foreach (var move in moves){
        //     if (move.Defender == _kings[(int) _player])
        //         return true;
        // }
        //
        // return false;
    }

    public bool CanMove(){
        foreach (var piece in _teamPieces[(int) _player]){
            if (piece.PossibleMoves(this).Count > 0) return true;
        }

        return false;
    }

    //Joues un coups, ou revient un coups en arriere
    public void Do(Move move){
        
        move.Do(_board);
        _player = CurrentPlayer == Team.Black? Team.White : Team.Black;
        _moves.Push(move);
        if (move.Defender != null) _teamPieces[(int)move.Defender.Team].Remove(move.Defender);
    }
    public void Undo(){
        if (!_moves.TryPop(out var move)) return;
        
        move.Undo(_board);
        _player = CurrentPlayer == Team.Black? Team.White : Team.Black;
        if (move.Defender != null) _teamPieces[(int)move.Defender.Team].Add(move.Defender);
    }

    //Methodes d'aide au calcul de la valeur
    public bool HaveBishopPair(Team team){
        var count = 0;
        foreach (var piece in _teamPieces[(int) team].Where(piece => piece.GetTypeOfPiece() == typeof(Bishop))){
            count++;
            if (count >= 2) return true;
        }

        return false;
    }
    public bool HaveBishopPairAdvantage(Team team){
        if (HaveBishopPair(team) && !HaveBishopPair(team == Team.Black ? Team.White : Team.Black)) return true;
        return false;
    }

    public int GetMobility(){
        int count = 0;
        foreach (var piece in _teamPieces[(int)_player]){
            count += piece.PossibleMoves(this).Count;
        }

        return count;
    }

    //Misc
    public bool IsInBounds(Vector2Int actualCoordinates){
        return actualCoordinates.x > 0 && actualCoordinates.x < 8 && actualCoordinates.y > 0 &&
               actualCoordinates.y < 8;
    }
    
    //Getter
    public List<Piece> GetPieceFromTeam(Team team){
        return _teamPieces[(int)team];
    }

    //Methode de debug, ne pas utiliser
    public Vector2Int GetCoordinates(Piece piece){
        for (int x = 0; x < 8; x++){
            for (int y = 0; y < 8; y++){
                if (_board[x, y] == piece)
                    return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(0, 0);
    }
}