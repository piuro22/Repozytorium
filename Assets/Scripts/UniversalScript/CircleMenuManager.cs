using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CircleMenuManager : MonoBehaviour
{
    [SerializeField] private SwipeController swipeController;
    [SerializeField] private Transform circleMenuTransform;
    [SerializeField] private float rotationAngle;
    [SerializeField] private Vector3 prefabScale;
    [SerializeField] private List<Unit> units = new List<Unit>();
    [SerializeField] private AnimationCurve swipeCurve;
    [SerializeField] private float swipeTime;
    [SerializeField] private TMP_Text unityNameText;
    [SerializeField] private GameObject panelUnitSelectorGrid;
    [SerializeField] private Transform gridPanel;
    [SerializeField] private CanvasGroup circleMenuCanvasGroup;
    [SerializeField] private GameObject gridUnitPrefab;
    [SerializeField] private ParticleSystemRenderer cloudParticles;
    private List<GameObject> spawnedUnits = new List<GameObject>();
    private float currentRotation;
    private int currentUnitIndex;
    private Tweener swipeTween;
    private bool canSwipe;
    private Vector3 startPosition;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;
    private void OnEnable()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        startPosition = transform.localPosition;
        swipeController.OnSwipeLeft += SwipeLeft;
        swipeController.OnSwipeUpperLeft += SwipeLeft;
        swipeController.OnSwipeLowerLeft += SwipeLeft;
        swipeController.OnSwipeRight += SwipeRight;
        swipeController.OnSwipeUpperRight += SwipeRight;
        swipeController.OnSwipeLowerRight += SwipeRight;
        leftArrow.onClick.AddListener(delegate () { SwipeLeft(Vector2.zero); });
        rightArrow.onClick.AddListener(delegate () { SwipeRight(Vector2.zero); });
        InstantiateUnits();
        canSwipe = true;
    }

    private void OnDisable()
    {
        swipeController.OnSwipeLeft -= SwipeLeft;
        swipeController.OnSwipeUpperLeft -= SwipeLeft;
        swipeController.OnSwipeLowerLeft -= SwipeLeft;
        swipeController.OnSwipeRight -= SwipeRight;
        swipeController.OnSwipeUpperRight -= SwipeRight;
        swipeController.OnSwipeLowerRight -= SwipeRight;
    }

    private void InstantiateUnits()
    {
        foreach (Unit unit in units)
        {
            GameObject unitObject = Instantiate(unit.UnitPrefab, transform);
            unitObject.transform.localScale = prefabScale;
            unitObject.GetComponent<UnitController>().Initialize(unit);
            spawnedUnits.Add(unitObject);

            GameObject gridUnitObject = Instantiate(gridUnitPrefab, gridPanel);
            gridUnitObject.GetComponent<GridUnit>().Initialize(unit);
        }

        SetupUnits();
    }

    private void SetupUnits()
    {
        foreach (GameObject unitObject in spawnedUnits)
        {
            unitObject.SetActive(false);
        }

        spawnedUnits[0].SetActive(true);
        spawnedUnits[0].transform.localEulerAngles = Vector3.zero;
    }

    private void SwipeLeft(Vector2 tapPosition)
    {
        if (!canSwipe || swipeTween != null)
            return;

        int previousIndex = currentUnitIndex;
        currentUnitIndex = (currentUnitIndex - 1 + spawnedUnits.Count) % spawnedUnits.Count;
  
        currentRotation -= rotationAngle;

        swipeTween = circleMenuTransform.DOLocalRotate(new Vector3(0, 0, 90), swipeTime, RotateMode.LocalAxisAdd)
            .SetEase(swipeCurve)
            .OnComplete(() =>
            {
                spawnedUnits[previousIndex].SetActive(false);
                swipeTween = null;
            });

        spawnedUnits[currentUnitIndex].SetActive(true);
        spawnedUnits[currentUnitIndex].transform.localEulerAngles = new Vector3(0, 0, currentRotation);
        unityNameText.SetText(units[currentUnitIndex].unitName);
    }

    private void SwipeRight(Vector2 tapPosition)
    {
        if (!canSwipe || swipeTween != null)
            return;

        int previousIndex = currentUnitIndex;
        currentUnitIndex = (currentUnitIndex + 1) % spawnedUnits.Count;
        currentRotation += rotationAngle;

        swipeTween = circleMenuTransform.DOLocalRotate(new Vector3(0, 0, -90), swipeTime, RotateMode.LocalAxisAdd)
            .SetEase(swipeCurve)
            .OnComplete(() =>
            {
                spawnedUnits[previousIndex].SetActive(false);
                swipeTween = null;
            });

        spawnedUnits[currentUnitIndex].SetActive(true);
        spawnedUnits[currentUnitIndex].transform.localEulerAngles = new Vector3(0, 0, currentRotation);
        unityNameText.SetText(units[currentUnitIndex].unitName);
    }

    public void OnGridButtonPressed()
    {
        if (panelUnitSelectorGrid.activeSelf)
        {
            panelUnitSelectorGrid.GetComponent<CanvasGroup>().DOFade(0, 0.4f).OnComplete(() =>
            {
                panelUnitSelectorGrid.SetActive(false);
            });

            transform.DOLocalMoveY(startPosition.y, 0.75f);
            circleMenuCanvasGroup.DOFade(1, 0.5f);
            cloudParticles.material.DOFade(1, 0.5f);
            canSwipe = true;

        }
        else
        {
            panelUnitSelectorGrid.GetComponent<CanvasGroup>().DOFade(1, 0.7f);
            transform.DOLocalMoveY(-30, 0.75f);
            circleMenuCanvasGroup.DOFade(0, 0.5f);
            cloudParticles.material.DOFade(0, 0.5f);
            panelUnitSelectorGrid.SetActive(true);
            canSwipe = false;
        }
    }

    public void BackToUnitSelect()
    {
        SceneManager.LoadScene("Scene Login");
    }
}

[Serializable]
public class Unit
{
    public GameObject UnitPrefab;
    public string unitName;
    public string sceneToLoadName;
}