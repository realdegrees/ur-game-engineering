using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class StoryManager : MonoBehaviour
{
    public TextMeshProUGUI storyTextContainer;
    public Button continueButton;
    public Color sceneBackgroundColor = Color.gray;
    public float lettersPerSecond = 10f; // Speed of text display
    public List<AudioClip> audioOnLetter; // List of audio clips to play on each letter
    private AudioSource audioSource;
    private string storyText;

    private Camera mainCamera;
    private Color cachedBackgroundColor;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UIManager.Instance.Disable();
        mainCamera = Camera.main;
        cachedBackgroundColor = mainCamera.backgroundColor;
        mainCamera.backgroundColor = sceneBackgroundColor;

        storyText = storyTextContainer.text; // Store the original text
        StartCoroutine(ShowTextGradual());
        continueButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.NextLevel();
        });
    }

    IEnumerator ShowTextGradual()
    {
        storyTextContainer.text = ""; // Clear the text container
        bool playSound = true; // Toggle to control sound playback
        foreach (char letter in storyText.ToCharArray())
        {
            storyTextContainer.text += letter; // Add one letter at a time
            if (playSound && audioOnLetter.Count > 0)
            {
                audioSource.PlayOneShot(audioOnLetter[Random.Range(0, audioOnLetter.Count)]); // Play a random audio clip from the list
            }
            playSound = !playSound; // Toggle the sound playback
            yield return new WaitForSeconds(1f / lettersPerSecond); // Wait for a short duration before adding the next letter
        }
        continueButton.gameObject.SetActive(true); // Show the continue button after the text is fully displayed
    }
    void OnDestroy()
    {
        UIManager.Instance.Enable();
        mainCamera.backgroundColor = cachedBackgroundColor; // Restore the original background color
        continueButton.onClick.RemoveAllListeners(); // Remove all listeners from the continue button
    }
}
