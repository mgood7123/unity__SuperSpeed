using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class player_kmh_view : MonoBehaviour
{
    private TextMeshProUGUI player_speed;
    public static player_kmh_view instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        player_speed = GameObject.FindGameObjectWithTag("player_speed").GetComponent<TextMeshProUGUI>();
        update_speed("    0", "00");
    }
    
    // a 0 takes up 2 spaces
    public void update_speed(string text, string ghost_text)
    {
        player_speed.text = text + " KMH";
    }
}
