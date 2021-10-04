using System.Collections;
using Misc;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class CameraController : MonoBehaviour
    {
        private readonly (Vector3 position, Vector3 rotation) _menuPoint = (
            new Vector3(3.25f, 2.75f, -2.18f),
            new Vector3(38.19f,324.52f,0f)
        );

        private readonly (Vector3 position, Vector3 rotation)[] _whitePlayerPoints = new[]
        {
            (new Vector3(3.26f, 4.45f, -0.78f), new Vector3(60.02f, 329f, 0f)),
            (new Vector3(1.72f, 4.87f, -0.34f), new Vector3(66.53f, 0f, 0f)),
            (new Vector3(0.2f, 4.45f, -0.78f), new Vector3(60.02f, -329f, 0f))
        };

        private readonly (Vector3 position, Vector3 rotation)[] _blackPlayerPoints = new[]
        {
            (new Vector3(0.2f, 4.45f, 4.21f), new Vector3(60.02f, 165f, 0f)),
            (new Vector3(1.72f, 4.87f, 3.8f), new Vector3(66.53f, 180f, 0f)),
            (new Vector3(3.26f, 4.45f, 4.21f), new Vector3(60.02f, -165f, 0f))
        };

        private int WhitePlayerCameraPoint
        {
            get => _whitePlayerCameraPoint;
            set => _whitePlayerCameraPoint = value % _whitePlayerPoints.Length;
        }
        private int _whitePlayerCameraPoint = 1;

        private int BlackPlayerCameraPoint
        {
            get => _blackPlayerCameraPoint;
            set => _blackPlayerCameraPoint = value % _blackPlayerPoints.Length;
        }
        private int _blackPlayerCameraPoint = 1;
        private EventDeliveryService _eventDeliveryService;
        private BoardService _boardService;

        [Inject]
        private void Construct(EventDeliveryService eventDeliveryService, BoardService boardService)
        {
            _eventDeliveryService = eventDeliveryService;
            _boardService = boardService;
            _eventDeliveryService.OnSwitchPlayerSide += SwitchPlayerCamera;
            _eventDeliveryService.OnUIGameStart += SetCameraGamePosition;
            _eventDeliveryService.OnUIRetryGame += SetCameraGamePosition;
            _eventDeliveryService.OnUICameraButtonClicked += NextPointCamera;
        }

        public void Awake()
        {
            transform.position = _menuPoint.position;
            transform.eulerAngles = _menuPoint.rotation;
        }

        private void NextPointCamera()
        {
            (Vector3 position, Vector3 rotation) point;
            if (_boardService.ActivePlayer == FigureColor.White)
            {
                ++WhitePlayerCameraPoint;
                point = _whitePlayerPoints[WhitePlayerCameraPoint];
            }
            else
            {
                ++BlackPlayerCameraPoint;
                point = _blackPlayerPoints[BlackPlayerCameraPoint];
            }

            StartCoroutine(ChangeCameraPointCoroutine(point));
        }

        private void SwitchPlayerCamera()
        {
            (Vector3 position, Vector3 rotation) point = _boardService.ActivePlayer == FigureColor.White
                ? _whitePlayerPoints[WhitePlayerCameraPoint]
                : _blackPlayerPoints[BlackPlayerCameraPoint];
            StartCoroutine(ChangeCameraPointCoroutine(point));
        }

        private IEnumerator ChangeCameraPointCoroutine((Vector3 position, Vector3 rotation) point)
        {
            _eventDeliveryService.UICameraButtonEnabled(false);
            float t = 0f;
            Vector3 prevPos = transform.position;
            Vector3 prevRot = transform.rotation.eulerAngles;
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(prevPos, point.position, t);
                transform.eulerAngles = new Vector3(
                    Mathf.LerpAngle(prevRot.x, point.rotation.x, t),   
                    Mathf.LerpAngle(prevRot.y, point.rotation.y, t),   
                    Mathf.LerpAngle(prevRot.z, point.rotation.z, t)    
                );
                t += Time.deltaTime;
                yield return null;
            }
            _eventDeliveryService.UICameraButtonEnabled(true);
        }

        private void SetCameraGamePosition(GameMode mode)
        {
            _whitePlayerCameraPoint = 1;
            _blackPlayerCameraPoint = 1;
            StartCoroutine(ChangeCameraPointCoroutine(_whitePlayerPoints[WhitePlayerCameraPoint]));
        }

        private void SetCameraGamePosition()
        {
            _whitePlayerCameraPoint = 1;
            _blackPlayerCameraPoint = 1;
            StartCoroutine(ChangeCameraPointCoroutine(_whitePlayerPoints[WhitePlayerCameraPoint]));
        }
    }
}