using UnityEngine;
using UnityEngine.UI; // Import the UI namespace
using UnityEngine.EventSystems; // Import if you need more detailed event handling

using AssetKits.ParticleImage;
using DG.Tweening;

public class QuizPicturesObjectController : MonoBehaviour, IPointerClickHandler
{
    public Image pictureImage;
    public ParticleImage goodAnswerParticles;
    public ParticleImage badAnswerParticles;
    public SingleQuizPictures SingleQuizPicture;
    private Sequence answerSequence;
    [HideInInspector]
    public QuizPucturesGameController quizPucturesGameController;


    public void SetupPicture()
    {
        goodAnswerParticles.Stop();
        goodAnswerParticles.loop = false;
        badAnswerParticles.Stop();
        badAnswerParticles.loop = false;
        pictureImage.sprite = SingleQuizPicture.picture;
        pictureImage.rectTransform.sizeDelta = new Vector2( SingleQuizPicture.picture.rect.width ,SingleQuizPicture.picture.rect.height);

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick();
    }

    private void OnButtonClick()
    {
        if (SingleQuizPicture.isCorrect)
        {
            OnGoodAnswer();

        }
        else
        {
            OnBadAnswer();

        }
    }

    private void OnGoodAnswer()
    {

        if (answerSequence != null) return;
        answerSequence = DOTween.Sequence();
        answerSequence.AppendCallback(() =>
        {
            goodAnswerParticles.Play();
            quizPucturesGameController.audioSource.PlayOneShot(quizPucturesGameController.gameProperties.goodAnswerSound);
        });
        answerSequence.AppendInterval(quizPucturesGameController.gameProperties.goodAnswerSound.length);

        if (SingleQuizPicture.additionalPictureSound != null)
        {
            answerSequence.AppendCallback(() =>
            {
                quizPucturesGameController.audioSource.PlayOneShot(SingleQuizPicture.additionalPictureSound);
            });
            answerSequence.AppendInterval(SingleQuizPicture.additionalPictureSound.length);

            answerSequence.AppendCallback(() =>
            {
                quizPucturesGameController.OnGoodAnswer();
                answerSequence.Kill();
                answerSequence = null;
            });

          
        }
        else
        {
            answerSequence.AppendCallback(() =>
            {
                quizPucturesGameController.OnGoodAnswer();
               
                answerSequence.Kill();
                answerSequence = null;
            });
        }
    

    }

    private void OnBadAnswer()
    {
        if (answerSequence != null) return;
        answerSequence = DOTween.Sequence();
        transform.DOShakeRotation(0.25f,30,5);
        answerSequence.AppendCallback(() =>
        {
            
            badAnswerParticles.Play();
            quizPucturesGameController.audioSource.PlayOneShot(quizPucturesGameController.gameProperties.badAnswerSound);
        });
        answerSequence.AppendInterval(quizPucturesGameController.gameProperties.badAnswerSound.length);
        if (SingleQuizPicture.additionalPictureSound != null)
        {
            answerSequence.AppendCallback(() =>
            {
                quizPucturesGameController.audioSource.PlayOneShot(SingleQuizPicture.additionalPictureSound);
            });
            answerSequence.AppendInterval(SingleQuizPicture.additionalPictureSound.length);

            answerSequence.AppendCallback(() =>
            {
                quizPucturesGameController.OnBadAnswer();
                answerSequence.Kill();
                answerSequence = null;
            });


        }
        else
        {
            answerSequence.AppendCallback(() =>
            {
                quizPucturesGameController.OnBadAnswer();

                answerSequence.Kill();
                answerSequence = null;
            });
        }
    }
}
