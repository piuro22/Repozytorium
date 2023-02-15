using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
public class ButtonOpenChapter : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 onEnterScale;//skala powiekszenia
    [SerializeField] private float onEnterScaleAnimationTime;//czas powiekszenia po najechaniu myszka
    [SerializeField] private Vector3 progressPanelOffset;//przesuniecie panelu pokazujacego postep gracza
    public TMP_Text chapterTitleText;// tytul poziomu
    private Vector3 startScale; //skala poczatkowa
    private Sequence animationSequnce;
    //[HideInInspector] public Transform chapterProgressPanel;//panel pokazujacy postep gracza

    public GameType gameType;//referencja do gry
    public Object gameProperties;


    private void Start()
    {
        startScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)// gdy klikniemy
    {
        GameManager.Instance.OpenGame(gameType, gameProperties);

    }

    public void OnPointerEnter(PointerEventData eventData) //Gdy najedziemy myszka
    {
        if (animationSequnce != null)
        {
            animationSequnce.Kill(); // Usuwamy sekwencje aby nie dublowac animacji
        }

        animationSequnce = DOTween.Sequence(); //deklarujemy nowa sekwencje
        animationSequnce.Append(transform.DOScale(onEnterScale, onEnterScaleAnimationTime)); //powiekszamy przycisk po najechaniu myszka
                                                                                             //  chapterProgressPanel.transform.position = transform.position + progressPanelOffset;
                                                                                             // chapterProgressPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) // gdy wyjdziemy kursorem poza przycisk
    {

        if (animationSequnce != null)
        {
            animationSequnce.Kill(); // Usuwamy sekwencje aby nie dublowac animacji
        }
        animationSequnce = DOTween.Sequence(); //deklarujemy nowa sekwencje
        animationSequnce.Append(transform.DOScale(startScale, onEnterScaleAnimationTime));//przywracamy poprzednia skale  przycisku
                                                                                          //   chapterProgressPanel.gameObject.SetActive(false);
    }
}
