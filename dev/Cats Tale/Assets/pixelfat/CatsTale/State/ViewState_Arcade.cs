﻿using pixelfat.CatsTale;
using Pixelfat.Unity;
using System;
using UnityEngine;

public class ViewState_Arcade : ViewState
{

    public Panel_ArcadeGamePlayUI.PlayerCommandEvent OnPlayerCommand;
    public Panel_ArcadeGamePlayUI.PlayerOptionEvent OnRestartGameSelected, OnResetLevelSelected, OnSettingsSelected, OnReturnToMenuSelected;

    private Panel_ArcadeGamePlayUI playerControls;
    private ArcadeLevelCompleteDialogue Panel_LevelComplete;
    private GameOverDialogue Panel_GameOver;
    private ConfirmDialogue Panel_ConfirmRestartLevel;

    private GameView gameView;
    private GameData gameData;

    public void Set(GameData gameData)
    {

        Debug.Log("Setting up Arcade game view.");

        if (gameView != null)
        {

            Debug.LogWarning("Removing game view.");
            Destroy(gameView.gameObject);

        }

        this.gameData = gameData;

        gameView = new GameObject("Game").AddComponent<GameView>();
        gameView.gameObject.transform.SetParent(transform);
        gameView.Set(gameData);

        gameData.OnStateChanged += HandleGameStateChanged;

        playerControls.Set(gameData);

    }

    protected override void Init()
    {

        base.Init();

        // UI
        playerControls = Add<Panel_ArcadeGamePlayUI>("ui/Sketch/Panel - Arcade Player Controls", false);
        playerControls.OnPlayerCommand += HandlePlayerCommand;
        playerControls.OnRestartLevelSelected += HandleRestartLevelSelected;
        playerControls.OnSettingsSelected += HandleViewSettingsSelected;


    }
    protected override void OnDestroy()
    {

        gameData.OnStateChanged -= HandleGameStateChanged;

        playerControls.OnPlayerCommand -= HandlePlayerCommand;
        playerControls.OnRestartLevelSelected -= HandleRestartLevelSelected;
        playerControls.OnSettingsSelected -= HandleViewSettingsSelected;

        Destroy(gameView.gameObject);

        base.OnDestroy();

    }

    private void HandleGameStateChanged()
    {

        playerControls.livesRemainingDisplay.SetLivesRemaining(PersistentSaveGameData.Persistent.arcadeRestartsRemaining);

        switch (gameData.state)
        {

            case GameData.State.START: break;
            case GameData.State.IN_PLAY: break;
            case GameData.State.FALL: if(PersistentSaveGameData.Persistent.arcadeRestartsRemaining == 1) ShowGameOver(); break;
            case GameData.State.STUCK: break;
            case GameData.State.POSSIBLE_COMPLETION: break;
            case GameData.State.COMPLETED: break;

        }

    }

    private void ShowGameOver()
    {
        if (Panel_GameOver != null)
            return;

        Panel_GameOver = Add<GameOverDialogue>("ui/Panel - Arcade Game Over", false);
        Panel_GameOver.OnRestartSelected += HandleRestartGameSelected;
        Panel_GameOver.OnExitToMenuSelected += HandleExitGameSelected;

        Panel_GameOver.SetMessageText("Level #" + PersistentSaveGameData.Persistent.currentArcadeLvl);

    }

    private void HandleRestartGameSelected()
    {
        Panel_GameOver.OnRestartSelected -= HandleRestartGameSelected;
        Panel_GameOver.OnExitToMenuSelected -= HandleExitGameSelected;

        Remove(Panel_GameOver);

        Debug.Log("Restart Game!");
        OnRestartGameSelected?.Invoke();

    }

    private void HandleExitGameSelected()
    {
        Panel_GameOver.OnRestartSelected -= HandleRestartGameSelected;
        Panel_GameOver.OnExitToMenuSelected -= HandleExitGameSelected;

        Remove(Panel_GameOver);

        Debug.Log("Exit Game!");

        OnReturnToMenuSelected?.Invoke();

    }

    /// <summary>
    /// Relays player commands from UI to game view
    /// </summary>
    /// <param name="command"></param>
    private void HandlePlayerCommand(Panel_ArcadeGamePlayUI.PlayerCommand command)
    {

        switch (command)
        {

            case Panel_ArcadeGamePlayUI.PlayerCommand.JUMP: gameView.player.DoMove(Move.Type.JUMP); break;
            case Panel_ArcadeGamePlayUI.PlayerCommand.HOP: gameView.player.DoMove(Move.Type.HOP); break;
            case Panel_ArcadeGamePlayUI.PlayerCommand.LEFT: gameView.player.TurnLeft(); break;
            case Panel_ArcadeGamePlayUI.PlayerCommand.RIGHT: gameView.player.TurnRight(); break;

        }

    }

    private void HandleRestartLevelSelected()
    {

        if (Panel_ConfirmRestartLevel != null)
            RemoveConfirmRestartLevelDialogue();

        Panel_ConfirmRestartLevel = Add<ConfirmDialogue>("ui/Panel - Confirm", false);
        Panel_ConfirmRestartLevel.SetMessageText("Restart this level?");
        Panel_ConfirmRestartLevel.OnYesSelected += HandleRestartLevelConfirmed;
        Panel_ConfirmRestartLevel.OnNoSelected += RemoveConfirmRestartLevelDialogue;

    }

    private void RemoveConfirmRestartLevelDialogue()
    {
        Panel_ConfirmRestartLevel.OnYesSelected -= HandleRestartLevelConfirmed;
        Panel_ConfirmRestartLevel.OnNoSelected -= RemoveConfirmRestartLevelDialogue;

        Remove(Panel_ConfirmRestartLevel);

    }

    private void HandleRestartLevelConfirmed()
    {
        RemoveConfirmRestartLevelDialogue();
        OnResetLevelSelected?.Invoke();
    }

    private void HandleViewSettingsSelected()
    {
        // show settings UI
        //throw new NotImplementedException();
    }

}