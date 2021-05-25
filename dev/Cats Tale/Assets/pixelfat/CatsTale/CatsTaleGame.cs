using Pixelfat.Unity;
using UnityEngine;


/// <summary>
/// TODO:
/// - Settings - Save Level (for use in story mode / debugging)
/// - Respawns in arcade mode when stuck
/// - Camera movement - drag around?
/// - Flashing tile type
///     - Implement fall & respawn / automatic restart?
///     
/// - Story-mode 
///  - Show story slides
///  - Load 3d Env
///  - Load preselected puzzles
///  - Mini games? - Fruit drop, frogger, etc?
/// </summary>
public class CatsTaleGame : MonoBehaviour
{

    void Start()
    {
        AppState.SetAppState<AppState_Start>();
    }

}
