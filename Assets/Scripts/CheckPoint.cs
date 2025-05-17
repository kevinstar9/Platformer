using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour
{
    public ParticleSystem flame;
    public ParticleSystem glow;
    public ParticleSystem spark;
    public GameObject checkpointTextObject;
    private TextMeshPro checkpointText;
    void Start()
    {
        ActiveParticles(false);
        checkpointText = checkpointTextObject.GetComponent<TextMeshPro>();
        checkpointTextObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActiveParticles(true);
            PlayerHP player = collision.GetComponent<PlayerHP>();
            if (player != null)
            {
                player.SetCheckpoint(transform.position);
                ShowCheckpointText();
            }
        }
    }
    void ActiveParticles(bool isActive)
    {
        if (isActive)
        {
            if (!flame.isPlaying) flame.Play();
            if (!glow.isPlaying) glow.Play();
            if (!spark.isPlaying) spark.Play();
        }
    }
    void ShowCheckpointText()
    {
        if (checkpointText == null) return;

        checkpointTextObject.SetActive(true);
        Color originalColor = checkpointText.color;
        checkpointText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        StartCoroutine(FadeText());
    }
    IEnumerator FadeText()
    {
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color originalColor = checkpointText.color;
            checkpointText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        checkpointTextObject.SetActive(false);
    }

}
