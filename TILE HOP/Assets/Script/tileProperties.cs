using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileProperties : MonoBehaviour
{
    internal float angle;
    [SerializeField]
    internal GameObject Diamonds; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void showOrHideDiamonds(bool isOn)
    {
        Diamonds.SetActive(isOn);
    }


}
