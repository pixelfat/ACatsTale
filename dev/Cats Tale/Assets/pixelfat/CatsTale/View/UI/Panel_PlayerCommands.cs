using Pixelfat.Unity;
using UnityEngine.UI;

/// <summary>
/// Reusable D-pad and other common controls
/// </summary>
public class Panel_PlayerCommands : ViewPanel
{

    public enum PlayerCommand
    {

        JUMP, HOP,
        LEFT, RIGHT

    }

    public delegate void PlayerCommandEvent(PlayerCommand command);
    public delegate void PlayerOptionEvent();

    public PlayerCommandEvent OnPlayerCommand;
    public PlayerOptionEvent OnSettingsSelected, OnResetGameSelected;

    public Button
        Button_Jump,
        Button_Hop, 
        Button_Left, 
        Button_Right, 
        Button_Reset,
        Button_Settings;

    protected override void Start()
    {

        base.Start();

        Button_Hop.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.HOP); });
        Button_Jump.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.JUMP); });
        
        Button_Left.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.LEFT); });
        Button_Right.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.RIGHT); });

        Button_Reset.onClick.AddListener(delegate () { OnResetGameSelected?.Invoke(); });
        Button_Settings.onClick.AddListener(delegate () { OnSettingsSelected?.Invoke(); });

    }

    private void OnDestroy()
    {

        Button_Jump.onClick.RemoveAllListeners();
        Button_Hop.onClick.RemoveAllListeners();
        Button_Left.onClick.RemoveAllListeners();
        Button_Right.onClick.RemoveAllListeners();
        Button_Reset.onClick.RemoveAllListeners();
        Button_Settings.onClick.RemoveAllListeners();

    }

}