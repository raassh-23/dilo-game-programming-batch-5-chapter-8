using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private Button _localButton;
    [SerializeField] private Button _cloudButton;
    private void Start() {
        _localButton.onClick.AddListener(() => {
            SetButtonInteractable(false);
            UserDataManager.LoadFromLocal();
            SceneManager.LoadScene(1);
        });

        _cloudButton.onClick.AddListener(() => {
            SetButtonInteractable(false);
            StartCoroutine(UserDataManager.LoadFromCloud(() => {
                SceneManager.LoadScene(1);
            }));
        });
    }

    private void SetButtonInteractable(bool isInteractable) {
        _localButton.interactable = isInteractable;
        _cloudButton.interactable = isInteractable;
    }
}
