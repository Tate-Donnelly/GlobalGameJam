using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Menu : MonoBehaviour
{
    public enum PageType { Title = 1, Controls = 2, Settings = 3, Credits = 4 }
    private PageType activePage;

    [SerializeField] private UI_Page titlePage;
    [SerializeField] private UI_Page controlsPage;
    [SerializeField] private UI_Page settingsPage;
    [SerializeField] private UI_Page creditsPage;

    private void Awake()
    {
        ChangePage(PageType.Title);
    }

    public void ChangePage(int pageTypeIndex)
    {
        ChangePage((PageType)pageTypeIndex);
    }

    public void ChangePage(PageType pageType)
    {
        activePage = pageType;
        titlePage.gameObject.SetActive(pageType == PageType.Title);
        controlsPage.gameObject.SetActive(pageType == PageType.Controls);
        settingsPage.gameObject.SetActive(pageType == PageType.Settings);
        creditsPage.gameObject.SetActive(pageType == PageType.Credits);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
