using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {

    public Vector3 DefaultPos;


    private Material cursorMat;
    public Material CursorMat { get { return cursorMat; } }

    private Color idleColor;
    public Color IdleColor {  get { return idleColor; } }

    private Color selectColor;
    public Color SelectColor {  get { return selectColor; } }

    private Color holdColor;
    public Color HoldColor { get { return holdColor;  } }
    


    public static Cursor Instance;

    // Use this for initialization
    void Start () {
        Instance = this;
        cursorMat = GameObject.Find("Cursor geo").GetComponent<Renderer>().material;
        idleColor = new Color(.646f, .646f, .646f);
        selectColor = new Color(0, .588f, 1);
        holdColor = new Color(1, .588f, 0);
    }
	
	// Update is called once per frame
	void Update () {

        if (FocusManager.Instance.Focus)
        {
            transform.position = FocusManager.Instance.HitInfo.point;
            DefaultPos.z = transform.position.z;
        }

        float d = 1.1f*Vector3.Distance( FocusManager.Instance.transform.position, transform.position );

        transform.localScale = new Vector3( d, d, d);

    }

}
