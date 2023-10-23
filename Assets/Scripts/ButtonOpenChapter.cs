using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ButtonOpenChapter : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [BoxGroup("Button Properties")]
    [SerializeField] private Vector3 onEnterScale;//skala powiekszenia
    [BoxGroup("Button Properties")]
    [SerializeField] private float onEnterScaleAnimationTime;//czas powiekszenia po najechaniu myszka
    [BoxGroup("Button Properties")]
    public TMP_Text chapterTitleText;// tytul poziomu
    private Vector3 startScale; //skala poczatkowa
    private Sequence animationSequnce;
    //[HideInInspector] public Transform chapterProgressPanel;//panel pokazujacy postep gracza

    [BoxGroup("Game Properties")]
    public GameType gameType;//referencja do gry
    [BoxGroup("Game Properties")]
    public List<Object> gameProperties;

    [BoxGroup("Game Properties")]
    [OnValueChanged("ChangeTitleText")]
    public string chapterTitle;

    private void Start()
    {
        startScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)// gdy klikniemy
    {
        PlayerPrefs.SetString("LastChoseGameScene", gameObject.scene.name);
        GameManager.Instance.OnGameFirstStart(gameType, gameProperties);

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
#if UNITY_EDITOR
    private void ChangeTitleText()
    {
        EditorUtility.SetDirty(chapterTitleText);
        chapterTitleText.text = chapterTitle;
    }
#endif
}
