using pixelfat.CatsTale;
using Pixelfat.Unity;
using System;
using UnityEngine;

public class AppState_Arcade : AppState
{

    ViewState_Arcade viewState;

    public GameData gameData;
    private string _serializedGameData;

    //private int level = 5;

    protected override void Init()
    {

        base.Init();

        viewState = ViewState.Set<ViewState_Arcade>();
        viewState.OnPlayerCommand += HandlePlayerCommand;
        viewState.OnResetGameSelected += HandleResetGameSelected;

        //_serializedGameData = GameData.GenerateGameData(PersistentSaveGameData.Persistent.currentArcadeLvl + 5).ToJson();

        GameData storedData;
        // load a copy of the saved state if there is one
        if (PersistentSaveGameData.Persistent.currentArcadeLvl > 0)
        {
            Debug.Log("???" + PersistentSaveGameData.Persistent.currentArcadeBoard);
            storedData = PersistentSaveGameData.Persistent.currentArcadeBoard;
            Debug.Log("Stored arcade game data loaded. " + storedData.ToJson());
        }
        else
        {
            PersistentSaveGameData.Persistent.currentArcadeLvl = 0;

            storedData = GameData.GenerateGameData(PersistentSaveGameData.Persistent.currentArcadeLvl + 5);

            PersistentSaveGameData.Persistent.currentArcadeBoard = storedData;
            PersistentSaveGameData.Save();

            Debug.Log("New arcade game data created.");

        }

        // make a copy (don't directly play PersistentSaveGameData.Persistent.currentArcadeBoard)
        _serializedGameData = storedData.ToJson();

        //gameData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(_serializedGameData);

        //gameData.OnStateChanged += HandleGameDataStateChange;

        //viewState.Set(gameData);
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

    private void ResetGame()
    {

        if(gameData != null)
            gameData.OnStateChanged -= HandleGameDataStateChange;

        Debug.Log("Resetting game data: " + _serializedGameData);

        StartLevel();

    }

    private void HandleResetGameSelected()
    {

        ResetGame();
        
    }

    private void HandleGameDataStateChange()
    {

        Debug.Log("State change:" + gameData.state);

        switch (gameData.state)
        {
            case GameData.State.START: break;
            case GameData.State.IN_PLAY: break;
            case GameData.State.FAILED: break;
            case GameData.State.POSSIBLE_COMPLETION: break;
            case GameData.State.COMPLETED: Debug.Log("LEVEL COMPLETE!"); StartNextLevel(); break;
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