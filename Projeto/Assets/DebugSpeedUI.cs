using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugSpeedUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (1.0f / Time.deltaTime).ToString();
    }
}
