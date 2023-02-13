using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MemoryProperties", menuName = "ScriptableObjects/MemoryProperties", order = 1)]
public class MemoryGameProperties : ScriptableObject
{
    [InfoBox("Tekstura maski karty")]
    public Sprite cardMaskTexture;
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

    [ShowIf("memorySize", MemorySize.S2x3)]
    public List<MemoryCardData> memoryCardDatas3x2;
    [ShowIf("memorySize", MemorySize.S4x3)]
    public List<MemoryCardData> memoryCardDatas4x3;
    [ShowIf("memorySize", MemorySize.S4x4)]
    public List<MemoryCardData> memoryCardDatas4x4;
    [ShowIf("memorySize", MemorySize.S5x4)]
    public List<MemoryCardData> memoryCardDatas5x4;
    [ShowIf("memorySize", MemorySize.S6x4)]
    public List<MemoryCardData> memoryCardDatas6x4;

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

        if (memoryCardDatas4x3.Count != 6)
        {
            memoryCardDatas4x3 = new List<MemoryCardData>();
            for (int i = 0; i < 6; i++)
            {
                memoryCardDatas4x3.Add(new MemoryCardData());
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

        if (memoryCardDatas5x4.Count != 10)
        {
            memoryCardDatas5x4 = new List<MemoryCardData>();
            for (int i = 0; i < 10; i++)
            {
                memoryCardDatas5x4.Add(new MemoryCardData());
            }
        }

        if (memoryCardDatas6x4.Count != 12)
        {
            memoryCardDatas6x4 = new List<MemoryCardData>();
            for (int i = 0; i < 12; i++)
            {
                memoryCardDatas6x4.Add(new MemoryCardData());
            }
        }
    }

    public List<MemoryCardData> GetMemoryCardsData(MemorySize memorySize)
    {
        switch (memorySize)
        {
            case MemorySize.S2x3:
                return memoryCardDatas3x2;

            case MemorySize.S4x3:
                return memoryCardDatas4x3;

            case MemorySize.S4x4:
                return memoryCardDatas4x4;

            case MemorySize.S5x4:
                return memoryCardDatas5x4;

            case MemorySize.S6x4:
                return memoryCardDatas6x4;

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
    S2x3,
    S4x3,
    S4x4,
    S5x4,
    S6x4
}