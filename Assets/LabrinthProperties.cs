using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "labrinth", menuName = "ScriptableObjects/labrinth",order =1)]

public class LabrinthProperties : ScriptableObject
{
    public int id;
    public string title;
    public Color backgroundColor;
}
