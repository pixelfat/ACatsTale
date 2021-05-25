using Pixelfat.Unity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Panel_ConfirmDialogue : ViewPanel
{

    public delegate void ConfirmDialogueEvent();
    public ConfirmDialogueEvent OnYesSelected, OnNoSelected;

    public TMPro.TMP_Text message;
    public Button Button_Yes, Button_No;

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

        Button_Yes.onClick.AddListener(delegate () { OnYesSelected?.Invoke(); });
        Button_No.onClick.AddListener(delegate () { OnNoSelected?.Invoke(); });

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

    }

    private void OnDestroy()
    {

        Button_Yes.onClick.RemoveAllListeners();
        Button_No.onClick.RemoveAllListeners();

    }

}
