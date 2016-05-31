using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
    public class PinchEventData : BaseEventData
    {
        public List<PointerEventData> data;

        public PinchEventData(EventSystem ES) : base(ES)
        {
            data = new List<PointerEventData>();
        }
    }

    public interface IPinchHandler : IEventSystemHandler
    {
        void OnPinch(PinchEventData data);
    }


    public static class PinchModuleEvents
    {
        private static void Execute(IPinchHandler handler, BaseEventData eventData)
        {
            handler.OnPinch(ExecuteEvents.ValidateEventData<PinchEventData>(eventData));
        }

        public static ExecuteEvents.EventFunction<IPinchHandler> pinchHandler
        {
            get { return Execute; }
        }
    }


    public class PinchInputModule : PointerInputModule
    {
        private PinchEventData _pinchData = null;

        public override void Process()
        {
            if (!Input.touchSupported) {
                // 不支持触摸事件的情况
                if (Input.GetKey(KeyCode.LeftAlt)) {
                    //var pointerData = GetMousePointerEventData();

                    //var leftPressData = pointerData.GetButtonState(PointerEventData.InputButton.Left).eventData;
                    //var secondData = pointerData.GetButtonState(PointerEventData.InputButton.Left).eventData;
                    //secondData.buttonData.position = GetPinchTwist2Finger();

                    //_pinchData = new PinchEventData(eventSystem);
                    //_pinchData.data.Add(leftPressData);
                    //_pinchData.data.Add(secondData);

                    //eventSystem.RaycastAll(touchData0, m_RaycastResultCache);
                    //RaycastResult firstHit = FindFirstRaycast(m_RaycastResultCache);

                    //eventSystem.RaycastAll(touchData1, m_RaycastResultCache);

                    //if (FindFirstRaycast(m_RaycastResultCache).gameObject.Equals(firstHit.gameObject)) {
                    //    ExecuteEvents.Execute(firstHit.gameObject, _pinchData, PinchModuleEvents.pinchHandler);
                    //}

                    //_pinchData.data.Clear();
                }
            } else if (Input.touchCount == 2) {
                bool pressed, released;

                PointerEventData touchData0 = GetTouchPointerEventData(Input.GetTouch(0), out pressed, out released);
                PointerEventData touchData1 = GetTouchPointerEventData(Input.GetTouch(1), out pressed, out released);

                _pinchData = new PinchEventData(eventSystem);
                _pinchData.data.Add(touchData0);
                _pinchData.data.Add(touchData1);

                eventSystem.RaycastAll(touchData0, m_RaycastResultCache);
                RaycastResult firstHit = FindFirstRaycast(m_RaycastResultCache);

                eventSystem.RaycastAll(touchData1, m_RaycastResultCache);

                if (FindFirstRaycast(m_RaycastResultCache).gameObject.Equals(firstHit.gameObject)) {
                    ExecuteEvents.Execute(firstHit.gameObject, _pinchData, PinchModuleEvents.pinchHandler);
                }

                _pinchData.data.Clear();
            }
        }

        public override string ToString()
        {
            return string.Format("[PinchInputModule]");
        }

        private Vector2 GetPinchTwist2Finger(bool newSim = false)
        {

            Vector2 position;

            position.x = (Screen.width / 2.0f) - (Input.mousePosition.x - (Screen.width / 2.0f));
            position.y = (Screen.height / 2.0f) - (Input.mousePosition.y - (Screen.height / 2.0f));

            return position;
        }
    }
}
