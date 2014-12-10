/*Sean Maltz 2014*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Soomla.Store.Saplings
{
	public class IAPAssets : IStoreAssets
	{	
		public int GetVersion()
		{
			return 5;
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
			return new VirtualGood[] {REVIVE_GOOD, REVIVE_PACK_3, REVIVE_PACK_7, REVIVE_PACK_20, REVIVE_PACK_150};
		}
		
		
		
		/* Virtual Goods */
		public static VirtualGood REVIVE_GOOD = new SingleUseVG(
			"Revive",													//name
			"Bring your plant back to life!",							//description
			Constants.REVIVE_ID,										//item ID
			new PurchaseWithMarket(Constants.REVIVE_ID, .99f)			//purchase type
		);
		
		public static VirtualGood REVIVE_PACK_3 = new SingleUsePackVG(
			Constants.REVIVE_ID,										//good item ID
			3,															//quantity
			"Revive 3 Pack",											//name
			"A pack of 3 revives",										//description
			Constants.REVIVE_3_PACK_ID,									//item ID
			new PurchaseWithMarket(Constants.REVIVE_3_PACK_ID, .99f)	//purchase type
		);
		
		public static VirtualGood REVIVE_PACK_7 = new SingleUsePackVG(
			Constants.REVIVE_ID,										//good item ID
			7,															//quantity
			"Revive 7 Pack",											//name
			"A pack of 7 revives",										//description
			Constants.REVIVE_7_PACK_ID,									//item ID
			new PurchaseWithMarket(Constants.REVIVE_7_PACK_ID, 1.99f)	//purchase type
		);
		
		public static VirtualGood REVIVE_PACK_20 = new SingleUsePackVG(
			Constants.REVIVE_ID,										//good item ID
			20,															//quantity
			"Revive 20 Pack",											//name
			"A pack of 20 revives",										//description
			Constants.REVIVE_20_PACK_ID,								//item ID
			new PurchaseWithMarket(Constants.REVIVE_20_PACK_ID, 4.99f)	//purchase type
			);	
			
		public static VirtualGood REVIVE_PACK_150 = new SingleUsePackVG(
			Constants.REVIVE_ID,											//good item ID
			150,															//quantity
			"Revive 150 Pack",												//name
			"A pack of 150 revives",										//description
			Constants.REVIVE_150_PACK_ID,									//item ID
			new PurchaseWithMarket(Constants.REVIVE_150_PACK_ID, 29.99f)	//purchase type
			);

		/** Virtual Categories **/
		public static VirtualCategory POWERUPS = new VirtualCategory(
			"Powerups", new List<string>(new string[] { Constants.REVIVE_ID })
		);
			
	}

}
