using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MemoryProperties", menuName = "ScriptableObjects/MemoryProperties", order = 1)]
public class MemoryGameProperties : ScriptableObject
{

    [LabelText("Tekstura maski karty")]
    public Sprite cardFrontTexture;
    [LabelText("Tekstura maski karty")]
    public Sprite cardBackTexture;
    [LabelText("Siła wstrząsu")]
    public float shakePower;
    [LabelText("Czas trwania wstrząsu")]
    public float shakeDouration;
    [LabelText("Użyj animacji rotacji")]
    public bool useRotationAnimation;
    [LabelText("Użyj animacji przenikania")]
    public bool useFadeAnimation;
    [LabelText("Użyj animacji maski")]
    public bool useClippingAnimation;
    [LabelText("Użyj animacji podskoku na starcie")]
    public bool useJumpOnStartAnimation;
    [LabelText("Użyj alternatywnej tekstury dla drugiej karty")]
    public bool useAlternativeTexture;


    [LabelText("Tekstura tła gry")]
    public Sprite backGroundTexture;
    [LabelText("Czas po którym odpowiedzi się zakryją")]
    public float waitTimeOnIncorrectReveal = 1.5f;
    [LabelText("Dźwięk po poprawnym dopasowaniu kart")]
    public AudioClip OnRevealCorrectSound;
    [LabelText("Dźwięk po niepoprawnym dopasowaniu kart")]
    public AudioClip OnRevealInCorrectSound;
    [LabelText("Dźwięk ukończenia gry")]
    public AudioClip OnGameFinishedSound;
    [LabelText("Dźwięk podniesienia pierwszej karty")]
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


    [InfoBox("Muzyka gry")]
    public AudioClip gameMusic;

    [LabelText("Dźwięk polecenia do gry")]
    public AudioClip gameCommandAudioClip;



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

    [OnInspectorGUI]
    public void Variable()
    {
        foreach (MemoryCardData x in memoryCardDatas3x2)
        {
            x.useAlternativeTexture = useAlternativeTexture;
        }
        foreach (MemoryCardData x in memoryCardDatas4x2)
        {
            x.useAlternativeTexture = useAlternativeTexture;
        }
        foreach (MemoryCardData x in memoryCardDatas4x3)
        {
            x.useAlternativeTexture = useAlternativeTexture;
        }
        foreach (MemoryCardData x in memoryCardDatas5x2)
        {
            x.useAlternativeTexture = useAlternativeTexture;
        }
        foreach (MemoryCardData x in memoryCardDatas4x4)
        {
            x.useAlternativeTexture = useAlternativeTexture;
        }
    }


}
[Serializable]
public class MemoryCardData
{
    [HideInInspector] public bool useAlternativeTexture;

   
    [BoxGroup("Card")]
    [TableColumnWidth(57, Resizable = false)]
    [PreviewField(Alignment = ObjectFieldAlignment.Left)]
    public Sprite sprite;

    [BoxGroup("Card")]
    public AudioClip audioclip;

    [BoxGroup("Card")]
    [LabelText("Wiadomość po odkryciu karty")]
    public string message;


    [BoxGroup("Second Card")]
    [TableColumnWidth(57, Resizable = false)]

    [ShowIf("CheckAlternativeTexture")]
    [BoxGroup("Second Card")]
    [PreviewField(Alignment = ObjectFieldAlignment.Left)]
    [LabelText("Alternatywna tekstura karty")]
    public Sprite alternativeSecondSprite;

    [ShowIf("CheckAlternativeTexture")]
    [BoxGroup("Second Card")]
    [LabelText("Alternatywny klip audio")]
    public AudioClip alternativeAudioclip;

    [ShowIf("CheckAlternativeTexture")]
    [BoxGroup("Second Card")]
    [LabelText("Wiadomość po odkryciu alternatywnej karty")]
    public string alternativeMessage;


    private bool CheckAlternativeTexture()
    {
        return useAlternativeTexture;
    }
}


public enum MemorySize
{
    S3x2,
    S4x2,
    S4x3,
    S5x2,
    S4x4
}