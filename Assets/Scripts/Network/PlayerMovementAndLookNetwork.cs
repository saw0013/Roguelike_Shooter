using Cinemachine;
using Mirror;
using MirrorBasics;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovementAndLookNetwork : NetworkBehaviour
{
    #region Variables
    [Header("Panels")]
    [SerializeField] private GameObject _panelEscape;
    [SerializeField] private GameObject _panelSetting;
    [SerializeField] private GameObject _panelExit;
    [SerializeField] private GameObject _panelMain;

    [Header("Camera")]
    public Camera mainCamera;


    [Header("Movement")]
    public Rigidbody playerRigidbody;
    public float speed = 4.5f;
    private Vector3 inputDirection;
    private Vector3 movement;

    //Rotation

    private Plane playerMovementPlane;

    private RaycastHit floorRaycastHit;

    private Vector3 playerToMouse;

    [SerializeField] private PlayerData playerData;


    [Header("Animation")]
    public Animator playerAnimator;


    [Header("Audio VFX")]
    [SerializeField] private AudioSource _runPlayer;

    #endregion

   

    //��� ������ ����� ������������ � �������
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //TODO : ��������� ������� �� �����?
        //GameObject.FindGameObjectWithTag("VirtualFollowCamera").GetComponent<CinemachineVirtualCamera>().Follow = transform;
    }

    

    #region Awake, Start, Update, FixedUpdate

    void Awake()
    {
        playerMovementPlane = new Plane(transform.up, transform.position + transform.up);
    }

    
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (playerData.InputActive)
            {
                StartMenu();
                EscapeMenu(true, false);
            }
            else EscapeMenu(false, true);
        }
    }

    private void FixedUpdate()
    {
        if (hasAuthority)
        {
            //Arrow Key Input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (playerData.InputActive) inputDirection = new Vector3(h, 0, v);
            else inputDirection = new Vector3(0, 0, 0);

            //Camera Direction
            var cameraForward = mainCamera.transform.forward;
            var cameraRight = mainCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            //Try not to use var for roadshows or learning code
            Vector3 desiredDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;

            //Why not just pass the vector instead of breaking it up only to remake it on the other side?

            if (inputDirection.z > 0 || inputDirection.z < 0 || inputDirection.x > 0 || inputDirection.x < 0)
            {
                if (!_runPlayer.isPlaying)
                    _runPlayer.Play();
            }

            MoveThePlayer(desiredDirection);

            if (playerData.InputActive) TurnThePlayer();

            AnimateThePlayer(desiredDirection);
        }
    }

    #endregion

    #region ������� ����
    public void EscapeMenu(bool active, bool input)
    {
        _panelEscape.SetActive(active);
        playerData.InputActive = input;
    }

    public void StartMenu()
    {
        _panelMain.SetActive(true);
        _panelExit.SetActive(false);
        _panelSetting.SetActive(false);

        _panelExit.GetComponent<RectTransform>().localPosition = new Vector3(0, 150.6f, 0);
        _panelSetting.GetComponent<RectTransform>().localPosition = new Vector3(0, 150.6f, 0);
    }

    #endregion

    #region ������������ � �������� ���������

    void MoveThePlayer(Vector3 desiredDirection)
    {
        movement.Set(desiredDirection.x, 0f, desiredDirection.z);

        movement = movement.normalized * speed * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void TurnThePlayer()
    {
        Vector3 cursorScreenPosition = Input.mousePosition;

        Vector3 cursorWorldPosition = ScreenPointToWorldPointOnPlane(cursorScreenPosition, playerMovementPlane, mainCamera);

        playerToMouse = cursorWorldPosition - transform.position;

        playerToMouse.y = 0f;

        playerToMouse.Normalize();

        Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

        playerRigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 10));
    }

    Vector3 PlaneRayIntersection(Plane plane, Ray ray)
    {
        float dist = 0.0f;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(screenPoint);
        return PlaneRayIntersection(plane, ray);
    }

    #endregion

    #region �������� ���������
    void AnimateThePlayer(Vector3 desiredDirection)
    {
        if (!playerAnimator)
            return;

        Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
        float forw = Vector3.Dot(movement, transform.forward);
        float stra = Vector3.Dot(movement, transform.right);

        playerAnimator.SetFloat("Forward", forw);
        playerAnimator.SetFloat("Strafe", stra);

        /*
		bool walking = h != 0f || v != 0f;

		if(walking)
		{
			Vector3 movement = new Vector3(h, 0f, v);
			float forw = Vector3.Dot(movement, transform.forward);
			float stra = Vector3.Dot(movement, transform.right);

			playerAnimator.SetFloat("Forward", forw);
			playerAnimator.SetFloat("Strafe", stra);
		}
		*/

    }
    #endregion
}
