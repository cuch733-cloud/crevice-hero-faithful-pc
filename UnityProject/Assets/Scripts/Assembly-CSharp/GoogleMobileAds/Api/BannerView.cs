using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	public class BannerView : IAdListener
	{
		private IGoogleMobileAdsBannerClient client;

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

		public BannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			client = GoogleMobileAdsClientFactory.GetGoogleMobileAdsBannerClient(this);
			client.CreateBannerView(adUnitId, adSize, position);
		}

		void IAdListener.FireAdLoaded()
		{
			AdLoaded(this, EventArgs.Empty);
		}

		void IAdListener.FireAdFailedToLoad(string message)
		{
			AdFailedToLoadEventArgs e = new AdFailedToLoadEventArgs();
			e.Message = message;
			AdFailedToLoad(this, e);
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

		public void Hide()
		{
			client.HideBannerView();
		}

		public void Show()
		{
			client.ShowBannerView();
		}

		public void Destroy()
		{
			client.DestroyBannerView();
		}
	}
}
