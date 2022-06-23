using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Zoxter_API : MonoBehaviour {
    public static Zoxter_API instance;

    AudioSource _soundSource;

    private void Start()
    {
        instance = this;
        _soundSource = GetComponent<AudioSource>();


    }
    public void CloseApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OpenURL(string url) => Application.OpenURL(url);
    public void TimedCloseApp(float t) => StartCoroutine(CloseAppDelayed(t));
    IEnumerator CloseAppDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        CloseApp();
    }
    public static void PlaySound(AudioClip sfx) => instance._soundSource.PlayOneShot(sfx);
}
