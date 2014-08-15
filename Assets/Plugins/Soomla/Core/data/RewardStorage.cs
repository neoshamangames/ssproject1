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

namespace Soomla
{
	public class RewardStorage
	{

		protected const string TAG = "SOOMLA RewardStorage"; // used for Log error messages

		static RewardStorage _instance = null;
		static RewardStorage instance {
			get {
				if(_instance == null) {
					#if UNITY_ANDROID && !UNITY_EDITOR
					_instance = new RewardStorageAndroid();
					#elif UNITY_IOS && !UNITY_EDITOR
					_instance = new RewardStorageIOS();
					#else
					_instance = new RewardStorage();
					#endif
				}
				return _instance;
			}
		}
			

		public static void SetRewardStatus(Reward reward, bool give) {
			SetRewardStatus(reward, give, true);
		}

		public static void SetRewardStatus(Reward reward, bool give, bool notify) {
			instance._setRewardStatus(reward, give, notify);
		}

		public static bool IsRewardGiven(Reward reward) {
			return instance._isRewardGiven(reward);
		}

		/// <summary>
		/// Retrieves the index of the last reward given in a sequence of rewards.
		/// </summary>
		public static int GetLastSeqIdxGiven(SequenceReward reward) {
			return instance._getLastSeqIdxGiven(reward);
		}

		public static void SetLastSeqIdxGiven(SequenceReward reward, int idx) {
			instance._setLastSeqIdxGiven(reward, idx);
		}

		
		virtual protected void _setRewardStatus(Reward reward, bool give, bool notify) {
#if UNITY_EDITOR
			string key = keyRewardGiven(reward.ID);
			
			if (give) {
				PlayerPrefs.SetString(key, "yes");
				
				if (notify) {
					CoreEvents.OnRewardGiven(reward);
				}
			} else {
				PlayerPrefs.DeleteKey(key);
				if (notify) {
					CoreEvents.OnRewardTaken(reward);
				}
			}
#endif
		}

		virtual protected bool _isRewardGiven(Reward reward) {
#if UNITY_EDITOR
			string key = keyRewardGiven(reward.ID);
			string val = PlayerPrefs.GetString (key);
			return !string.IsNullOrEmpty(val);
#else
			return false;
#endif
		}

		virtual protected int _getLastSeqIdxGiven(SequenceReward seqReward) {
#if UNITY_EDITOR
			string key = keyRewardIdxSeqGiven(seqReward.ID);
			string val = PlayerPrefs.GetString (key);
			if (string.IsNullOrEmpty(val)) {
				return -1;
			}
			return int.Parse(val);
#else
			return 0;
#endif
		}

		virtual protected void _setLastSeqIdxGiven(SequenceReward seqReward, int idx) {
#if UNITY_EDITOR
			string key = keyRewardIdxSeqGiven(seqReward.ID);
			PlayerPrefs.SetString (key, idx.ToString());
#endif
		}



		/** keys **/
#if UNITY_EDITOR
		private static string keyRewards(string rewardId, string postfix) {
			return CoreSettings.DB_KEY_PREFIX + "rewards." + rewardId + "." + postfix;
		}
		
		private static string keyRewardGiven(string rewardId) {
			return keyRewards(rewardId, "given");
		}
		
		private static string keyRewardIdxSeqGiven(string rewardId) {
			return keyRewards(rewardId, "seq.idx");
		}
#endif
	}
}

