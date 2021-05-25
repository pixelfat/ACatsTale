using Pixelfat.Unity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Panel_ArcadeLevelCompleteDialogue : ViewPanel
{

    public delegate void ArcadeLevelCompleteDialogueEvent();
    public ArcadeLevelCompleteDialogueEvent OnContinueSelected;

    public TMPro.TMP_Text message;
    public Button Button_Continue;

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

        Button_Continue.onClick.AddListener(delegate () { OnContinueSelected?.Invoke(); });

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

    }

    private void OnDestroy()
    {

        Button_Continue.onClick.RemoveAllListeners();

    }

}
