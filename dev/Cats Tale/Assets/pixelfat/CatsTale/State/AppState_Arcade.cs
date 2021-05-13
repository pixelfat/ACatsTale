using pixelfat.CatsTale;
using Pixelfat.Unity;
using System;
using UnityEngine;

public class AppState_Arcade : AppState
{

    ViewState_Arcade viewState;

    public GameData gameData;
    private string _serializedGameData;

    protected override void Init()
    {

        base.Init();

        viewState = ViewState.Set<ViewState_Arcade>();
        viewState.OnPlayerCommand += HandlePlayerCommand;
        viewState.OnResetGameSelected += HandleResetGameSelected;

        GameData newGameData = new GameData(10);
        _serializedGameData = newGameData.Board.ToJson();

        gameData = new GameData(BoardData.FromJson(_serializedGameData));

        viewState.Set(gameData);

    }

    private void ResetGame()
    {

        if(gameData != null)
            gameData.Board.OnStateChanged -= HandleGameDataStateChange;

        Debug.Log("Resetting game data: " + _serializedGameData);

        gameData = new GameData(BoardData.FromJson(_serializedGameData));
        gameData.Board.OnStateChanged += HandleGameDataStateChange;

        viewState.Set(gameData);

    }

    private void HandleResetGameSelected()
    {

        ResetGame();
        
    }

    private void HandleGameDataStateChange()
    {
        switch (gameData.Board.state)
        {
            case BoardData.State.START: break;
            case BoardData.State.IN_PLAY: break;
            case BoardData.State.FAILED: break;
            case BoardData.State.POSSIBLE_COMPLETION: break;
            case BoardData.State.COMPLETED: Debug.Log("LEVEL COMPLETE!"); break;
        }
    }

    private void HandlePlayerCommand(Panel_PlayerCommands.PlayerCommand command)
    {
        switch (command)
        {
            case Panel_PlayerCommands.PlayerCommand.JUMP: break;
            case Panel_PlayerCommands.PlayerCommand.HOP: break;
            case Panel_PlayerCommands.PlayerCommand.LEFT: break;
            case Panel_PlayerCommands.PlayerCommand.RIGHT: break;
        }
    }

}