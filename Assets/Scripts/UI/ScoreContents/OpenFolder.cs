using Assets.Scripts.Managers;
using Assets.Scripts.PatientData.AlgoData;
using Assets.Scripts.UI.ScoreContents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.GameData;

public class OpenFolder : MonoBehaviour
{
    [SerializeField] private ButtonPressDetector openDetailsButton;
    [SerializeField] private RectTransform folderDivider, folderInside, folderCover;
    [SerializeField] private GameObject[] folderHideElements;
    [SerializeField] private RectTransform[] pages;
    [SerializeField] private Button[] pagesNavigationButs;


    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform parentTransform;



    private bool folderOpen = false;
    private int pageId = 0;

    private Dictionary<string, StepRecords> records;
    private List<RectTransform> pagesSteps;

    void OnEnable()
    {
        openDetailsButton.OnPress.AddListener(OpenFolderVoid);
        pagesNavigationButs[0].onClick.AddListener(delegate { SwitchPages(true); });
        pagesNavigationButs[1].onClick.AddListener(delegate { SwitchPages(false); });
        InstanciatePages();
        HideButtons();
    }

    void OnDisable()
    {
        openDetailsButton.OnPress.RemoveListener(OpenFolderVoid);
        pagesNavigationButs[0].onClick.RemoveAllListeners();
        pagesNavigationButs[1].onClick.RemoveAllListeners();
    }

    private void InstanciatePages()
    {
        pagesSteps = new List<RectTransform>();

        foreach (var step in records)
        {
            GameObject instance = Instantiate(prefab, parentTransform);
            PrefabStepDisplayUi ui = instance.GetComponent<PrefabStepDisplayUi>();
            
            // set text fields
            ui.SetText(step.Key, step.Value.diagnosticAnswer, step.Value.actionAnswer);
            pagesSteps.Add(instance.GetComponent<RectTransform>());
            instance.SetActive(false);
        }
        pagesSteps[0].gameObject.SetActive(true);
    }

    public void OpenFolderVoid()
    {
        if (!folderOpen) { folderOpen = true; StartCoroutine(OpenFolderCoroutine(1)); }
        else { folderOpen = false; StartCoroutine(CloseFolderCoroutine(1)); }
    }

    private IEnumerator OpenFolderCoroutine(float duration)
    {
        float time = 0;
        bool elementsHided = false;
        while (time < duration)
        {
            if (!elementsHided && time >= duration / 2)
            {
                elementsHided = true; HideElements(true);
                for (int i = 0; i < pageId; i++)
                {
                    pagesSteps[i].gameObject.SetActive(true);
                }
                folderCover.GetComponent<Image>().color = new Color(1, 0.9557926f, 0.8283019f);
            }
            folderDivider.localScale = new Vector3(Mathf.Lerp(1, -1, time / duration), 1, 1);
            folderDivider.localPosition = new Vector3(Mathf.Lerp(0, -370, time / duration), 0, 0);
            folderInside.localPosition = new Vector3(Mathf.Lerp(0, 370, time / duration), 0, 0);
            time += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator CloseFolderCoroutine(float duration)
    {
        float time = 0;
        bool elementsShowed = false;
        while (time < duration)
        {
            if (!elementsShowed && time >= duration / 2)
            {
                elementsShowed = true; HideElements(false);
                for (int i = 0; i < pageId; i++)
                {
                    pagesSteps[i].gameObject.SetActive(false);
                }
                folderCover.GetComponent<Image>().color = new Color(1, 1, 1);
            }
            folderDivider.localScale = new Vector3(Mathf.Lerp(-1, 1, time / duration), 1, 1);
            folderDivider.localPosition = new Vector3(Mathf.Lerp(-370, 0, time / duration), 0, 0);
            folderInside.localPosition = new Vector3(Mathf.Lerp(370, 0, time / duration), 0, 0);
            time += Time.deltaTime;
            yield return null;
        }
    }

     public void SwitchPages(bool nextPage)
    {
        InteractableButtons(false);
        if (nextPage) StartCoroutine(NextPage(1));
        else StartCoroutine(PreviousPage(1));
    }

    private IEnumerator NextPage(float duration)
    {
        pagesSteps[pageId].SetParent(folderDivider);
        pagesSteps[pageId+1].gameObject.SetActive(true);

        float time = 0;
        bool elementsHided = false;
        while (time < duration)
        {
            if (!elementsHided && time >= duration/2) { elementsHided = true; pagesSteps[pageId].GetChild(0).gameObject.SetActive(false); }
            pagesSteps[pageId].localScale = new Vector3(Mathf.Lerp(-1, 1, time / duration), 1, 1);
            pagesSteps[pageId].localPosition = new Vector3(Mathf.Lerp(-742, -30, time / duration), 0, 0);
            time += Time.deltaTime;
            yield return null;
        }
        pageId++;
        HideButtons();
    }

    private IEnumerator PreviousPage(float duration)
    {
        pageId--;
        float time = 0;
        bool elementsShowed = false;
        while (time < duration)
        {
            if (!elementsShowed && time >= duration/2) { elementsShowed = true; pagesSteps[pageId].GetChild(0).gameObject.SetActive(true); }
            pagesSteps[pageId].localScale = new Vector3(Mathf.Lerp(1, -1, time / duration), 1, 1);
            pagesSteps[pageId].localPosition = new Vector3(Mathf.Lerp(-30, -742, time / duration), 0, 0);
            time += Time.deltaTime;
            yield return null;
        }
        pagesSteps[pageId].SetParent(folderInside);
        HideButtons();
    }

    private void InteractableButtons(bool interactable)
    {
        pagesNavigationButs[1].interactable = interactable;
        pagesNavigationButs[0].interactable = interactable;
    }
    
    private void HideButtons()
    {
        InteractableButtons(true);
        pagesNavigationButs[1].gameObject.SetActive(pageId > 0);
        pagesNavigationButs[0].gameObject.SetActive(pageId < pagesSteps.Count - 1);
    }

    private void HideElements(bool hide)
    {
        foreach (GameObject elements in folderHideElements)
        {
            elements.SetActive(!hide);
        }
    }

    public void SetStepsRecords(Dictionary<string, StepRecords> dict)
    {
        records = dict;
    }

}
