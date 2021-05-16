using pixelfat.CatsTale;
using Pixelfat.Unity;
using UnityEngine.UI;

/// <summary>
/// Reusable D-pad and other common controls
/// </summary>
public class Panel_ArcadeGamePlayUI : ViewPanel
{

    public enum PlayerCommand
    {

        JUMP, HOP,
        LEFT, RIGHT

    }

    public delegate void PlayerCommandEvent(PlayerCommand command);
    public delegate void PlayerOptionEvent();

    public PlayerCommandEvent OnPlayerCommand;

    public PlayerOptionEvent 
        OnSettingsSelected,
        OnRestartLevelSelected;

    public Button
        Button_Jump,
        Button_Hop, 
        Button_Left, 
        Button_Right, 
        Button_ResetLevel, // in game restart (any time)
        Button_ExitToMenu,
        Button_Settings;

    public LivesRemainingDisplay livesRemainingDisplay;

    public TMPro.TMP_Text 
        Text_TilesRemaining;

    private GameData gameData;

    public void Set(GameData gameData)
    {

        this.gameData = gameData;

        gameData.OnStateChanged += HandleGameStateChanged;
        gameData.OnPlayerMove += HandlePlayerMoved;

        livesRemainingDisplay.SetLivesRemaining(PersistentSaveGameData.Persistent.arcadeRestartsRemaining);
        Button_ResetLevel.gameObject.SetActive(PersistentSaveGameData.Persistent.arcadeRestartsRemaining > 1);

    }

    protected override void Start()
    {

        base.Start();

        Button_Hop.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.HOP); });
        Button_Jump.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.JUMP); });
        
        Button_Left.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.LEFT); });
        Button_Right.onClick.AddListener(delegate () { OnPlayerCommand?.Invoke(PlayerCommand.RIGHT); });

        Button_ResetLevel.onClick.AddListener(delegate () { OnRestartLevelSelected?.Invoke(); });
        Button_Settings.onClick.AddListener(delegate () { OnSettingsSelected?.Invoke(); });

    }

    private void OnDestroy()
    {

        Button_Jump.onClick.RemoveAllListeners();
        Button_Hop.onClick.RemoveAllListeners();
        Button_Left.onClick.RemoveAllListeners();
        Button_Right.onClick.RemoveAllListeners();

        Button_ResetLevel.onClick.RemoveAllListeners();
        Button_Settings.onClick.RemoveAllListeners();

    }

    private void HandlePlayerMoved()
    {
        Text_TilesRemaining.text = $"{gameData.GetTiles().Length - 1} / {gameData.solution.Length}";
    }

    private void HandleGameStateChanged()
    {

        livesRemainingDisplay.SetLivesRemaining(PersistentSaveGameData.Persistent.arcadeRestartsRemaining);
        Button_ResetLevel.gameObject.SetActive(PersistentSaveGameData.Persistent.arcadeRestartsRemaining > 1);

        switch (gameData.state)
        {
            case GameData.State.START: break;
            case GameData.State.IN_PLAY: break;
            case GameData.State.FALL: break;
            case GameData.State.STUCK: break;
            case GameData.State.POSSIBLE_COMPLETION: break;
            case GameData.State.COMPLETED: break;

        }

    }

}