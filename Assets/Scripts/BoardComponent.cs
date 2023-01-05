using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
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
        CheckMateTest,
        BishopPairTest
    }
    private enum GameMode{
        AIvsAI,
        PlayerVsAI
    }
    
    private Piece[,] CurrentBoard=> _board.CurrentBoard;
    
    [NonSerialized] public List<Piece> PiecesEaten;
    [Header("Sprites")] 
    public PieceSprites Sprites;
    [SerializeField] private GameObject _pieceParent;
    [SerializeField] private GameObject _tileParent;
    [SerializeField] private GameObject _tileBase;

    [Header("Current player, do not touch")]
    [Range(0,1)]public Team CurrentPlayer;

    [Header("AI settings")]
    [SerializeField] private int _depth;
    [SerializeField] private float _timeBetweenMoves;
    
    [Header("Game settings")]
    [SerializeField] private StartingPosition _startingPosition;
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private Team _humanPlayer;
    
    //Actual board
    private Board _board;
    
    //Base board
    private int[,] _baseBoard;
    
    //Lists for rendering
    private GameObject[,] _tiles;
    private List<GameObject> _possibleMoves;
    private GameObject[,] _pieces;

    //Used to update the piece positions without efficiency issues
    private Move _lastMove;

    //Used for the timer
    private float _time;

    private void Start(){
        _pieces = new GameObject[8,8];
        //Get the starting position info
        GetStartingPosition();
        //Set up arrays
        _possibleMoves = new List<GameObject>();

        Piece[,] currentBoard = new Piece[8, 8];
        //Instantiate the pieces
        for (int x = 0; x < currentBoard.GetLength(0); x++){
            for (int y = 0; y < currentBoard.GetLength(1); y++){
                if (_baseBoard[x,y] == 0) currentBoard[x,y] = null;

                int team = 0;
                if (_baseBoard[x, y] < 0) team = 1;

                switch (Math.Abs(_baseBoard[x,y])){
                    case 1:
                        currentBoard[x, y] = new Pawn(team, new Vector2Int(x,y));
                        break;
                    case 2:
                        currentBoard[x, y] = new Knight(team, new Vector2Int(x,y));
                        break;
                    case 3:
                        currentBoard[x, y] = new Bishop(team, new Vector2Int(x,y));
                        break;
                    case 4:
                        currentBoard[x, y] = new Rook(team, new Vector2Int(x,y));
                        break;
                    case 5:
                        currentBoard[x, y] = new Queen(team, new Vector2Int(x,y));
                        break;
                    case 6:
                        currentBoard[x, y] = new King(team, new Vector2Int(x,y));
                        break;
                }
            }
        }

        _board = new Board(currentBoard, (int)CurrentPlayer);
        //Draw the background tiles
        DrawBoard();
        InstantiatePieces();
    }
    private void Update(){
        _time += Time.deltaTime;
        if (_time < _timeBetweenMoves) return;
        _time = 0;
        MinMaxAlphaBetaSetUp();
        CurrentPlayer = CurrentPlayer == Team.Black ? Team.White : Team.Black;
        
        UpdatePieces();
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
                    
                    var newRenderer = newPiece.AddComponent<SpriteRenderer>() as SpriteRenderer;
                    newRenderer.sprite = CurrentBoard[x, y].GetSprite(this);
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

    private void MinMaxAlphaBetaSetUp(){
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string log;
        
        Node currentPosition = new Node(_board, null, CurrentPlayer);

        if (currentPosition.IsTerminal){
            if (_board.IsInCheck())
                log = "CheckMate";
            else 
                log = "Pat";
            Debug.Log(log);
            return;
        }
        
        currentPosition.GenerateChildren();
        int value = int.MinValue;
        Node bestNode = null;
        foreach (var child in currentPosition.Children){
            int childValue = MinMax(child, _depth -1, int.MinValue, int.MaxValue, false);
            if (value < childValue){
                value = childValue;
                bestNode = child;
            }
        }

        log = "Valeur du coups: " + value + "\nCe coups a pris: " + stopwatch.Elapsed;
        Debug.Log(log);
        
        _board.Do(bestNode.Move);
        _lastMove = bestNode.Move;

        stopwatch.Stop();
    }

    private int MinMax(Node node, int depth, float alpha, float beta, bool maximizing){
        node.DoMove();

        if (depth == 0 || node.IsTerminal){
            node.UndoMove();
            return node.CalculateValue();
        }

        node.GenerateChildren();

        int value;
        if (maximizing){
            value = int.MinValue;
            foreach (var child in node.Children){
                value = Mathf.Max(value, MinMax(child, depth - 1, alpha, beta, false));

                if (value > beta) break;
                alpha = Math.Max(alpha, value);
            }
        }
        else{
            value = int.MaxValue;
            foreach (var child in node.Children){
                value = Mathf.Min(value, MinMax(child, depth - 1, alpha, beta, true));
                
                if (value < alpha) break;
                beta = Math.Max(beta, value);
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
            case StartingPosition.BishopPairTest:
                _baseBoard = BaseBoards.BishopPairTest;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}