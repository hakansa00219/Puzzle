using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImageManager : MonoBehaviour
{
    public UIManager uiManager;
    public PuzzleData puzzleData;

    private void Start()
    {
        puzzleData = Resources.Load<PuzzleData>("Data/PuzzleData/PuzzleData");
    }

    public void ShowMediaPicker()
    {
        if (Application.isEditor)
        {
            // Do something else, since the plugin does not work inside the editor
        }
        else
        {
            NativeGallery.GetImageFromGallery((path) =>
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //LocalProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                uiManager.AfterImageSelected(texture);

            });
        }
    }

    public void PrepareStarting()
    {
        //save the sprite cause new scene
        puzzleData.selectedImage = uiManager.panelImage.sprite;
        //load scene
        SceneManager.LoadSceneAsync(1);
        //loading panel active

    }
    public void Test(Texture2D texture)
    {
        uiManager.AfterImageSelected(texture);

    }
}
