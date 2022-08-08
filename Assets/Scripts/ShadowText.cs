using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShadowText : MonoBehaviour
{
    public TextMeshProUGUI sourceText;
    public TextMeshProUGUI myText;

    private void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        myText.text = sourceText.text;
    }
}
