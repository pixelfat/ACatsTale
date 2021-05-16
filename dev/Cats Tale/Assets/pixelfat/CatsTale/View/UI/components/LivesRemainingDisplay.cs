using System.Collections.Generic;
using UnityEngine;

public class LivesRemainingDisplay : MonoBehaviour
{

    public RectTransform lifeItem;
    public TMPro.TMP_Text Text_LivesRemaining;

    public List<RectTransform> lifeItems = new List<RectTransform>();

    public void SetLivesRemaining(int lives)
    {

        if (lives < 0)
            lives = 0;

        Debug.Log("Setting life count display: " + lives);

        while(lifeItems.Count > lives)
        {

            Destroy(lifeItems[0].gameObject);
            lifeItems.RemoveAt(0);

        }

        while (lifeItems.Count < lives)
        {

            RectTransform newLifeItem = Instantiate(lifeItem);
            newLifeItem.gameObject.SetActive(true);
            newLifeItem.transform.SetParent(lifeItem.transform.parent);
            lifeItems.Add(newLifeItem);

        }

        Text_LivesRemaining.text = $"{lives}x Tries remaining.";

    }

    private void Start()
    {
        lifeItem.gameObject.SetActive(false);
    }

}
