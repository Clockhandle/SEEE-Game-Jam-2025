using UnityEngine;


public class MoveUpDownPlatform : MonoBehaviour
{
    public enum MoveDirection { Horizontal, Vertical }

    [Header("Movement Settings")]
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Vertical;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float pauseTime = 1f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Start()
    {
        startPos = transform.position;

        // Decide target based on movement direction
        if (moveDirection == MoveDirection.Vertical)
            targetPos = startPos + Vector3.up * moveDistance;
        else
            targetPos = startPos + Vector3.right * moveDistance;

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        LeanTween.move(gameObject, targetPos, moveDuration)
                 .setEaseInOutSine()
                 .setOnComplete(() =>
                 {
                     LeanTween.delayedCall(pauseTime, MoveToStart);
                 });
    }

    private void MoveToStart()
    {
        LeanTween.move(gameObject, startPos, moveDuration)
                 .setEaseInOutSine()
                 .setOnComplete(() =>
                 {
                     LeanTween.delayedCall(pauseTime, MoveToTarget);
                 });
    }
}
