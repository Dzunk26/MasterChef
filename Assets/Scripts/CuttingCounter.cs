using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {
    [SerializeField] private CuttingRecipeSO[] cutKitchenObjectSOArray;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // there is no KitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    // Player is carrying something that can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
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
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
            // there is a KitchenObject and it can be cut
            KitchenObjectSO output = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            GetKitchenObject().SelfDestroy();

            KitchenObject.SpawnKitchenObject(output, this);
        }
        else {
            // there is no KitchenObject

        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input) {
        foreach (CuttingRecipeSO child in cutKitchenObjectSOArray) {
            if (child.input == input) {
                return true;
            }
        }
        return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input) {
        foreach(CuttingRecipeSO child in cutKitchenObjectSOArray) {
            if(child.input == input) {
                return child.output;
            }
        }
        return null;
    }
}
