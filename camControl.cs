using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;


public class camControl : MonoBehaviour
{

    public GameObject FPSCanvas;
    public camDataObject dataObj;
    public int animFrameIndexA;
    public int animFrameIndexB;
    public int stepSpeed;
    public int camHeight = 3;
    public int walkPath;
    public int viewMode;

    public string lastSelectedObjName = "";
    Dictionary<string,GameObject> allMeshDict;
    // Start is called before the first frame update



    protected enum CameraModeCode {Orbit,Walk};
    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;

    protected Vector3 _LocalRotation;
    // for set init/reset camera position
    public Vector3 initCamLocalRotation = new Vector3(113.4f, 32.8f, 0.0f);
    protected float _CameraDefaultDistance = 2525f;
    protected float _CameraDistance = 0f;

    // public int _CameraMode = (int)CameraModeCode.Walk;
    public int _CameraMode = (int)CameraModeCode.Orbit;

    public float MouseDragMoveSensitivity = 50f;
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

    // private Vector3 _CameraTargetPositionMark;

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

    void hideObj(string name){
        // GameObject.Find(name).GetComponent<MeshRenderer>().enabled = false;

        // move to behind collideblockplane by minus 20000 on 
        allMeshDict[name].GetComponent<MeshRenderer>().enabled = false;
        Vector3 hidePos = allMeshDict[name].transform.position;
        hidePos.y -= 20000f;
        allMeshDict[name].transform.position = hidePos;
        
    }

    void tryToGetValFromUnity(string val){
        Debug.LogWarning(val);
    }
    void unHideObj(string name){
        if(!allMeshDict[name].GetComponent<MeshRenderer>().enabled && allMeshDict[name].transform.position[1] < -10000f ){
            Vector3 unHidePos = allMeshDict[name].transform.position;
            unHidePos.y += 20000f;
            allMeshDict[name].transform.position = unHidePos;

            allMeshDict[name].GetComponent<MeshRenderer>().enabled = true;
        }
        

    }

    void hideCurrentSelected(){

        hideObj(lastSelectedObjName);
        lastSelectedObjName = "";

    }
    void testMultiParams(string p1,string p2,string p3){
        Debug.Log(p1);
        Debug.Log(p2);
        Debug.Log(p3);
    }

    void selectedAsFocus(){
        if(lastSelectedObjName != ""){
            SetCameraTargetPosition(allMeshDict[lastSelectedObjName].transform.position);
            this._CameraDistance = Vector3.Distance(transform.position,allMeshDict[lastSelectedObjName].transform.position);

        }
    }
    // List<GameObject> FindSceneObjects(string sceneName){
    //     List<GameObject> objs = new List<GameObject>();
    //         foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
    //         {
    //             if(obj.scene.name.CompareTo(sceneName) == 0){
    //                 objs.Add(obj);
    //                 // Debug.Log(obj.name);
    //             }
    //         }
    //         return objs;
    // }

    // string[] FindSceneObjectsName(string sceneName){
    //     string[] objs = new string[];
    //         foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
    //         {
    //             if(obj.scene.name.CompareTo(sceneName) == 0){
    //                 objs.Add(obj.name);
    //                 // Debug.Log(obj.name);
    //             }
    //         }
    //         return objs;
    // }


