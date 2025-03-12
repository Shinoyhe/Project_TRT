using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarterWinScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(bool win)
    {
        if (win)
        {
            resultText.text = "BARTER SUCCESS";
        } else
        {
            resultText.text = "BARTER FAILED";
        }
    }
}
