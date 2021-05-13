using UnityEngine;
using System;
using System.Collections.Generic;

namespace Pixelfat.Unity
{
    public class ViewState : Singleton<ViewState>
    {

        #region Events

        public delegate void ViewStateChangeEventDelegate(ViewState s);
        public static event ViewStateChangeEventDelegate ViewStateChangeEvent;

        #endregion
		
        public static ViewState Current { get { return mCurrentState; } }

		public static int Layer { get { return mScreenSpaceLayer; } set { mScreenSpaceLayer = value; SetLayer (Instance.gameObject.transform, value); } }

        public static Canvas ScreenSpaceCanvas { get { return sScreenSpaceCanvas; } }

        protected static ViewState mCurrentState;
        protected static Type prev_stateType;
        protected static Type next_stateType;
        protected bool isInstanceDone = false;
        protected bool Error = false;

        private static readonly Dictionary<ViewPanel, Canvas> tempPanels = new Dictionary<ViewPanel, Canvas>();
        private static readonly Dictionary<ViewPanel, Canvas> persistentPanels = new Dictionary<ViewPanel, Canvas>();

        private static int mScreenSpaceLayer = 5; // render layer for the screen-space UI element

		private const int
            Width = 1920,
            Height = 1080;

        private static Canvas sScreenSpaceCanvas;

        private bool initd = false;

        public static T Set<T>() where T : ViewState
        {

            if (!typeof(T).IsSubclassOf(typeof(ViewState)))
            {

                Console.WriteLine("Error: Not a viewstate! " + typeof(T));
                return null;

            }

            next_stateType = typeof(T);

            if (mCurrentState == null)
                mCurrentState = ViewState.Instance;

            mCurrentState.Next();

            return (T)mCurrentState;

        }

        protected void Next()
        {
            if (next_stateType != null)
            {

                if (next_stateType == this.GetType())
                {

                    Debug.Log("ERROR! State is already: " + this.GetType().ToString());
                    return;
                }

               // try
               // {
                    ViewState _newState = (ViewState)gameObject.AddComponent(next_stateType);

                    _newState.name = $"[View State]: { GetType().ToString().Substring(GetType().ToString().LastIndexOf('.') + 1) } > { next_stateType.ToString().Substring(next_stateType.ToString().LastIndexOf('.') + 1)}";
                    
                    next_stateType = _newState.GetType();

                    prev_stateType = this.GetType();

                    mCurrentState = _newState;

                    ViewStateChangeEvent?.Invoke(_newState);

                    Debug.Log("[View State]: ...changed view state from " + prev_stateType.ToString() + " to " + mCurrentState.ToString());

                    Dispose();

                    mCurrentState.Init();

               // }
                //catch(Exception e)
                //{
                //    Debug.LogError("[View State]: ERROR: Cannot use state: " + next_stateType.ToString() + "\nAre you sure it inherits from State and all events are decoupled? \n" + e.Message);
                //}

            }
            else
            {
                Error = true;
                Debug.LogError("[View State]: " + name + " The next state has not been defined!");
            }

        }

        public virtual void Dispose()
        {

            Clear();

            Destroy(this); // destroys just the component to be replaced, not the gameobject

        }

        protected T Add<T>(string prefabPath, bool persistent) where T : ViewPanel {

            return Add<T>(prefabPath, persistent, sScreenSpaceCanvas);

		}

        protected T Add<T>(string prefabPath, bool persistent, Canvas canvas) where T : ViewPanel
        {

            T _newVpResource = Resources.Load<T>(prefabPath);

            if (_newVpResource == null)
            {

                Debug.LogError("Not a ViewPanel prefab?!: " + prefabPath);
                return null;

            }

            GameObject _panelGo = Instantiate(_newVpResource).gameObject;
            _panelGo.transform.SetParent(canvas.transform);
            _panelGo.transform.localScale = Vector3.one;
            _panelGo.transform.localPosition = Vector3.zero;
            _panelGo.GetComponent<RectTransform>().sizeDelta = Vector3.zero;

            if (persistent)
                persistentPanels.Add(_panelGo.GetComponent<ViewPanel>(), canvas);
            else
                tempPanels.Add(_panelGo.GetComponent<ViewPanel>(), canvas);

            SetLayer(_panelGo.transform, canvas.gameObject.layer);

            return _panelGo.GetComponent<ViewPanel>() as T;

        }

		protected bool Remove(ViewPanel panel) {
			
			if (tempPanels.ContainsKey(panel)) {
		
			    tempPanels.Remove (panel);
			    Destroy (panel.gameObject);

			    return true;

            }
            else if (persistentPanels.ContainsKey(panel))
            {

                persistentPanels.Remove(panel);
                Destroy(panel.gameObject);

                return true;
            }
                
            Debug.LogError("Not a panel?");
                
            return false;

		}
		
		protected static void Clear() {

			foreach (ViewPanel _pv in tempPanels.Keys)
				Destroy(_pv.gameObject);

			tempPanels.Clear ();
			
		}

		private static void SetLayer(Transform t, int l) {

			t.gameObject.layer = l;

			foreach(Transform _t in t)
				SetLayer(_t, l);

		}

       
       protected override void Init()
       {
           

           base.Init();

           if (initd)
               return;

           initd = true;

           if (gameObject.GetComponent<UnityEngine.EventSystems.EventSystem>() == null)
               gameObject.AddComponent<UnityEngine.EventSystems.EventSystem>();

           if (gameObject.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>() == null)
             gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

			if(sScreenSpaceCanvas==null)
				CreateScreenSpaceCanvas();

		}

		private void CreateScreenSpaceCanvas() {
			
			if(sScreenSpaceCanvas!=null)
				return;

            sScreenSpaceCanvas = gameObject.AddComponent<Canvas>();

            sScreenSpaceCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            sScreenSpaceCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            sScreenSpaceCanvas.overrideSorting = true;
            sScreenSpaceCanvas.sortingOrder = 10;
            sScreenSpaceCanvas.pixelPerfect = false;
            
            UnityEngine.UI.CanvasScaler _canvasScaler = sScreenSpaceCanvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();

			_canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
			_canvasScaler.referenceResolution = new Vector2 (Width, Height);
            _canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.matchWidthOrHeight = 0.5f;

            SetLayer(sScreenSpaceCanvas.transform, mScreenSpaceLayer);

		}

		public virtual void UpdateState() {}

	}
	
}