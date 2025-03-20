using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public int pieceIndex;
    public bool isInTrueGridPlace = false;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;


    public void Init(int pieceIndex, Mesh mesh, Material material, Texture2D texture, Material outlineMat)
    {
        this.meshFilter = gameObject.GetComponent<MeshFilter>();
        this.meshCollider = gameObject.GetComponent<MeshCollider>();
        this.meshRenderer = gameObject.GetComponent<MeshRenderer>();
        this.pieceIndex = pieceIndex;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        meshRenderer.sharedMaterial.mainTexture = texture;

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        //Creating outline objects
        for (int i = 0; i < 4; i++)
        {
            GameObject outline = new GameObject("Outline " + i);
            outline.transform.parent = gameObject.transform;
            outline.transform.position = i == 0 ? new Vector3(1, 0, 0) : i == 1 ? new Vector3(-1, 0, 0) : i == 2 ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            outline.AddComponent<MeshFilter>().mesh = mesh;
            outline.AddComponent<MeshRenderer>().sharedMaterial = outlineMat;
        }
        


    }
}
