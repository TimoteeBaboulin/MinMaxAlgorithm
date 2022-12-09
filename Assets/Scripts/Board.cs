using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Board : MonoBehaviour{
    private enum StartingPosition{
        Standard,
        PawnTest,
        KnightTest,
        BishopTest,
        RookTest,
        QueenTest,
        KingTest
    }
    
    public Piece[,] CurrentBoard;
    [NonSerialized] public List<Piece> PiecesEaten;
    public PieceSprites Sprites;
    [Range(0,1)]public int CurrentPlayer;

    [SerializeField] private int _depth;
    
    [SerializeField] private GameObject _pieceParent;
    [SerializeField] private GameObject _tileParent;
    [SerializeField] private GameObject _tileBase;

    [SerializeField] private StartingPosition _startingPosition;
    private int[,] _baseBoard;
    
    private GameObject[,] _tiles;
    private List<GameObject> _objects;
    private List<GameObject> _possibleMoves;
    private Move _bestMove;

    private float _time;

    private void Start(){
        GetStartingPosition();
        CurrentBoard = new Piece[8, 8];
        _objects = new List<GameObject>();
        _possibleMoves = new List<GameObject>();

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
        
        // NegaMax();
        // CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
        // UpdatePieces();
        DrawBoard();
    }
    private void Update(){
        NegaMaxSetUp();
        CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
        
        UpdatePieces();
        DrawPossibleMoves();
    }

    private void UpdatePieces(){
        foreach (var gameObject in _objects){
            Destroy(gameObject);
        }

        foreach (var move in _possibleMoves){
            Destroy(move);
        }

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
                    _objects.Add(newPiece);
                }
            }
        }
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
        
        List<Move> possibleMoves = new List<Move>();

        for (int x = 0; x < CurrentBoard.GetLength(0); x++){
            for (int y = 0; y < CurrentBoard.GetLength(1); y++){
                Piece piece = CurrentBoard[x, y];
                if (piece == null || piece.Team != CurrentPlayer) continue;
                var moves = piece.PossibleMoves(CurrentBoard, new Vector2Int(x,y));
                if (moves != null) possibleMoves.AddRange(moves);
            }
        }

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
    
    private void NegaMaxSetUp(){
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        _bestMove = null;
        
        Node currentPosition = new Node(CurrentBoard, null);
        float value = NegaMax(currentPosition, _depth, -1000, 1000, 1);
        Debug.Log("Chose move with value: " + value);

        if (_bestMove == null){
            Debug.Log("Couldn't find move");
            return;
        }
        
        _bestMove.Do(CurrentBoard);
        
        Debug.Log("Ce coups a pris: " + stopwatch.Elapsed.ToString() + " secondes.");
        stopwatch.Stop();
    }

    private float NegaMax(Node node, int depth, int color){
        node.GenerateMoves(CurrentPlayer);
        if (depth == 0 || node.IsTerminal)
            return node.CalculateValue(CurrentPlayer) * color;
        
        node.GenerateChildren();
        float value = -1000;
        foreach (var child in node.Children){
            float childValue = -NegaMax(child, depth - 1, -color);
            if (value < childValue){
                value = childValue;
                _bestMove = child.Move;
            }
        }

        return value;
    }
    private float NegaMax(Node node, int depth, float alpha, float beta, int color){
        node.GenerateMoves(CurrentPlayer);
        if (depth == 0 || node.IsTerminal)
            return node.CalculateValue(CurrentPlayer) * color;
        
        node.GenerateChildren();
        float value = -1000;
        foreach (var child in node.Children){
            float childValue = -NegaMax(child, depth - 1, -beta, -alpha, -color);
            if (value < childValue){
                value = childValue;
                _bestMove = child.Move;
            }

            alpha = Math.Max(alpha, value);
            if (alpha >= beta)
                break;

        }

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
            default:
                throw new ArgumentOutOfRangeException();
        }

        _baseBoard = BaseBoards.Test;
    }
}