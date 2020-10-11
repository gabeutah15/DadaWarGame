using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragCamera : MonoBehaviour
{
    [SerializeField]
    int cameraDragSpeed = 80;

    [SerializeField]
    float xMaxBound = 30;
    [SerializeField]
    float xMinBound = -30;
    [SerializeField]
    public static float zMaxBound = -30;
    [SerializeField]
    float zMinBound = -30;

    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = 10f;

    private Vector3 lastMousePos;

    Vector3 touchStart;
    public float zoomOutMin = 1;
    public float zoomOutMax = 40;
    


    void zoom(float increment)
    {
        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
        var thisCamPos = Camera.main.transform.position;
        var change = increment * 5;
        var newY = thisCamPos.y - change;
        newY = Mathf.Clamp(newY, zoomOutMin, zoomOutMax);
        //Debug.Log(newY);
        var newCamerPos =  new Vector3(thisCamPos.x, newY, thisCamPos.z);
        Camera.main.transform.position = newCamerPos;

    }


    private void Start()
    {
        zMinBound += Camera.main.transform.position.z;
        zMaxBound += Camera.main.transform.position.z;

        xMinBound += Camera.main.transform.position.x;
        xMaxBound += Camera.main.transform.position.x;

        lastMousePos = Vector3.zero;

    }
    [SerializeField]
    //float speedButtonMoveCam = 1;
    //public void MoveCameraUP()
    //{
    //   //this.transform.Translate(0, 0, step);
    //    Camera.main.transform.position -= new Vector3(0, 0, step);
    //}

    private bool upIsPressed;
    private bool downIsPressed;
    private bool leftIsPressed;
    private bool rightIsPressed;

    float step = 2;//find a proper value for this

    public void UpPressed()
    {
        upIsPressed = true;
    }
    public void UpReleased()
    {
        upIsPressed = false;
    }
    public void DownPressed()
    {
        downIsPressed = true;
    }
    public void DownReleased()
    {
        downIsPressed = false;
    }
    public void RightPressed()
    {
        rightIsPressed = true;
    }
    public void RightReleased()
    {
        rightIsPressed = false;
    }
    public void LeftPressed()
    {
        leftIsPressed = true;
    }
    public void LeftReleased()
    {
        leftIsPressed = false;
    }

    void ReceiveButtonInput()
    {
        if (upIsPressed && Camera.main.transform.position.z < zMaxBound)
            Camera.main.transform.position -= new Vector3(0, 0, -step);
        if (downIsPressed && Camera.main.transform.position.z > zMinBound)
            Camera.main.transform.position -= new Vector3(0, 0, step);
        if (leftIsPressed && Camera.main.transform.position.x > xMinBound)
            Camera.main.transform.position -= new Vector3(step, 0, 0);
        if (rightIsPressed && Camera.main.transform.position.x < xMaxBound)
            Camera.main.transform.position -= new Vector3(-step, 0, 0);

    }

    Vector3 cameraTarget;
    [SerializeField]
    private bool dragTest = true;
    private bool isDragging = false;

    private void Update()
    {
        ReceiveButtonInput();

        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f);
        }

        //will the below being in here mess with the above zoom function?
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            zoom(scrollInput);

        }

        //if (dragTest)
        //{
        //    Vector3 moveVector = new Vector3(0, 0, 0);
        //    if (Input.GetMouseButton(2) )
        //    {
        //        Vector3 deltaMousePos = (Input.mousePosition - lastMousePos);
        //        moveVector += new Vector3(-deltaMousePos.x, 0, -deltaMousePos.y) * 0.1f;// mousePanMultiplier;
        //    }
        //    var effectivePanSpeed = moveVector;
        //    cameraTarget = cameraTarget + transform.TransformDirection(effectivePanSpeed) * /*panSpeed*/15 * /*panMultiplier*/1 * Time.unscaledDeltaTime;
        //    transform.position = cameraTarget;
        //    lastMousePos = Input.mousePosition;
        //}

        //old zoom, distorts by changing field of view not actual zoom position
        //float fov = Camera.main.fieldOfView;
        //fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        //fov = Mathf.Clamp(fov, minFov, maxFov);
        //Camera.main.fieldOfView = fov;
        bool prevDrag = isDragging;

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        float dragMargin = 0.05f;
        //so the idea of added stuff below with the dragging and so on is to only take drag input if you drag at least a bit so
        //that touch input of select unit is not mistaken for drag move camera, not sure if it works yet or the .5f margin should be larger
        if (/*Input.GetMouseButton(1)*//*(Input.touchCount == 2) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) && !IsPointerOverUIObject()*/false)//not using click input anymore
        {
            float speed = cameraDragSpeed * Time.deltaTime;
            var xInput = Input.GetAxis("Mouse X");
            var yInput = Input.GetAxis("Mouse Y");
            var xAbs = Mathf.Abs(xInput);
            var yAbs = Mathf.Abs(yInput);


            if (xAbs > dragMargin || (yAbs > dragMargin))
            {
                isDragging = true;
            }
            //else
            //{
            //    isDragging = false;
            //}
            //**no no, instead set to false on click up only
            if(prevDrag != isDragging)
            {
                //Debug.Log(isDragging);
            }

            if (xAbs > dragMargin || (yAbs > dragMargin) || isDragging)//is this performant? how hard are math.abs calls and input checks?
            {
                if (isDragging && (xAbs < dragMargin) && (yAbs < dragMargin))
                {
                    //Debug.Log("dragging on none");
                }

                Camera.main.transform.position -= new Vector3(xInput * speed, 0, yInput * speed);

                if (Camera.main.transform.position.x > xMaxBound)
                {
                    Camera.main.transform.position = new Vector3(xMaxBound, Camera.main.transform.position.y, Camera.main.transform.position.z);
                }

                if (Camera.main.transform.position.x < xMinBound)
                {
                    Camera.main.transform.position = new Vector3(xMinBound, Camera.main.transform.position.y, Camera.main.transform.position.z);
                }

                if (Camera.main.transform.position.z > zMaxBound)
                {
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, zMaxBound);
                }

                if (Camera.main.transform.position.z < zMinBound)
                {
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, zMinBound);
                }
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}