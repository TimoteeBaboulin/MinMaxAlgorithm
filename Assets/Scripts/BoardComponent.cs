using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum Team{
    Black = 1,
    White = 0
}

public class BoardComponent : MonoBehaviour{
    private enum StartingPosition{
        Standard,
        PawnTest,
        KnightTest,
        BishopTest,
        RookTest,
        QueenTest,
        KingTest,
        CheckTest,
        CheckMateTest
    }
    private enum GameMode{
        AIvsAI,
        PlayerVSAI
    }

    
    public Piece[,] CurrentBoard;
    private Board _board;
    [NonSerialized] public List<Piece> PiecesEaten;
    [Header("Sprites")] 
    public PieceSprites Sprites;
    [SerializeField] private GameObject _pieceParent;
    [SerializeField] private GameObject _tileParent;
    [SerializeField] private GameObject _tileBase;

    [Header("Current player, do not touch")]
    [Range(0,1)]public int CurrentPlayer;

    [Header("AI settings")]
    [SerializeField] private int _depth;
    [SerializeField] private float _timeBetweenMoves;
    
    [Header("Game settings")]
    [SerializeField] private StartingPosition _startingPosition;
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private Team _humanPlayer;
    
    //Base board
    private int[,] _baseBoard;
    
    //Lists for rendering
    private GameObject[,] _tiles;
    private List<GameObject> _possibleMoves;
    private GameObject[,] _pieces;
    private List<GameObject> _piecesTaken;

    //Useless once Negamax is reworked
    private Move _bestMove;
    //Used to update the piece positions without efficiency issues
    private Move _lastMove;

    //Used for the timer
    private float _time;

    private void Start(){
        _pieces = new GameObject[8,8];
        //Get the starting position info
        GetStartingPosition();
        //Set up arrays
        CurrentBoard = new Piece[8, 8];
        _possibleMoves = new List<GameObject>();
        _piecesTaken = new List<GameObject>();

        //Instantiate the pieces
        for (int x = 0; x < CurrentBoard.GetLength(0); x++){
            for (int y = 0; y < CurrentBoard.GetLength(1); y++){
                if (_baseBoard[x,y] == 0) CurrentBoard[x,y] = null;

                int team = 0;
                if (_baseBoard[x, y] < 0) team = 1;

                switch (Math.Abs(_baseBoard[x,y])){
                    case 1:
                        CurrentBoard[x, y] = new Pawn(team);
                        break;
                    
                    case 2:
                        CurrentBoard[x, y] = new Knight(team);
                        break;
                    
                    case 3:
                        CurrentBoard[x, y] = new Bishop(team);
                        break;
                    
                    case 4:
                        CurrentBoard[x, y] = new Rook(team);
                        break;
                    
                    case 5:
                        CurrentBoard[x, y] = new Queen(team);
                        break;
                    
                    case 6:
                        CurrentBoard[x, y] = new King(team);
                        break;
                }
            }
        }

        _board = new Board(CurrentBoard, CurrentPlayer);
        //Draw the background tiles
        DrawBoard();
        InstantiatePieces();
    }
    private void Update(){
        _time += Time.deltaTime;
        if (_time < _timeBetweenMoves) return;
        _time = 0;
        MinMaxAlphaBetaSetUp();
        CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
        
        UpdatePieces();
        // DrawPossibleMoves();
    }

    private void InstantiatePieces(){
        float height = CurrentBoard.GetLength(0);
        float length = CurrentBoard.GetLength(1);

        for (int x = 0; x < height; x++){
            for (int y = 0; y < length; y++){
                if (CurrentBoard[x, y] != null){
                    GameObject newPiece = new GameObject(CurrentBoard[x,y].GetSprite(this).name);
                    newPiece.transform.position = new Vector3(length/2 - y -0.5f , height/2 - x -0.5f, 0);
                    if (_pieceParent != null) newPiece.transform.parent = _pieceParent.transform;
                    
                    var renderer = newPiece.AddComponent<SpriteRenderer>() as SpriteRenderer;
                    renderer.sprite = CurrentBoard[x, y].GetSprite(this);
                    _pieces[x, y] = newPiece;
                }
            }
        }
    }
    
