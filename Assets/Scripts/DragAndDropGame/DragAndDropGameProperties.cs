using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "DragAndDropGameProperties", menuName = "ScriptableObjects/DragAndDropGameProperties", order = 1)]
public class DragAndDropGameProperties : ScriptableObject
{
    // ??? dodane z high...
    [BoxGroup("Ogólne")]
    [LabelText("Tekstura tła gry")]
    public Sprite backGroundTexture;

    public List<DragAndDropObjectDecorationsProperties> dragAndDropObjectDecorations = new List<DragAndDropObjectDecorationsProperties>();

    [BoxGroup("Ogólne")]
    [LabelText("Muzyka gry")]
    public AudioClip gameMusic;

    [BoxGroup("Ogólne")]
    [LabelText("Dźwięk polecenia do gry")]
    public AudioClip gameCommandAudioClip;

    [BoxGroup("Ogólne")]
    [LabelText("Polecenie w formie tekstu")]
    public string commandText;






    [BoxGroup("Właściwości pojedynczych obiektów")]
    [LabelText("Użyj losowej pozycji obiektu do podnoszenia")]
    public bool useRandomPositionForDragObjects;

    [BoxGroup("Właściwości pojedynczych obiektów")]
    [LabelText("Użyj losowej rotacji obiektu do podnoszenia")]
    public bool useRandomRotationForDragObjects;


    [BoxGroup("Właściwości pojedynczych obiektów")]
    [LabelText("Użyj unikalnych dźwięków dla pojedynczych obiektów Gdy poprawnie dopasujemy obrazek")]
    public bool useCustomAudioClipForSingleObjectOnSuccesfullDrag;

    [BoxGroup("Właściwości pojedynczych obiektów")]
    [LabelText("Użyj unikalnych dźwięków dla pojedynczych obiektów podczas podnoszenia")]
    public bool useCustomAudioClipForSingleObjectOnDrag;

    [BoxGroup("Właściwości pojedynczych obiektów")]
    [LabelText("Prędkość dopasowania do kontenera (w sekundach)")]
    public float snapToContainterTime = 0.25f;

    [BoxGroup("Właściwości pojedynczych obiektów")]
    [LabelText("Dopasuj od razu po kliknięciu")]
    public bool snapOnClick;


    [BoxGroup("Obiekt kontenera")]
    [LabelText("Użyj losowej pozycji kontenera")]
    public bool useRandomPositionForContainerObjects;
    [BoxGroup("Obiekt kontenera")]
    [LabelText("Użyj losowej rotacji kontenera")]
    public bool useRandomRotationForContainerObjects;

    [BoxGroup("Obiekt kontenera")]
    [LabelText("Użyj innej tekstury dla kontenera")]
    public bool useOtherTextureForContainer;

    [BoxGroup("Obiekt kontenera")]
    [LabelText("Przezroczystość kontenera")]
    [Range(0, 1)]
    public float containerTransparency;

    [BoxGroup("Obiekt kontenera")]
    [LabelText("Zakoloruj kontener gdy prawda")]
    [Range(0, 1)]
    public bool containerFillTransparency;




    [BoxGroup("Obiekt kontenera")]
    [LabelText("Nadpisz kolor kontenera")]
    public bool overrideContainerColor;

    [BoxGroup("Obiekt kontenera")]
    [LabelText("Kolor kontenera")]
    public Color containerColor;




    //[LabelText("Użyj tylko niektórych obiektów")]
    //public bool useOnlyPartOfPictures;
    //[ShowIf("CheckUseOnlyPartOfPictures")]
    //[LabelText("Ilość obiektów do użycia")]
    //public int partOfPicturesAmount;




    [BoxGroup("Sekwencja")]
    [LabelText("Użyj sekwencji")]
    public bool useSequence;

    [BoxGroup("Sekwencja")]
    [ShowIf("CheckUseSequence")]
    [LabelText("Dźwięk odtwarzany gdy złapiemy zły obrazek")]
    public AudioClip onWrongObjectAudioClip;

    [BoxGroup("Sekwencja")]
    [LabelText("Sekwencja")]
    [ListDrawerSettings(ListElementLabelName = "ListName")]
    [ShowIf("CheckUseSequence")]
    [InfoBox("Liczba sekwencji musi odpowiadać ilości pojedynczych obiektów!", InfoMessageType.Error, "CheckSequenceLenght")]
    public List<DragAndDropGameSequence> dragAndDropGameSequences = new List<DragAndDropGameSequence>();



    [BoxGroup("Gdy dobrze dopasujemy obrazek")]
    [LabelText("Dźwięk odtwarzany gdy dobrze dopasujemy obrazki")]
    public AudioClip onGoodContainerAudioClip;

    [BoxGroup("Gdy źle dopasujemy obrazek")]
    [LabelText("Siła wstrząśnięcia gdy obrazek jest źle dopasowany")]
    public float onWrongContainerShakePower = 0.3f;

    [BoxGroup("Gdy źle dopasujemy obrazek")]
    [LabelText("Czas trwania wstrząśnięcia gdy obrazek jest źle dopasowany")]
    public float onWrongContainerShakeDuration = 0.5f;

