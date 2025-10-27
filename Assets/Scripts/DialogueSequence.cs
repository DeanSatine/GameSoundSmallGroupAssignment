using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueSequence : MonoBehaviour
{
    [Header("References")]
    public Transform monsterTransform;
    public Transform monsterHead;
    public Transform playerCamera;
    public FirstPersonController playerController;
    public Image fadeImage;
    public AudioSource monsterAudioSource;
    public AudioSource playerAudioSource;
    public Animator monsterAnimator;

    [Header("Audio Clips")]
    public AudioClip monsterLine1;
    public AudioClip playerLine1;
    public AudioClip monsterLine2;
    public AudioClip laughClip;

    [Header("Animation Settings")]
    public string walkAnimationName = "Walk";
    public string idleAnimationName = "Idle";

    [Header("Sequence Settings")]
    public Transform monsterApproachTarget;
    public float approachSpeed = 2f;
    public float stopDistance = 0.5f;
    public float jumpDistance = 1f;
    public float spazzDuration = 2f;
    public float spazzIntensity = 0.5f;
    public float cameraFollowSpeed = 5f;
    public string menuSceneName = "MainMenu";

    private bool hasTriggered = false;
    private bool shouldFollowMonster = false;

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.enabled = false;
        }
    }

    void Update()
    {
        if (shouldFollowMonster && monsterTransform != null && playerCamera != null)
        {
            Transform lookTarget = monsterHead != null ? monsterHead : monsterTransform;
            Vector3 directionToMonster = lookTarget.position - playerCamera.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToMonster);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, cameraFollowSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        playerController.SetCanMove(false);
        shouldFollowMonster = true;

        yield return StartCoroutine(MoveMonsterToTarget());

        if (monsterAnimator != null)
        {
            monsterAnimator.Play(idleAnimationName, 0, 0f);
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(PlayVoiceLine(monsterAudioSource, monsterLine1));

        yield return StartCoroutine(PlayVoiceLine(playerAudioSource, playerLine1));

        yield return StartCoroutine(PlayVoiceLine(monsterAudioSource, monsterLine2));

        yield return StartCoroutine(MonsterJumpScare());

        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(menuSceneName);
    }

    IEnumerator MoveMonsterToTarget()
    {
        if (monsterAnimator != null)
        {
            monsterAnimator.Play(walkAnimationName, 0, 0f);
        }

        Vector3 startPosition = monsterTransform.position;
        Vector3 targetPosition = new Vector3(
            monsterApproachTarget.position.x,
            startPosition.y,
            monsterApproachTarget.position.z
        );

        while (Vector3.Distance(
            new Vector3(monsterTransform.position.x, 0, monsterTransform.position.z),
            new Vector3(targetPosition.x, 0, targetPosition.z)) > stopDistance)
        {
            Vector3 newPosition = Vector3.MoveTowards(monsterTransform.position, targetPosition, approachSpeed * Time.deltaTime);
            newPosition.y = startPosition.y;
            monsterTransform.position = newPosition;
            monsterTransform.LookAt(new Vector3(playerCamera.position.x, monsterTransform.position.y, playerCamera.position.z));
            yield return null;
        }
    }

    IEnumerator PlayVoiceLine(AudioSource source, AudioClip clip)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator MonsterJumpScare()
    {
        Vector3 jumpTarget = playerCamera.position + playerCamera.forward * jumpDistance;
        monsterTransform.position = jumpTarget;

        if (laughClip != null && monsterAudioSource != null)
        {
            monsterAudioSource.PlayOneShot(laughClip);
        }

        float spazzTime = spazzDuration;
        if (laughClip != null && laughClip.length > spazzDuration)
        {
            spazzTime = laughClip.length;
        }

        float elapsed = 0f;
        Vector3 startPosition = monsterTransform.position;

        while (elapsed < spazzTime)
        {
            monsterTransform.position = startPosition + Random.insideUnitSphere * spazzIntensity;
            monsterTransform.rotation = Random.rotation;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage != null)
        {
            fadeImage.enabled = true;
        }

        float duration = 1f;
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadeImage.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }
}
