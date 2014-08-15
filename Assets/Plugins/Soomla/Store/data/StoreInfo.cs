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
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace Soomla.Store
{
	/// <summary>
	/// This class holds the store's meta data including:
	/// virtual currencies definitions, 
	/// virtual currency packs definitions, 
	/// virtual goods definitions, 
	/// virtual categories definitions, and 
	/// virtual non-consumable items definitions
	/// </summary>
	public class StoreInfo
	{

		protected const string TAG = "SOOMLA StoreInfo"; // used for Log error messages

		static StoreInfo _instance = null;
		static StoreInfo instance {
			get {
				if(_instance == null) {
					#if UNITY_ANDROID && !UNITY_EDITOR
					_instance = new StoreInfoAndroid();
					#elif UNITY_IOS && !UNITY_EDITOR
					_instance = new StoreInfoIOS();
					#else
					_instance = new StoreInfo();
					#endif
				}
				return _instance;
			}
		}
			
		/// <summary>
		/// Initializes <c>StoreInfo</c>. 
		/// On first initialization, when the database doesn't have any previous version of the store
		/// metadata, <c>StoreInfo</c> gets loaded from the given <c>IStoreAssets</c>.
		/// After the first initialization, <c>StoreInfo</c> will be initialized from the database.
	    /// 
	    /// IMPORTANT: If you want to override the current <c>StoreInfo</c>, you'll have to bump
		/// the version of your implementation of <c>IStoreAssets</c> in order to remove the
		/// metadata when the application loads. Bumping the version is done by returning a higher 
		/// number in <c>IStoreAssets</c>'s <c>getVersion</c>.
		/// </summary>
		/// <param name="storeAssets">your game's economy</param>
		public static void Initialize(IStoreAssets storeAssets) {
			Debug.Log ("Initializing");
//			SoomlaUtils.LogDebug(TAG, "Adding currency");
			JSONObject currencies = new JSONObject(JSONObject.Type.ARRAY);
			foreach(VirtualCurrency vi in storeAssets.GetCurrencies()) {
				currencies.Add(vi.toJSONObject());
			}
			
//			SoomlaUtils.LogDebug(TAG, "Adding packs");
			JSONObject packs = new JSONObject(JSONObject.Type.ARRAY);
			foreach(VirtualCurrencyPack vi in storeAssets.GetCurrencyPacks()) {
				packs.Add(vi.toJSONObject());
			}
			
//			SoomlaUtils.LogDebug(TAG, "Adding goods");
		    JSONObject suGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject ltGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject eqGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject upGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject paGoods = new JSONObject(JSONObject.Type.ARRAY);
		    foreach(VirtualGood g in storeAssets.GetGoods()){
		        if (g is SingleUseVG) {
		            suGoods.Add(g.toJSONObject());
		        } else if (g is EquippableVG) {
		            eqGoods.Add(g.toJSONObject());
			    } else if (g is UpgradeVG) {
   		            upGoods.Add(g.toJSONObject());
   		        } else if (g is LifetimeVG) {
		            ltGoods.Add(g.toJSONObject());
		        } else if (g is SingleUsePackVG) {
		            paGoods.Add(g.toJSONObject());
		        }
		    }
			JSONObject goods = new JSONObject(JSONObject.Type.OBJECT);
			goods.AddField(JSONConsts.STORE_GOODS_SU, suGoods);
			goods.AddField(JSONConsts.STORE_GOODS_LT, ltGoods);
			goods.AddField(JSONConsts.STORE_GOODS_EQ, eqGoods);
			goods.AddField(JSONConsts.STORE_GOODS_UP, upGoods);
			goods.AddField(JSONConsts.STORE_GOODS_PA, paGoods);
			
//			SoomlaUtils.LogDebug(TAG, "Adding categories");
			JSONObject categories = new JSONObject(JSONObject.Type.ARRAY);
			foreach(VirtualCategory vi in storeAssets.GetCategories()) {
				categories.Add(vi.toJSONObject());
			}
			
//			SoomlaUtils.LogDebug(TAG, "Adding nonConsumables");
			JSONObject nonConsumables = new JSONObject(JSONObject.Type.ARRAY);
			foreach(NonConsumableItem vi in storeAssets.GetNonConsumableItems()) {
				nonConsumables.Add(vi.toJSONObject());
			}
			
//			SoomlaUtils.LogDebug(TAG, "Preparing StoreAssets  JSONObject");
			JSONObject storeAssetsObj = new JSONObject(JSONObject.Type.OBJECT);
			storeAssetsObj.AddField(JSONConsts.STORE_CATEGORIES, categories);
			storeAssetsObj.AddField(JSONConsts.STORE_CURRENCIES, currencies);
			storeAssetsObj.AddField(JSONConsts.STORE_CURRENCYPACKS, packs);
			storeAssetsObj.AddField(JSONConsts.STORE_GOODS, goods);
			storeAssetsObj.AddField(JSONConsts.STORE_NONCONSUMABLES, nonConsumables);
			
			string storeAssetsJSON = storeAssetsObj.print();
			Debug.Log ("about to reference instance");
			instance._initialize(storeAssets.GetVersion(), storeAssetsJSON);
		}

		/// <summary>
		/// Gets the item with the given <c>itemId</c>.
		/// </summary>
		/// <param name="itemId">Item id.</param>
		/// <exception cref="VirtualItemNotFoundException">Exception is thrown if item is not found.</exception>
		/// <returns>Item with the given id.</returns>
		public static VirtualItem GetItemByItemId(string itemId) {
			SoomlaUtils.LogDebug(TAG, "Trying to fetch an item with itemId: " + itemId);
			return instance._getItemByItemId(itemId);
		}

		/// <summary>
		/// Gets the purchasable item with the given <c>productId</c>.
		/// </summary>
		/// <param name="productId">Product id.</param>
		/// <exception cref="VirtualItemNotFoundException">Exception is thrown if item is not found.</exception>
		/// <returns>Purchasable virtual item with the given id.</returns>
		public static PurchasableVirtualItem GetPurchasableItemWithProductId(string productId) {
			return instance._getPurchasableItemWithProductId(productId);
		}

		/// <summary>
		/// Gets the category that the virtual good with the given <c>goodItemId</c> belongs to.
		/// </summary>
		/// <param name="goodItemId">Item id.</param>
		/// <exception cref="VirtualItemNotFoundException">Exception is thrown if category is not found.</exception>
		/// <returns>Category that the item with given id belongs to.</returns>
		public static VirtualCategory GetCategoryForVirtualGood(string goodItemId) {
			return instance._getCategoryForVirtualGood(goodItemId);
		}

		/// <summary>
		/// Gets the first upgrade for virtual good with the given <c>goodItemId</c>.
		/// </summary>
		/// <param name="goodItemId">Item id.</param>
		/// <returns>The first upgrade for virtual good with the given id.</returns>
		public static UpgradeVG GetFirstUpgradeForVirtualGood(string goodItemId) {
			return instance._getFirstUpgradeForVirtualGood(goodItemId);
		}

		/// <summary>
		/// Gets the last upgrade for the virtual good with the given <c>goodItemId</c>.
		/// </summary>
		/// <param name="goodItemId">item id</param>
		/// <returns>last upgrade for virtual good with the given id</returns>
		public static UpgradeVG GetLastUpgradeForVirtualGood(string goodItemId) {
			return instance._getLastUpgradeForVirtualGood(goodItemId);
		}

		/// <summary>
		/// Gets all the upgrades for the virtual good with the given <c>goodItemId</c>.
		/// </summary>
		/// <param name="goodItemId">Item id.</param>
		/// <returns>All upgrades for virtual good with the given id.</returns>
		public static List<UpgradeVG> GetUpgradesForVirtualGood(string goodItemId) {
			SoomlaUtils.LogDebug(TAG, "Trying to fetch upgrades for " + goodItemId);
			return instance._getUpgradesForVirtualGood(goodItemId);
		}

		/// <summary>
		/// Fetches the virtual currencies of your game.
		/// </summary>
		/// <returns>The virtual currencies.</returns>
		public static List<VirtualCurrency> GetVirtualCurrencies() {
			SoomlaUtils.LogDebug(TAG, "Trying to fetch currencies");
			return instance._getVirtualCurrencies();
		}

		/// <summary>
		/// Fetches the virtual goods of your game.
		/// </summary>
		/// <returns>All virtual goods.</returns>
		public static List<VirtualGood> GetVirtualGoods() {
			SoomlaUtils.LogDebug(TAG, "Trying to fetch goods");
			return instance._getVirtualGoods();
		}

		/// <summary>
		/// Fetches the virtual currency packs of your game.
		/// </summary>
		/// <returns>All virtual currency packs.</returns>
		public static List<VirtualCurrencyPack> GetVirtualCurrencyPacks() {
			SoomlaUtils.LogDebug(TAG, "Trying to fetch packs");
			return instance._getVirtualCurrencyPacks();
		}

		/// <summary>
		/// Fetches the non consumable items of your game.
		/// </summary>
		/// <returns>All non consumable items.</returns>
		public static List<NonConsumableItem> GetNonConsumableItems() {
			SoomlaUtils.LogDebug(TAG, "Trying to fetch noncons");
			return instance._getNonConsumableItems();
		}

		/// <summary>
		/// Fetches the virtual categories of your game.
		/// </summary>
		/// <returns>All virtual categories.</returns>
		public static List<VirtualCategory> GetVirtualCategories() {
			SoomlaUtils.LogDebug(TAG, "Trying to fetch categories");
			return instance._getVirtualCategories();
		}

		virtual protected void _initialize(int version, string storeAssetsJSON) {
		}

		virtual protected VirtualItem _getItemByItemId(string itemId) {
			return null;
		}

		virtual protected PurchasableVirtualItem _getPurchasableItemWithProductId(string productId) {
			return null;
		}

		virtual protected VirtualCategory _getCategoryForVirtualGood(string goodItemId) {
			return null;
		}

		virtual protected UpgradeVG _getFirstUpgradeForVirtualGood(string goodItemId) {
			return null;
		}

		virtual protected UpgradeVG _getLastUpgradeForVirtualGood(string goodItemId) {
			return null;
		}

		virtual protected List<UpgradeVG> _getUpgradesForVirtualGood(string goodItemId) {
			return new List<UpgradeVG>();
		}

		virtual protected List<VirtualCurrency> _getVirtualCurrencies() {
			return new List<VirtualCurrency>();
		}

		virtual protected List<VirtualGood> _getVirtualGoods() {
			return new List<VirtualGood>();
		}

		virtual protected List<VirtualCurrencyPack> _getVirtualCurrencyPacks() {
			return new List<VirtualCurrencyPack>();
		}
		
		virtual protected List<NonConsumableItem> _getNonConsumableItems() {
			return new List<NonConsumableItem>();
		}

		virtual protected List<VirtualCategory> _getVirtualCategories() {
			return new List<VirtualCategory>();
		}
	}
}

