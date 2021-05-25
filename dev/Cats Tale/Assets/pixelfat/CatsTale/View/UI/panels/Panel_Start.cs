using Pixelfat.Unity;
using UnityEngine.UI;

public class Panel_Start : ViewPanel
{

    public delegate void Panel_StartEvent();
    public Panel_StartEvent 
        OnBegin, 
        OnLoad,
        OnArcade, 
        OnArcadeAdv,
        OnSettings;

    public Button
        Button_Begin, // load play levels as defined in levels.json
        Button_LoadStored, // load levels in /saved folder (can add to / edit levels.tojson)
        Button_Arcade, // Play generated levels of length n++ (with varying weights)
        Button_ArcadeAdv, // As with arcade but with ability to define direction, TP and trap weights
        Button_Settings;

    protected override void Start()
    {
        
        base.Start();

        Button_Begin.onClick.AddListener(delegate() { OnBegin?.Invoke(); });
        Button_LoadStored.onClick.AddListener(delegate () { OnLoad?.Invoke(); });
        Button_Arcade.onClick.AddListener(delegate () { OnArcade?.Invoke(); });
        Button_ArcadeAdv.onClick.AddListener(delegate () { OnArcadeAdv?.Invoke(); });
        Button_Settings.onClick.AddListener(delegate () { OnSettings?.Invoke(); });

    }

    private void OnDestroy()
    {

        Button_Begin.onClick.RemoveAllListeners();
        Button_Begin.onClick.RemoveAllListeners();
        Button_Begin.onClick.RemoveAllListeners();
        Button_Begin.onClick.RemoveAllListeners();
        Button_Settings.onClick.RemoveAllListeners();

    }

}