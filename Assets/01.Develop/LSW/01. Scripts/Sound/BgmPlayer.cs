using System;
using Ami.BroAudio;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Sound
{
    public class BgmPlayer : MonoBehaviour
    {
        [SerializeField] private SoundID bgmAudio;
        [SerializeField] private BroAudioType audioType;
        
        private void Start()
        {
            BroAudio.Stop(audioType);
            BroAudio.Play(bgmAudio);
        }

        public void StopBGM()
        {
            BroAudio.Stop(BroAudioType.Music);
        }
    }
}