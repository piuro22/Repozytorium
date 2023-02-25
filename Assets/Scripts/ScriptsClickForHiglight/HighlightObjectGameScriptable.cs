using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "HighlightObjectGameProperties", menuName = "ScriptableObjects/HighlightObjectGameProperties", order = 1)]
public class HighlightObjectGameScriptable : ScriptableObject
{
    public List<HighlightGameSequence> gameSequence = new List<HighlightGameSequence>();
    public List<HiglightObject> higlightObjects = new List<HiglightObject>();
}
[Serializable]
public class HighlightGameSequence
{
    public AudioClip audioClipOnSingleSequenceStart;
   
    public List<ObjectsToHighlight> objectsToHighlights = new List<ObjectsToHighlight>();
    public float SequenceTime;
    public bool waitForClick;
    [ShowIf("CheckWaitForClickState")]
    public AudioClip audioClipOnClick;


    private bool CheckWaitForClickState()
    {
        return waitForClick;
    }

}
[Serializable]
public class ObjectsToHighlight
{
    public int ObjectID;
}


[Serializable]
public class HiglightObject
{
    public int id;
    public Texture2D texture;
    public Vector2 position;
    public Vector2 scale;
    public float outlineOnTime;
    public float outlineWidth;
    public Color outlineColor;
    public AudioClip soundOnClick;
}
