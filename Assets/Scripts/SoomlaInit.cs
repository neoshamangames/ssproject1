using UnityEngine;
using System.Collections;

namespace Soomla.Store.Saplings
{
	public class SoomlaInit : SingletonMonoBehaviour<SoomlaInit> {
	
		void Start () {
			SoomlaStore.Initialize(new IAPAssets());
		}
	}
}