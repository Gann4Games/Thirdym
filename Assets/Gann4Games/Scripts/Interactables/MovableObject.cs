using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gann4Games.Thirdym.Interactables
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class MovableObject : SwitchListener
    {
        [Serializable]
        public class KeyFrame
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public Ease easing;
            public float time;
            public UnityEvent onStart;

            public KeyFrame(Vector3 position, Vector3 rotation, Vector3 scale, Ease easing, float time) {
                this.position = position;
                this.rotation = rotation;
                this.scale = scale;
                this.easing = easing;
                this.time = time;
            }
        }

        [SerializeField] List<KeyFrame> keyframes;
        [SerializeField] AudioClip moveStartSFX, moveEndSFX;

        AudioSource _audioSource;
        Rigidbody _rigidbody;
        int _currentFrame;

        public bool IsMoving { get; private set; }

        KeyFrame CurrentFrame => keyframes[_currentFrame % keyframes.Count];

        private void Awake()
        {
            if (TryGetComponent(out Rigidbody rb)) _rigidbody = rb;
            _audioSource = GetComponent<AudioSource>();
        }
        public override void InteractableSwitch_Signal(object sender, EventArgs e) => Move();
        public void Move()
        {
            if (IsMoving) return;
            OnMoveStart();
            _rigidbody.DOMove(CurrentFrame.position, CurrentFrame.time).SetEase(CurrentFrame.easing).OnComplete(() => OnMoveEnd());
            _rigidbody.DORotate(CurrentFrame.rotation, CurrentFrame.time).SetEase(CurrentFrame.easing);
            transform.DOScale(CurrentFrame.scale, CurrentFrame.time).SetEase(CurrentFrame.easing);
        }

        void OnMoveStart()
        {
            SetMoveState(true);
            CurrentFrame.onStart.Invoke();
            _audioSource.Stop();
            _audioSource.PlayOneShot(moveStartSFX);
            _currentFrame++;
        }

        void OnMoveEnd()
        {
            SetMoveState(false);
            _audioSource.Stop();
            _audioSource.PlayOneShot(moveEndSFX);
        }

        void SetMoveState(bool value) => IsMoving = value;

        [ContextMenu("Generate keyframe")]
        public void GenerateKeyframe()
        {
            KeyFrame newKeyframe = new KeyFrame(transform.position, transform.eulerAngles, transform.localScale, Ease.Linear, 5);
            keyframes.Add(newKeyframe);
        }

        [ContextMenu("Reset transform")]
        public void ResetPosition()
        {
            KeyFrame firstFrame = keyframes[0];
            transform.position = firstFrame.position;
            transform.eulerAngles = firstFrame.rotation;
            transform.localScale = firstFrame.scale;
        }
    }
}