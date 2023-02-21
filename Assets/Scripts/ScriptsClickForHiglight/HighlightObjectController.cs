using DigitalRuby.AdvancedPolygonCollider;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;
public class HighlightObjectController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public AdvancedPolygonCollider polygonCollider;
    public Texture2D _texture;
    private Sprite tempSprite;
    public Material material;
    [HideInInspector] public int index;
    [HideInInspector] public HiglightObject higlightObjectProperites;
    private AudioSource audioSource;
    private Sequence onClickSequence;
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
        polygonCollider.RecalculatePolygon();
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
        if (onClickSequence != null) onClickSequence.Kill();
        onClickSequence = DOTween.Sequence();

        onClickSequence.Append( spriteRenderer.material.DOFloat(1,"_OutlineAlpha", higlightObjectProperites.outlineOnTime));
        onClickSequence.Join( spriteRenderer.material.DOColor(higlightObjectProperites.outlineColor, "_OutlineColor", 0));
        onClickSequence.Join(spriteRenderer.material.DOFloat(higlightObjectProperites.outlineWidth, "_OutlineWidth", 0));
        onClickSequence.Append(spriteRenderer.material.DOFloat(0, "_OutlineAlpha", higlightObjectProperites.outlineOnTime));
     audioSource.PlayOneShot(higlightObjectProperites.soundOnClick);
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
