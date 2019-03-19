using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public float Amount = 2f;

	void LateUpdate () {
        transform.Rotate(new Vector3(0, Amount * Time.deltaTime, 0));
	}
}
