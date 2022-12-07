using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour{
    public Piece[,] CurrentBoard;
    [NonSerialized] public List<Piece> PiecesEaten;
    public PieceSprites Sprites;
    [Range(0,1)]public int CurrentPlayer;

    [SerializeField] private GameObject _pieceParent;
    [SerializeField] private GameObject _tileParent;
    [SerializeField] private GameObject _tileBase;
    
    private int[,] _baseBoard = BaseBoards.Standard;
    
    private GameObject[,] _tiles;
    private List<GameObject> _objects;
    private List<GameObject> _possibleMoves;

    public void Start(){
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
                        CurrentBoard[x, y] = new Pawn(team, x, y);
                        break;
                    
                    case 2:
                        CurrentBoard[x, y] = new Knight(team, x, y);
                        break;
                    
                    case 3:
                        CurrentBoard[x, y] = new Bishop(team, x, y);
                        break;
                    
                    case 4:
                        CurrentBoard[x, y] = new Rook(team, x, y);
                        break;
                    
                    case 5:
                        CurrentBoard[x, y] = new Queen(team, x, y);
                        break;
                    
                    case 6:
                        CurrentBoard[x, y] = new King(team, x, y);
                        break;
                }
            }
        }
        
        NegaMax();
        DrawBoard();
        UpdateBoard();
    }

    private void Update(){
        // NegaMax();
        // UpdateBoard();
        // DrawPossibleMoves();
    }

    private void UpdateBoard(){
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
        
        List<Vector2Int[]> possibleMoves = new List<Vector2Int[]>();
        foreach (var piece in CurrentBoard){
            if (piece == null || piece.Team != CurrentPlayer) continue;
            var moves = piece.PossibleMoves(CurrentBoard);
            if (moves != null) possibleMoves.AddRange(moves);
        }
        
        int height = CurrentBoard.GetLength(0);
        int length = CurrentBoard.GetLength(1);
        foreach (var possibleMove in possibleMoves){
            var newTile = Instantiate(_tileBase, new Vector3(length/2 - possibleMove[1].y -0.5f , height/2 - possibleMove[1].x -0.5f, -1),
                Quaternion.identity);
            newTile.GetComponent<MeshRenderer>().material.color = Color.blue;
            _possibleMoves.Add(newTile);
        }
    }

    private void NegaMax(){
        Node currentPosition = new Node(CurrentBoard, CurrentPlayer);
        CurrentBoard = currentPosition.GetMove(3, CurrentPlayer == 0 ? 1 : -1);
    }
}