    Dictionary<string,GameObject> myFindSceneObjects(string sceneName){
        Dictionary<string,GameObject> objs = new Dictionary<string,GameObject>();
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if(obj.scene.name.CompareTo(sceneName) == 0){
                objs.Add(obj.name,obj);
            }
        }
        return objs;
    }

    void ResetView(){
        SceneManager.LoadScene("Intereact");
    }

    void SetViewMode(int mode){
        if(mode == 1){
            SetCameraMode((int)CameraModeCode.Orbit);
            // switch hide/unhide
            // foreach (string objName in dataObj.chenqiViewObjNameArr)
            // {
            //     allMeshDict[objName].GetComponent<MeshRenderer>().enabled = false;
            // }

            // foreach (string objName in dataObj.chenqiObjNameArr)
            // {
            //     allMeshDict[objName].GetComponent<MeshRenderer>().enabled = true;
            // }
            InitCamPosition();
        }

        if(mode == 2){
            SetCameraMode((int)CameraModeCode.Walk);
            SetCameraDistance(0);
            // switch hide/unhide
            // foreach (string objName in dataObj.chenqiObjNameArr)
            // {
            //     allMeshDict[objName].GetComponent<MeshRenderer>().enabled = false;
            // }
            // foreach (string objName in dataObj.chenqiViewObjNameArr)
            // {
            //     allMeshDict[objName].GetComponent<MeshRenderer>().enabled = true;
            // }
            animFrameIndexA = 0;
            animFrameIndexB = 0;
        }

    }

    void SetWalkPath(int path){
        walkPath = path;
    }

    void SetWalkSpeed(int speed){
        stepSpeed = speed;
    }

    void InitCamPosition(){
        Debug.Log("init cam position");
        SetCameraTargetPosition(initCameraTargetPosition);
        this._LocalRotation = initCamLocalRotation;
        this._CameraDistance = this._CameraDefaultDistance;
    }

    void initMatAndColor(){
        Debug.Log("init mat and color by config");


        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        Debug.Log(camDataObject.testColor);

        foreach(KeyValuePair<string, Color> entry in dataObj.initColorDict)
        {

            propertyBlock.SetColor("_BaseColor",entry.Value);
            propertyBlock.SetFloat("_RimLight",0);
            // do something with entry.Value or entry.Key
            allMeshDict[entry.Key].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
        }
    }

    void toggleDebugInfo(int val){
        if(val > 0){
            FPSCanvas.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        }else{
            FPSCanvas.transform.localScale = new Vector3(1.0f,0.0f,1.0f);
        }
    }

    void gotoViewScene(){
        SceneManager.LoadScene("View");
    }

    void SetMouseDragMoveSensitivity(float val){
        MouseDragMoveSensitivity = val;
    }

    void Start()
    {


        FPSCanvas = GameObject.Find("LiteFPSCounter");
        // Debug.Log(FPSCanvas.transform.localScale);

        Debug.LogWarning("tim: hello from unity");
        dataObj = gameObject.GetComponent<camDataObject>();
        // Debug.Log(UnityEngine.GameObject.Find("Cube").transform.position);
        // Debug.Log(GameObject.Find("/Example Assets/Workshop Set/Stud Pile").transform.position);
        // Debug.Log(dataObj.chengqi_lumian_Y4cm_Z4cm_Y4cm_anim_path_pos[0][0]);
        // Debug.Log(dataObj.chengqi_lumian_Y4cm_Z4cm_Z4cm_anim_path_pos[100][2]);
        // Debug.Log(dataObj.exportStr);

        stepSpeed = 1;


        Application.targetFrameRate = 30;

        // init dict of all mesh
        allMeshDict = myFindSceneObjects("Intereact");
        Debug.Log(allMeshDict["diceng__huangtuQ3al"].transform.position);

        Debug.Log(allMeshDict.Count);


        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;


        
        SetViewMode(1);
        animFrameIndexA = 0;
        animFrameIndexB = 0;
        walkPath = 1;




        // check object list 
        foreach(KeyValuePair<string, Color> entry in dataObj.initColorDict)
        {

            if (allMeshDict.ContainsKey(entry.Key)) {
                
            }else{
                Debug.Log(entry.Key);
            }

        }





        initMatAndColor();


// ---------------------  TEST DISPLAY BEFORE ADD ALL OBJES
        // SetViewMode(1);
        SetCameraMode((int)CameraModeCode.Orbit);
        // InitCamPosition();

        // Debug.Log(Camera.main);
        Debug.Log(getMyTimestampInMil(DateTime.Now));


        // SceneManager.LoadScene("Intereact");
    }

    // Update is called once per frame
    void Update()
    {

        // Debug.Log("camera x axis");
        // Debug.Log(this.transform.right);
        // Debug.Log("camera y axis");
        // Debug.Log(this.transform.up);

        // Debug.Log("parent forward");
        // Debug.Log(this._XForm_Parent.forward);
        // Debug.Log("camera forward");
        // Debug.Log(this._XForm_Camera.forward);

        // Debug.Log(this._CameraDistance);
        // Debug.Log(transform.position);

        // Debug.Log(this._LocalRotation);


        if(_CameraMode == (int)CameraModeCode.Walk){
            if(walkPath == 1){
                    if(animFrameIndexA < dataObj.chengqi_lumian_Y4cm_Z4cm_Z4cm_anim_path_pos.Length - 2){
                        Vector3 savedPos = dataObj.chengqi_lumian_Y4cm_Z4cm_Z4cm_anim_path_pos[animFrameIndexA];
                        Vector3 actualPos = new Vector3(savedPos[0],savedPos[1] + camHeight,savedPos[2]);
                        Vector3 targetSavedPos = dataObj.chengqi_lumian_Y4cm_Z4cm_Z4cm_anim_path_pos[animFrameIndexA + 1];
                        Vector3 targetPos = new Vector3(targetSavedPos[0],targetSavedPos[1] + camHeight,targetSavedPos[2]);

                        // SetCameraTargetPosition(actualPos);
                        // SetCameraDistance(0);
                        transform.position = actualPos;
                        transform.LookAt(targetPos);
                        animFrameIndexA += stepSpeed;

                        // Debug.Log(savedPos);
                        // Debug.Log(actualPos);

                    }

                }
                if(walkPath == 2){
                    if(animFrameIndexA < dataObj.chengqi_lumian_Y4cm_Z4cm_Y4cm_anim_path_pos.Length - 2){
                        Vector3 savedPos = dataObj.chengqi_lumian_Y4cm_Z4cm_Y4cm_anim_path_pos[animFrameIndexA];
                        Vector3 actualPos = new Vector3(savedPos[0],savedPos[1] + camHeight,savedPos[2]);
                        Vector3 targetSavedPos = dataObj.chengqi_lumian_Y4cm_Z4cm_Y4cm_anim_path_pos[animFrameIndexA + 1];
                        Vector3 targetPos = new Vector3(targetSavedPos[0],targetSavedPos[1] + camHeight,targetSavedPos[2]);
                        SetCameraTargetPosition(actualPos);
                        SetCameraDistance(0);

                        transform.position = actualPos;
                        transform.LookAt(targetPos);
                        animFrameIndexA += stepSpeed;

                        // Debug.Log(savedPos);
                        // Debug.Log(actualPos);

                    }

                }
        
        }



               // Debug.Log(Time.deltaTime * 1000);



        if(Input.GetMouseButtonDown(1)){
            this._bMouseRightHold = true;
            this._MouseRightPressedTs = getMyTimestampInMil(DateTime.Now);

            Debug.Log("Mouse right Hold " + this._MouseRightPressedTs);

        }

        if (Input.GetMouseButtonUp(1)){

            this._bMouseRightHold = false;
            this._MouseRightReleasedTs = getMyTimestampInMil(DateTime.Now);


            Debug.Log("Mouse right Not Hold " + this._MouseRightReleasedTs);
        }


        if(Input.GetMouseButtonDown(0)){
            this._bMouseLeftHold = true;
            this._MouseLeftPressedTs = getMyTimestampInMil(DateTime.Now);

            Debug.Log("Mouse Hold " + this._MouseLeftPressedTs);

        }
        
        if (Input.GetMouseButtonUp(0)){
        

            this._bMouseLeftHold = false;
            this._MouseLeftReleasedTs = getMyTimestampInMil(DateTime.Now);


            Debug.Log("Mouse Not Hold " + this._MouseLeftReleasedTs);



                   // distinguish click and drag by 300 milliseconds

            if (this._MouseLeftReleasedTs - this._MouseLeftPressedTs <= 300){ // if left button pressed...

                // Debug.Log(this._MouseLeftReleasedTs - this._MouseLeftPressedTs);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // the object identified by hit.transform was clicked
                    // do whatever you want
                    // Debug.Log("Clicked on gameobject: " +  hit.collider.name);
                    Debug.LogWarning("tim Clicked on gameobject:" +  hit.collider.name);
                    // Debug.Log(allMeshDict[hit.collider.name].GetComponent<MeshRenderer>().enabled);
                    // if(hit.collider.name != "collideBlockPlane"){
                    if(dataObj.initColorDict.ContainsKey(hit.collider.name)){
                        // hideObj(hit.collider.name);

                        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                        propertyBlock.SetColor("_BaseColor",dataObj.initColorDict[hit.collider.name]);
                        // first selected
                        if(lastSelectedObjName == "" || lastSelectedObjName == hit.collider.name){
                            propertyBlock.SetFloat("_RimLight",1);
                            allMeshDict[hit.collider.name].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                            lastSelectedObjName = hit.collider.name;
                        }
                        // check if new one selected    
                        if(hit.collider.name != lastSelectedObjName){
                                propertyBlock.SetFloat("_RimLight",0);
                                Color lastObjColor = dataObj.initColorDict[lastSelectedObjName];
                                propertyBlock.SetColor("_BaseColor",lastObjColor);
                                GameObject lastObj =allMeshDict[lastSelectedObjName];
                                lastObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);

                                propertyBlock.SetFloat("_RimLight",1);
                                Color newObjColor = dataObj.initColorDict[hit.collider.name];
                                propertyBlock.SetColor("_BaseColor",newObjColor);
                                GameObject newObj = allMeshDict[hit.collider.name];
                                newObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                                
                                // update record
                                lastSelectedObjName = hit.collider.name;

                        }

                    }


                }
            }

        }



 

    }


        // late Update is called once per frame after update()
    void LateUpdate()
    {
        // pause game for ignore key input
        if(CameraDisabled){
            Time.timeScale = 0;
        }else{
            Time.timeScale = 1;
        }







        if(this._CameraMode == (int)CameraModeCode.Orbit){
            // switch from walk mode as distance is 0
            // if(this._CameraDistance == 0){
            //     this._CameraDistance = this._CameraDefaultDistance;

            //     // reset parent target cube position
            //     // this._XForm_Parent.position = new Vector3(0f,0f,0f);

            // }


            SetCameraDistance(this._CameraDistance);


            // right mouse control
            if(!CameraDisabled & this._bMouseRightHold){
                if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0){
                    _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                    _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;

                    // // clamp the y rotation not flipping
                    // if(_LocalRotation.y < 0f)
                    //     _LocalRotation = 0f;
                    // else if(_LocalRotation.y > 90f)
                    //     _LocalRotation.y = 90f;

                    // _LocalRotation.y = Mathf.Clamp(_LocalRotation.y,0f,90f);
                    
                }
            }

            // Debug.Log("mouse input");
            // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // left mouse control

            if(!CameraDisabled & this._bMouseLeftHold){
                if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0){

                    // old version , not responsive to camera rotate =============
                    // Vector3 direction = this.forward - new Vector3(0.0f,0.0f,1.0f);
                    // SetParentPosition(
                    //             new Vector3(this._XForm_Parent.position.x + Input.GetAxis("Mouse X") * MouseDragMoveSensitivity ,
                    //             this._XForm_Parent.position.y - Input.GetAxis("Mouse Y") * MouseDragMoveSensitivity,
                    //             this._XForm_Parent.position.z));



                    //  camera and camera target has same forward vector, therefore camera.transform.right and camera.transform.up is same to camera target , get mouse move on the camera plane is equal to get on camera target perpenticual plane to camera forward

                    // TODO  add distance from camera to target to factor mouse maybe
                    Vector3 cam_X_move = Input.GetAxis("Mouse X") * MouseDragMoveSensitivity * this.transform.right;

                    Vector3 cam_Y_move = Input.GetAxis("Mouse Y") * MouseDragMoveSensitivity * this.transform.up;

                    // after test to make sure + or - the move value
                    this._XForm_Parent.position -= cam_Y_move;
                    this._XForm_Parent.position -= cam_X_move;


                }
            }



            // wheel mouse control
            if(!CameraDisabled & this._bInputCap){
                // Zooming input from mouse scrool wheel or slide control  -- TODO
                if(Input.GetAxis("Mouse ScrollWheel") != 0f){
                    float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity;
                    // makes camera zoom faster the futher away from the target
                    ScrollAmount *= (this._CameraDistance * 0.3f);

                    this._CameraDistance += ScrollAmount * -1f;
                    // clamp camera distance to the target in meters
                    this._CameraDistance = Mathf.Clamp(this._CameraDistance,_MinCameraDistance,_MaxCameraDistance);

                }
            }

            // do the transform to camera
            Quaternion QT = Quaternion.Euler(_LocalRotation.y,_LocalRotation.x,0);
            
            // animate to rotate the target A.K.A the camera
            this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation,QT,Time.deltaTime * OrbitSpeed);

            if(this._XForm_Camera.localPosition.z != this._CameraDistance * -1f){
                this._XForm_Camera.localPosition = new Vector3(0f,0f,Mathf.Lerp(this._XForm_Camera.localPosition.z,this._CameraDistance * -1f,Time.deltaTime * ScrollSpeed));

        }

        // if(this._CameraMode == (int)CameraModeCode.Walk){
        //     SetCameraDistance(0);

        //     if(!CameraDisabled & this._bMouseLeftHold){
        //         if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0){
        //             _LocalRotation.x -= Input.GetAxis("Mouse X") * MouseSensitivity;
        //             _LocalRotation.y += Input.GetAxis("Mouse Y") * MouseSensitivity;

        //             // walk mode needs look from botton to top 
        //             _LocalRotation.y = Mathf.Clamp(_LocalRotation.y,-90f,90f);
                    
        //         }

        //     }

        // }
        }


    }
}
