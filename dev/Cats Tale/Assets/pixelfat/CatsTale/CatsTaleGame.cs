using Pixelfat.Unity;
using UnityEngine;


/// <summary>
/// 
/// 
/// NOTES:
/// 
/// PROBLEM: Lives & refusing to jump don't work together - reset when lives remain, but what to do when on last life, quit button?
///     - when on final life, 'reset' should be replaced with 'quit?' :(
///     - allow cat to jump off (auto reset / game-over)
/// 
/// SOLUTION 1: Player must suicide in arcade mode
/// - In Story mode the can cannot jump to an empty space, when stuck the must reset and does not lose progress.
/// - In Arcade mode the cat can jump off and the user has a limited number of lives. 
///   - The player will have to jump off in order to force a game over screen when no solution is left
/// 
/// -- SOLUTION 2: no lives 
/// - Arcade mode: Infinite levels, no lives - can play forever..?
/// - 
/// 
/// WHen the user knows they can't complete the level 
/// without allowing the cat to jump off, there's no way to eaily reset or quit (force gameover by jumping off)
/// 
/// TODO:
/// - Save Level (for use in story mode / debugging)
/// - Respawns in arcade mode when stuck
/// - Camera movement - drag around?
/// - Flashing tile type
///     - Implement fall & respawn / automatic restart?
/// - Story-mode 
///   - Load preselected puzzles
///   - Show story slides
///   - Mini games? - Fruit drop, frogger, etc?
/// </summary>
public class CatsTaleGame : MonoBehaviour
{

    void Start()
    {
        AppState.SetAppState<AppState_Start>();
    }

}
