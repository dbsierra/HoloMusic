using UnityEngine;
using System.Collections;

public class ShaderGazer : MonoBehaviour {

    public Transform cursor;
    public Renderer target;
    Material mat;

	// Use this for initialization
	void Start () {
        mat = target.material;
	}
	
	// Update is called once per frame
	void Update () {
	

        Vector3 point = FocusManager.Instance.HitInfo.point;
        mat.SetVector("_Gaze", cursor.position );
        

	}
}
