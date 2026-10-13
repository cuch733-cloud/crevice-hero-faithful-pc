using GoogleMobileAds.Android;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
	internal class GoogleMobileAdsClientFactory
	{
		internal static IGoogleMobileAdsBannerClient GetGoogleMobileAdsBannerClient(IAdListener listener)
		{
			return new AndroidBannerClient(listener);
		}

		internal static IGoogleMobileAdsInterstitialClient GetGoogleMobileAdsInterstitialClient(IAdListener listener)
		{
			return new AndroidInterstitialClient(listener);
		}
	}
}
