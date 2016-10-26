using UnityEngine;
using System.Collections;

public class ShaderGazer : MonoBehaviour {


    public Material[] MaterialGazeList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        foreach( Material m in MaterialGazeList)
        {
            Vector3 point = FocusManager.Instance.HitInfo.point;

            m.SetVector("_Gaze", point);
        }

	}
}