    [BoxGroup("Gdy źle dopasujemy obrazek")]
    [LabelText("Grubość podświetlenia obrazka")]
    [Range(0,1)]
    public float onWrongContainerOutlineWidth = 0.5f;

    [BoxGroup("Gdy źle dopasujemy obrazek")]
    [LabelText("Czas przejścia podświetlenia")]
    [Range(0, 2)]
    public float onWrongContainerOutlineFadeTime = 0.5f;

    [BoxGroup("Gdy źle dopasujemy obrazek")]
    [LabelText("Kolor podświetlenia")]
    public Color onWrongContainerOutlineColor = Color.red;

    [BoxGroup("Gdy źle dopasujemy obrazek")]
    [LabelText("Dźwięk odtwarzany gdy źle dopasujemy obrazki")]
    public AudioClip onWrongContainerAudioClip;



    [ListDrawerSettings(ShowIndexLabels = true)]
    [LabelText("Pojedyncze obiekty")]
    public List<DragAndDropObjectProperties> objects = new List<DragAndDropObjectProperties>();



    [OnInspectorGUI]
    public void Variable()
    {
        foreach (DragAndDropObjectProperties x in objects)
        {
            x.useRandomPositionForDragObjects = useRandomPositionForDragObjects;
            x.useRandomPositionForContainerObjects = useRandomPositionForContainerObjects;
            x.useRandomRotationForDragObjects = useRandomRotationForDragObjects;
            x.useRandomRotationForContainerObjects = useRandomRotationForContainerObjects;
            x.useOtherTextureForContainer = useOtherTextureForContainer;
            x.useCustomAudioClipForSingleObjectOnDrag = useCustomAudioClipForSingleObjectOnDrag;
            x.useCustomAudioClipForSingleObjectOnSuccesfullDrag = useCustomAudioClipForSingleObjectOnSuccesfullDrag;
            x.useSequence = useSequence;
        }
    }

    //private bool CheckUseOnlyPartOfPictures()
    //{
    //    return useOnlyPartOfPictures;
    //}
    private bool CheckUseSequence()
    {
        return useSequence;
    }
    private bool CheckSequenceLenght()
    {
        if (dragAndDropGameSequences.Count != objects.Count)
            return true;
        else
            return false;
    }
}


[Serializable]
public class DragAndDropObjectDecorationsProperties
{
    [LabelText("Tekstura dekoracji")]
    public Texture2D texture;
    [LabelText("Pozycja dekoracji")]
    public Vector2 position;
    [LabelText("Skala dekoracji")]
    public Vector2 scale;
    [LabelText("Warstwa tekoracji")]
    public int layer;
}



[Serializable]
public class DragAndDropObjectProperties
{

    [HideInInspector] public bool useRandomPositionForDragObjects;
    [HideInInspector] public bool useRandomPositionForContainerObjects;

    [HideInInspector] public bool useRandomRotationForDragObjects;
    [HideInInspector] public bool useRandomRotationForContainerObjects;

    [HideInInspector] public bool useOtherTextureForContainer;
    [HideInInspector] public bool useCustomAudioClipForSingleObjectOnDrag;
    [HideInInspector] public bool useCustomAudioClipForSingleObjectOnSuccesfullDrag;
    [HideInInspector] public bool useSequence;

    [BoxGroup("Obiekt do podnoszenia")]
    [LabelText("Unikalne ID potrzebne tylko do sekwencji")]
    public int id;


    [BoxGroup("Obiekt do podnoszenia")]
    [PreviewField(ObjectFieldAlignment.Left)]
    [LabelText("Tekstura obiektu do podnoszenia")]
    public Texture2D texture;



    [BoxGroup("Obiekt do podnoszenia")]
    [HideIf("CheckUseRandomPositionForDragObjects")]
    [LabelText("Pozycja obiektu do podnoszenia")]
    public Vector2 startPosition;

    [BoxGroup("Obiekt do podnoszenia")]
    [ShowIf("CheckUseRandomPositionForDragObjects")]
    [LabelText("Pozycja losowa obiektu do podnoszenia w osi X")]
    public Vector2 startPositionMinMaxX;

    [BoxGroup("Obiekt do podnoszenia")]
    [ShowIf("CheckUseRandomPositionForDragObjects")]
    [LabelText("Pozycja losowa obiektu do podnoszenia w osi Y")]
    public Vector2 startPositionMinMaxY;



    [BoxGroup("Obiekt do podnoszenia")]
    [LabelText("Skala obiektu do podnoszenia")]
    public Vector2 scale = Vector2.one;



    [BoxGroup("Obiekt do podnoszenia")]
    [HideIf("CheckUseRandomRotationForDragObjects")]
    [LabelText("Rotacja obiektu do podnoszenia")]
    public float rotationAngle = 0;

    [BoxGroup("Obiekt do podnoszenia")]
    [ShowIf("CheckUseRandomRotationForDragObjects")]
    [LabelText("Losowa rotacja obiektu do podnoszenia")]
    public Vector2 rotationMinMaxAngle;

