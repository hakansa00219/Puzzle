using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grid : MonoBehaviour
{
    private List<GridValue> values = new List<GridValue>();

    public void Init(Transform puzzleScreen)
    {
        CreateGrid(puzzleScreen);
    }

    public void AddValue(GridValue value) => values.Add(value);
    public int GetIndexFromGridIndex((int,int) gridIndex) => values.Find((x) => x.gridIndex == gridIndex).pieceIndex;

    public void CreateGrid(Transform puzzleScreen)
    {
        if(values != null)
        {

            int pieceSize = PuzzleGenerator.GetPuzzlePieceSize();
            GameObject container = new GameObject("GridContainer");
            container.transform.parent = puzzleScreen;

            for (int i = 0; i < values.Count; i++)
            {
                GameObject value = new GameObject(values[i].gridIndex.Item1 + "," + values[i].gridIndex.Item2);
                value.transform.position = new Vector3((values[i].gridIndex.Item1 + 0.5f) * pieceSize, (values[i].gridIndex.Item2 + 0.5f) * pieceSize, -1);

                TextMeshPro textComp = value.AddComponent<TextMeshPro>();
                textComp.text = values[i].pieceIndex + "\n" + values[i].gridIndex.Item1 + "," + values[i].gridIndex.Item2;
                textComp.alignment = TextAlignmentOptions.Center;
                textComp.enableAutoSizing = true;
                textComp.fontSizeMax = 100;
                textComp.color = Color.red;
                textComp.isOrthographic = true;



                value.GetComponent<RectTransform>().sizeDelta = new Vector3(PuzzleGenerator.GetPuzzlePieceSize(), PuzzleGenerator.GetPuzzlePieceSize());

                value.transform.parent = container.transform;

                BoxCollider2D collider = value.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(PuzzleGenerator.GetPuzzlePieceSize(), PuzzleGenerator.GetPuzzlePieceSize());
            }
            
        }
    }
}
public class GridValue
{
    public int pieceIndex;
    public (int,int) gridIndex;
}
