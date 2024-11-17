using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Grocery : MonoBehaviour
{
    [SerializeField] private float _mouseDragAggression;

    private Rigidbody2D _rigidbody;
    private bool _dragging;
    private Vector3 _positionClicked;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _dragging = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_dragging) {
            var mousePosition = GetMousePosition();
            var delta =
                mousePosition - transform.TransformPoint(_positionClicked);
            _rigidbody.AddForceAtPosition(
                DeltaPositionToAcceleration(
                    new Vector2(delta.x, delta.y),
                    Time.deltaTime
                )*_mouseDragAggression,
                mousePosition
            );
        }
    }

    void OnMouseDown() {

        _dragging = true;
        _positionClicked = transform.InverseTransformPoint(GetMousePosition());
    }

    void OnMouseUp() {
        _dragging = false;
    }

    private Vector3 GetMousePosition() {
        var mousePosition = Vector2.zero;
        var mousePositionControl = Mouse.current.position;
        mousePosition.x = mousePositionControl.x.ReadValue();
        mousePosition.y = mousePositionControl.y.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return new Vector3(
            mousePosition.x,
            mousePosition.y,
            transform.position.z
        );
    }

    private Vector2 DeltaPositionToAcceleration(
        Vector2 delta, float deltaTime
    ) {
        // position = time^2*acceleration/2 + time*velocity
        // time^2*acceleration/2 = position - time*velocity
        // acceleration = 2*(position - time*velocity)/time^2
        return 2.0f*(
            delta - deltaTime*_rigidbody.velocity
        )/(deltaTime*deltaTime);
    }
}
