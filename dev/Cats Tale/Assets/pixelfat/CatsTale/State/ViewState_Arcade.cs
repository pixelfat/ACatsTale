using pixelfat.CatsTale;
using Pixelfat.Unity;
using System;
using UnityEngine;

public class ViewState_Arcade : ViewState
{

    public Panel_PlayerCommands.PlayerCommandEvent OnPlayerCommand;
    public Panel_PlayerCommands.PlayerOptionEvent OnResetGameSelected, OnSettingsSelected;

    private Panel_PlayerCommands playerControls;
    private GameView gameView;
    private GameData gameData;

    public void Set(GameData data)
    {

        Debug.Log("Setting up Arcade game view.");

        if (gameView != null)
        {

            Debug.LogWarning("Removing game view.");
            Destroy(gameView.gameObject);

        }

        this.gameData = data;

        gameView = new GameObject("Game").AddComponent<GameView>();
        gameView.gameObject.transform.SetParent(transform);
        gameView.Set(gameData);
        gameData.Board.OnPlayerMove += HandlePlayerMoved;

        HandlePlayerMoved();

    }

    private void HandlePlayerMoved()
    {
        playerControls.Text_TilesRemaining.text = $"{gameData.Board.GetTiles().Length - 1} / {gameData.Board.solution.Length}";
    }

    protected override void Init()
    {

        base.Init();

        // UI
        playerControls = Add<Panel_PlayerCommands>("ui/Panel - Player Controls", false);
        playerControls.OnPlayerCommand += HandlePlayerCommand;
        playerControls.OnResetGameSelected += HandleResetGameSelected;
        playerControls.OnSettingsSelected += HandleViewSettingsSelected;

    }

    /// <summary>
    /// Relays player commands from UI to game view
    /// </summary>
    /// <param name="command"></param>
    private void HandlePlayerCommand(Panel_PlayerCommands.PlayerCommand command)
    {

        switch (command)
        {
            case Panel_PlayerCommands.PlayerCommand.JUMP: gameView.player.DoMove(Move.Type.JUMP); break;
            case Panel_PlayerCommands.PlayerCommand.HOP: gameView.player.DoMove(Move.Type.HOP); break;
            case Panel_PlayerCommands.PlayerCommand.LEFT: gameView.player.TurnLeft(); break;
            case Panel_PlayerCommands.PlayerCommand.RIGHT: gameView.player.TurnRight(); break;

        }

    }
    private void HandleResetGameSelected()
    {
        OnResetGameSelected?.Invoke();
    }

    private void HandleViewSettingsSelected()
    {
        // show settings UI
        throw new NotImplementedException();
    }

}