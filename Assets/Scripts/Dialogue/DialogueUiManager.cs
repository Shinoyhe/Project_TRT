using System;
using UnityEngine;

public class DialogueUiManager : MonoBehaviour
{

    public delegate void FinishLineCallBack();

    /// <summary>
    /// Type out a line of text.
    /// </summary>
    /// <param name="text"> Text to display. </param>
    /// <param name="callback"> Callback after line of text is fully displayed.</param>
    public void DisplayLine(String text, FinishLineCallBack callback)
    {

    }

    /// <summary>
    /// Skip the typing animation of the current line of text.
    /// </summary>
    public void SkipLineAnimation()
    {

    }

}
