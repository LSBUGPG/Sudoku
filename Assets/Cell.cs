using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public TMP_Text content;

    internal void Reset()
    {
        content.text = " ";
    }

    internal void Set(int value)
    {
        content.text = value.ToString();
    }
}
