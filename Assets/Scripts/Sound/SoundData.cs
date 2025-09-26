using UnityEngine;

public class SoundData : MonoBehaviour
{
    [SerializeField] int soundId = 0;
    public AudioSource Bgm; 
    public AudioClip clip;         

    void Start()
    {
        //Bgm = Shared.Instance.SoundManager.Get(soundId);
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        audioSource.clip = clip;
    //        audioSource.Play();

    //        // audioSource.PlayOneShot(clip);
    //    }
    //}

}
