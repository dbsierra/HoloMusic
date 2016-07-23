using UnityEngine;
using System.Collections;
using Sequencer.PianoRoll;

public class PRoll_Note : MonoBehaviour {

    public PRoll_Slot Slot;
    public byte PitchIndex;
    public byte PositionIndex;
    private Material mat;

	public void Init( PRoll_Slot Slot, byte PitchIndex, byte PositionIndex )
    {
        this.Slot = Slot;
        this.PitchIndex = PitchIndex;
        this.PositionIndex = PositionIndex;
    }

    void OnEnable()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_EmissionColor", new Color(.1f, .1f, .1f));
    }

    public void Delete()
    {
        Slot.Controller.RemoveNote(PositionIndex, PitchIndex);
    }

    private void OnTap()
    {
        Delete();
    }

    public void PlayingAnimation()
    {
        mat.SetColor("_EmissionColor", new Color(0, .5f, .85f));
    }
    public void StoppingAnimation()
    {
        mat.SetColor("_EmissionColor", new Color(.1f, .1f, .1f));
    }
}
