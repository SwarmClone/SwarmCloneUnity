using System.Collections;
using System.Collections.Generic;
using Tomlyn.Syntax;
using UnityEngine;
using UnityEngine.UI;
public class scrollbarautomovedown : MonoBehaviour
{
    Scrollbar bar;
    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void Update()
    {
        bar.value = 0;
    }
}
