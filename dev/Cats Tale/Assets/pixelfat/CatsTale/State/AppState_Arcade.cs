using pixelfat.CatsTale;
using Pixelfat.Unity;
using System;
using UnityEngine;

/// <summary>
/// The cat will jump off tiles
/// Limited retries (3 + pickups)
/// </summary>
public class AppState_Arcade : AppState
{

    public GameData gameData;

    private string _serializedGameData;

    private ViewState_Arcade viewState;

    protected override void Init()
    {

        base.Init();

        viewState = ViewState.Set<ViewState_Arcade>();
        viewState.OnPlayerCommand += HandlePlayerCommand;
        viewState.OnRestartGameSelected += HandleRestartGameSelected;
        viewState.OnResetLevelSelected += HandleResetLevelSelected;
        viewState.OnReturnToMenuSelected += HandleReturnToMenuSelected;

        // load a copy of the saved state if there is one
        if (PersistentSaveGameData.Persistent.currentArcadeLvl > 0)
            _serializedGameData = PersistentSaveGameData.Persistent.currentArcadeBoard.ToJson();
        else
            ResetGame();

        StartLevel();

    }

    private void StartLevel()
    {

        gameData = GameData.FromJson(_serializedGameData);
        gameData.OnStateChanged += HandleGameDataStateChange;

        viewState.Set(gameData);

    }

    private void StartNextLevel()
    {

        PersistentSaveGameData.Persistent.currentArcadeLvl++;

        GameData newGameData = GameData.GenerateGameData(PersistentSaveGameData.Persistent.currentArcadeLvl + 5);
        _serializedGameData = newGameData.ToJson();

        PersistentSaveGameData.Persistent.currentArcadeBoard = newGameData;
        PersistentSaveGameData.Save();

        StartLevel();

    }

    private void ResetLevel()
    {

        PersistentSaveGameData.Persistent.arcadeRestartsRemaining--;
        PersistentSaveGameData.Save();

        if (gameData != null)
            gameData.OnStateChanged -= HandleGameDataStateChange;

        Debug.Log("Resetting game data: " + _serializedGameData);

        StartLevel();

    }

    private void HandleRestartGameSelected()
    {

        ResetGame();
        StartLevel();

    }

    private void ResetGame()
    {

        PersistentSaveGameData.Persistent.currentArcadeLvl = 0;

        GameData storedData = GameData.GenerateGameData(PersistentSaveGameData.Persistent.currentArcadeLvl + 5);

        PersistentSaveGameData.Persistent.currentArcadeBoard = storedData;
        PersistentSaveGameData.Persistent.arcadeRestartsRemaining = 3;

        PersistentSaveGameData.Save();

        _serializedGameData = storedData.ToJson();

        Debug.Log("New arcade game data created.");

    }

    private void HandleReturnToMenuSelected()
    {
        
        viewState.OnPlayerCommand -= HandlePlayerCommand;
        viewState.OnRestartGameSelected -= HandleRestartGameSelected;
        viewState.OnResetLevelSelected -= HandleResetLevelSelected;
        viewState.OnReturnToMenuSelected -= HandleReturnToMenuSelected;

        SetAppState<AppState_Start>();
    }

    private void HandleResetLevelSelected()
    {
        ResetLevel();
    }

    private void HandleGameDataStateChange()
    {

        Debug.Log("State change:" + gameData.state);

        switch (gameData.state)
        {
            case GameData.State.START: break;
            case GameData.State.IN_PLAY: break;
            case GameData.State.STUCK: break;
            case GameData.State.FALL:

                if (PersistentSaveGameData.Persistent.arcadeRestartsRemaining == 1)
                    Debug.Log("<color=red>GAME OVER!</color>");
                else
                {
                    ResetLevel();
                    Debug.Log("<color=yellow>FAILED, RESTART?</color>");
                }
                break;

            case GameData.State.POSSIBLE_COMPLETION: break;
            case GameData.State.COMPLETED: Debug.Log("LEVEL COMPLETE!"); StartNextLevel(); break;
            
        }

    }

    private void HandlePlayerCommand(Panel_ArcadeGamePlayUI.PlayerCommand command)
    {
        switch (command)
        {
            case Panel_ArcadeGamePlayUI.PlayerCommand.JUMP: break;
            case Panel_ArcadeGamePlayUI.PlayerCommand.HOP: break;
            case Panel_ArcadeGamePlayUI.PlayerCommand.LEFT: break;
            case Panel_ArcadeGamePlayUI.PlayerCommand.RIGHT: break;
        }
    }

}