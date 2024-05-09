using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundEffectController : MonoBehaviour
{   
    private AudioSource audioSource;
    private Transform Player;

    /// <summary>
    /// Sound 재생을 위한 해당 Object와 Player간의 거리
    /// </summary>
    public float playDistance = 3.2f;
    public float distance;

    /// <summary>
    /// 사정거리 안에 들어왔는지 체크
    /// </summary>
    private bool inDistance = false;
    public bool useFadeVolume = false;

    public GameObject backgroundAudio;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Player = Camera.main.transform.parent;
    }

    private void Start()
    {
        audioSource.playOnAwake = false;
        //audioSource.maxDistance = 2.8f;
    }


    void Update()
    {
        distance = Vector3.Distance(transform.position, Player.position);

        if (distance < playDistance && inDistance == false)
        {
            //Debug.Log($"{gameObject.name}/ {audioSource.clip} Play/ {audioSource.loop}");
            audioSource.Play();
            inDistance = true;

            if(backgroundAudio != null)
            {
                StartCoroutine(FadeBackgroundVolume(backgroundAudio,6f));
            }
        }

        if(useFadeVolume && distance < 1.3f)
        {
            StartCoroutine(FadeVolume(5f));
        }
    }

    private IEnumerator FadeBackgroundVolume(GameObject target, float fadeTime)
    {
        Debug.Log("FadeBackgroundVolume");
        AudioSource[] backgroundAudioList = target.GetComponentsInChildren<AudioSource>();

        float checkTime = 0;        
        float volume = 0;
        List<float> startVolumes = new List<float>();

        for (int i = 0; i < backgroundAudioList.Length; i++)
        {
            startVolumes.Add(backgroundAudioList[i].volume);
        }

        while (checkTime < 1.0f)
        {
            checkTime += Time.deltaTime / fadeTime;            

            for (int i = 0; i < backgroundAudioList.Length; i++)
            {
                volume = Mathf.Lerp(startVolumes[i], 0, checkTime);
                backgroundAudioList[i].volume = volume;
                //Debug.Log($"{backgroundAudioList[i].clip}/ {backgroundAudioList[i].volume}");
            }

            yield return null;
        }

        for (int i = 0; i < backgroundAudioList.Length; i++)
        {
            backgroundAudioList[i].volume = 0;
        }
    }

    private IEnumerator FadeVolume(float fadeTime)
    {
        float checkTime = 0;
        float startVolume = audioSource.volume;
        float volume = 0;

        while (checkTime < 1.0f)
        {
            checkTime += Time.deltaTime / fadeTime;
            volume = Mathf.Lerp(startVolume, 0, checkTime);
            audioSource.volume = volume;

            yield return null;
        }

        audioSource.volume = 0;
    }
}
