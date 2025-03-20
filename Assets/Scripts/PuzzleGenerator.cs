using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    public const int PIECE_MINIMUM_SIZE = 50;
    public const int MINIMUM_PUZZLE_RESOLUTION = 1;

    //public List<Vector3> oldPositions = new List<Vector3>();
    //public List<Vector3> newPositions = new List<Vector3>();
    //public List<GameObject> pieces = new List<GameObject>();

    public MeshCreator meshCreator;
    public Material blackOutline;
    public GameObject puzzleScreen;

    private AllPieces piecesData;
    private static PuzzleData puzzleData;

    private int puzzleSize;
    private Vector2Int textureResolution;

    public static List<GameObject> pieces = new List<GameObject>();

    private void Start()
    {
        puzzleData = Resources.Load<PuzzleData>("Data/PuzzleData/PuzzleData");

        puzzleSize = (int)puzzleData.variables.resolution.x / (int)puzzleData.variables.type.x;
        textureResolution = new Vector2Int((int)puzzleData.variables.resolution.x,(int)puzzleData.variables.resolution.y);



        GeneratePuzzle(puzzleData.selectedImage);
    }

    public void GeneratePuzzle(Sprite image)
    {
        piecesData = Resources.Load<AllPieces>("Data/PuzzleData/AllPieces");
        piecesData.ClearData();
        pieces.Clear();

        Transform parent = new GameObject("Puzzle Pieces").transform;

        Grid grid = puzzleScreen.AddComponent<Grid>();

        Vector2Int puzzleResolution = textureResolution / puzzleSize;

        for (int i = puzzleResolution.y - 1; i >= 0; i--)
        {
            for (int j = 0; j < puzzleResolution.x; j++)
            {

                int puzzleIndex = ((i + 1) * puzzleResolution.x) - j;
                //int puzzleXPos = textureResolution.x - ((puzzleResolution.x - j) * (textureResolution.x / puzzleResolution.x)) - (textureResolution.x / (2 * puzzleResolution.x));
                //int puzzleYPos = textureResolution.y - ((puzzleResolution.y - i) * (textureResolution.y / puzzleResolution.y)) - (textureResolution.y / (2 * puzzleResolution.y));

                Mesh mesh = meshCreator.CreateMesh(puzzleIndex, puzzleSize, textureResolution, piecesData);
                Material defaultShader = Resources.Load<Material>("DefaultMaterial") as Material;
                Texture2D texture = image.ConvertToTexture();

                GameObject pieceObj = new GameObject("Puzzle Piece " + puzzleIndex, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(PuzzlePiece));
                pieceObj.GetComponent<PuzzlePiece>().Init(puzzleIndex, mesh, defaultShader, texture, blackOutline);
                pieceObj.transform.position = new Vector3(textureResolution.x * 0.5f, textureResolution.y * 0.5f, 0);/*(puzzleXPos + (puzzleSize / 2)), (puzzleYPos + (puzzleSize / 2)), - puzzleIndex);*/

                pieceObj.transform.parent = parent;

                pieces.Add(pieceObj);
                grid.AddValue(new GridValue() { gridIndex = (j, i), pieceIndex = puzzleIndex });

            }
        }

        FixCamera();
        FixPuzzleScreen();

        grid.Init(puzzleScreen.transform);

        PuzzleRandomizer(textureResolution);
        LayerAllPieces();

        Destroy(FindObjectOfType<Polygon>().GetComponent<PolygonCollider2D>());
    }

    private void PuzzleRandomizer(Vector2 puzzleResolution)
    {
        //bool isXBigger = puzzleResolution.x >= puzzleResolution.y;
        //float positiveXMax = isXBigger ? puzzleResolution.x * 1.25f : puzzleResolution.y * 1.25f;
        //float positiveXMin = puzzleResolution.x;
        //float negativeXMax = -50;
        //float negativeXMin = isXBigger ? puzzleResolution.x * (-0.25f) : (puzzleResolution.y * 1.25f - puzzleResolution.x) * (-1);
        //float positiveYMax = isXBigger ? puzzleResolution.x * 1.25f - 20 : puzzleResolution.y * 1.25f - 20;
        //float positiveYMin = puzzleResolution.y;
        //float negativeYMax = -50;
        //float negativeYMin = isXBigger ? (puzzleResolution.x * 1.25f - puzzleResolution.y) * (-1) + 50 : puzzleResolution.y * (-0.25f) + 50;
        int counter = 0;
        foreach (var piece in PuzzleGenerator.pieces)
        {
            //set a random position
            //int random = UnityEngine.Random.Range(0, 4); //0 left , 1 right, 2 down , 3 up
            Vector3 rnd = new Vector3(UnityEngine.Random.Range((Camera.main.transform.position.x - Camera.main.orthographicSize * 1.77f) * 0.9f, (Camera.main.transform.position.x + Camera.main.orthographicSize * 1.77f) * 0.9f),
                                      UnityEngine.Random.Range((Camera.main.transform.position.y - Camera.main.orthographicSize) * 0.9f, (Camera.main.transform.position.y + Camera.main.orthographicSize) * 0.9f));
            //switch(random)
            //{
            //    case 0:
            //        rnd = new Vector3(UnityEngine.Random.Range(negativeXMin, negativeXMax), UnityEngine.Random.Range(negativeYMax, positiveYMin), 0);
            //        break;
            //    case 1:
            //        rnd = new Vector3(UnityEngine.Random.Range(positiveXMin, positiveXMax), UnityEngine.Random.Range(negativeYMax, positiveYMin), 0);
            //        break;
            //    case 2:
            //        rnd = new Vector3(UnityEngine.Random.Range(negativeXMin, positiveXMax), UnityEngine.Random.Range(negativeYMin, negativeYMax), 0);
            //        break;
            //    case 3:
            //        rnd = new Vector3(UnityEngine.Random.Range(negativeXMin, positiveXMax), UnityEngine.Random.Range(positiveYMin, positiveYMax), 0);
            //        break;
            //    default:
            //        rnd = new Vector3(UnityEngine.Random.Range(negativeXMin, positiveXMax), UnityEngine.Random.Range(positiveYMin, positiveYMax), 0);
            //        break;

            //}

            //piece.transform.position = rnd;

            StartCoroutine(PuzzleStartAnimation(piece, rnd, counter));
            counter++;
        }
    }

    IEnumerator PuzzleStartAnimation(GameObject piece,Vector3 newPos,float delay)
    {
        yield return new WaitForSeconds(1f + delay / 120);
        float t = 0;
        Vector3 currenPos = piece.transform.position;
        while(t < 1)
        {
            piece.transform.position = Vector3.Lerp(currenPos, newPos, t);
            t += Time.deltaTime;
            yield return null;
        }
    }
    public void LayerAllPieces()
    {
        foreach (var piece in PuzzleGenerator.pieces)
        {
            piece.transform.position = new Vector3(piece.transform.position.x, piece.transform.position.y, piece.transform.GetSiblingIndex() * -1);
        }
    }
    public void FixCamera()
    {
        Camera.main.transform.position = GetCameraPosition();
        Camera.main.orthographicSize = GetCameraOrtoSize();
    }

    public void FixPuzzleScreen()
    {
        puzzleScreen.transform.position = new Vector3(GetCameraPosition().x, GetCameraPosition().y, 0);
        puzzleScreen.transform.localScale = new Vector3(textureResolution.x, textureResolution.y, 1) * 100;
    }
    public static int GetPuzzlePieceSize() => (int)(puzzleData.variables.resolution.x / puzzleData.variables.type.x);
    public float GetCameraOrtoSize() => textureResolution.x >= textureResolution.y ? textureResolution.x * 0.75f : textureResolution.y * 0.75f;
    public Vector3 GetCameraPosition() => new Vector3(textureResolution.x / 2, textureResolution.y / 2, -99999f);
}
