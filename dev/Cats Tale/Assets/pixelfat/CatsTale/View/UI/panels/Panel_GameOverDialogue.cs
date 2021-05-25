using Pixelfat.Unity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Panel_GameOverDialogue : ViewPanel
{

    public delegate void GameOverDialogueDialogueEvent();
    public GameOverDialogueDialogueEvent OnRestartSelected, OnExitToMenuSelected;

    public TMPro.TMP_Text message;
    public Button Button_Restart, Button_Exit;

    public CanvasGroup canvasGroup;

    public void SetMessageText(string msg)
    {
        message.text = msg;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected override void Start()
    {

        base.Start();

        Button_Restart.onClick.AddListener(delegate () { OnRestartSelected?.Invoke(); });
        Button_Exit.onClick.AddListener(delegate () { OnExitToMenuSelected?.Invoke(); });

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

    }

    private void OnDestroy()
    {

        Button_Restart.onClick.RemoveAllListeners();
        Button_Exit.onClick.RemoveAllListeners();

    }

}
