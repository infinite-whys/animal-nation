using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CameraMovementManager : MonoBehaviour
{
    public static CameraMovementManager Instance;


    public float rotationSpeed = 2.0f; // Speed at which the camera rotates
    public float zoomSpeed = 5.0f; // Speed at which the camera zooms
    public float rotationSmoothing = 0.1f; // Smoothing factor for rotation
    public float zoomSmoothing = 0.1f; // Smoothing factor for zoom

    public Canvas fadeCanvas; // Canvas with the fade overlay
    public Image fadeImage; // Image component used for fading

    public UnityEvent OnCameraMovementFinishedEvent;

    public GameObject CameraRegionSelectionPosition;

    public AudioClip rotationSound; // Sound effect for rotation
    public AudioClip zoomSound; // Sound effect for zoom

    private AudioSource audioSource;

    bool CanStopCoroutines = true;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        OnCameraMovementFinishedEvent.AddListener(OnCameraMovementFinished);

        // Ensure the fade image is initially fully transparent
        if (fadeImage)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }

    public void MoveCamera(Camera camera, Vector3 end, bool fade)
    {
        if (CanStopCoroutines)
        {
            StopAllCoroutines();
            StartCoroutine(MoveCameraCoroutine(camera, end, 0f, fade));
        }
    }
    public void MoveAndRotateCamera(Camera camera, Vector3 end, float rotationAmount, bool fade)
    {
        if (CanStopCoroutines)
        {
            StopAllCoroutines();
            StartCoroutine(MoveCameraCoroutine(camera, end, rotationAmount, fade));
        }
    }

    private IEnumerator MoveCameraCoroutine(Camera camera, Vector3 end, float rotationAmount, bool fade)
    {
        CanStopCoroutines = !fade;

        // Play rotation sound effect
        if (rotationSound && audioSource && !fade)
        {
            audioSource.clip = rotationSound;
            audioSource.loop = false;
            audioSource.Play();
        }

        // Prepare for rotation if rotationAmount is greater than 0
        Quaternion initialRotation = camera.transform.rotation;
        Quaternion targetRotation = initialRotation;
        if (rotationAmount > 0f)
        {
            // Calculate target rotation based on rotationAmount
            targetRotation = Quaternion.Euler(rotationAmount, camera.transform.rotation.eulerAngles.y, camera.transform.rotation.eulerAngles.z);
        }

        // Fade in if needed
        if (fade && fadeImage)
        {
            StartCoroutine(FadeToBlack(fadeImage, 0f, 1f, zoomSmoothing / 3));
        }

        // Play zoom sound effect
        if (zoomSound && audioSource && fade)
        {
            audioSource.PlayOneShot(zoomSound);
        }

        // Simultaneous movement and rotation
        float elapsed = 0f;
        Vector3 initialPosition = camera.transform.position;
        float totalDuration = Mathf.Max(rotationSmoothing, zoomSmoothing); // Use max of both smoothing durations for synchronized transition

        while (elapsed < totalDuration)
        {
            float t = elapsed / totalDuration;

            // Smooth movement to end position
            camera.transform.position = Vector3.Lerp(initialPosition, end, t);

            // Smooth rotation if rotationAmount is greater than 0
            if (rotationAmount > 0f)
            {
                camera.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and rotation
        camera.transform.position = end;
        if (rotationAmount > 0f)
        {
            camera.transform.rotation = targetRotation;
        }

        // Stop rotation sound effect
        if (audioSource && rotationSound)
        {
            audioSource.Stop();
        }

        // Fade out after movement completes if needed
        /*if (fadeImage)
        {
            StartCoroutine(FadeToBlack(fadeImage, 1f, 0f, zoomSmoothing));
        }*/
        if (fade)
            OnCameraMovementFinishedEvent?.Invoke();
    }

    void OnCameraMovementFinished()
    {
        Camera cam = Camera.main;
        cam.transform.position = CameraRegionSelectionPosition.transform.position;
        cam.transform.rotation = CameraRegionSelectionPosition.transform.rotation;
    }

    private IEnumerator FadeToBlack(Image image, float startAlpha, float endAlpha, float duration)
    {
        fadeCanvas.gameObject.SetActive(true);
        float elapsed = 0f;
        Color color = image.color;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            color.a = alpha;
            image.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = endAlpha;
        image.color = color; // Ensure final alpha

        if (endAlpha == 0)
            fadeCanvas.gameObject.SetActive(false);
    }

    public void Restart()
    {
        StartCoroutine(FadeToBlack(fadeImage, 1f, 0f, zoomSmoothing));
    }
}
