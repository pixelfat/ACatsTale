using Pixelfat.Unity;
using System;
using UnityEngine.UI;

public class Panel_Settings : ViewPanel
{

    const string
    str_enableMusic = "Enable Music",
    str_disableMusic = "Disable Music",

    str_enableSfx = "Enable SFX",
    str_disableSfx = "Disable SFX",

    str_enableFalling = "Allow Falling",
    str_disableFalling = "Disable Falling";

    public Action
        OnClose;
    public Action<bool>
        OnToggleFalling,
        OnToggleSfx,
        OnToggleMusic;

    public Button
        Button_Close;

    public TextButton
        TextButton_AllowFall,
        TextButton_Music,
        TextButton_Sfx;

    public bool allowFall { get { return _allowFall; } set { SetFallingEnabled(value); } }
    public bool musicEnabled { get { return _musicEnabled; } set { SetMusicEnabled(value); } }
    public bool sfxEnabled { get { return _sfxEnabled; } set { SetSfxEnabled(value); } }

    private bool _allowFall = true;
    private bool _musicEnabled = true;
    private bool _sfxEnabled = true;

    protected override void Start()
    {

        base.Start();

        Button_Close.onClick.AddListener(delegate () { OnClose?.Invoke(); });

        TextButton_AllowFall.button.onClick.AddListener(delegate () { SetFallingEnabled(!allowFall); });
        TextButton_Music.button.onClick.AddListener(delegate () { SetMusicEnabled(!_musicEnabled); });
        TextButton_Sfx.button.onClick.AddListener(delegate () { SetSfxEnabled(!_sfxEnabled); });

        SetFallingEnabled(allowFall);
        SetMusicEnabled(musicEnabled);
        SetSfxEnabled(sfxEnabled);

    }

    private void SetFallingEnabled(bool value)
    {

        _allowFall = value;
        TextButton_AllowFall.text.text = _allowFall ? str_disableFalling : str_enableFalling;
        OnToggleFalling?.Invoke(_allowFall);

    }

    private void SetSfxEnabled(bool value)
    {

        _sfxEnabled = value;
        TextButton_Sfx.text.text = _sfxEnabled ? str_disableSfx : str_enableSfx;
        OnToggleSfx?.Invoke(sfxEnabled);
        
    }

    private void SetMusicEnabled(bool value)
    {

        _musicEnabled = value;
        TextButton_Music.text.text = _musicEnabled ? str_disableMusic : str_enableMusic;
        OnToggleMusic?.Invoke(_musicEnabled);

    }

}
