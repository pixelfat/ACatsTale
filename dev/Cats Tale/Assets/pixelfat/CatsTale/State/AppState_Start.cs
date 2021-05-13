using Pixelfat.Unity;
using System;

public class AppState_Start : AppState
{

    ViewState_Start view;

    protected override void Init()
    {
        base.Init();

        view = ViewState.Set<ViewState_Start>();

        view.OnArcade += HandleAcadeSelected;

    }

    private void HandleAcadeSelected()
    {
        SetAppState<AppState_Arcade>();
    }
}
