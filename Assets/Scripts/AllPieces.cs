using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllPieces",menuName = "Data/AllPieces")]
public class AllPieces : SerializedScriptableObject
{
    [SerializeField]
    public Dictionary<int, PieceData> allPieces = new Dictionary<int, PieceData>();

    public void AddPiece(int key, PieceData value)
    {
        allPieces.Add(key, value);
    }
    public PieceData GetPiece(int index) => allPieces[index];
    public void ClearData()
    {
        allPieces.Clear();
    }
}
public class PieceData
{
    public List<MeshCreator.PieceEdge> pieceEdgeData = new List<MeshCreator.PieceEdge>();
    public Dictionary<Direction, Seed> data = new Dictionary<Direction, Seed>();
}
public struct Seed
{
    public bool isEdgeContainsKnob;
    [ShowIf("isEdgeContainsKnob")]
    public Side side;
    [ShowIf("isEdgeContainsKnob")]
    public Values values;
}

public class Values
{
    public int offset;
    public int offset2;
    public int pos;
    public int pos2;
    public int startPos;
    public int endPos;
}
public enum Direction { LEFT, RIGHT, DOWN, UP }
public enum Side { Inside, Outside};