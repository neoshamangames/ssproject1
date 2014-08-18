using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Soomla.Store.Saplings
{
	public class IAPAssets : IStoreAssets
	{	
		public int GetVersion()
		{
			return 4;
		}
		
		public VirtualCurrency[] GetCurrencies() {
			return new VirtualCurrency[]{};
		}
		
		public VirtualCurrencyPack[] GetCurrencyPacks() {
			return new VirtualCurrencyPack[] {};
		}
		
		public VirtualCategory[] GetCategories() {
			return new VirtualCategory[]{POWERUPS};
		}
		
		public NonConsumableItem[] GetNonConsumableItems() {
			return new NonConsumableItem[]{};
		}
		
		public VirtualGood[] GetGoods()
		{
			return new VirtualGood[] {REVIVE_GOOD, REVIVE_PACK_3, REVIVE_PACK_5, REVIVE_PACK_10, REVIVE_PACK_15};
		}
		
		
		
		/* Virtual Goods */
		public static VirtualGood REVIVE_GOOD = new SingleUseVG(
			"Revive",													//name
			"Bring your plant back to life!",							//description
			Constants.REVIVE_ID,										//item ID
			new PurchaseWithMarket(Constants.REVIVE_ID, .99f)	//purchase type
		);
		
		public static VirtualGood REVIVE_PACK_3 = new SingleUsePackVG(
			Constants.REVIVE_ID,										//good item ID
			3,															//quantity
			"Revive 3 Pack",											//name
			"A pack of 3 revives",										//description
			Constants.REVIVE_3_PACK_ID,									//item ID
			new PurchaseWithMarket(Constants.REVIVE_3_PACK_ID, .99f)	//purchase type
		);
		
		public static VirtualGood REVIVE_PACK_5 = new SingleUsePackVG(
			Constants.REVIVE_ID,										//good item ID
			5,															//quantity
			"Revive 5 Pack",											//name
			"A pack of 5 revives",										//description
			Constants.REVIVE_5_PACK_ID,									//item ID
			new PurchaseWithMarket(Constants.REVIVE_5_PACK_ID, 1.45f)	//purchase type
		);
		
		public static VirtualGood REVIVE_PACK_10 = new SingleUsePackVG(
			Constants.REVIVE_ID,										//good item ID
			10,															//quantity
			"Revive 10 Pack",											//name
			"A pack of 10 revives",										//description
			Constants.REVIVE_10_PACK_ID,								//item ID
			new PurchaseWithMarket(Constants.REVIVE_10_PACK_ID, 2.40f)	//purchase type
			);	
			
		public static VirtualGood REVIVE_PACK_15 = new SingleUsePackVG(
			Constants.REVIVE_ID,										//good item ID
			15,															//quantity
			"Revive 15 Pack",											//name
			"A pack of 15 revives",										//description
			Constants.REVIVE_15_PACK_ID,								//item ID
			new PurchaseWithMarket(Constants.REVIVE_15_PACK_ID, 2.99f)	//purchase type
			);	

		/** Virtual Categories **/
		public static VirtualCategory POWERUPS = new VirtualCategory(
			"Powerups", new List<string>(new string[] { Constants.REVIVE_ID })
		);
			
	}

}
