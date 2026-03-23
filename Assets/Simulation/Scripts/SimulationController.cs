using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class SimulationController : MonoBehaviour
{
    public static SimulationController Instance { get; private set; }

    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private TextMeshProUGUI fireState;
    [SerializeField] private TextMeshProUGUI rainState;

    private bool isRainning = false;

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        rainParticles.Stop();
        fireParticles.GetComponent<FireBehaviour>().Initialize();
        fireState.text = "";
        rainState.text = "";
    }

    /// <summary>
    /// Switch to FSM Scene.
    /// </summary>
    public void OnFSMScene()
    {
        SceneManager.LoadScene("FSM_Scene");
    }

    /// <summary>
    /// Button calls.
    /// </summary>
    public void OnFire()
    {
        int maxParticles = fireParticles.GetComponent<FireBehaviour>().MaxFireParticles;
        SetMaxParticles(fireParticles, maxParticles);
        fireState.text = "Play";

        StartCoroutine(SetTextDefault(fireState));
    }

    /// <summary>
    /// Button calls.
    /// </summary>
    public void OnRain()
    {
        if (isRainning)
        {
            rainParticles.Stop();
            rainState.text = "Stop";
        }
        else
        {
            rainParticles.Play();
            rainState.text = "Play";
        }

        isRainning = !isRainning;
        StartCoroutine(SetTextDefault(rainState));
    }

    /// <summary>
    /// Set default value.
    /// </summary>
    /// <param name="textMeshProUGUI"></param>
    /// <returns></returns>
    private IEnumerator SetTextDefault(TextMeshProUGUI textMeshProUGUI)
    {
        yield return new WaitForSeconds(0.5f);

        textMeshProUGUI.text = "";
    }

    /// <summary>
    /// Set the maximum number of particles for a ParticleSystem.MainModule.
    /// </summary>
    /// <param name="m"></param>
    /// <param name="max"></param>
    public void SetMaxParticles(ParticleSystem particles, int max)
    {
        ParticleSystem.MainModule m = particles.main;
        m.maxParticles = max;
        Debug.Log($"{particles.main.maxParticles} {particles}");
    }

    /// <summary>
    /// OnQuitButton.
    /// </summary>
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(0);
#endif
    }
}
