using Pixelfat.Unity;
using System;

public class ViewState_Start : ViewState
{
    public delegate void ViewState_StartEvent();
    public ViewState_StartEvent
        OnBegin,
        OnLoad,
        OnArcade,
        OnArcadeAdv;

    public Panel_Start gui;

    protected override void Init()
    {

        base.Init();

        gui = Add<Panel_Start>("ui/Sketch/Panel - Start", false);

        gui.OnBegin += HandleOnBeginSelected;
        gui.OnLoad += HandleOnLoadSelected;
        gui.OnArcade += HandleOnArcadeSelected;
        gui.OnArcadeAdv += HandleOnArcadeAdvSelected;

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        gui.OnBegin -= HandleOnBeginSelected;
        gui.OnLoad -= HandleOnLoadSelected;
        gui.OnArcade -= HandleOnArcadeSelected;
        gui.OnArcadeAdv -= HandleOnArcadeAdvSelected;

    }

    private void HandleOnBeginSelected()
    {
        OnBegin?.Invoke();
    }

    private void HandleOnLoadSelected()
    {
        OnLoad?.Invoke();
    }
    private void HandleOnArcadeSelected()
    {
        OnArcade?.Invoke();
    }
    private void HandleOnArcadeAdvSelected()
    {
        OnArcadeAdv?.Invoke();
    }
}
