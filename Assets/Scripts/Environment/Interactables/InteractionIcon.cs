using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class InteractionIcon : MonoBehaviour
{
    [Header("General Parameters")]
    [Tooltip("How fast the icon spins around."), SerializeField] private float rotationSpeed = 45;
    [Tooltip("How fast the icon bobs up and down."), SerializeField] private float bobSpeed = 1;
    [Tooltip("How much does the icon bob up and down."), SerializeField] private float bobIntensity = 0.1f;

    [Header("Tween Parameters")]
    [Tooltip("Show animation duration"), SerializeField] private float showDuration = 0.25f;
    [Tooltip("Show animation ease type"), SerializeField] private Ease showEase = Ease.OutBack;
    [Tooltip("Hide animation duration"), SerializeField] private float hideDuration = 0.25f;
    [Tooltip("Hide animation ease type"), SerializeField] private Ease hideEase = Ease.OutSine;
    [Tooltip("For when the interaction icon is moving to a different point while visible."), SerializeField] private float moveSpeed = 10f;

    private Transform _targetTransfrom = null;
    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _position = Vector3.zero;
    private bool _active = false;


    /// <summary>
    /// Shows the icon at the worldPosition
    /// </summary>
    public void Show(Vector3 worldPosition, float scale = 1)
    {
        if (_active && worldPosition != _targetPosition) return;

        _active = true;
        MoveTo(worldPosition);
        AnimateShow(scale);
        if (!IsVisible()) _position = _targetPosition;
    }


    /// <summary>
    /// Shows the icon at the Transform's position
    /// </summary>
    public void Show(Transform transform, float scale = 1)
    {
        if (_active && transform != _targetTransfrom) return;

        _active = true;
        MoveTo(transform);
        AnimateShow(scale);
        if (!IsVisible()) _position = _targetPosition;
    }


    /// <summary>
    /// Hide the icon
    /// </summary>
    public void Hide()
    {
        if (!_active) return;

        _active = false;
        transform.DOKill();
        transform.DOScale(0, hideDuration).SetEase(hideEase);
    }


    private void AnimateShow(float scale)
    {
        transform.DOKill();
        transform.DOScale(scale, showDuration).SetEase(showEase);
    }


    private void MoveTo(Vector3 worldPosition)
    {
        _targetTransfrom = null;
        _targetPosition = worldPosition;
    }

    private void MoveTo(Transform transform)
    {
        _targetTransfrom = transform;
        _targetPosition = _targetTransfrom.position;
    }


    private void Animate()
    {
        // Rotate the icon
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Bob the icon up and down
        transform.position = _targetPosition + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobIntensity;
    }

    private void UpdatePosition()
    {
        // Update from Target Transform
        if (_targetTransfrom != null) _targetPosition = _targetTransfrom.position;

        // Update position
        _position = Vector3.Lerp(_position, _targetPosition, Time.deltaTime * moveSpeed);
    }


    private bool IsVisible()
    {
        return transform.localScale.x > 0.1f;
    }


    void Start()
    {

    }


    void Update()
    {
        if (!IsVisible()) return;

        Animate();
        UpdatePosition();

        // Update the target position if we have a target transform
        if (_targetTransfrom != null) _targetPosition = _targetTransfrom.position;
    }
}
