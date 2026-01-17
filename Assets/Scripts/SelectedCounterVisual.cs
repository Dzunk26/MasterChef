using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

    [SerializeField] private BaseCounter selectedCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start() {
        Player.Instance.OnSelectedCounter += Player_OnSelectedCounter;
    }

    private void Player_OnSelectedCounter(object sender, Player.OnSeclectedCounterEventArgs e) {
        if (e.seclectedCounter == selectedCounter) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }
}