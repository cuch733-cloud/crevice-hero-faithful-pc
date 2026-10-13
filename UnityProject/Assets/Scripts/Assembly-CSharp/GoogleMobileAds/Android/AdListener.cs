using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	internal class AdListener : AndroidJavaProxy
	{
		private IAdListener listener;

		internal AdListener(IAdListener listener)
			: base("com.google.unity.ads.UnityAdListener")
		{
			this.listener = listener;
		}

		private void onAdLoaded()
		{
			listener.FireAdLoaded();
		}

		private void onAdFailedToLoad(string errorReason)
		{
			listener.FireAdFailedToLoad(errorReason);
		}

		private void onAdOpened()
		{
			listener.FireAdOpened();
		}

		private void onAdClosed()
		{
			listener.FireAdClosing();
			listener.FireAdClosed();
		}

		private void onAdLeftApplication()
		{
			listener.FireAdLeftApplication();
		}
	}
}
