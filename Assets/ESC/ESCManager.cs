using Ami.BroAudio;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _01.Develop.LSW._01._Scripts.Manager;

public class ESCManager : MonoSingleton<ESCManager>
{
    [Header("UI")]
    [SerializeField] private CanvasGroup escCanvas;

    [Header("Audio Groups (BroAudioType)")]
    [SerializeField] private BroAudioType _bgm;
    [SerializeField] private BroAudioType _sfx;
    [SerializeField] private BroAudioType _main;

    [Header("Sliders")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Display")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("ESC Fade")]
    [SerializeField] private float escFadeDuration = 0.2f;
    [SerializeField] private bool ignoreTimeScaleForEscFade = true;

    private bool isOn;
    private Tween escTween;

    private List<Resolution> resolutions = new();
    private bool _isInitialized;

    private void Awake()
    {
        if (escCanvas != null)
        {
            escCanvas.alpha = 0f;
            escCanvas.blocksRaycasts = false;
            escCanvas.interactable = false;
        }
        isOn = false;
    }

    private void Start()
    {
        SetupResolutionDropdown();

        if (fullscreenToggle != null)
        {
            // 현재 상태 반영 (환경마다 fullScreen 값이 초기화되는 타이밍이 다를 수 있어서)
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);

            fullscreenToggle.onValueChanged.RemoveListener(SetupFullscreenToggle);
            fullscreenToggle.onValueChanged.AddListener(SetupFullscreenToggle);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionSelected);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionSelected);
        }
    }

    private void OnDestroy()
    {
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionSelected);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(SetupFullscreenToggle);

        if (escCanvas != null)
            escCanvas.DOKill();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ESC();
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        _isInitialized = false;
        resolutions.Clear();

        // 16:9 + 1280 이상만
        foreach (var r in Screen.resolutions)
        {
            float aspect = (float)r.width / r.height;
            if (Mathf.Abs(aspect - (16f / 9f)) < 0.05f && r.width >= 1280)
                resolutions.Add(r);
        }

        // 같은 해상도(가로/세로) 중복 제거: 주사율 높은 것 하나만 남김
        resolutions = resolutions
            .GroupBy(r => (r.width, r.height))
            .Select(g => g.OrderByDescending(x => x.refreshRateRatio.value).First())
            .OrderByDescending(r => r.width)
            .ThenByDescending(r => r.height)
            .ToList();

        resolutionDropdown.ClearOptions();

        // 중복 구분용으로 Hz까지 표기 (원하면 "WxH"만으로 바꿔도 됨)
        var options = resolutions
            .Select(r =>
            {
                float hz = (float)r.refreshRateRatio.numerator / r.refreshRateRatio.denominator;
                return $"{r.width} x {r.height} ({hz:0.#}Hz)";
            })
            .ToList();

        resolutionDropdown.AddOptions(options);

        // 현재 해상도에 맞춰 드롭다운 선택
        int curIndex = FindCurrentResolutionIndex(Screen.width, Screen.height);
        if (curIndex < 0) curIndex = 0;

        resolutionDropdown.SetValueWithoutNotify(curIndex);
        resolutionDropdown.RefreshShownValue();

        _isInitialized = true;
    }

    private int FindCurrentResolutionIndex(int w, int h)
    {
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == w && resolutions[i].height == h)
                return i;
        }
        return -1;
    }

    private void SetupFullscreenToggle(bool isOn)
    {
        Screen.fullScreen = isOn;

        // 풀스크린 토글 바꾸면, 현재 드롭다운 선택 해상도로 한 번 더 적용해주는 게 안정적일 때가 있음
        if (resolutionDropdown != null && resolutions.Count > 0)
        {
            int idx = resolutionDropdown.value;
            idx = Mathf.Clamp(idx, 0, resolutions.Count - 1);
            var res = resolutions[idx];
            Screen.SetResolution(res.width, res.height, isOn);
        }
    }

    private void OnResolutionSelected(int index)
    {
        if (!_isInitialized) return;
        if (resolutions == null || resolutions.Count == 0) return;

        index = Mathf.Clamp(index, 0, resolutions.Count - 1);
        bool isFullscreen = fullscreenToggle != null && fullscreenToggle.isOn;

        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, isFullscreen);
    }

    public void ESC()
    {
        isOn = !isOn;

        if (escCanvas == null) return;

        // 트윈 중첩 방지
        escCanvas.DOKill();
        escTween?.Kill();
        escTween = null;

        if (isOn)
        {
            // 켤 때: 먼저 입력 받게 열어두고 페이드 인
            escCanvas.alpha = 0f;
            escCanvas.blocksRaycasts = true;
            escCanvas.interactable = true;

            escTween = escCanvas
                .DOFade(1f, escFadeDuration)
                .SetUpdate(ignoreTimeScaleForEscFade);
        }
        else
        {
            // 끌 때: 페이드 아웃 끝난 뒤 입력 차단 해제
            escTween = escCanvas
                .DOFade(0f, escFadeDuration)
                .SetUpdate(ignoreTimeScaleForEscFade)
                .OnComplete(() =>
                {
                    escCanvas.blocksRaycasts = false;
                    escCanvas.interactable = false;
                });
        }
    }

    public void BGM(float volume)
    {
        if (_bgmSlider != null) _bgmSlider.SetValueWithoutNotify(volume);
        BroAudio.SetVolume(_bgm, volume);
    }

    public void SFX(float volume)
    {
        if (_sfxSlider != null) _sfxSlider.SetValueWithoutNotify(volume);
        BroAudio.SetVolume(_sfx, volume);
    }

    public void Master(float volume)
    {
        if (_masterSlider != null) _masterSlider.SetValueWithoutNotify(volume);
        BroAudio.SetVolume(_main, volume);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
