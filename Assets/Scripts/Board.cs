using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Board{
    
    //Etat du plateau
    private readonly Piece[,] _board;
    public Piece[,] CurrentBoard => _board;
    private Team _player;
    public Team CurrentPlayer => _player;

    //Pieces rangées
    private readonly List<Piece>[] _teamPieces;
    private readonly Piece[] _kings;
    private readonly List<Piece>[] _bishops;
    private readonly List<Piece>[] _rooks;
    
    //Hashes
    private readonly int[,] _pieceHashes = new int[64, 12];
    private readonly int[] _playerturnHashes = new int[2];

    public int Hash => _currentHash;
    private int _currentHash;
    
    //Liste des coups
    private readonly Stack<Move> _moves;
    
    //Utile pour le check
    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, 
        new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
    };
    
    public Board(Piece[,] baseBoard, int currentPlayer){
        _board = baseBoard;
        _player = (Team)currentPlayer;
        _moves = new Stack<Move>();

        _kings = new Piece[2];
        _rooks = new List<Piece>[2];
        _bishops = new List<Piece>[2];
        _teamPieces = new List<Piece>[2];
        for (int x = 0; x < 2; x++){
            _teamPieces[x] = new List<Piece>();
            _bishops[x] = new List<Piece>();
            _rooks[x] = new List<Piece>();
        }
        
        foreach (var piece in _board){
            if (piece == null) continue;
            
            _teamPieces[(int)piece.Team].Add(piece);
            
            if (piece.GetType() == typeof(King)) _kings[(int)piece.Team] = piece;
            else if (piece.GetType() == typeof(Rook)) _rooks[(int)piece.Team].Add(piece);
            else if (piece.GetTypeOfPiece() == typeof(Bishop)) _bishops[(int)piece.Team].Add(piece);
        }
        GenerateNumbers();
        _currentHash = InitHashCode();
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
            var range = 1;
            actualCoordinates = king.Coordinates + direction * range;
            
            while (IsInBounds(actualCoordinates) && _board[actualCoordinates.x, actualCoordinates.y] == null){
                range++;
                actualCoordinates.x += direction.x;
                actualCoordinates.y += direction.y;
            }
            
            if (!IsInBounds(actualCoordinates)) continue;
            var piece = _board[actualCoordinates.x, actualCoordinates.y];
            if (piece.Team == _player) continue;
            
            if (direction.x != 0 && direction.y != 0){
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

                var piece = _board[actualCoordinates.x, actualCoordinates.y];
                if (piece != null && piece.Team != _player && piece.GetType() == typeof(Knight)) return true;
            }
        }

        return false;
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

        _currentHash ^= _pieceHashes[move.StartingPosition.x * 8 + move.StartingPosition.y, move.Attacker.GetID()];
        _currentHash ^= _pieceHashes[move.EndingPosition.x * 8 + move.EndingPosition.y, move.Attacker.GetID()];
        
        if (move.Defender == null) return;

        _currentHash ^= _pieceHashes[move.EndingPosition.x * 8 + move.EndingPosition.y, move.Defender.GetID()];
        
        var defender = move.Defender;
        _teamPieces[(int)defender.Team].Remove(defender);

        if (defender.GetTypeOfPiece() == typeof(Rook)) _rooks[(int)defender.Team].Remove(defender);
        else if (defender.GetTypeOfPiece() == typeof(Bishop)) _bishops[(int)defender.Team].Remove(defender);
    }
    public void Undo(){
        if (!_moves.TryPop(out var move)) return;
        
        move.Undo(_board);
        _player = CurrentPlayer == Team.Black? Team.White : Team.Black;
        
        _currentHash ^= _pieceHashes[move.StartingPosition.x * 8 + move.StartingPosition.y, move.Attacker.GetID()];
        _currentHash ^= _pieceHashes[move.EndingPosition.x * 8 + move.EndingPosition.y, move.Attacker.GetID()];
        
        if (move.Defender == null) return;
        
        _currentHash ^= _pieceHashes[move.EndingPosition.x * 8 + move.EndingPosition.y, move.Defender.GetID()];
        
        var defender = move.Defender;
        _teamPieces[(int)defender.Team].Add(defender);
            
        if (defender.GetTypeOfPiece() == typeof(Rook)) _rooks[(int)defender.Team].Add(defender);
        else if (defender.GetTypeOfPiece() == typeof(Bishop)) _bishops[(int)defender.Team].Add(defender);
    }

    //Methodes d'aide au calcul de la valeur
    private bool HaveBishopPair(Team team){
        return _bishops[(int)team].Count >= 2;
    }
    public bool HaveBishopPairAdvantage(Team team){
        return HaveBishopPair(team) && !HaveBishopPair(team == Team.Black ? Team.White : Team.Black);
    }

    public int GetMobility(){
        int count = 0;
        foreach (var piece in _teamPieces[(int)_player]){
            count += piece.PossibleMoves(this).Count;
        }

        return count;
    }

    //Transposition Tables
    private void GenerateNumbers(){
        Random random = new Random(1);
        for (int x = 0; x < 64; x++){
            for (int y = 0; y < 12; y++){
                _pieceHashes[x, y] = random.Next();
            }
        }

        for (int x = 0; x < 2; x++){
            _playerturnHashes[x] = random.Next();
        }
    }
    private int InitHashCode(){
        int hashCode = 0;
        for (int x = 0; x < 8; x++){
            for (int y = 0; y < 8; y++){
                if (_board[x,y] == null) continue;
                var piece = _board[x, y];
                hashCode ^= _pieceHashes[x * 8 + y, piece.GetID()];
            }
        }

        hashCode ^= _playerturnHashes[(int)CurrentPlayer];
        return hashCode;
    }
    
    //Misc
    private bool IsInBounds(Vector2Int actualCoordinates){
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