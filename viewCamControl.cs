using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class viewCamControl : MonoBehaviour
{
    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;

    protected Vector3 _LocalRotation;

    public Vector3 initCamLocalRotation = new Vector3(113.4f, 32.8f, 0.0f);
    protected float _CameraDefaultDistance = 2525f;
    protected float _CameraDistance = 0f;

        // public int _CameraMode = (int)CameraModeCode.Walk;
    protected enum CameraModeCode {Orbit,Walk};
    public int _CameraMode = (int)CameraModeCode.Orbit;

    public float MouseMoveSensitivity = 30f;
    public float MouseSensitivity = 4f;
    public float ScrollSensitivity = 2f;
    public float OrbitSpeed = 10f;
    public float ScrollSpeed = 6f;
    public float MoveSpeed = 0.05f;
    public bool CameraDisabled = false;


    private bool _bMouseLeftHold = false;
    private bool _bMouseRightHold = false;
    
    private bool _bInputCap = true;

    private long _MouseLeftPressedTs;
    private long _MouseLeftReleasedTs;

    private long _MouseRightPressedTs;
    private long _MouseRightReleasedTs;


    public float _MaxCameraDistance = 5000f;
    public float _MinCameraDistance = 5.0f;

    public Vector3 initCameraTargetPosition = new Vector3(24.7416763f,-64.7137604f,22.3032436f);


    public static long getMyTimestampInMil(DateTime dateTimeToConvert) {
        // According to Wikipedia, there are 10,000,000 ticks in a second, and Now.Ticks is the span since 1/1/0001. 
        long NumSeconds= dateTimeToConvert.Ticks / 10000000;
        long NumMillisencods = dateTimeToConvert.Ticks / 10000;
        // return NumSeconds;
        return NumMillisencods;
    }

    Vector3 getCameraTargetPositionMark(string name){
        Vector3 res;
        res = GameObject.Find(name).transform.position;
        return res;
    }

    void SetCameraTargetPosition(Vector3 position){
        SetParentPosition(position);
    }

    void SetParentPosition(Vector3 position){
        this._XForm_Parent.position = position;

    }

    Vector3 getCameraCurrentPosition(){
        Vector3 position = this._XForm_Parent.position;
        Debug.Log(position);
        Debug.Log(this._LocalRotation);
        return position;
    }

    void SetInputCap(int val){
        if(val > 0){
            this._bInputCap = true;
        }else{
            this._bInputCap = false;
        }
    }

    
    void SetCameraDisable(int val){
    // mouse not over unity canvas
        if (val > 0)
        {
            CameraDisabled = true;
        }else{
            CameraDisabled = false;
        }
    }

    void SetCameraDistance(float val){
        this._CameraDistance = val;
    }

    void SetCameraMode(int val){
        this._CameraMode = val;
    }

    int getCameraMode(){
        return this._CameraMode;
    }

    void gotoInterScene(){
        SceneManager.LoadScene("Intereact");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
