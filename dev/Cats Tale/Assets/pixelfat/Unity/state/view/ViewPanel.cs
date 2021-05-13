using System;
using UnityEngine;

namespace Pixelfat.Unity
{

	public class ViewPanel : MonoBehaviour
	{
		
		public delegate void PanelViewControllerEvent (ViewPanel source);
		public event PanelViewControllerEvent OnReady;

		protected virtual void Start() {

            OnReady?.Invoke(this);

        }

	}

}

