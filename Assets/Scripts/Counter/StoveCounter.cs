using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoveCounter : BaseCounter, IHasProgress {
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventAgrs> OnProgressChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    private State currentState;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;

    private void Start() {
        currentState = State.Idle;
    }

    private void Update() {
        if (HasKitchenObject()) {
            switch (currentState) {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventAgrs{
                       progressNormalized = fryingTimer / this.fryingRecipeSO.fryingTimerMax });

                    if (fryingTimer >= this.fryingRecipeSO.fryingTimerMax) {
                        // Fried
                        GetKitchenObject().SelfDestroy();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        burningTimer = 0f;
                        currentState = State.Fried;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = currentState
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventAgrs {
                        progressNormalized = burningTimer / this.burningRecipeSO.burnTimerMax
                    });

                    if (burningTimer >= this.burningRecipeSO.burnTimerMax) {
                        // Burning
                        GetKitchenObject().SelfDestroy();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                        currentState = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = currentState
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventAgrs {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }


    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // there is no KitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player is carrying something that can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    this.fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    currentState = State.Frying;
                    fryingTimer = 0f;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventAgrs {
                        progressNormalized = fryingTimer / this.fryingRecipeSO.fryingTimerMax
                    });

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = currentState
                    });
                }
            }
            else {
                // Player is not carrying anything
            }
        }
        else {
            // there is a KitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
            }
            else {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                currentState = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = currentState
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventAgrs {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(input);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(input);
        if (fryingRecipeSO != null) {
            return fryingRecipeSO.output;
        }
        else {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO input) {
        foreach (FryingRecipeSO child in fryingRecipeSOArray) {
            if (child.input == input) {
                return child;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO input) {
        foreach (BurningRecipeSO child in burningRecipeSOArray) {
            if (child.input == input) {
                return child;
            }
        }
        return null;
    }
}