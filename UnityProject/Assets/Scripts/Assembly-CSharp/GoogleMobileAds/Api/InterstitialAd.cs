using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	public class InterstitialAd : IAdListener
	{
		private IGoogleMobileAdsInterstitialClient client;

		public event EventHandler<EventArgs> AdLoaded = delegate
		{
		};

		public event EventHandler<AdFailedToLoadEventArgs> AdFailedToLoad = delegate
		{
		};

		public event EventHandler<EventArgs> AdOpened = delegate
		{
		};

		public event EventHandler<EventArgs> AdClosing = delegate
		{
		};

		public event EventHandler<EventArgs> AdClosed = delegate
		{
		};

		public event EventHandler<EventArgs> AdLeftApplication = delegate
		{
		};

		public InterstitialAd(string adUnitId)
		{
			client = GoogleMobileAdsClientFactory.GetGoogleMobileAdsInterstitialClient(this);
			client.CreateInterstitialAd(adUnitId);
		}

		void IAdListener.FireAdLoaded()
		{
			AdLoaded(this, EventArgs.Empty);
		}

		void IAdListener.FireAdFailedToLoad(string message)
		{
			AdFailedToLoadEventArgs e = new AdFailedToLoadEventArgs();
			e.Message = message;
			AdFailedToLoadEventArgs e2 = e;
			AdFailedToLoad(this, e2);
		}

		void IAdListener.FireAdOpened()
		{
			AdOpened(this, EventArgs.Empty);
		}

		void IAdListener.FireAdClosing()
		{
			AdClosing(this, EventArgs.Empty);
		}

		void IAdListener.FireAdClosed()
		{
			AdClosed(this, EventArgs.Empty);
		}

		void IAdListener.FireAdLeftApplication()
		{
			AdLeftApplication(this, EventArgs.Empty);
		}

		public void LoadAd(AdRequest request)
		{
			client.LoadAd(request);
		}

		public bool IsLoaded()
		{
			return client.IsLoaded();
		}

		public void Show()
		{
			client.ShowInterstitial();
		}

		public void Destroy()
		{
			client.DestroyInterstitial();
		}
	}
}
