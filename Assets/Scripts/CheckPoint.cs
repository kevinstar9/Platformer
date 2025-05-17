using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public ParticleSystem flame;
    public ParticleSystem glow;
    public ParticleSystem spark;
    void Start()
    {
        ActiveParticles(false);
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
}
