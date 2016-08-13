using UnityEngine;
using System.Collections;

public class TapToPlaceIcon : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (FocusManager.Instance.Focus)
        {
           // Debug.Log(FocusManager.Instance.HitInfo.point.z);
            //transform.localPosition = new Vector3(0, 0, 0);

            //transform.position = FocusManager.Instance.HitInfo.point;
        }
       
        float d = 2.1f * Vector3.Distance(FocusManager.Instance.transform.position, transform.position);
       transform.localScale = new Vector3(d, d, d);


    }

}
