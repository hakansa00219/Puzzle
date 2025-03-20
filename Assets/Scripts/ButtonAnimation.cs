using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    public List<Sprite> textures = new List<Sprite>();

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        
    }
    public void ButtonReleased()
    {
        image.sprite = textures[0];
    }
    public void ButtonPressed()
    {
        image.sprite = textures[1];
    }

    private void OnDisable()
    {
        image.sprite = textures[0];
    }
}