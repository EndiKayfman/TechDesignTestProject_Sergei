using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ClickableObject : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string clickAnimTrigger = "Clicked";
    [SerializeField] private float animationCooldown = 0.5f;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem clickParticles;
    private bool particlesPlaying = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    
    private bool isAnimating = false;
    private float lastAnimationTime = 0f;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        if (clickParticles == null)
            clickParticles = GetComponent<ParticleSystem>();
    }

    private void OnMouseDown()
    {
        TryPlayEffects();
    }

    private void TryPlayEffects()
    {
        if (animator != null)
        {
            if (isAnimating || Time.time < lastAnimationTime + animationCooldown)
                return;

            StartAnimationAndSound();
        }
        else if (clickParticles != null)
        {
            ToggleParticles();
        }
        else
        {
            PlaySound();
        }
    }

    private void StartAnimationAndSound()
    {
        isAnimating = true;
        lastAnimationTime = Time.time;

        animator.SetTrigger(clickAnimTrigger);
        float animationLength = GetAnimationLength(clickAnimTrigger);
        
        if (animationLength <= 0)
            animationLength = animationCooldown;
        
        Invoke("OnAnimationComplete", animationLength);
    }

    private void ToggleParticles()
    {
        if (particlesPlaying)
        {
            clickParticles.Stop();
            PlaySound();
        }
        else
        {
            clickParticles.Play();
            PlaySound();
        }

        particlesPlaying = !particlesPlaying;
    }

    private float GetAnimationLength(string triggerName)
    {
        if (animator == null) return 0;
        
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains(triggerName))
            {
                return clip.length;
            }
        }
        return 0;
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clickSound, volume);
        }
    }

    private void OnAnimationComplete()
    {
        PlaySound();
        isAnimating = false;
    }
}