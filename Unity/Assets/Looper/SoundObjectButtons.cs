using UnityEngine;
using System.Collections;

public class SoundObjectButtons : MonoBehaviour {

    [Header("Button Types")]
    public bool Mute;
    public bool Delete;

    [Space(10)]
    public SoundObject TargetSoundObject;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTap()
    {

        if (Mute)
        {
            TargetSoundObject.MutePlayback();
        }

        if( Delete )
        {
            RootController.Instance.DeleteSoundObject(TargetSoundObject.Index);
        }

    }
}
