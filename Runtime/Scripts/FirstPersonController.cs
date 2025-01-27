using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ImprovedFirstPersonController { 

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("References")]
        // This is the child object that will be used to deform when the player is crouching
        [SerializeField] private Transform ChildCapsule;
        // Cinemachine camera follows this object
        public GameObject CameraTarget;

        [Header("Player Movement")]
        public float MoveSpeed = 6.0f;
        public float SprintSpeed = 12.0f;
        public float CrouchSpeed = 3.0f;
        public float SlideSpeed = 3.0f;
        public float SlideAcceleration = 7.0f;
        public float SlideRecoveryTime = 0.3f;
        public float SpeedChangeRate = 20.0f;
        public float JumpHeight = 1.2f;
        public float JumpCooldown = 0.03f;
        public float Gravity = -9.81f;

        [Header("Player Grounded")]
        public bool isGrounded = true;
        public float GroundedCheckOffset = -0.75f;
        public float GroundedCheckRadius = 0.5f;
        public float MinAngleToSlide = 20f;
        public float GroundAngleDetectionDistance = 1.5f;
        public LayerMask GroundLayers;

        [Header("Interaction")]
        public float InteractionRange = 5.0f;

        [Header("Camera Settings")]
        public float CameraSensitivity = 1.0f;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;
        public float CrouchedOffset = 0.5f;

        private CharacterController _controller;
        private GetPlayerInputs _input;
        private GameObject _mainCamera;

        private Vector3 _hitNormal;
        private float _currentAngle;
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _slideVelocity;
        private readonly float _maxVerticalVelocity = 50.0f;
        private float _jumpCooldownTimer;
        private float _slideRecoveryTimer;
        private Vector3 _lastSlideDirection;
        private float _originalControllerHeight;
        private Vector3 _originalControllerCenter;
        private float _originalChildCapsuleHeight;
        private Vector3 _originalCameraTargetPosition;

        private void Awake() {
            if (_mainCamera == null) {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start() {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<GetPlayerInputs>();
            GroundedCheckRadius = _controller.radius;
            _jumpCooldownTimer = JumpCooldown;
            _slideRecoveryTimer = SlideRecoveryTime;
            _originalControllerHeight = _controller.height;
            _originalChildCapsuleHeight = ChildCapsule.localScale.y;
            _originalControllerCenter = _controller.center;
            _originalCameraTargetPosition = CameraTarget.transform.localPosition;
        }

        private void Update() {
            JumpAndGravity();
            GroundedCheck();
            Crouch();
            Move();
            Attack();
            InteractAction();
        }

        private void LateUpdate() {
            CameraRotation();
        }

        private void JumpAndGravity() {
            if (isGrounded) {

                //Stay grounded to the floor with a small force
                if (_verticalVelocity < 0.0f) {
                    _verticalVelocity = -2f;
                }

                //Jump
                if (_input.jump && _jumpCooldownTimer <= 0.0f) {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                if (_jumpCooldownTimer >= 0.0f) {
                    _jumpCooldownTimer -= Time.deltaTime;
                }

            } else {
                _jumpCooldownTimer = JumpCooldown;
                _input.jump = false;
            }

            //Apply gravity
            if (_verticalVelocity < _maxVerticalVelocity) {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void Crouch() {
            //Sprint cancels crouch
            if (_input.sprint) {
                _input.crouch = false;
            }

            //Handling the character controller
            float targetHeight = _input.crouch ? _originalControllerHeight / 2 : _originalControllerHeight;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, Time.deltaTime * 10.0f);
            float heightDifference = _originalControllerHeight - _controller.height;
            _controller.center = _originalControllerCenter - new Vector3(0, heightDifference / 2, 0);

            //Handling the capsule child
            float targetChildHeight = _input.crouch ? _originalChildCapsuleHeight / 2 : _originalChildCapsuleHeight;
            ChildCapsule.localScale = new Vector3(ChildCapsule.localScale.x, Mathf.Lerp(ChildCapsule.localScale.y, targetChildHeight, Time.deltaTime * 10.0f), ChildCapsule.localScale.z);
            ChildCapsule.localPosition = _controller.center;

            //Handling the camera target
            float cameraTargetHeight = _input.crouch ? _originalCameraTargetPosition.y - CrouchedOffset : _originalCameraTargetPosition.y;
            CameraTarget.transform.localPosition = Vector3.Lerp(CameraTarget.transform.localPosition, new Vector3(CameraTarget.transform.localPosition.x, cameraTargetHeight, CameraTarget.transform.localPosition.z), Time.deltaTime * 10.0f);
        }

        private void Move() {
            if (_currentAngle > MinAngleToSlide) {
                SlideDownSlope();
                _slideRecoveryTimer = SlideRecoveryTime;
            } else if (_slideRecoveryTimer > 0) { //Keep sliding for a short time after leaving the slope
                _slideRecoveryTimer -= Time.deltaTime;
                SlideAfterLanding();
            } else {
                NormalMovement();
            }
        }

        private void SlideDownSlope() {
            Vector3 slideDirection = new(_hitNormal.x, -_hitNormal.y, _hitNormal.z);
            _lastSlideDirection = slideDirection;

            //Accelerate while sliding
            if (_slideVelocity < _maxVerticalVelocity) {
                _slideVelocity += SlideAcceleration * Time.deltaTime;
            }
            _controller.Move(slideDirection * (_slideVelocity * Time.deltaTime) + _verticalVelocity * Time.deltaTime * Vector3.up);
        }

        private void SlideAfterLanding() {
            Vector3 slideDirection = _lastSlideDirection;
            _controller.Move(slideDirection * (SlideSpeed / 2 * Time.deltaTime) + _verticalVelocity * Time.deltaTime * Vector3.up);
        }

        private void NormalMovement() {
            //Reset slide velocity
            _slideVelocity = SlideSpeed;

            //Movement
            float targetSpeed = _input.crouch ? CrouchSpeed : MoveSpeed;
            targetSpeed = _input.sprint ? SprintSpeed : targetSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            if (currentHorizontalSpeed < targetSpeed || currentHorizontalSpeed > targetSpeed) {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            } else {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            //Rotation
            if (_input.move != Vector2.zero) {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            //Apply movement
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + _verticalVelocity * Time.deltaTime * Vector3.up);
        }

        private void Attack() {
            if (_input.attack) {
                if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, 10f)) {
                    Debug.Log($"Attacked: {hit.transform.name}");
                    Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * 10f, Color.red, 10f);
                }
                _input.attack = false;
            }
        }

        private void InteractAction() {
            if(_input.interact) {
                if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, InteractionRange)) {
                    Debug.Log($"Interacted with: {hit.transform.name}");
                    Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * InteractionRange, Color.red, 10f);
                }
                _input.interact = false;
            }
        }

        private void GroundedCheck() {
            Vector3 spherePosition = transform.position + Vector3.up * GroundedCheckOffset;
            isGrounded = Physics.CheckSphere(spherePosition, GroundedCheckRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (isGrounded) {
                //Detect ground angle
                _hitNormal = Physics.Raycast(spherePosition, Vector3.down, out RaycastHit hit, GroundAngleDetectionDistance, GroundLayers) ? hit.normal : Vector3.up;
                _currentAngle = Vector3.Angle(Vector3.up, _hitNormal);

                //Final isGrounded check
                isGrounded = _currentAngle <= MinAngleToSlide;

                //Cancels the jump if the angle is too steep
                if (_currentAngle > MinAngleToSlide) {
                    _verticalVelocity = -2f;
                }
            }
        }

        private void CameraRotation() {
            if (_input.look.sqrMagnitude > 0) {
                //Get rotation
                _cinemachineTargetPitch += _input.look.y * CameraSensitivity;
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
                _rotationVelocity = _input.look.x * CameraSensitivity;

                //Apply rotation
                CameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected() {
            Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

            if (isGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Vector3 spherePosition = transform.position + Vector3.up * GroundedCheckOffset;
            Gizmos.DrawSphere(spherePosition, GroundedCheckRadius);
        }
    }
}
