using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class player_kmh_view : MonoBehaviour
{
    // a 0 takes up 2 spaces
    public TextMeshProUGUI text;
    public static player_kmh_view instance;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        text = GameObject.FindGameObjectWithTag("player_speed").GetComponent<TextMeshProUGUI>();
    }
}
