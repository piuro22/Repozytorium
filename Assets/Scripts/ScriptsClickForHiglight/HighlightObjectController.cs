using DigitalRuby.AdvancedPolygonCollider;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Collections;
using EPOOutline;

public class HighlightObjectController : MonoBehaviour
{
    private bool isScaled = false;   //CHAT 4

    public SpriteRenderer spriteRenderer;
    public Texture2D _texture;
    private Sprite tempSprite;
    public Material material;
    [HideInInspector] public int index;
    [HideInInspector] public HiglightObject higlightObjectProperites;
    public AudioSource audioSource;
    private Sequence highlightSequence;
    private Outlinable outlinable;
    public bool shouldPlayAudioOnClick;
    [HideInInspector] public bool isLocked;
    [HideInInspector] public bool shouldCheckClickedAction;
    [HideInInspector] public HighlightObjectGameController highlightObjectGameController;
    public bool WasClicked
    {
        get { return wasClicked; }
        set
        {
            if (value && shouldCheckClickedAction)
            {
                highlightObjectGameController.objectClickedAction?.Invoke();
                Debug.Log($"WasClicked set to true for {name}");
            }
            wasClicked = value;
        }
    }
    public bool wasClicked;






    private void Start()
    {
        outlinable = GetComponent<Outlinable>();
        audioSource = GetComponent<AudioSource>();
    }
    [Button]
    public void SetupSprite()
    {
        outlinable = GetComponent<Outlinable>();
        Texture2D texture = duplicateTexture(_texture);
        tempSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        spriteRenderer.sprite = tempSprite;
        spriteRenderer.material = material;

        outlinable.OutlineParameters.DilateShift = 0;
        outlinable.OutlineParameters.BlurShift = 0;
        gameObject.AddComponent<PolygonCollider2D>();
    }

    private void Update()
    {




        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null)
            {
                if (hit.collider.transform == transform)
                {
                    if (isLocked)
                        audioSource.PlayOneShot(highlightObjectGameController.highlightObjectGameScriptable.wrongClickAudioClip);

                    OnClick();
                    return;
                }
                else
                {
                    
                }
              

            }

           // audioSource.PlayOneShot(highlightObjectGameController.highlightObjectGameScriptable.goodClickAudioClip);
        }
    }


    public void OnClick()
    {
        if (!isLocked)
        {
            isLocked = true;
            WasClicked = true;
            PlayAudio(); // Play sound for the clicked object
        }
    }

    public void OnMouseDown()
    {
        if (!isLocked && !audioSource.isPlaying) // Ignore clicks if locked or sound is playing
        {
            PlayAudio();
        }
    }


    public void PlayAudio()
    {
        if (isScaled) return; // Je�li obiekt ju� zosta� powi�kszony, zako�cz dzia�anie CHAT4

        isScaled = true; // Oznacz, �e obiekt zosta� powi�kszony  CHAT4

        // Play the click sound
        audioSource.PlayOneShot(higlightObjectProperites.soundOnClick);

        // Scale the object up by 20% and back to default
        Vector3 originalScale = transform.localScale; // Save the original scale
        transform.DOScale(originalScale * highlightObjectGameController.highlightObjectGameScriptable.scaleOnClickMultiplier, highlightObjectGameController.highlightObjectGameScriptable.scaleOnClickTime) // Scale up
            .OnComplete(() => transform.DOScale(originalScale, 0.2f)); // Scale back

        // Wait for the sound to finish, then unlock the object
        StartCoroutine(WaitForStopSound());
    }
    IEnumerator WaitForStopSound()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        isLocked = false; // Unlock the object after the sound finishes
        DeselectObject();
    }

    public void HighlightObjectOnClick()
    {
        if (highlightSequence != null) highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        highlightSequence.Append(DOTween.To(() => outlinable.OutlineParameters.DilateShift, x => outlinable.OutlineParameters.DilateShift = x, higlightObjectProperites.outlineWidth, higlightObjectProperites.outlineOnTime));
        highlightSequence.Join(DOTween.To(() => outlinable.OutlineParameters.BlurShift, x => outlinable.OutlineParameters.BlurShift = x, higlightObjectProperites.outlineWidth, higlightObjectProperites.outlineOnTime));
        highlightSequence.Append(DOTween.To(() => outlinable.OutlineParameters.DilateShift, x => outlinable.OutlineParameters.DilateShift = x, 0, higlightObjectProperites.outlineOnTime));
        highlightSequence.Join(DOTween.To(() => outlinable.OutlineParameters.BlurShift, x => outlinable.OutlineParameters.BlurShift = x, 0, higlightObjectProperites.outlineOnTime))
            .OnComplete(() => UnlockObject()); // Unlock the object after animation
    }
    private void UnlockObject()
    {
        isLocked = false; // Unlock the object
    }
    public void HiglightObject()
    {
        if (highlightSequence != null) highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        highlightSequence.Append(DOTween.To(() => outlinable.OutlineParameters.DilateShift, x => outlinable.OutlineParameters.DilateShift = x, higlightObjectProperites.outlineWidth, higlightObjectProperites.outlineOnTime));
        highlightSequence.Join(DOTween.To(() => outlinable.OutlineParameters.BlurShift, x => outlinable.OutlineParameters.BlurShift = x, higlightObjectProperites.outlineWidth, higlightObjectProperites.outlineOnTime));
    }
    public void DeselectObject()
    {
        if (highlightSequence != null) highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        highlightSequence.Append(DOTween.To(() => outlinable.OutlineParameters.DilateShift, x => outlinable.OutlineParameters.DilateShift = x, 0, higlightObjectProperites.outlineOnTime));
        highlightSequence.Join(DOTween.To(() => outlinable.OutlineParameters.BlurShift, x => outlinable.OutlineParameters.BlurShift = x, 0, higlightObjectProperites.outlineOnTime));
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
