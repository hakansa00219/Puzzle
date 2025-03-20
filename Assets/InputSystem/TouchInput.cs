using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInput : MonoBehaviour
{
    private PuzzleGenerator puzzleGenerator;

    private float zoomOutMin = 100;
    private float zoomOutMax;

    //private const int CameraXYValue = 100;
    public float cameraXMin, cameraXMax, cameraYMin, cameraYMax;
    private float startCamPosX, startCamPosY;
    private Vector2 middlePoint;
    private Vector3 middlePointWorldPos;

    private float cameraSpeed = 50f;
    private float cameraSwipeSpeed = 100f;

    private TouchControls controls;
    private Transform cameraTransform;
    private float previousDst, dst;

    private int puzzlePieceSizeOffset;

    private GameObject draggedPiece;
    private bool isDragging = false;

    private Grid grid;

    private void Awake()
    {
        puzzleGenerator = FindObjectOfType<PuzzleGenerator>();
        controls = new TouchControls();
        cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if(Camera.main.transform.hasChanged)
        {
            float ortho = Camera.main.orthographicSize;
            cameraSwipeSpeed = ortho / 10f;
            cameraXMax = zoomOutMax - ortho; //
            cameraXMin = - zoomOutMax + ortho;
            cameraYMax = zoomOutMax - ortho;
            cameraYMin = -zoomOutMax + ortho;
        }

    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        zoomOutMax = puzzleGenerator.GetCameraOrtoSize();
        grid = FindObjectOfType<Grid>();
        startCamPosX = Camera.main.transform.position.x;
        startCamPosY = Camera.main.transform.position.y;

        puzzlePieceSizeOffset = (int)(PuzzleGenerator.GetPuzzlePieceSize() * 0.5f);

        controls.Touch.PrimaryTouchContact.canceled += _ =>
        {
            if (controls.Touch.SecondaryTouchContact.phase == InputActionPhase.Waiting)
            {
                StopAllCoroutines();
            }
        };
        controls.Touch.SecondaryTouchContact.started += _ =>
        {
            if (controls.Touch.PrimaryTouchContact.phase == InputActionPhase.Performed)
            {
                ZoomStart();
            }
        };

        controls.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();
        controls.Touch.PrimaryTouchContact.started += _ => StartCameraMove();
        controls.Touch.PrimaryTouchContact.canceled += _ => OnTouchContactEnd();
    }

    private void OnTouchContactEnd()
    {
        if(isDragging)
        {
            Vector2 touchPos = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
            if (hit.Length != 0)
            {
                Debug.Log(hit[0].transform.name);

                foreach (var gridItem in hit)
                {
                    if (gridItem.collider is BoxCollider2D)
                    {
                        draggedPiece.transform.position = new Vector3((int)(int.Parse(gridItem.transform.name.Split(',')[0]) * PuzzleGenerator.GetPuzzlePieceSize()), (int)(int.Parse(gridItem.transform.name.Split(',')[1]) * PuzzleGenerator.GetPuzzlePieceSize()));

                        PuzzlePiece piece = draggedPiece.GetComponent<PuzzlePiece>();
                        int? index = grid.GetIndexFromGridIndex(new(int.Parse(gridItem.transform.name.Split(',')[0]), int.Parse(gridItem.transform.name.Split(',')[1])));
                        if (index != null && index.Value == piece.pieceIndex)
                        {
                            draggedPiece.GetComponent<PuzzlePiece>().isInTrueGridPlace = true;
                            Debug.Log("Correct place!");
                        }
                        else
                        {
                            draggedPiece.GetComponent<PuzzlePiece>().isInTrueGridPlace = false;
                            Debug.Log("Wrong place!");
                        }

                        //check if puzzle done
                        bool isPuzzleDone = true;
                        foreach(PuzzlePiece p in PuzzleGenerator.pieces.ConvertAll((x) => x.GetComponent<PuzzlePiece>()))
                        {
                            if(!p.isInTrueGridPlace)
                            {
                                isPuzzleDone = false;
                                break;
                            }
                            
                        }

                        //puzzle is done.
                        if(isPuzzleDone)
                        {
                            Debug.Log("Puzzle done!");
                            UnityEditor.EditorApplication.isPlaying = false;
                        }
                        

                    }
                }
            }


            draggedPiece = null;
            isDragging = false;
        }
        StopAllCoroutines();
    }

    private void StartCameraMove()
    {
        Vector2 fingerPos = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        //Vector2 clickedPos = Camera.main.ScreenToWorldPoint(new Vector2(fingerPos.x, fingerPos.y));
        //clickedPos.z = 100;
        Ray ray = Camera.main.ScreenPointToRay(fingerPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.TryGetComponent<PuzzlePiece>(out _))
            {
                StartCoroutine(PieceDragging(hit.transform.gameObject));
                draggedPiece = hit.transform.gameObject;
            }
            else
            {
                StartCoroutine(CameraMove());
            }
        }
        else
        {
            StartCoroutine(CameraMove());
        }

    }
    IEnumerator PieceDragging(GameObject piece)
    {
        Vector2 pos;
        Vector2 realPos;
        while(true)
        {
            pos = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();


            if(pos != null)
            {
                
                realPos = Camera.main.ScreenToWorldPoint(pos);
                piece.transform.position = new Vector3(realPos.x - puzzlePieceSizeOffset, realPos.y - puzzlePieceSizeOffset, piece.transform.position.z);
                isDragging = true;
                if (piece.transform.GetSiblingIndex() != piece.transform.parent.childCount - 1) 
                {
                    Debug.Log("piece");
                    piece.transform.SetAsLastSibling();
                    puzzleGenerator.LayerAllPieces();
                }
            }

            yield return null;
        }
    }

    IEnumerator CameraMove()
    {
        Vector2? previousPos = null;
        Vector2? pos = null;
        Vector3? dir = null;

        while (true)
        {
            pos = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();

            if (previousPos != null)
            {
                dir = new Vector3(previousPos.Value.x - pos.Value.x, previousPos.Value.y - pos.Value.y,0);
                dir.Value.Normalize();

                cameraTransform.position += dir.Value * Time.deltaTime * cameraSwipeSpeed;

                cameraTransform.position = new Vector3(Mathf.Clamp(cameraTransform.position.x, startCamPosX + cameraXMin, startCamPosX + cameraXMax), Mathf.Clamp(cameraTransform.position.y, startCamPosY + cameraYMin, startCamPosY + cameraYMax), -99999f);
            }


            previousPos = pos;
            yield return null;
        }
    }


    private void ZoomStart()
    {
        Vector2 firstTouchPos = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        Vector2 secondTouchPos = controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();
        middlePoint = (firstTouchPos + secondTouchPos) / 2;
        middlePointWorldPos = Camera.main.ScreenToWorldPoint(middlePoint);
        middlePointWorldPos.y = Camera.main.transform.position.y;
        base.StopAllCoroutines();
        StartCoroutine(ZoomDetection());

    }

    private void ZoomEnd()
    {
        base.StopAllCoroutines();
    }

    IEnumerator ZoomDetection()
    {
        previousDst = 0;
        dst = 0;
        while (true)
        {
            dst = Vector2.Distance(controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(), controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());

            // Detection

            if (dst > previousDst)
            {
                Debug.Log("ZoomOUT");
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, middlePointWorldPos, Time.deltaTime);

                Camera.main.orthographicSize -= cameraSpeed;
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomOutMin, zoomOutMax);
            }

            else if (dst < previousDst)
            {
                Debug.Log("ZoomIN");
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, puzzleGenerator.GetCameraPosition(), Time.deltaTime);
                Camera.main.orthographicSize += cameraSpeed;
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomOutMin, zoomOutMax);
            }
            previousDst = dst;
            yield return null;
        }
    }
}
