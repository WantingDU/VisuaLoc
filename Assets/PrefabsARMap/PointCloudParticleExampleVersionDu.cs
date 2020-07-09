using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class PointCloudParticleExampleVersionDu : MonoBehaviour {
    public ParticleSystem pointCloudParticlePrefab;
    public int maxPointsToShow;
    public float particleSize = 1.0f;
    private Vector3[] m_PointCloudData;
    private bool frameUpdated = false;
    private ParticleSystem currentPS;
    private ParticleSystem.Particle [] particles;
    private Text cloudPNumber;
    public static int CPNumber;
    // Use this for initialization
    private void OnDestroy()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdated;
    }
    void Start () {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
        currentPS = Instantiate (pointCloudParticlePrefab);
        frameUpdated = false;
        cloudPNumber = GameObject.Find("CPCurrent").GetComponent<Text>();
    }
	
    public void ARFrameUpdated(UnityARCamera camera)
    {
        switch (camera.trackingState)
        {
            case ARTrackingState.ARTrackingStateNotAvailable:
                cloudPNumber.color = Color.black;
                break;
            case ARTrackingState.ARTrackingStateLimited:
                cloudPNumber.color = Color.red;
                break;
            case ARTrackingState.ARTrackingStateNormal:
                cloudPNumber.color = Color.green;

                //Stop timer when tracking succesful 
                if (firestore.AllFilesReady())
                {
                    firestore.sw.Stop();
                    PersistenceTest.writeNewRebuild_simple(StaticObject.myARmapID,"1 ",firestore.sw.ElapsedMilliseconds.ToString(), CommonVariables.GetTimestamp(System.DateTime.Now), StaticObject.myARmapName, Auth.UserSelfId);
                    //PersistenceTest.writeNewRebuild_simple(StaticObject.myARmapID, "0", "timeout,>=20s", CommonVariables.GetTimestamp(System.DateTime.Now), StaticObject.myARmapName);
                    firestore.FilesLoaded = 0;
                    StaticObject.isTracked = true;
                }
                break;
        }
        if (firestore.sw != null)
        {
            if(firestore.sw.ElapsedMilliseconds >= 20000)
            {
                firestore.sw.Stop();
                //PersistenceTest.writeNewRebuild(StaticObject.myARmapID, "0", "timeout,>=20s",CommonVariables.GetTimestamp(System.DateTime.Now));
                PersistenceTest.writeNewRebuild_simple(StaticObject.myARmapID, "0", "timeout,>=20s", CommonVariables.GetTimestamp(System.DateTime.Now),StaticObject.myARmapName,Auth.UserSelfId);
                firestore.sw.Reset();
                firestore.FilesLoaded = 0;
            }
        }
           
        //camera.trackingReason;
        m_PointCloudData = camera.pointCloudData;
        frameUpdated = true;
    }

	// Update is called once per frame
	void Update () {
        if (frameUpdated) {
            if (m_PointCloudData != null && m_PointCloudData.Length > 0 && maxPointsToShow > 0) {
                cloudPNumber.text = currentPS.particleCount.ToString();
                CPNumber = currentPS.particleCount;
                int numParticles = Mathf.Min (m_PointCloudData.Length, maxPointsToShow);
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
                int index = 0;
                foreach (Vector3 currentPoint in m_PointCloudData) {     
                    particles [index].position = currentPoint;
                    particles [index].startColor = new Color (1.0f, 1.0f, 1.0f);
                    particles [index].startSize = particleSize;
                    index++;
                    if (index >= numParticles) break;
                }
                currentPS.SetParticles (particles, numParticles);
            } else {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
                particles [0].startSize = 0.0f;
                currentPS.SetParticles (particles, 1);
            }
            frameUpdated = false;
        }
	}



}
