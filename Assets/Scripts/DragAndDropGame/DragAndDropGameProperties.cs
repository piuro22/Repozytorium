using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

[CreateAssetMenu(fileName = "DragAndDropGameProperties", menuName = "ScriptableObjects/DragAndDropGameProperties", order = 1)]
public class DragAndDropGameProperties : ScriptableObject
{

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



    [BoxGroup("Obiekt kontenera")]
    [LabelText("Użyj losowej pozycji kontenera")]
    public bool useRandomPositionForContainerObjects;
    [BoxGroup("Obiekt kontenera")]
    [LabelText("Użyj losowej rotacji kontenera")]
    public bool useRandomRotationForContainerObjects;

    [BoxGroup("Obiekt kontenera")]
    [LabelText("Użyj innej tekstury dla kontenera")]
    public bool useOtherTextureForContainer;

    [LabelText("Użyj tylko niektórych obiektów")]
    public bool useOnlyPartOfPictures;
    [ShowIf("CheckUseOnlyPartOfPictures")]
    [LabelText("Ilość obiektów do użycia")]
    public int partOfPicturesAmount;

    [BoxGroup("Sekwencja")]
    [LabelText("Użyj sekwencji")]
    public bool useSequence;

    [LabelText("Siła wstrząśnięcia gdy obrazek jest źle dopasowany")]
    public float onWrongContainerShakePower = 0.3f;
    [LabelText("Czas trwania wstrząśnięcia gdy obrazek jest źle dopasowany")]
    public float onWrongContainerShakeDuration = 0.5f;

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

    private bool CheckUseOnlyPartOfPictures()
    {
        return useOnlyPartOfPictures;
    }

   
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






    [BoxGroup("Obiekt kontenera")]
    [ShowIf("CheckUseOtherTextureForContainerObjects")]
    [PreviewField(ObjectFieldAlignment.Left)]
    [LabelText("Alternatywny obrazek kontenera")]
    public Texture2D alternativeTargetTexture;


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

    [BoxGroup("Sekwencja")]
    [ShowIf("CheckUseSequence")]
    [LabelText("@dialogOnSequenceStart.lenght")]
    public AudioClip dialogOnSequenceStart;


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

    private string DialogTime()
    {
        if (dialogOnSequenceStart != null)
        {
            return dialogOnSequenceStart.length.ToString("0.00");
        }
        else { return "Brak klipu"; }
    }


}
[OnInspectorGUI]
public class DragAndDropDrawGizmo : MonoBehaviour
{
   
    void OnDrawGizmos()
    {
        Gizmos.DrawCube(Vector3.zero, Vector3.one*50);
    }
}