using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LabrinthController : MonoBehaviour
{
    public LabrinthProperties labrinthProperties;

    [SerializeField] private Image background;
    [SerializeField] private TMP_Text titleText;

    public void Initialize()
    {
        titleText.text = labrinthProperties.title;
        background.color = labrinthProperties.backgroundColor;
    }
}
