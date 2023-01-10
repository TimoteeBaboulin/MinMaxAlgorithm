using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Board{

    public static readonly int[][] NumSquaresToEdge = new int[64][];

    //Etat du plateau
    private readonly Piece[] _board;
    public Piece[] CurrentBoard => _board;
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
    //Top Right Bottom Left
    //TR BR BL TL
    private static readonly int[] Directions = {
        -8, +1, +8, -1, 
        -7, 9, 7, -9
    };

    public static void PrecomputedMoveData(){
        for (int x = 0; x < 8; x++){
            for (int y = 0; y < 8; y++){
                int numNorth = x;
                int numSouth = 7 - x;
                int numWest = y;
                int numEast = 7 - y;

                NumSquaresToEdge[x * 8 + y] = new[]{
                    numNorth,
                    numEast,
                    numSouth,
                    numWest,
                    Math.Min(numNorth, numEast),
                    Math.Min(numSouth, numEast),
                    Math.Min(numSouth, numWest),
                    Math.Min(numNorth, numWest)
                };
            }
        }
    }
    
    public Board(Piece[] baseBoard, int currentPlayer){
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
        
        for (int direction = 0; direction < 8; direction++){
            var actualCoordinates = king.Coordinates;
            for (int range = 0; range < Board.NumSquaresToEdge[king.Coordinates][direction]; range++){
                actualCoordinates += Directions[direction];
                if (IsFriendly(actualCoordinates, king.Team))
                    break;

                if (_board[actualCoordinates] == null) continue;

                var piece = _board[actualCoordinates];
                if (piece.GetTypeOfPiece() == typeof(Queen))
                    return true;
                
                switch (direction){
                    case < 4:
                        if (piece.GetTypeOfPiece() == typeof(Rook))
                            return true;
                        break;
                    default:
                        if (piece.GetTypeOfPiece() == typeof(Bishop))
                            return true;
                        break;
                }
            }
        }
        
        int[] numSquaresToEdge = Board.NumSquaresToEdge[king.Coordinates];
        var Coordinates = king.Coordinates;
        if (numSquaresToEdge[0] >= 2){
            //2 haut 1 droit
            if (numSquaresToEdge[1] >= 1)
                if (IsEnemy(Coordinates - 15, king.Team) && _board[Coordinates - 15].GetTypeOfPiece() == typeof(Knight))
                    return true;
            if (numSquaresToEdge[3] >= 1)
                if (IsEnemy(Coordinates - 17, king.Team) && _board[Coordinates - 17].GetTypeOfPiece() == typeof(Knight))
                    return true;
        }

        if (numSquaresToEdge[1] >= 2){
            if (numSquaresToEdge[0] >= 1)
                if (IsEnemy(Coordinates - 6, king.Team) && _board[Coordinates - 6].GetTypeOfPiece() == typeof(Knight))
                    return true;
            if (numSquaresToEdge[2] >= 1)
                if (IsEnemy(Coordinates +10, king.Team) && _board[Coordinates + 10].GetTypeOfPiece() == typeof(Knight))
                    return true;
        }
        
        if (numSquaresToEdge[2] >= 2){
            if (numSquaresToEdge[1] >= 1)
                if (IsEnemy(Coordinates + 17, king.Team) && _board[Coordinates + 17].GetTypeOfPiece() == typeof(Knight))
                    return true;
            if (numSquaresToEdge[3] >= 1)
                if (IsEnemy(Coordinates + 15, king.Team) && _board[Coordinates + 15].GetTypeOfPiece() == typeof(Knight))
                    return true;
        }
        
        if (numSquaresToEdge[3] >= 2){
            if (numSquaresToEdge[0] >= 1)
                if (IsEnemy(Coordinates - 10, king.Team) && _board[Coordinates - 10].GetTypeOfPiece() == typeof(Knight))
                    return true;
            if (numSquaresToEdge[2] >= 1)
                if (IsEnemy(Coordinates + 6, king.Team) && _board[Coordinates + 6].GetTypeOfPiece() == typeof(Knight))
                    return true;
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

        _currentHash ^= _playerturnHashes[(int) CurrentPlayer];
        
        move.Do(_board);
        _player = CurrentPlayer == Team.Black? Team.White : Team.Black;
        _currentHash ^= _playerturnHashes[(int) CurrentPlayer];
        
        _moves.Push(move);

        _currentHash ^= _pieceHashes[move.StartingPosition, move.Attacker.GetID()];
        _currentHash ^= _pieceHashes[move.EndingPosition, move.Attacker.GetID()];
        
        if (move.Defender == null) return;

        _currentHash ^= _pieceHashes[move.EndingPosition, move.Defender.GetID()];
        
        var defender = move.Defender;
        _teamPieces[(int)defender.Team].Remove(defender);

        if (defender.GetTypeOfPiece() == typeof(Rook)) _rooks[(int)defender.Team].Remove(defender);
        else if (defender.GetTypeOfPiece() == typeof(Bishop)) _bishops[(int)defender.Team].Remove(defender);
    }
    public void Undo(){
        if (!_moves.TryPop(out var move)) return;
        
        _currentHash ^= _playerturnHashes[(int) CurrentPlayer];
        move.Undo(_board);
        _player = CurrentPlayer == Team.Black? Team.White : Team.Black;
        _currentHash ^= _playerturnHashes[(int) CurrentPlayer]; 
        
        _currentHash ^= _pieceHashes[move.StartingPosition, move.Attacker.GetID()];
        _currentHash ^= _pieceHashes[move.EndingPosition, move.Attacker.GetID()];
        
        if (move.Defender == null) return;
        
        _currentHash ^= _pieceHashes[move.EndingPosition, move.Defender.GetID()];
        
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
        for (int x = 0; x < 64; x++){
            if (_board[x] == null) continue;
            var piece = _board[x];
            hashCode ^= _pieceHashes[x, piece.GetID()];
        }

        hashCode ^= _playerturnHashes[(int)CurrentPlayer];
        return hashCode;
    }
    
    //Misc
    private bool IsInBounds(Vector2Int actualCoordinates){
        return actualCoordinates.x > 0 && actualCoordinates.x < 8 && actualCoordinates.y > 0 &&
               actualCoordinates.y < 8;
    }

    public bool IsFriendly(int coordinates, Team team){
        return _board[coordinates] != null && _board[coordinates].Team == team;
    }
    public bool IsEnemy(int coordinates, Team team){
        return _board[coordinates] != null && _board[coordinates].Team != team;
    }

    //Getter
    public List<Piece> GetPieceFromTeam(Team team){
        return _teamPieces[(int)team];
    }

    // //Methode de debug, ne pas utiliser
    // public Vector2Int GetCoordinates(Piece piece){
    //     for (int x = 0; x < 8; x++){
    //         for (int y = 0; y < 8; y++){
    //             if (_board[x, y] == piece)
    //                 return new Vector2Int(x, y);
    //         }
    //     }
    //
    //     return new Vector2Int(0, 0);
    // }
}