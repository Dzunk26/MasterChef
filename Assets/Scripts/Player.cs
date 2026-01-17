using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform playerHoldPoint;
    private KitchenObject kitchenObject;
    private bool isWalking = false;
    private Vector3 lastInteractDir;
    private BaseCounter seclectedCounter;

    public event EventHandler<OnSeclectedCounterEventArgs> OnSelectedCounter;
    public class OnSeclectedCounterEventArgs : EventArgs {
        public BaseCounter seclectedCounter;
    }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (seclectedCounter != null) {
            seclectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (seclectedCounter != null) {
            seclectedCounter.Interact(this);
        }
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                SetSelectedCounter(baseCounter);
            }
            else {
                SetSelectedCounter(null);
            }
        }
        else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.6f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove) {
            // can't move toward
            // attempt only X direction
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove) {
                //can move on X direction
                moveDir = moveDirX;
            }
            else {
                //can't move on X direction
                //attempt only Z direction
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove) {
                    //can move on Z direction
                    moveDir = moveDirZ;
                }
                else {
                    // can move any direction
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }
        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter baseCounter) {
        this.seclectedCounter = baseCounter;

        OnSelectedCounter?.Invoke(this, new OnSeclectedCounterEventArgs {
            seclectedCounter = seclectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return playerHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObjectParent) {
        this.kitchenObject = kitchenObjectParent;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}