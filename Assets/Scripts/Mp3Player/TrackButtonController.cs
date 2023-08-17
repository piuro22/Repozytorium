using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TrackButtonController : MonoBehaviour
{
    private Mp3TrackProperties mp3TrackProperties;
    [SerializeField] private TMP_Text textTitle;
    public void Initialize(Mp3TrackProperties mp3TrackProperties)
    {
        this.mp3TrackProperties = mp3TrackProperties;
        textTitle.SetText(mp3TrackProperties.title);
    }

    public void OnButtonClick()
    {
        Mp3PlayerController.Instance.PlayTrack(mp3TrackProperties.title);
    }

      public void HighLightButton(bool state)
    {
        if (state == false)
        {
            string currentText = textTitle.text;
            string newText = currentText.Replace("<b>", "").Replace("</b>", "");
            textTitle.text = newText;
        }
        else
        {
            string currentText = textTitle.text;
            string newText = "<b>" + currentText + "</b>";
            textTitle.text = newText;
        }
    }
}

