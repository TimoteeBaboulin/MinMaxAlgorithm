using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Board : MonoBehaviour{
    public Piece[,] CurrentBoard;
    [NonSerialized] public List<Piece> PiecesEaten;
    public PieceSprites Sprites;

    private int[,] _baseBoard ={
        {-4, -2, -3, -5, -6, -3, -2, -4 },
        {-1, -1, -1, -1, -1, -1, -1, -1 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 1,  1,  1,  1,  1,  1,  1,  1 },
        { 4,  2,  3,  5,  6,  3,  2,  4 },
    };
    private List<GameObject> _objects;

    public Vector2Int GetIndex(Piece piece){
        for (int x = 0; x < CurrentBoard.GetLength(0); x++){
            for (int y = 0; y < CurrentBoard.GetLength(1); y++){
                if (CurrentBoard[x, y] == piece) return new Vector2Int(x, y);
            }
        }

        throw new NullReferenceException("Can't find piece");
    }

    public Piece GetPieceAt(Vector2Int coordinates){
        return CurrentBoard[coordinates.x, coordinates.y];
    }

    public bool IsOutOfBounds(Vector2Int coordinates){
        if (coordinates.x > 7 || coordinates.y > 7 || coordinates.x < 0 || coordinates.y < 0) return false;
        return true;
    }

    public void Start(){
        CurrentBoard = new Piece[8, 8];
        _objects = new List<GameObject>();

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
        
        UpdateBoard();
    }

    private void Update(){
        
    }

    private void UpdateBoard(){
        foreach (var gameObject in _objects){
            Destroy(gameObject);
        }
        
        for (int x = 0; x < CurrentBoard.GetLength(0); x++){
            for (int y = 0; y < CurrentBoard.GetLength(1); y++){
                if (CurrentBoard[x, y] != null){
                    GameObject newPiece = new GameObject();
                    newPiece.transform.position = new Vector3(x, y, 0);
                    var renderer = newPiece.AddComponent<SpriteRenderer>() as SpriteRenderer;
                    renderer.sprite = CurrentBoard[x, y].GetSprite(this);
                    _objects.Add(newPiece);
                }
            }
        }
    }
}