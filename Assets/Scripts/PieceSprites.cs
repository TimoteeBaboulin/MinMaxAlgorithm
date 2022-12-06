using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Create PieceSprites", fileName = "PieceSprites", order = 0)]
public class PieceSprites : ScriptableObject{
    public Pieces Black;
    public Pieces White;
}

[Serializable]
public struct Pieces{
    public Sprite Pawn;
    public Sprite Knight;
    public Sprite Bishop;
    public Sprite Rook;
    public Sprite Queen;
    public Sprite King;
}