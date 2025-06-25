using Unity.Mathematics;
using UnityEngine;

public class lightBehaviour : MonoBehaviour
{
    private Light sceneLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneLight = GetComponent<Light>();
        sceneLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {

        sceneLight.intensity = math.lerp(sceneLight.intensity, 10000, Time.deltaTime * 2);
        sceneLight.intensity = math.clamp(sceneLight.intensity, 0, 10000);
    }
}
