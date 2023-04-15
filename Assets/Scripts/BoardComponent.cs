using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

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
    private enum OccupancyType{
        Occupied,
        Empty,
        None
    }
    
    private Piece[] CurrentBoard=> _board.CurrentBoard;
    
    [Header("Sprites")] 
    public PieceSprites Sprites;
    [SerializeField] private GameObject _pieceParent;
    [SerializeField] private GameObject _tileParent;
    [SerializeField] private GameObject _tileBase;
    
    private Team CurrentPlayer => _board.CurrentPlayer;

    [Header("AI settings")]
    [SerializeField] private int _depth;
    [SerializeField] private float _timeBetweenMoves;
    
    [Header("Game settings")]
    [SerializeField] private StartingPosition _startingPosition;
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private Team _humanPlayer;

    [Header("BitBoard Debug")] 
    [SerializeField] private OccupancyType _occupancyType;
    [SerializeField] [Range(1, 8)] private int _file;
    [SerializeField] [Range(0, 15)] private int _diagonal;
    
    //Actual board
    private Board _board;
    private int[,] _baseBoard;
    
    //Lists for rendering
    private GameObject[,] _tiles;
    private List<GameObject> _possibleMoves;
    private List<GameObject> _occupancyTiles = new();
    private GameObject[,] _pieces;

    //Used to update the piece positions without efficiency issues
    private Move _lastMove;

    //Used for the timer
    private float _time;

    private int _hashCutoffs;

    [SerializeField] private Piece _selected;
    
    private void Start(){
        _pieces = new GameObject[8,8];
        //Get the starting position info
        GetStartingPosition();
        //Set up arrays
        _possibleMoves = new List<GameObject>();

        Piece[] currentBoard = new Piece[64];
        //Instantiate the pieces
        for (int x = 0; x < 8; x++){
            for (int y = 0; y < 8; y++){
                int coord = (7 - x) * 8 + 7 - y;
                
                if (_baseBoard[x,y] == 0) currentBoard[coord] = null;

                int team = 0;
                if (_baseBoard[x, y] < 0) team = 1;

                
                
                switch (Math.Abs(_baseBoard[x,y])){
                    case 1:
                        currentBoard[coord] = new Pawn(team, coord);
                        break;
                    case 2:
                        currentBoard[coord] = new Knight(team, coord);
                        break;
                    case 3:
                        currentBoard[coord] = new Bishop(team, coord);
                        break;
                    case 4:
                        currentBoard[coord] = new Rook(team, coord);
                        break;
                    case 5:
                        currentBoard[coord] = new Queen(team, coord);
                        break;
                    case 6:
                        currentBoard[coord] = new King(team, coord);
                        break;
                }
            }
        }
        Board.PrecomputedMoveData();

        _board = new Board(currentBoard, Team.White);
        //Draw the background tiles
        DrawBoard();
        InstantiatePieces();
        
        BitBoards.PreComputeBitBoards();
        string log = "";
        for (int x = 0; x < 8; x++){
            log += Convert.ToString(BitBoards.Files[x], 2).PadLeft(64, '0') + "\n";
        }
        Debug.Log(log);
    }
    private void Update(){
        // _time += Time.deltaTime;
        // if (_time < _timeBetweenMoves) return;
        // _time = 0;
        //
        // _hashCutoffs = 0;
        // MinMaxAlphaBetaSetUp();
        // Debug.Log("Transposition cutoffs: " + _hashCutoffs);
        // CurrentPlayer = CurrentPlayer == Team.Black ? Team.White : Team.Black;
        //
        // UpdatePieces();

        DrawPossibleMoves();
        
        if (Input.GetMouseButtonDown(0)){
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int mouseX = Mathf.RoundToInt(mousePosition.x);
            int mouseY = Mathf.RoundToInt(mousePosition.y);

            if (_selected != null && _selected.Team == CurrentPlayer){
                foreach (var move in _selected.PossibleMoves(_board).Where(move => move.EndingPosition == mouseY * 8 + mouseX)){
                    _board.Do(move);
                    _lastMove = move;
                    _selected = null;
                    return;
                }
            }
            _selected = CurrentBoard[mouseY * 8 + mouseX];
        }

        UpdatePieces();
        
        DrawOccupancy();
    }

    private void InstantiatePieces(){
        float height = 8;
        float length = 8;

        for (int x = 0; x < height; x++){
            for (int y = 0; y < length; y++){
                if (CurrentBoard[x * 8 + y] != null){
                    GameObject newPiece = new GameObject(CurrentBoard[x * 8 + y].GetSprite(this).name);
                    newPiece.transform.position = new Vector3(y,x, 0);
                    if (_pieceParent != null) newPiece.transform.parent = _pieceParent.transform;
                    
                    var newRenderer = newPiece.AddComponent<SpriteRenderer>() as SpriteRenderer;
                    newRenderer.sprite = CurrentBoard[x * 8 + y].GetSprite(this);
                    _pieces[x, y] = newPiece;
                }
            }
        }
    }
    
    private void UpdatePieces(){
        if (_lastMove == null) return;

        var start = _lastMove.StartingPosition;
        var end = _lastMove.EndingPosition;
        
        int startY = start % 8;
        int startX = (start - startY) / 8;
        
        int endY = end % 8;
        int endX = (end - endY) / 8;
        if (_lastMove.Defender != null){
            _pieces[endX, endY].SetActive(false);
        }
        _pieces[startX, startY].transform.position =
            new Vector3(endY, endX, 0);
        _pieces[endX, endY] = _pieces[startX, startY];
        _pieces[startX, startY] = null;
        _lastMove = null;
    }
    
    private void DrawBoard(){
        int height = 8;
        int length = 8;
        _tiles = new GameObject[height, length];
        
        for (int x = 0; x < height; x++){
            for (int y = 0; y < length; y++){
                var newTile = Instantiate(_tileBase, new Vector3(y, x, 1),
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
        
        if (_selected == null) return;
        
        // List<Move> possibleMoves = _selected.PossibleMoves(_board);

        Color color = Color.blue;
        color.a = 0.5f;

        // if (Board.AttackToBitBoards[_selected.Coordinates] == 0) return;

        long diagonal = BitBoards.CalculateDiagonal(_selected.Coordinates);
        long antiDiagonal = BitBoards.CalculateAntiDiagonal(_selected.Coordinates);

        diagonal |= antiDiagonal;

        diagonal = _selected.GetAttackLines();
        
        for (int x = 0; x < 64; x++){
            // if ((Board.AttackToBitBoards[_selected.Coordinates] & ((ulong) 1 << x)) != 0){
            //     int endY = x % 8;
            //     int endX = (x - endY) / 8;
            //     
            //     var newTile = Instantiate(_tileBase, new Vector3(endY , endX, -1),
            //         Quaternion.identity);
            //
            //     newTile.GetComponent<MeshRenderer>().material.color = color;
            //     _possibleMoves.Add(newTile);
            // }

            // if ((BitBoards.Files[_file - 1] & ((long)1 << x)) != 0){
            //     int endY = x % 8;
            //     int endX = (x - endY) / 8;
            //     
            //     var newTile = Instantiate(_tileBase, new Vector3(endY , endX, -1),
            //         Quaternion.identity);
            //     
            //     newTile.GetComponent<MeshRenderer>().material.color = color;
            //     _possibleMoves.Add(newTile);
            // }

            if ((diagonal & ((long)1 << x)) != 0){
                int endY = x % 8;
                int endX = (x - endY) / 8;
                
                var newTile = Instantiate(_tileBase, new Vector3(endY , endX, -1),
                    Quaternion.identity);
                
                newTile.GetComponent<MeshRenderer>().material.color = color;
                _possibleMoves.Add(newTile);
            }
        }
    }

    private void DrawOccupancy(){
        foreach (var move in _occupancyTiles){
            Destroy(move);
        }
        
        if (_occupancyType == OccupancyType.None) return;
        
        Color red = Color.red;
        red.a = 0.5f;
        
        long bitBoard = _occupancyType == OccupancyType.Occupied ? BitBoards.OccupiedSquares : BitBoards.EmptySquares;

        for (int x = 0; x < 64; x++){
            if ((bitBoard & ((long) 1 << x)) != 0){
                int endY = x % 8;
                int endX = (x - endY) / 8;
                
                var newTile = Instantiate(_tileBase, new Vector3(endY , endX, -1),
                    Quaternion.identity);
            
                newTile.GetComponent<MeshRenderer>().material.color = red;
                _occupancyTiles.Add(newTile);
            }
        }
    }

    private void MinMaxAlphaBetaSetUp(){
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string log;
        
        Node currentPosition = new Node(_board, null, CurrentPlayer);

        if (currentPosition.IsTerminal){
            log = _board.IsInCheck() ? "CheckMate" : "Pat";
            Debug.Log(log);
            return;
        }

        int alpha = int.MinValue;
        int beta = int.MaxValue;
        
        currentPosition.GenerateChildren();
        int value = int.MinValue;
        Node bestNode = null;
        foreach (var child in currentPosition.Children){
            int childValue = MinMax(child, _depth -1, int.MinValue, int.MaxValue, false);
            if (value < childValue){
                value = childValue;
                bestNode = child;
            }

            alpha = Math.Max(alpha, value);
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
            var nodeValue = node.CalculateValue();
            TranspositionTable.Add(_board.Hash, nodeValue, depth);
            return nodeValue;
        }

        if (TranspositionTable.Cutoff(_board.Hash, depth)){
            _hashCutoffs++;
            var hashValue = TranspositionTable.Table[_board.Hash][0];
            node.UndoMove();
            return hashValue;
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
                beta = Math.Min(beta, value);
            }
        }

        if (!TranspositionTable.Add(_board.Hash, value, depth)){
            return TranspositionTable.Table[_board.Hash][0];
        }    
        
        node.UndoMove();
        return value;
    }

    private void GetStartingPosition(){
        _baseBoard = _startingPosition switch{
            StartingPosition.Standard => BaseBoards.Standard,
            StartingPosition.PawnTest => BaseBoards.PawnMoveTest,
            StartingPosition.KnightTest => BaseBoards.KnightMoveTest,
            StartingPosition.BishopTest => BaseBoards.BishopMoveTest,
            StartingPosition.RookTest => BaseBoards.RookMoveTest,
            StartingPosition.QueenTest => BaseBoards.QueenMoveTest,
            StartingPosition.KingTest => BaseBoards.KingMoveTest,
            StartingPosition.CheckTest => BaseBoards.CheckTest,
            StartingPosition.CheckMateTest => BaseBoards.CheckMateTest,
            StartingPosition.BishopPairTest => BaseBoards.BishopPairTest,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}