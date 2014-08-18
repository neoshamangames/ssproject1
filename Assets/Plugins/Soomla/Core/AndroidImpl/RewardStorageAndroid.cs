/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

using UnityEngine;
using System;

namespace Soomla {
	
	public class RewardStorageAndroid : RewardStorage {

#if UNITY_ANDROID && !UNITY_EDITOR
		
		override protected void _setRewardStatus(Reward reward, bool give, bool notify) {
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniRewardStorage = new AndroidJavaClass("com.soomla.data.RewardStorage")) {
				jniRewardStorage.CallStatic("setRewardStatus", reward.toJNIObject(), give, notify);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		
		override protected bool _isRewardGiven(Reward reward) {
			bool given = false;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniRewardStorage = new AndroidJavaClass("com.soomla.data.RewardStorage")) {
				given = jniRewardStorage.CallStatic<bool>("isRewardGiven", reward.toJNIObject());
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return given;
		}
		
		override protected int _getLastSeqIdxGiven(SequenceReward reward) {
			int idx = -1;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniRewardStorage = new AndroidJavaClass("com.soomla.data.RewardStorage")) {
				idx = jniRewardStorage.CallStatic<int>("getLastSeqIdxGiven", reward.toJNIObject());
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return idx;
		}
		
		override protected void _setLastSeqIdxGiven(SequenceReward reward, int idx) {
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniRewardStorage = new AndroidJavaClass("com.soomla.data.RewardStorage")) {
				jniRewardStorage.CallStatic("setLastSeqIdxGiven", reward.toJNIObject(), idx);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

#endif
	}
}