    [BoxGroup("Obiekt do podnoszenia")]
    [LabelText("Warstwa obiektu do podnoszenia")]
    public int layer;




    [BoxGroup("Obiekt kontenera/Alternatywny obrazek")]
    [ShowIf("CheckUseOtherTextureForContainerObjects")]
    [PreviewField(ObjectFieldAlignment.Left)]
    [LabelText("Alternatywny obrazek kontenera")]
    public Texture2D alternativeTargetTexture;

    [BoxGroup("Obiekt kontenera/Alternatywny obrazek")]
    [ShowIf("CheckUseOtherTextureForContainerObjects")]
    [LabelText("Dodatkowe przesunięcie dla przesuwanego obiektu")]
    public Vector2 additionalOffset;

    [BoxGroup("Obiekt kontenera/Alternatywny obrazek")]
    [ShowIf("CheckUseOtherTextureForContainerObjects")]
    [LabelText("Skala obiektu do przesuwania po dopasowaniu")]
    public Vector2 endScaleObjectIfAlternativeTexture;

    [BoxGroup("Obiekt kontenera/Alternatywny obrazek")]
    [ShowIf("CheckUseOtherTextureForContainerObjects")]
    [LabelText("Rotacja obiektu do przesuwania po dopasowaniu")]
    public float endRotationObjectIfAlternativeTexture;



    [BoxGroup("Obiekt kontenera")]
    [HideIf("CheckUseRandomPositionForContainerObjects")]
    [LabelText("Pozycja obiektu kontenera")]
    public Vector2 targetPosition;

    [BoxGroup("Obiekt kontenera")]
    [ShowIf("CheckUseRandomPositionForContainerObjects")]
    [LabelText("Pozycja losowa obiektu kontenera w osi X")]
    public Vector2 targetPositionMinMaxX;

    [BoxGroup("Obiekt kontenera")]
    [ShowIf("CheckUseRandomPositionForContainerObjects")]
    [LabelText("Pozycja losowa obiektu kontenera w osi Y")]
    public Vector2 targetPositionMinMaxY;



    [BoxGroup("Obiekt kontenera")]
    [LabelText("Skala obiektu kontenera")]
    public Vector2 targetScale = Vector2.one;



    [BoxGroup("Obiekt kontenera")]
    [HideIf("CheckUseRandomRotationForContainerObjects")]
    [LabelText("Rotacja obiektu kontenera")]
    public float targetRotationAngle;

    [BoxGroup("Obiekt kontenera")]
    [ShowIf("CheckUseRandomRotationForContainerObjects")]
    [LabelText("Losowa Rotacja obiektu kontenera")]
    public Vector2 targetRotationMinMaxAngle;

    [BoxGroup("Obiekt kontenera")]
    [LabelText("Warstwa obiektu kontenera")]
    public int targetLayer;


    [ShowIf("CheckUseCustomAudioClipForSingleObjectOnSuccesfullDrag")]
    [LabelText("Unikalny dźwięk odtwarzany gdy poprawnie dopasujemy dany obrazek")]
    public AudioClip onSuccesfullDragAudioClip;


    [ShowIf("CheckUseCustomAudioClipForSingleObjectOnDrag")]
    [LabelText("Unikalny dźwięk odtwarzany gdy podniesiemy obrazek")]
    public AudioClip onDragAudioClip;



    private bool CheckAlternativeTexture()
    {
        return alternativeTargetTexture;
    }
    private bool CheckUseRandomPositionForDragObjects()
    {
        return useRandomPositionForDragObjects;
    }
    private bool CheckUseRandomPositionForContainerObjects()
    {
        return useRandomPositionForContainerObjects;
    }

    private bool CheckUseRandomRotationForDragObjects()
    {
        return useRandomRotationForDragObjects;
    }
    private bool CheckUseRandomRotationForContainerObjects()
    {
        return useRandomRotationForContainerObjects;
    }
    private bool CheckUseOtherTextureForContainerObjects()
    {
        return useOtherTextureForContainer;
    }
    private bool CheckUseCustomAudioClipForSingleObjectOnDrag()
    {
        return useCustomAudioClipForSingleObjectOnDrag;
    }
    private bool CheckUseCustomAudioClipForSingleObjectOnSuccesfullDrag()
    {
        return useCustomAudioClipForSingleObjectOnSuccesfullDrag;
    }
    private bool CheckUseSequence()
    {
        return useSequence;
    }




}
[Serializable]
public class DragAndDropGameSequence
{
   
    [LabelText("ID chwytanego obiektu")]
    public int objectID;
    [LabelText("Tekst wyświetlany podczas sekwencji")]
    public string textMessage;

    [LabelText("@DialogTime()")]
    public AudioClip dialogOnSequenceStart;


    private string ListName()
    {
        return "sekwencja nr: " + objectID;
    }

    private string DialogTime()
    {
        if (dialogOnSequenceStart != null)
        {
            return $"Dialog na Start sekwencji Czas:{dialogOnSequenceStart.length.ToString("0.00")}s";
        }
        else { return "Dialog na Start sekwencji: Brak klipu"; }
    }
}