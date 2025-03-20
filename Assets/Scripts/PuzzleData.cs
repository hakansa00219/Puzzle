using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PuzzleData")]
public class PuzzleData : ScriptableObject
{
    public Sprite selectedImage;
    public Variables variables;


    [System.Serializable]
    public class Variables
    {
        public Vector2 resolution;
        public int pieceAmount;
        public Vector2 type;
    }
}
