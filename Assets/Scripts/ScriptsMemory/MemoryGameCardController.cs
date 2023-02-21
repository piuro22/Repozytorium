using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MPUIKIT;

public class MemoryGameCardController : MonoBehaviour
{
    public SpriteRenderer cardBackImage;
    public SpriteRenderer cardFrontImage;
    public SpriteRenderer cardTopImage;
    [SerializeField] private float revealSpeed = 0.5f;
    private AudioSource audioSource;
    private AudioClip revealAudioClip;
    private BoxCollider boxCollider;
    private Sequence rotateSequence;
    [SerializeField] private AnimationCurve curve1;
    [SerializeField] private AnimationCurve curve2;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();
    }
    void OnMouseDown()
    {
        OnCardClick();
    }
    public void Initialize(Sprite cardBackTexture,Sprite cardFrontTexture , Sprite cardTopSprite, AudioClip audioClip)
    {
        cardBackImage.sprite = cardBackTexture;
        cardFrontImage.sprite = cardFrontTexture;
        cardTopImage.sprite = cardTopSprite;
        revealAudioClip = audioClip;
    }

    public void OnCardClick()
    {

        if (!MemoryGameController.Instance.CheckSequencesCompleted())
            return;
        boxCollider.enabled = false;
        audioSource.PlayOneShot(revealAudioClip);

        if (rotateSequence != null) rotateSequence.Kill();


        rotateSequence = DOTween.Sequence();
        rotateSequence.Append(transform.DOLocalRotate(new Vector3(0, 180, 0), revealSpeed).SetEase(curve2));

        MemoryGameController.Instance.RevealCard(this);
    }

    public void HideCard()
    {
        boxCollider.enabled = true;
        transform.DOLocalRotate(new Vector3(0, 0, 0), revealSpeed);
    }

}
