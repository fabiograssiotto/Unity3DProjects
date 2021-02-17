using UnityEngine;
using System.Collections;

public class OnHit : MonoBehaviour {

    // For the cell positions
    public int position = 0;

    void OnMouseDown() {
        GameManager.Instance.Play(this.gameObject, position);
    }
} 
 