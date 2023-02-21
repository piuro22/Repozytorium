using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MemoryProperties", menuName = "ScriptableObjects/MemoryProperties", order = 1)]
public class MemoryGameProperties : ScriptableObject
{
    [InfoBox("Tekstura maski karty")]
    public Sprite cardFrontTexture;
    [InfoBox("Tekstura maski karty")]
    public Sprite cardBackTexture;



    [InfoBox("Tekstura tła gry")]
    public Sprite backGroundTexture;
    [InfoBox("Czas po którym odpowiedzi się zakryją")]
    public float waitTimeOnIncorrectReveal = 1.5f;
    [InfoBox("Dźwięk po poprawnym dopasowaniu kart")]
    public AudioClip OnRevealCorrectSound;
    [InfoBox("Dźwięk po niepoprawnym dopasowaniu kart")]
    public AudioClip OnRevealInCorrectSound;
    [InfoBox("Dźwięk ukończenia gry")]
    public AudioClip OnGameFinishedSound;
    [InfoBox("Dźwięk podniesienia pierwszej karty")]
    public AudioClip OnFirstCardRevealSound;

    public MemorySize memorySize;

    [ShowIf("memorySize", MemorySize.S3x2)]
    public List<MemoryCardData> memoryCardDatas3x2;
    [ShowIf("memorySize", MemorySize.S4x2)]
    public List<MemoryCardData> memoryCardDatas4x2;
    [ShowIf("memorySize", MemorySize.S4x3)]
    public List<MemoryCardData> memoryCardDatas4x3;
    [ShowIf("memorySize", MemorySize.S5x2)]
    public List<MemoryCardData> memoryCardDatas5x2;
    [ShowIf("memorySize", MemorySize.S4x4)]
    public List<MemoryCardData> memoryCardDatas4x4;

    [OnInspectorInit]
    private void CreateData()
    {
        if (memoryCardDatas3x2.Count != 3)
        {
            memoryCardDatas3x2 = new List<MemoryCardData>();
            for (int i = 0; i < 3; i++)
            {
                memoryCardDatas3x2.Add(new MemoryCardData());
            }
        }

        if (memoryCardDatas4x2.Count != 4)
        {
            memoryCardDatas4x2 = new List<MemoryCardData>();
            for (int i = 0; i < 4; i++)
            {
                memoryCardDatas4x2.Add(new MemoryCardData());
            }
        }

        if (memoryCardDatas4x3.Count != 6)
        {
            memoryCardDatas4x3 = new List<MemoryCardData>();
            for (int i = 0; i < 6; i++)
            {
                memoryCardDatas4x3.Add(new MemoryCardData());
            }
        }

        if (memoryCardDatas5x2.Count != 5)
        {
            memoryCardDatas5x2 = new List<MemoryCardData>();
            for (int i = 0; i < 5; i++)
            {
                memoryCardDatas5x2.Add(new MemoryCardData());
            }
        }

        if (memoryCardDatas4x4.Count != 8)
        {
            memoryCardDatas4x4 = new List<MemoryCardData>();
            for (int i = 0; i < 8; i++)
            {
                memoryCardDatas4x4.Add(new MemoryCardData());
            }
        }
    }

    public List<MemoryCardData> GetMemoryCardsData(MemorySize memorySize)
    {
        switch (memorySize)
        {
            case MemorySize.S3x2:
                return memoryCardDatas3x2;

            case MemorySize.S4x2:
                return memoryCardDatas4x2;

            case MemorySize.S4x3:
                return memoryCardDatas4x3;

            case MemorySize.S5x2:
                return memoryCardDatas5x2;

            case MemorySize.S4x4:
                return memoryCardDatas4x4;

             default:
                return null;
        }
        
    }




}
[Serializable]
public class MemoryCardData
{
    [TableColumnWidth(57, Resizable = false)]
    [VerticalGroup("Card")]
    [PreviewField(Alignment = ObjectFieldAlignment.Left)]
    public Sprite sprite;
    [VerticalGroup("Card")]
    public AudioClip audioclip;
}


public enum MemorySize
{
    S3x2,
    S4x2,
    S4x3,
    S5x2,
    S4x4
}