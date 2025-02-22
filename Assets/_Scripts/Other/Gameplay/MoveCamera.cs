using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField, Range(0f, 1f)] private float smoothFactor;
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private Transform rotateAroundJoint;
    [SerializeField] private float angleChange;
    [SerializeField] private AudioClip moveSound;

    [Header("Guards relative positions")]
    [SerializeField] private Guard leftGuard;
    [SerializeField] private Guard currentGuard;
    [SerializeField] private Guard rightGuard;

    [Header("Controls")]
    [SerializeField] private Button goLeftButt;
    [SerializeField] private Button goRightButt;

    private float _posX;
    private float _posZ;
    private Transform _thatTrans;
    private Guard _tempLeft, _tempCurrent, _tempRight;
    private AudioSource _source;

    private void Awake()
    {
        _thatTrans = transform;
        _source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventManager.OnGuardDischarged += ReEnableControls;
    }

    private void OnDisable()
    {
        EventManager.OnGuardDischarged -= ReEnableControls;
    }

    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _posX = Mathf.Lerp(_thatTrans.position.x, targetToFollow.position.x, smoothFactor);
        _posZ = Mathf.Lerp(_thatTrans.position.z, targetToFollow.position.z, smoothFactor);
        _thatTrans.position = new Vector3(_posX, _thatTrans.position.y, _posZ);
    }

    public void TurnLeft()
    {
        if (!IsGuardActive(leftGuard))
        {
            return;
        }
        SwapGuardsOnSwapLeft();

        targetToFollow.RotateAround(rotateAroundJoint.position, Vector3.up, angleChange);
        _thatTrans.Rotate(angleChange * Vector3.up);

        _source.pitch = 0.95f;
        PlayMoveSound();
    }

    public void TurnRight()
    {
        if (!IsGuardActive(rightGuard))
        {
            return;
        }
        SwapGuardsOnSwapRight();

        targetToFollow.RotateAround(rotateAroundJoint.position, Vector3.down, angleChange);
        _thatTrans.Rotate(angleChange * Vector3.down);

        _source.pitch = 1.05f;
        PlayMoveSound();
    }

    private void SwapGuardsOnSwapLeft()
    {
        AssignTempGuards();

        leftGuard = _tempRight;
        currentGuard = _tempLeft;
        rightGuard = _tempCurrent;

        ReactivateNotifications();
        ReEnableControls();
    }

    private void SwapGuardsOnSwapRight()
    {
        AssignTempGuards();

        leftGuard = _tempCurrent;
        currentGuard = _tempRight;
        rightGuard = _tempLeft;

        ReactivateNotifications();
        ReEnableControls();
    }

    private void AssignTempGuards()
    {
        _tempLeft = leftGuard;
        _tempCurrent = currentGuard;
        _tempRight = rightGuard;
    }

    public void ReactivateNotifications()
    {
        leftGuard.RelatedDangerNotifications.alpha = 0;
        rightGuard.RelatedDangerNotifications.alpha = 0;

        currentGuard.RelatedDangerNotifications.alpha = 1;
    }

    private void ReEnableControls()
    {
        if (!leftGuard.IsHaveEnergy)
            goLeftButt.interactable = false;
        else
            goLeftButt.interactable = true;

        if (!rightGuard.IsHaveEnergy)
            goRightButt.interactable = false;
        else
            goRightButt.interactable = true;
    }

    private bool IsGuardActive(Guard guard) => guard.IsHaveEnergy;
    private void PlayMoveSound() => _source.PlayOneShot(moveSound);
}
