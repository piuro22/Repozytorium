using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChapterButtonsParrentController : MonoBehaviour
{
    public List<ButtonOpenChapter> chapterButtons = new List<ButtonOpenChapter>(); //deklaracja listy przyciskow
    [SerializeField] private Transform chapterProgressPanel; // panel pokazujacy postep gracza

    private void Start()
    {
        foreach (ButtonOpenChapter chapterButton in chapterButtons)//petla przechodzaca po kazdym przycisku (skrypcie buttonOpenChapter)
        {
            chapterButton.chapterTitleText.text = (chapterButton.transform.GetSiblingIndex() +1).ToString();//nadanie numerka widoczne w grze
            chapterButton.chapterProgressPanel = chapterProgressPanel;//nadanie referencji do panelu pokazujacego postep gracza pojedynczym przyciskom
        }
    }
}
