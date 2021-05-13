using UnityEngine;
using System;

namespace Pixelfat.Unity
{

    /// <summary>
    /// Super simple state controller specifically for app / game state 
    /// view states control view resources, app states control data (including binary data for view)
    /// </summary>
    public class AppState : Singleton<AppState>
    {

        private static Type CurrentStateType { get { return current_StateInstance.GetType(); } }

        protected static AppState current_StateInstance;
        protected static Type prev_stateType;
        protected static Type next_stateType;

        protected bool isInstanceDone = false;
        protected bool Error = false;

        public static T SetAppState<T>() where T : AppState
        {

            if (!typeof(T).IsSubclassOf(typeof(AppState)))
            {

                Console.WriteLine("Error: Not a runstate! " + typeof(T));
                return null;

            }

            next_stateType = typeof(T);

            if (current_StateInstance == null)
                current_StateInstance = AppState.Instance;

            current_StateInstance.Next();

            return (T)current_StateInstance;

        }

        protected void Next()
        {

            if (next_stateType != null)
            {
                try
                {
                    AppState _newState = (AppState)gameObject.AddComponent(next_stateType);

                    Debug.Log($"Changing run state from {this.GetType()} to {next_stateType}.");


                    _newState.name = $"[Run State]: {GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1)} > { next_stateType.ToString().Substring(next_stateType.ToString().LastIndexOf('.') + 1)}...";

                    next_stateType = _newState.GetType();

                    prev_stateType = this.GetType();

                    current_StateInstance = _newState;

                    Debug.Log($"[Run State]: ...Changed run state from {prev_stateType} to {next_stateType}");

                    Dispose();

                    current_StateInstance.Init();

                }
                catch
                {
                    Console.WriteLine($"ERROR: Cannot use state: {next_stateType}\nAre you sure it inherits from State and all events are decoupled?");
                }

            }
            else
            {
                Error = true;
                Console.WriteLine("ERROR! The next state has not been defined!");
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public virtual void Dispose()
        {

            DestroyImmediate(this); // should destroy just the component, not the gameobject

        }

        protected override void Init()
        {

            base.Init();

        }

    }

}