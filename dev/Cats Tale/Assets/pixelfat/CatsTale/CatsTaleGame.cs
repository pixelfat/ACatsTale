using Pixelfat.Unity;
using UnityEngine;


/// <summary>
/// TODO:
/// 
/// - Level reset / next level 
///     - Keep world position (move game to end position of prev game)
///     - Level complete / continue screen
/// 
/// - Settings 
///     - Save Level (for use in story mode / debugging)
///     - About / Help / Attributions
///
/// - Audio Controller
///     - Music
///     - SFX
///     
/// - Teleport FX
/// - Flashing tile type
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
