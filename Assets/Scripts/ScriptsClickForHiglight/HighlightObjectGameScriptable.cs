using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "HighlightObjectGameProperties", menuName = "ScriptableObjects/HighlightObjectGameProperties", order = 1)]
public class HighlightObjectGameScriptable : ScriptableObject
{
    [LabelText("Tekstura tła gry")]
    public Sprite backGroundTexture;

    [LabelText("Muzyka gry")]
    public AudioClip gameMusic;

    [LabelText("Dźwięk polecenia do gry")]
    public AudioClip gameCommandAudioClip;

    [LabelText("Dźwięk gdy źle klikniemy")]
    public AudioClip wrongClickAudioClip;

    [LabelText("Mnożnik skali przy kliknięciu")]
    public float scaleOnClickMultiplier = 1.2f;
    [LabelText("Czas skalowania przy kliknięciu")]
    public float scaleOnClickTime = 0.2f;

    // [LabelText("Dźwięk gdy dobrze klikniemy")]
    //  public AudioClip goodClickAudioClip;



    [LabelText("Sekwencje")]
    public List<HighlightGameSequence> gameSequence = new List<HighlightGameSequence>();
    [LabelText("Obiekty do podświetlenia")]
    public List<HiglightObject> higlightObjects = new List<HiglightObject>();
}
[Serializable]
public class HighlightGameSequence
{
    public AudioClip audioClipOnSingleSequenceStart;
   
    public List<ObjectsToHighlight> objectsToHighlights = new List<ObjectsToHighlight>();
    [LabelText("Czas sekwencji")]
    public float SequenceTime;
    [LabelText("Czy czekać na kliknięcie?")]
    public bool waitForClick;
    [ShowIf("CheckWaitForClickState")]
    [LabelText("dźwięk przy kliknięciu")]
    public AudioClip audioClipOnClick;


    private bool CheckWaitForClickState()
    {
        return waitForClick;
    }

}
[Serializable]
public class ObjectsToHighlight
{
    [LabelText("ID obiektu")]
    public int ObjectID;
}


[Serializable]
public class HiglightObject
{
    [LabelText("Unikalne ID dla obrazka")]
    public int id;
    [LabelText("Tekstura obrazka")]
    public Texture2D texture;
    [LabelText("Pozycja obrazka")]
    public Vector2 position;
    [LabelText("Skala obrazka")]
    public Vector2 scale;
    [LabelText("Czas podświetlenia")]
    public float outlineOnTime;
    [LabelText("Grubość podświetlenia")]
    [PropertyRange(0,1)]
    public float outlineWidth;
    [LabelText("Kolor podświetlenia")]
    public Color outlineColor;
    [LabelText("Dźwięk kliknięcia w obrazek")]
    public AudioClip soundOnClick;

}
