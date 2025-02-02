using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI doggoCountText;
    [SerializeField] private TextMeshProUGUI catAmmoText;
    [SerializeField] private float victoryDelay = 4f;
    [Header("Game Settings")]
    [SerializeField] private int maxCatAmmo = 5;

    private int currentCatAmmo;
    private int doggoCount;
    private LayerMask doggoLayer;

    void Start()
    {
        currentCatAmmo = maxCatAmmo;
        doggoLayer = LayerMask.NameToLayer("Doggo");
        UpdateUI();
    }

    void Update()
    {
        // Count all active doggos

        doggoCount = GameObject.FindGameObjectsWithTag("Doggo").Length;
        UpdateUI();
        if (doggoCount <= 0)
        {
            StartCoroutine(LoadVictoryScene());
        }
    }


    private IEnumerator LoadVictoryScene()
    {

        // Save current level before loading victory scene
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastCompletedLevel", currentSceneName);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(victoryDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Victory");
    }

    public void DecreaseCatAmmo()
    {
        if (currentCatAmmo > 0)
        {
            currentCatAmmo--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (doggoCountText != null)
            doggoCountText.text = $"X {doggoCount}";

        if (catAmmoText != null)
            catAmmoText.text = $"X {currentCatAmmo}";
    }

    public bool HasCatAmmo()
    {
        return currentCatAmmo > 0;
    }
}