    private void UpdatePieces(){
        if (_lastMove == null) return;

        var start = _lastMove.StartingPosition;
        var end = _lastMove.EndingPosition;
        
        if (_lastMove.Defender != null){
            _pieces[end.x, end.y].SetActive(false);
            _piecesTaken.Add(_pieces[end.x, end.y]);
        }
        _pieces[start.x, start.y].transform.position =
            new Vector3(4 - end.y - 0.5f, 4 - end.x - 0.5f, 0);
        _pieces[end.x, end.y] = _pieces[start.x, start.y];
        _pieces[start.x, start.y] = null;
        
    }
    
    private void DrawBoard(){
        int height = CurrentBoard.GetLength(0);
        int length = CurrentBoard.GetLength(1);

        _tiles = new GameObject[height, length];
        
        for (int x = 0; x < height; x++){
            for (int y = 0; y < length; y++){
                var newTile = Instantiate(_tileBase, new Vector3(length/2 - y -0.5f , height/2 - x -0.5f, 1),
                    Quaternion.identity);
                if (_tileParent != null) newTile.transform.parent = _tileParent.transform;
                
                Color color = (x + y) % 2 == 1 ? Color.white : Color.black;
                newTile.GetComponent<MeshRenderer>().material.color = color;
                _tiles[x, y] = newTile;
            }
        }
    }
    private void DrawPossibleMoves(){
        foreach (var move in _possibleMoves){
            Destroy(move);
        }
        
        List<Move> possibleMoves = _board.GetLegalMoves();

        Color color = Color.blue;
        color.a = 0.5f;
        
        int height = CurrentBoard.GetLength(0);
        int length = CurrentBoard.GetLength(1);
        foreach (var possibleMove in possibleMoves){
            var newTile = Instantiate(_tileBase, new Vector3(length/2 - possibleMove.EndingPosition.y -0.5f , height/2 - possibleMove.EndingPosition.x -0.5f, -1),
                Quaternion.identity);
            
            newTile.GetComponent<MeshRenderer>().material.color = color;
            _possibleMoves.Add(newTile);
        }
    }
    
    // private void NegaMaxSetUp(){
    //     Stopwatch stopwatch = new Stopwatch();
    //     stopwatch.Start();
    //
    //     _bestMove = null;
    //     
    //     Node currentPosition = new Node((Piece[,])CurrentBoard.Clone(), null);
    //     float value = -NegaMax(currentPosition, _depth, 1);
    //     Debug.Log("Chose move with value: " + value);
    //
    //     if (_bestMove == null){
    //         Debug.Log("Couldn't find move");
    //         return;
    //     }
    //     
    //     _bestMove.Do(CurrentBoard);
    //     
    //     Debug.Log("Ce coups a pris: " + stopwatch.Elapsed.ToString() + " secondes.");
    //     stopwatch.Stop();
    // }
    // private void NegaMaxAlphaBetaSetUp(){
    //     Stopwatch stopwatch = new Stopwatch();
    //     stopwatch.Start();
    //
    //     _bestMove = null;
    //     
    //     Node currentPosition = new Node((Piece[,])CurrentBoard.Clone(), null);
    //     float value = -NegaMax(currentPosition, _depth, -1000, 1000, 1);
    //     Debug.Log("Chose move with value: " + value);
    //
    //     if (_bestMove == null){
    //         Debug.Log("Couldn't find move");
    //         return;
    //     }
    //     
    //     _bestMove.Do(CurrentBoard);
    //     
    //     Debug.Log("Ce coups a pris: " + stopwatch.Elapsed.ToString() + " secondes.");
    //     stopwatch.Stop();
    // }
    //
    // private void MinMaxSetUp(){
    //     Stopwatch stopwatch = new Stopwatch();
    //     stopwatch.Start();
    //     
    //     Node currentPosition = new Node((Piece[,])CurrentBoard.Clone(), null);
    //     float value = MinMax(currentPosition, _depth, false, out Move bestMove);
    //     Debug.Log("Chose move with value: " + value);
    //     
    //     if (bestMove == null){
    //         Debug.Log("Couldn't find move");
    //         return;
    //     }
    //     
    //     bestMove.Do(CurrentBoard);
    //     
    //     Debug.Log("Ce coups a pris: " + stopwatch.Elapsed.ToString() + " secondes.");
    //     stopwatch.Stop();
    // }
    private void MinMaxAlphaBetaSetUp(){
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        Node currentPosition = new Node(_board, null);
        float value = MinMax(currentPosition, _depth, -1500, 1500, true, out Move bestMove);
        Debug.Log("Chose move with value: " + value);
        
        if (bestMove == null){
            Debug.Log("Couldn't find move");
            return;
        }
        
        _board.Do(bestMove);
        _lastMove = bestMove;

        Debug.Log("Ce coups a pris: " + stopwatch.Elapsed.ToString() + " secondes.");
        stopwatch.Stop();
    }

