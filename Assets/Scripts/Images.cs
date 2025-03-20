using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ImageData")]
public class Images : ScriptableObject
{
    public List<Sprite> images;
}
