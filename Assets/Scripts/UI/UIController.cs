using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public LoseScreen losePanel;
    public MenuScreen menuPanel;

    public IEnumerator OpenLosePanel()
    {
        yield return new WaitForSeconds(.4f);
        losePanel.gameObject.SetActive(true);
    }

    public void OpenMenuPanel()
    {
        OpenPanel(menuPanel.gameObject);
    }




    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
}
