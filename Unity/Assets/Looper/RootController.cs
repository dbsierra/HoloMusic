using UnityEngine;
using System.Collections;

public class RootController : MonoBehaviour {

    [Header("Assets")]
    public GameObject TapToPlace;
    public GameObject SoundObject;
    public LooperInterface Interface;

    private GameObject TapToPlaceInstance;
    private SoundObject SoundObjectInstance;

    public static RootController Instance;

    public bool recording;

    private SoundObject[] soundObjects;
    private int noOfSoundObjects = 0;

    private bool running;
    public bool Running { get { return running; } }

    // Use this for initialization
    void Start () {
        Instance = this;
        soundObjects = new SoundObject[100];
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Begin()
    {
        running = true;
    }

    public void TapToPlaceSoundObject()
    {
        if (noOfSoundObjects >= 100)
            return;

        if (TapToPlaceInstance == null )
        {
            Interface.gameObject.SetActive(false);
            TapToPlaceInstance = (GameObject)GameObject.Instantiate(TapToPlace, Vector3.zero, Quaternion.identity);
            TapToPlaceInstance.transform.SetParent(FocusManager.Instance.transform, false);
            TapToPlaceInstance.transform.localPosition = new Vector3(0, 0, 1f);
        }
    }

    public void FinalizeSoundObject()
    {
        if (noOfSoundObjects >= 100)
            return;

        SoundObjectInstance.transform.position = TapToPlaceInstance.transform.position;

        SoundObjectInstance.Enter();

        Interface.gameObject.SetActive(true);

        GameObject.Destroy(TapToPlaceInstance);
        TapToPlaceInstance = null;

        SoundObjectInstance.StopRecording();

        recording = false;
        
        
    }

    public void CreateSoundObject()
    {
        if (noOfSoundObjects >= 100)
            return;

        recording = true;

        GameObject soundObj = (GameObject)GameObject.Instantiate(SoundObject, Vector3.zero, Quaternion.identity);

        soundObj.transform.SetParent(FocusManager.Instance.transform, false);

        soundObj.transform.localPosition = new Vector3(0, 0, 1f);

        soundObj.transform.SetParent(transform, true);


        SoundObjectInstance = soundObj.GetComponent<SoundObject>();

        SoundObjectInstance.StartRecording();

        SoundObjectInstance.Index = noOfSoundObjects;

        Debug.Log(noOfSoundObjects);

        soundObjects[noOfSoundObjects++] = SoundObjectInstance;
    
    }

    public void DeleteSoundObject( int index )
    {
        //soundObjects[index].Exit();
        Debug.Log(index);
        GameObject.Destroy( soundObjects[index].gameObject );
    }



}
