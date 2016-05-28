using UnityEngine;
using System.Collections;

public class SequencerBlock : MonoBehaviour {

    public bool On;
    private Sequencer parent;
    public Renderer r;
    private Material m;

    public int NoteIndex;
    public int Beat;

	// Use this for initialization
	void Start () {
        parent = transform.parent.GetComponent<Sequencer>();
        m = r.material;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTap()
    {
        On = !On;

        if( On )
        {
            m.SetColor("_Color1", parent.ActiveColor);
            Highlight();
            parent.AddNote(Beat, NoteIndex);
        }
        else
        {
            m.SetColor("_Color1", parent.IdleColor);
            Highlight();
            parent.RemoveNote(Beat, NoteIndex);
        }
    }



    public void Highlight()
    {
        AddToColor(.5f);
    }
    public void DeHighlight()
    {
        AddToColor(-.5f);
    }


    private void AddToColor(float a)
    {
        Color c = m.GetColor("_Color1");
        c = new Color(c.r + a, c.g + a, c.b + a);
        m.SetColor("_Color1", c);
    }

    public void OnGazeEnter()
    {
        Highlight();

        transform.localScale = new Vector3(.15f, .15f, .15f);
    }

    public void OnGazeExit()
    {
        DeHighlight();

        transform.localScale = new Vector3(.1f, .1f, .1f);
    }
}
