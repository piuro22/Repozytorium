using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MPUIKIT;
[CreateAssetMenu(fileName = "HighlightObjectGameProperties", menuName = "ScriptableObjects/HighlightObjectGameProperties", order = 1)]
public class MemoryGameCardController : MonoBehaviour
{
    public SpriteRenderer cardBackImage;
    public SpriteRenderer cardFrontImage;
    public SpriteRenderer cardTopImage;
    public SpriteRenderer cardTop2Image;
    public Sprite targetImage;
    public Sprite alternativeImage;
    private string message;
    [SerializeField] private float revealSpeed = 0.5f;
    private AudioSource audioSource;
    private AudioClip revealAudioClip;
    private BoxCollider boxCollider;
    private Sequence animationSequence;
    [SerializeField] private AnimationCurve curve1;
    [SerializeField] private AnimationCurve curve2;
    [HideInInspector] public float shakePower;
    [HideInInspector] public float shakeDouration;
    private Vector3 startPosition;
    private Vector3 startScale;
    private Vector3 randomPosition;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();

    }

    void OnMouseDown()
    {
        OnCardClick();
    }
    public void Initialize(Sprite cardBackTexture, Sprite cardFrontTexture, MemoryCardData memoryCardData)
    {
        targetImage = memoryCardData.sprite;
        alternativeImage = memoryCardData.alternativeSecondSprite;
        cardBackImage.sprite = cardBackTexture;
        cardFrontImage.sprite = cardFrontTexture;

        if (MemoryGameController.Instance.memoryGameProperties.useAlternativeTexture && alternativeImage != null)
        {
            cardTopImage.sprite = alternativeImage;
            cardTop2Image.sprite = alternativeImage;
            message = memoryCardData.alternativeMessage;
            revealAudioClip = memoryCardData.alternativeAudioclip;
        }
        else
        {
            message = memoryCardData.message;
            cardTopImage.sprite = targetImage;
            cardTop2Image.sprite = targetImage;
            revealAudioClip = memoryCardData.alternativeAudioclip;
        }
        revealAudioClip = memoryCardData.audioclip;

        if (MemoryGameController.Instance.memoryGameProperties.useJumpOnStartAnimation)
        {
            startPosition = transform.position;
            randomPosition = startPosition + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
            transform.position = randomPosition;
            transform.DOJump(startPosition, 1, 0, 1);


            startScale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(startScale, 1);
        }
    }

    public void OnCardClick()
    {

        if (!MemoryGameController.Instance.CheckSequencesCompleted())
            return;
        boxCollider.enabled = false;
        MemoryGameController.Instance.messageText.text = message;


        audioSource.PlayOneShot(revealAudioClip);




        if (animationSequence != null) animationSequence.Kill();
        animationSequence = DOTween.Sequence();

        if (MemoryGameController.Instance.memoryGameProperties.useRotationAnimation)
        {

            animationSequence.Append(transform.DOLocalRotate(new Vector3(0, 180, 0), revealSpeed).SetEase(curve2));
            animationSequence.Join(transform.DOShakePosition(MemoryGameController.Instance.memoryGameProperties.shakeDouration, MemoryGameController.Instance.memoryGameProperties.shakePower));
        }
        if (MemoryGameController.Instance.memoryGameProperties.useFadeAnimation)
        {
            animationSequence.Append(cardBackImage.DOFade(0, revealSpeed).SetEase(curve2));
        }
        if (MemoryGameController.Instance.memoryGameProperties.useClippingAnimation)
        {
            cardBackImage.material.EnableKeyword("CLIPPING_ON");
            cardBackImage.material.EnableKeyword("OFFSETUV_ON");
            animationSequence.Append(cardBackImage.material.DOFloat(1, "_ClipUvDown", revealSpeed).SetEase(curve2));
            animationSequence.Join(cardBackImage.material.DOFloat(-1, "_OffsetUvY", revealSpeed).SetEase(curve2));
        }

        MemoryGameController.Instance.RevealCard(this);
    }

    public void HideCard()
    {
        boxCollider.enabled = true;
        if (MemoryGameController.Instance.memoryGameProperties.useRotationAnimation)
        {
            transform.DOLocalRotate(new Vector3(0, 0, 0), revealSpeed);
        }
        if (MemoryGameController.Instance.memoryGameProperties.useFadeAnimation)
        {
            animationSequence.Append(cardBackImage.DOFade(1, revealSpeed).SetEase(curve2));
        }
        if (MemoryGameController.Instance.memoryGameProperties.useClippingAnimation)
        {
            animationSequence.Append(cardBackImage.material.DOFloat(0, "_ClipUvDown", revealSpeed).SetEase(curve2));

            animationSequence.Join(cardBackImage.material.DOFloat(0, "_OffsetUvY", revealSpeed).SetEase(curve2));
     
        }
    }

}