    // private float NegaMax(Node node, int depth, int color){
    //     node.GenerateMoves(CurrentPlayer);
    //     if (depth == 0 || node.IsTerminal)
    //         return node.CalculateValue(CurrentPlayer) * color;
    //     
    //     node.GenerateChildren();
    //     float value = -1000;
    //     foreach (var child in node.Children){
    //         float childValue = NegaMax(child, depth - 1, -color);
    //         if (value < childValue){
    //             value = childValue;
    //             _bestMove = child.Move;
    //         }
    //     }
    //
    //     return value;
    // }
    // private float NegaMax(Node node, int depth, float alpha, float beta, int color){
    //     int player;
    //     if (color == -1)
    //         player = CurrentPlayer == 0 ? 1 : 0;
    //     else
    //         player = CurrentPlayer;
    //     
    //     node.GenerateMoves(player);
    //     if (depth == 0 || node.IsTerminal)
    //         return node.CalculateValue(CurrentPlayer) * color;
    //     
    //     node.GenerateChildren();
    //     float value = -1000;
    //     foreach (var child in node.Children){
    //         float childValue = -NegaMax(child, depth - 1, -beta, -alpha, -color);
    //         if (value < childValue){
    //             value = childValue;
    //             _bestMove = child.Move;
    //         }
    //
    //         alpha = Math.Max(alpha, value);
    //         if (alpha >= beta)
    //             break;
    //
    //     }
    //
    //     return value;
    // }
    //
    // private float MinMax(Node node, int depth, bool maximizing, out Move bestMove){
    //     int player;
    //     if (!maximizing)
    //         player = CurrentPlayer == 0 ? 1 : 0;
    //     else
    //         player = CurrentPlayer;
    //     node.GenerateMoves(player);
    //
    //     bestMove = node.Move;
    //     
    //     if (depth == 0 || node.IsTerminal){
    //         bestMove = node.Move;
    //         return node.CalculateValue(CurrentPlayer);
    //     }
    //     
    //     node.GenerateChildren();
    //
    //     float value;
    //     if (maximizing){
    //         value = -1000;
    //         foreach (var child in node.Children){
    //             float childValue = MinMax(child, depth - 1, false, out Move move);
    //             if (childValue > value){
    //                 bestMove = child.Move;
    //                 value = childValue;
    //             }
    //         }
    //     }
    //     else{
    //         value = 1000;
    //         foreach (var child in node.Children){
    //             float childValue = MinMax(child, depth - 1, true, out Move move);
    //             if (childValue < value){
    //                 bestMove = child.Move;
    //                 value = childValue;
    //             }
    //         }
    //     }
    //
    //     return value;
    // }
    private float MinMax(Node node, int depth, float alpha, float beta, bool maximizing, out Move bestMove){
        node.GenerateMoves();

        bestMove = node.Move;
        
        if (depth == 0 || node.IsTerminal){
            bestMove = node.Move;
            return node.CalculateValue(CurrentPlayer);
        }
        
        node.GenerateChildren();

        float value;
        if (maximizing){
            value = -1000;
            foreach (var child in node.Children){
                float childValue = MinMax(child, depth - 1, alpha, beta, false, out Move move);
                if (childValue > value){
                    bestMove = child.Move;
                    value = childValue;
                }

                alpha = Math.Max(alpha, value);
                if (value >= beta) break;
                
            }
        }
        else{
            value = 1000;
            foreach (var child in node.Children){
                float childValue = MinMax(child, depth - 1, alpha, beta, true, out Move move);
                if (childValue < value){
                    bestMove = child.Move;
                    value = childValue;
                }

                beta = Math.Max(beta, value);
                if (value <= alpha) break;
            }
        }

        node.UndoMove();
        return value;
    }

