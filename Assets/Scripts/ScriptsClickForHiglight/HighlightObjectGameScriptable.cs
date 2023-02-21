using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "HighlightObjectGameProperties", menuName = "ScriptableObjects/HighlightObjectGameProperties", order = 1)]
public class HighlightObjectGameScriptable : ScriptableObject
{
    public List<HiglightObject> higlightObjects = new List<HiglightObject>();
}
[Serializable]
public class HiglightObject
{
    public Texture2D texture;
    public Vector2 position;
    public Vector2 scale;
    public float outlineOnTime;
    public float outlineWidth;
    public Color outlineColor;
    public AudioClip soundOnClick;

}
