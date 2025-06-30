using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Audio;

public class GuiBehaviour : MonoBehaviour
{

    public TextMeshProUGUI score;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI endScore;
    public TextMeshProUGUI restartTip;
    public GameObject hand;
    public AudioMixer audioMixer;
    private GameData gameData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, "gameData.dat"), FileMode.OpenOrCreate);
        if (File.Exists(Path.Combine(Application.persistentDataPath, "gameData.dat")) && fileStream.Length > 0)
        {
            gameData = (GameData)formatter.Deserialize(fileStream);
            fileStream.Close();
        }
        else
        {
            gameData = new GameData();
            fileStream.Close();
        }
        highScore.gameObject.SetActive(false);
        endScore.gameObject.SetActive(false);
        restartTip.gameObject.SetActive(false);
        score.gameObject.SetActive(true);
        hand.SetActive(true);
        audioMixer.SetFloat("Volume", 0f);
    }
    private void Awake()
    {
        PlayerEvents.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world != null)
        {
            var entityManager = world.EntityManager;
            try
            {
                if (entityManager.Exists(entityManager.CreateEntityQuery(typeof(GameComponentData)).GetSingletonEntity()))
                {
                    var gameComponetData = entityManager.CreateEntityQuery(typeof(GameComponentData)).GetSingleton<GameComponentData>();
                    if (gameComponetData.score > gameData.highscore)
                    {
                        gameData.highscore = gameComponetData.score;
                        BinaryFormatter formatter = new BinaryFormatter();
                        FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, "gameData.dat"), FileMode.Create);
                        formatter.Serialize(fileStream, gameData);
                        fileStream.Close();
                    }
                }
            }
            catch (Exception)
            {
                Debug.LogError("Error while trying to save game data: " + Path.Combine(Application.persistentDataPath, "gameData.dat"));
            }
        }
        highScore.gameObject.SetActive(true);
        endScore.gameObject.SetActive(true);
        score.gameObject.SetActive(false);
        restartTip.gameObject.SetActive(true);
        endScore.text = score.text;
        highScore.text = "High Score: " + gameData.highscore.ToString();
        hand.SetActive(false);
        audioMixer.SetFloat("Volume", -80f);
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerDeath -= OnPlayerDeath;
    }
    void Update()
    {  
        
        var world = World.DefaultGameObjectInjectionWorld;
        if (world != null)
        {
            var entityManager = world.EntityManager;
            try
            {
                if (entityManager.Exists(entityManager.CreateEntityQuery(typeof(GameComponentData)).GetSingletonEntity()))
                {
                    var gameData = entityManager.CreateEntityQuery(typeof(GameComponentData)).GetSingleton<GameComponentData>();
                    score.text = gameData.score.ToString();
                }
            }
            catch (System.Exception)
            {
                score.text = 0.ToString();
            }
        }
    }

}
