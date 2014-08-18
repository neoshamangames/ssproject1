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
using System.Runtime.InteropServices;

namespace Soomla {
	
	public class RewardStorageIOS : RewardStorage {

#if UNITY_IOS && !UNITY_EDITOR

		[DllImport ("__Internal")]
		private static extern void rewardStorage_SetRewardStatus(string rewardJson,
		                                                         [MarshalAs(UnmanagedType.Bool)] bool give,
		                                                         [MarshalAs(UnmanagedType.Bool)] bool notify);
		[DllImport ("__Internal")]
		[return:MarshalAs(UnmanagedType.I1)]
		private static extern bool rewardStorage_IsRewardGiven(string rewardJson);
		[DllImport ("__Internal")]
		private static extern int rewardStorage_GetLastSeqIdxGiven(string rewardJson);
		[DllImport ("__Internal")]
		private static extern void rewardStorage_SetLastSeqIdxGiven(string rewardJson, int idx);

		/// <summary>
		/// Set the reward given status
		/// </summary>
		/// <param name="reward">to set status for</param>
		/// <param name="give">true to give, false to take</param>
		/// <param name="notify">should this post an event</param>
		/// <returns></returns>
		override protected void _setRewardStatus(Reward reward, bool give, bool notify) {
			string rewardJson = reward.toJSONObject().ToString();
			rewardStorage_SetRewardStatus(rewardJson, give, notify);
			
//			int err = rewardStorage_SetRewardStatus(rewardJson, give);
//			IOS_ErrorCodes.CheckAndThrowException(err);
		}

		/// <summary>
		/// Check the reward given status
		/// </summary>
		/// <param name="reward">to query</param>
		/// <returns>true if reward was given</returns>
		override protected bool _isRewardGiven(Reward reward) {
			string rewardJson = reward.toJSONObject().ToString();
			return rewardStorage_IsRewardGiven(rewardJson);
		}
		
		/// <summary>
		/// Get last id of given reward from a <c>SequenceReward</c>
		/// </summary>
		/// <param name="reward">to query</param>
		/// <returns>last index of sequence reward given</returns>
		override protected int _getLastSeqIdxGiven(SequenceReward seqReward) {
			string rewardJson = seqReward.toJSONObject().ToString();
			return rewardStorage_GetLastSeqIdxGiven(rewardJson);
		}
		
		/// <summary>
		/// Set last id of given reward from a <c>SequenceReward</c>
		/// </summary>
		/// <param name="reward">to set last id for</param>
		/// <param name="reward">the last id to to mark as given</param>
		override protected void _setLastSeqIdxGiven(SequenceReward seqReward, int idx) {
			string rewardJson = seqReward.toJSONObject().ToString();
			rewardStorage_SetLastSeqIdxGiven(rewardJson, idx);
		}		
#endif
	}
}