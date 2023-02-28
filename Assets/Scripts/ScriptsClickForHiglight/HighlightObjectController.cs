using DigitalRuby.AdvancedPolygonCollider;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Collections;

public class HighlightObjectController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Texture2D _texture;
    private Sprite tempSprite;
    public Material material;
    [HideInInspector] public int index;
    [HideInInspector] public HiglightObject higlightObjectProperites;
    private AudioSource audioSource;
    private Sequence highlightSequence;

    public bool shouldPlayAudioOnClick;
    [HideInInspector] public bool isLocked;
    [HideInInspector] public bool shouldCheckClickedAction;
    [HideInInspector] public HighlightObjectGameController highlightObjectGameController;
    public bool WasClicked
    {
        get { return wasClicked; }
        set
        {
            if (value == true)
            {
                if(shouldCheckClickedAction)
                highlightObjectGameController.objectClickedAction.Invoke();
            }
            wasClicked = value;
        }
    }
    public bool wasClicked;






    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    [Button]
    public void SetupSprite()
    {
        Texture2D texture = duplicateTexture(_texture);
        tempSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        spriteRenderer.sprite = tempSprite;
        spriteRenderer.material = material;


        spriteRenderer.material.EnableKeyword("OUTBASE_ON");
        spriteRenderer.material.SetFloat("_OutlineAlpha", 0);
        gameObject.AddComponent<PolygonCollider2D>();
    }

    private void Update()
    {




        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null)

                if (hit.collider.transform == transform)
                {
                    OnClick();
                }
        }
    }


    public void OnClick()
    {
        if (!isLocked)
        {
            WasClicked = true;
            HighlightObjectOnClick();
        }
    }

    public void Play()
    {
        if (shouldPlayAudioOnClick)
        {
            PlayAudio();
        }
        HiglightObject();
    }




    public void PlayAudio()
    {
        audioSource.PlayOneShot(higlightObjectProperites.soundOnClick);
        StartCoroutine(WaitForStopSound());
    }
    IEnumerator WaitForStopSound()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        DeselectObject();
    }

    public void HighlightObjectOnClick()
    {
        if (highlightSequence != null) highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        highlightSequence.Append(spriteRenderer.material.DOFloat(1, "_OutlineAlpha", higlightObjectProperites.outlineOnTime));
        highlightSequence.Join(spriteRenderer.material.DOColor(higlightObjectProperites.outlineColor, "_OutlineColor", 0));
        highlightSequence.Join(spriteRenderer.material.DOFloat(higlightObjectProperites.outlineWidth, "_OutlineWidth", 0));
        highlightSequence.Append(spriteRenderer.material.DOFloat(0, "_OutlineAlpha", higlightObjectProperites.outlineOnTime));
    }

    public void HiglightObject()
    {
        if (highlightSequence != null) highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        highlightSequence.Append(spriteRenderer.material.DOFloat(1, "_OutlineAlpha", higlightObjectProperites.outlineOnTime));
        highlightSequence.Join(spriteRenderer.material.DOColor(higlightObjectProperites.outlineColor, "_OutlineColor", 0));
        highlightSequence.Join(spriteRenderer.material.DOFloat(higlightObjectProperites.outlineWidth, "_OutlineWidth", 0));
    }
    public void DeselectObject()
    {
        if (highlightSequence != null) highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        highlightSequence.Append(spriteRenderer.material.DOFloat(0, "_OutlineAlpha", higlightObjectProperites.outlineOnTime));
    }



    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
