using UnityEngine;
using System.Collections;

public class RecordingDeviceButtons : MonoBehaviour {

    [Header("Button Types")]
    public bool Record;
    public bool TapToPlace;

    //tap to place shiz
    SoundObject soundObjectInstance;

    [Header("Tap to record assets")]
    public TextMesh textTapToRecord;
    public TextMesh textTapToFinish;
    public Renderer Cylinder;

    [Space(10)]
    public GameObject Arrow;
    bool done;


    // Use this for initialization
    void Start () {
        if( textTapToFinish != null )
            textTapToFinish.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
        if( Time.time >= 10 )
        {
            done = true;
            RootController.Instance.Begin();
            GameObject.Destroy(Arrow);
        }


	}

    public void OnGazeEnter()
    {
        Debug.Log("gazed");
        RootController.Instance.Begin();
        GameObject.Destroy(Arrow);
    }

    public void OnGazeExit()
    {

    }   

    public void OnTap()
    {
        
        if (Record )
        {
            if( !RootController.Instance.recording )
                RootController.Instance.TapToPlaceSoundObject();
            //gameObject.GetComponent<AudioSource>().Stop();
           // gameObject.GetComponent<AudioSource>().enabled = false;
            //GameObject.Destroy(gameObject.GetComponent<AudioSource>());
        }

        if (TapToPlace)
        {
            if( !RootController.Instance.recording )
            {
                textTapToRecord.gameObject.SetActive(false);
                textTapToFinish.gameObject.SetActive(true);
                RootController.Instance.CreateSoundObject();
                Cylinder.material.SetColor("_EmissionColor", new Color(1f, 0, 0.1448278f));
            }
            else
            {
                RootController.Instance.FinalizeSoundObject();
                Cylinder.material.SetColor("_EmissionColor", new Color(.11f, .11f, 0.11f));
            }
        }

    }



}
