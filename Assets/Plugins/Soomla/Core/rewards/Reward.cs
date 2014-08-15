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
using System.Collections;
using System;


namespace Soomla {	
	
	/// <summary>
	/// A reward is an entity which can be earned by the user for meeting certain
	/// criteria in game progress.  For example - a user can earn a badge for completing
	/// a mission. Dealing with <code>Reward</code>s is very similar to dealing with
	/// <code>VirtualItem</code>s: grant a reward by giving it and recall a
	/// reward by taking it.
	/// 
	/// In the Profile module, rewards can be attached to various actions. For example:
	/// You can give your user 100 coins for logging in through Facebook.
	/// </summary>
	public abstract class Reward : SoomlaEntity {
		private static string TAG = "SOOMLA Reward";

		public bool   Repeatable;
		
		public bool Owned {
			get {
				return RewardStorage.IsRewardGiven(this);
			}
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="id">the reward's ID (unique id to distinguish between rewards).</param>
		/// <param name="name">the reward's name.</param>
		public Reward(string id, string name)
			: base(id, name, "")
		{
			Repeatable = false;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="jsonReward">A JSONObject representation of <c>Reward</c>.</param>
		public Reward(JSONObject jsonReward) :
			base(jsonReward)
		{
			JSONObject repeatObj = jsonReward[JSONConsts.SOOM_REWARD_REPEAT];
			if (repeatObj) {
				Repeatable = repeatObj.b;
			} else {
				Repeatable = false;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <returns>A JSONObject representation of <c>Reward</c>.</return>
		public override JSONObject toJSONObject() {
			JSONObject obj = base.toJSONObject();
			obj.AddField(JSONConsts.SOOM_REWARD_REPEAT, Repeatable);
			obj.AddField(JSONConsts.SOOM_CLASSNAME, GetType().Name);
			
			return obj;
		}

		/// <summary>
		/// Factory method to easily create a reward according to the given JSONObject.
		/// </summary>
		/// <returns>A JSONObject representation of <c>Reward</c>.</returns>
		/// <param name="rewardObj">The actual reward according to the given JSONObject.</param>
		public static Reward fromJSONObject(JSONObject rewardObj) {
			string className = rewardObj[JSONConsts.SOOM_CLASSNAME].str;

			Reward reward = (Reward) Activator.CreateInstance(Type.GetType("Soomla." + className), new object[] { rewardObj });

			return reward;
		}

#if UNITY_ANDROID 
		//&& !UNITY_EDITOR
		public AndroidJavaObject toJNIObject() {
			using(AndroidJavaClass jniRewardClass = new AndroidJavaClass("com.soomla.rewards.Reward")) {
				return jniRewardClass.CallStatic<AndroidJavaObject>("fromJSONString", toJSONObject().print());
			}
		}
#endif

		public bool Take() {

			if (!RewardStorage.IsRewardGiven(this)) {
				SoomlaUtils.LogDebug(TAG, "Reward not given. id: " + ID);
				return false;
			}
			
			if (takeInner()) {
				RewardStorage.SetRewardStatus(this, false);
				return true;
			}
			
			return false;
		}

		public bool Give() {
			if (RewardStorage.IsRewardGiven(this) && !Repeatable) {
				SoomlaUtils.LogDebug(TAG, "Reward was already given and is not repeatable. id: " + ID);
				return false;
			}
			
			if (giveInner()) {
				RewardStorage.SetRewardStatus(this, true);
				return true;
			}
			
			return false;
		}

		protected abstract bool giveInner();

		protected abstract bool takeInner();

	}
}
