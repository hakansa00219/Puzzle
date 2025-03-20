using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public static bool isInGame = false;

    public static Saved_Data saved_Data;
    public static bool Save()
    {
        if (!isInGame) return false;

        if (!Directory.Exists(SAVE_FOLDER)) Directory.CreateDirectory(SAVE_FOLDER);

        string json = JsonUtility.ToJson(saved_Data);

        File.WriteAllText(SAVE_FOLDER + "/save.txt", json);

        return true;
    }

    //public static bool Load(out Saved_Data saved_Data)
    //{

    //}


   
}

public class Saved_Data
{
    public List<Pieces_Data> pieces = new List<Pieces_Data>();
    public string puzzleImagePath;
}
public class Pieces_Data
{
    public Vector3 position;
    public bool isInTrueLocation;

}
