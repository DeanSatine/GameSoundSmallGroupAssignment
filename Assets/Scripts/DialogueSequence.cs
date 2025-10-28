using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

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
    public GameObject monster;
    public GameObject player;

    [Header("Audio Clips")]
    public EventReference monsterLine1;
    public EventReference playerLine1;
    public EventReference monsterLine2;
    public EventReference laughClip;

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

        yield return StartCoroutine(PlayVoiceLine(monsterLine1, monster));

        yield return StartCoroutine(PlayVoiceLine(playerLine1, player));

        yield return StartCoroutine(PlayVoiceLine(monsterLine2,  monster));

        yield return StartCoroutine(MonsterJumpScare());
        AudioManager.instance.ReleaseEventInstance();

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

    IEnumerator PlayVoiceLine(EventReference clip, GameObject position)
    {
        if (clip.Path != null)
        {
            yield return AudioManager.instance.PlayVoiceLine(clip, position, true);
            AudioManager.instance.ReleaseEventInstance();
            //yield return new WaitForSeconds(clip.);
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

        if (laughClip.Path != null)
        {
             yield return AudioManager.instance.PlayVoiceLine(laughClip, monster, false);
        }

        float spazzTime = spazzDuration;
        float checkSpazzTime = AudioManager.instance.GetSoundLengthInSeconds(laughClip);
        if (laughClip.Path != null && checkSpazzTime > spazzDuration)
        {
            spazzTime = checkSpazzTime;
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
