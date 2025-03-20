using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Functions : MonoBehaviour
{
    private bool trigger = false;
    public PuzzleGenerator puzzleGenerator;
    //public void OnButtonClick()
    //{
    //    StopAllCoroutines();
    //    for (int i = 0; i < puzzleGenerator.pieces.Count; i++)
    //    {
    //        StartCoroutine(PieceRoutine(puzzleGenerator.pieces[i], !trigger ? puzzleGenerator.oldPositions[i] : puzzleGenerator.newPositions[i], trigger));
    //    }
    //    trigger = !trigger;
    //}

    //public IEnumerator PieceRoutine(GameObject obj, Vector3 to, bool trigger)
    //{

    //    float t = 0;
    //    Vector3 firstPos = obj.transform.position;

    //    while (t < 1)
    //    {
    //        obj.transform.position = !trigger ? Vector3.Lerp(firstPos, to, t) : Vector3.Lerp(firstPos, to, t);
    //        t += Time.deltaTime;
    //        yield return null;
    //    }

    //    obj.transform.position = to;

    //}
    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
