using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
  [SerializeField] private AudioSource bg_adudio;
  [SerializeField] private AudioSource audioPlayer_wl;
  [SerializeField] private AudioSource audioPlayer_button;
  [SerializeField] private AudioSource audioPlayer_spin_stop;
  [SerializeField] private AudioSource sidebar_sound;

  private bool isForceMuted;
  private List<AudioSource> allSources;
  private readonly Dictionary<AudioSource, bool> preFocusMuteState = new();

  [Header("clips")]
  [SerializeField] private AudioClip SpinButtonClip;
  [SerializeField] private AudioClip SpinStopClip;
  [SerializeField] private AudioClip Button;
  [SerializeField] private AudioClip SmallWin_Audio;
  [SerializeField] private AudioClip BigWin_Audio;
  [SerializeField] private AudioClip NormalBg_Audio;
  [SerializeField] private AudioClip FreeSpinBg_Audio;
  [SerializeField] private AudioClip sizeup_audio;
  [SerializeField] private AudioClip electricSound;

  private void Awake()
  {
    allSources = new List<AudioSource> { bg_adudio, audioPlayer_wl, audioPlayer_button, audioPlayer_spin_stop, sidebar_sound };
    sidebar_sound.clip = sizeup_audio;
    PlayBgAudio();
  }

  internal void PlayWLAudio(string type = "default")
  {
    StopWLAaudio();
    if (type == "big")
    {
      audioPlayer_wl.clip = BigWin_Audio;
      audioPlayer_wl.pitch = 1.2f;
    }
    else if (type == "electric")
    {
      audioPlayer_wl.clip = electricSound;
    }
    else
    {
      audioPlayer_wl.clip = SmallWin_Audio;
    }
    audioPlayer_wl.Play();
  }

  internal void PlaySpinStopAudio()
  {
    audioPlayer_spin_stop.clip = SpinStopClip;
    audioPlayer_spin_stop.Play();
  }

  internal void StopSpinAudio()
  {
    if (audioPlayer_spin_stop) audioPlayer_spin_stop.Stop();
  }

  private void OnApplicationFocus(bool focus)
  {
    SetMuteAll(!focus);
  }

  private void OnApplicationPause(bool pause)
  {
    SetMuteAll(pause);
  }

  // Focus-driven mute — called from both OnApplicationFocus (native) and GameManager.OnFocusChanged (WebGL/JS).
  // Reentrancy-guarded so a duplicate call for the same direction can't clobber the captured restore state.
  internal void SetMuteAll(bool forceMute)
  {
    if (forceMute == isForceMuted) return;
    isForceMuted = forceMute;

    foreach (var source in allSources)
    {
      if (source == null) continue;
      if (forceMute)
      {
        preFocusMuteState[source] = source.mute;
        source.mute = true;
      }
      else
      {
        source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
      }
    }
  }

  internal void PlayBgAudio(string type = "default")
  {
    StopBgAudio();
    bg_adudio.loop = true;
    if (bg_adudio)
    {
      if (type == "FP")
        bg_adudio.clip = FreeSpinBg_Audio;
      else
        bg_adudio.clip = NormalBg_Audio;
      bg_adudio.Play();
    }
  }

  internal void PlayButtonAudio(string type = "default")
  {
    if (type == "spin")
      audioPlayer_button.clip = SpinButtonClip;
    else
      audioPlayer_button.clip = Button;
    audioPlayer_button.Play();
  }

  internal void StopWLAaudio()
  {
    audioPlayer_wl.Stop();
    audioPlayer_wl.loop = false;
    audioPlayer_wl.pitch = 1f;
  }

  internal void PlaySizeUpSound(bool play)
  {
    if (play)
      sidebar_sound.Play();
    else
      sidebar_sound.Stop();
  }

  internal void StopBgAudio()
  {
    bg_adudio.Stop();
  }

  internal void ToggleMute(bool toggle, string type)
  {
    switch (type)
    {
      case "bg":
        bg_adudio.mute = toggle;
        break;
      case "button":
        audioPlayer_button.mute = toggle;
        audioPlayer_spin_stop.mute = toggle;
        break;
      case "wl":
        audioPlayer_wl.mute = toggle;
        break;
      case "all":
        bg_adudio.mute = toggle;
        audioPlayer_button.mute = toggle;
        audioPlayer_spin_stop.mute = toggle;
        audioPlayer_wl.mute = toggle;
        break;
    }
  }
}
