using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueControll : MonoBehaviour
{
    public List<AudioClip> prologue2_audio;
    public float WaitSeconds = 2.0f;
    private AudioSource prologue2_source;
    public Animator doorAnim;
        
    void Start()
    {
        prologue2_source = GetComponent<AudioSource>();  
        StartCoroutine("PlayList");
    }
    private IEnumerator PlayAudio(int indexNo)
    {
        yield return new WaitForSeconds(WaitSeconds);
        
        prologue2_source.clip = prologue2_audio[indexNo];
        prologue2_source.Play();
        
        while (prologue2_source.isPlaying)
        {
            yield return null;
        }
    }

    private IEnumerator PlayAni()
    {
        yield return new WaitForSeconds(WaitSeconds);
        
        doorAnim.SetTrigger("door_move");

        yield return null;
    }



    private IEnumerator PlayList()
    {
        yield return StartCoroutine(PlayAudio(0));

        yield return StartCoroutine(PlayAudio(1));

        yield return StartCoroutine(PlayAudio(2));

        yield return StartCoroutine(PlayAudio(3));

        yield return StartCoroutine(PlayAudio(4));

        yield return StartCoroutine(PlayAudio(5));

        yield return StartCoroutine(PlayAudio(6));

        yield return StartCoroutine(PlayAudio(7));

        yield return StartCoroutine(PlayAudio(8));

        yield return StartCoroutine(PlayAudio(9));

        yield return StartCoroutine(PlayAudio(10));

        yield return StartCoroutine("PlayAni");

    }

}
