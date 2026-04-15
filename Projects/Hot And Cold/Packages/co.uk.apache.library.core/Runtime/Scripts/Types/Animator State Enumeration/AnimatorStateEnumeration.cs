using System;
using System.Linq;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public struct AnimatorStateEnumeration {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		// N.B. suppressing unaccessed/unset as they are accessed and modified using reflection.
#pragma warning disable 649

		[SerializeField] private string[] layerNames;

		[SerializeField] private string[] selectedLayerStateNames;

		[SerializeField] private int selectedLayerIndex;

		[SerializeField] private int selectedStateIndex;

#pragma warning restore 649

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private int _selectedHash;
		private string _selectedLayerName;
		private string _selectedStateName;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
		public string[] States => selectedLayerStateNames;

		public int Hash {
			get {
				if (_selectedHash == 0) {
					_selectedHash = GetHash();
				}
				return _selectedHash;
			}
		}

		public string LayerName {
			get {
				if (string.IsNullOrEmpty(_selectedLayerName)) {
					_selectedLayerName = layerNames.ElementAtOrDefault(selectedLayerIndex);
				}
				return _selectedLayerName;
			}
		}

		// ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
		public int LayerIndex => selectedLayerIndex;

		public string StateName {
			get {
				if (string.IsNullOrEmpty(_selectedStateName)) {
					_selectedStateName = selectedLayerStateNames.ElementAtOrDefault(selectedStateIndex);
				}
				return _selectedStateName;
			}
		}

		// ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
		public int StateIndex => selectedLayerIndex;

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private int GetHash() {
			return Animator.StringToHash($"{ LayerName }.{ StateName }");
		}
	}
}