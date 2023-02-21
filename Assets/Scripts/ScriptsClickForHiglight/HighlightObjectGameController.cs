using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObjectGameController : MonoBehaviour
{
    public HighlightObjectGameScriptable HighlightObjectGameScriptable;
    public HighlightObjectController highlightObjectPrefab;


    private void Start()
    {
        SetupGame();
    }

    [Button]
    private void SetupGame()
    {
       
        int index = 0;
        foreach (HiglightObject higlightObject in HighlightObjectGameScriptable.higlightObjects)
        {
            HighlightObjectController spawnetObject = Instantiate(highlightObjectPrefab);
            spawnetObject.transform.position = higlightObject.position;
            spawnetObject._texture = higlightObject.texture;
            spawnetObject.transform.localScale = higlightObject.scale;
            spawnetObject.index = index;
            spawnetObject.higlightObjectProperites = higlightObject;
            spawnetObject.SetupSprite();
         
            index++;
        }
    }
}
