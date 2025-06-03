using UnityEngine;

public class GameVariables : MonoBehaviour
{
    public GameObject _player;

    public class staticVariables
    {
        public static GameObject player;
    }

    void Start()
    {
        if (staticVariables.player == null) staticVariables.player = _player;
    }
}
