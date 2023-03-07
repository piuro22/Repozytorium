using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "HighlightObjectGameProperties", menuName = "ScriptableObjects/HighlightObjectGameProperties", order = 1)]
public class HighlightObjectGameScriptable : ScriptableObject
{
    [InfoBox("Tekstura tła gry",InfoMessageType.None)]
    public Sprite backGroundTexture;

    [InfoBox("Muzyka gry")]
    public AudioClip gameMusic;

    public List<HighlightGameSequence> gameSequence = new List<HighlightGameSequence>();
    public List<HiglightObject> higlightObjects = new List<HiglightObject>();
}
[Serializable]
public class HighlightGameSequence
{
    public AudioClip audioClipOnSingleSequenceStart;
   
    public List<ObjectsToHighlight> objectsToHighlights = new List<ObjectsToHighlight>();
    [InfoBox("Czas sekwencji", InfoMessageType.None)]
    public float SequenceTime;
    [InfoBox("Czy czekać na kliknięcie?", InfoMessageType.None)]
    public bool waitForClick;
    [ShowIf("CheckWaitForClickState")]
    [InfoBox("dźwięk przy kliknięciu", InfoMessageType.None)]
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
    [InfoBox("Unikalne ID dla obrazka", InfoMessageType.None)]
    public int id;
    [InfoBox("Tekstura obrazka", InfoMessageType.None)]
    public Texture2D texture;
    [InfoBox("Pozycja obrazka", InfoMessageType.None)]
    public Vector2 position;
    [InfoBox("Skala obrazka", InfoMessageType.None)]
    public Vector2 scale;
    [InfoBox("Czas podświetlenia", InfoMessageType.None)]
    public float outlineOnTime;
    [InfoBox("Grubość podświetlenia", InfoMessageType.None)]
    [PropertyRange(0,1)]
    public float outlineWidth;
    [InfoBox("Kolor podświetlenia", InfoMessageType.None)]
    public Color outlineColor;
    [InfoBox("Dźwięk kliknięcia w obrazek", InfoMessageType.None)]
    public AudioClip soundOnClick;
}