    private void GetStartingPosition(){
        switch (_startingPosition){
            case StartingPosition.Standard:
                _baseBoard = BaseBoards.Standard;
                break;
            case StartingPosition.PawnTest:
                _baseBoard = BaseBoards.PawnMoveTest;
                break;
            case StartingPosition.KnightTest:
                _baseBoard = BaseBoards.KnightMoveTest;
                break;
            case StartingPosition.BishopTest:
                _baseBoard = BaseBoards.BishopMoveTest;
                break;
            case StartingPosition.RookTest:
                _baseBoard = BaseBoards.RookMoveTest;
                break;
            case StartingPosition.QueenTest:
                _baseBoard = BaseBoards.QueenMoveTest;
                break;
            case StartingPosition.KingTest:
                _baseBoard = BaseBoards.KingMoveTest;
                break;
            case StartingPosition.CheckTest:
                _baseBoard = BaseBoards.CheckTest;
                break;
            case StartingPosition.CheckMateTest:
                _baseBoard = BaseBoards.CheckMateTest;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public class Board{
    private Piece[,] _board;
    public Piece[,] CurrentBoard => _board;

    private Team _player;
    public Team CurrentPlayer => _player;

    private Stack<Move> _moves;

    public Board(Piece[,] baseBoard, int currentPlayer){
        _board = baseBoard;
        _player = (Team)currentPlayer;
        _moves = new Stack<Move>();
    }

    public List<Move> GetLegalMoves(){
        List<Move> moves = new List<Move>();
        for (int x = 0; x < _board.GetLength(0); x++){
            for (int y = 0; y < _board.GetLength(1); y++){
                Piece piece = _board[x, y];
                if (piece == null || piece.Team != _player) continue;
                moves.AddRange(piece.PossibleMoves(this, new Vector2Int(x,y)));
            }
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
        List<Move> moves = new List<Move>();
        for (int x = 0; x < _board.GetLength(0); x++){
            for (int y = 0; y < _board.GetLength(1); y++){
                Piece piece = _board[x, y];
                if (piece == null || piece.Team == _player) continue;
                moves.AddRange(piece.PossibleMoves(this, new Vector2Int(x,y)));
            }
        }

        foreach (var move in moves){
            if (move.Defender != null && move.Defender.GetValue() == 0)
                return true;
        }

        return false;
    }

    public void Do(Move move){
        
        move.Do(_board);
        _player = CurrentPlayer == Team.Black? Team.White : Team.Black;
        _moves.Push(move);
    }

    public void Undo(){
        if (!_moves.TryPop(out var move)) return;
        
        move.Undo(_board);
        _player = CurrentPlayer == Team.Black? Team.White : Team.Black;
    }

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