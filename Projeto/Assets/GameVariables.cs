using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject player;
    public GameObject audioManager;

    public class staticVariables
    {
        public static GameObject player;
        public static GameObject audioManager;
    }

    void Start()
    {
        if (staticVariables.player == null) staticVariables.player = player;
        if (staticVariables.audioManager == null) staticVariables.audioManager = audioManager;
    }
}
