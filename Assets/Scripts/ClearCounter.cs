using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClearCounter : BaseCounter {
    [SerializeField] private KitchenObjectSO kitchenObjectsSO;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // there is no KitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
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
}