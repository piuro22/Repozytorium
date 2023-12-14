using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUnitAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Button_touch()
    {
        GetComponent<Animation>().Play("Button Animation");
    }
}
