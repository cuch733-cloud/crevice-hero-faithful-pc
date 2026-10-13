using System;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAnalyticsAndroidV3 : IDisposable
{
	private string trackingCode;

	private string appVersion;

	private string appName;

	private string bundleIdentifier;

	private int dispatchPeriod;

	private float sampleFrequency;

	private GoogleAnalyticsV3.DebugMode logLevel;

	private bool anonymizeIP;

	private bool dryRun;

	private int sessionTimeout;

	private AndroidJavaObject tracker;

	private AndroidJavaObject logger;

	private AndroidJavaObject currentActivityObject;

	private AndroidJavaObject googleAnalyticsSingleton;

	private AndroidJavaObject gaServiceManagerSingleton;

	private AndroidJavaClass analyticsTrackingFields;

	private bool startSessionOnNextHit;

	private bool endSessionOnNextHit;

	internal void InitializeTracker()
	{
		Debug.Log("Initializing Google Analytics Android Tracker.");
		analyticsTrackingFields = new AndroidJavaClass("com.google.analytics.tracking.android.Fields");
		using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.google.analytics.tracking.android.GoogleAnalytics"))
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.analytics.tracking.android.GAServiceManager"))
			{
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					currentActivityObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
					googleAnalyticsSingleton = androidJavaObject.CallStatic<AndroidJavaObject>("getInstance", new object[1] { currentActivityObject });
					gaServiceManagerSingleton = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
					gaServiceManagerSingleton.Call("setLocalDispatchPeriod", dispatchPeriod);
					tracker = googleAnalyticsSingleton.Call<AndroidJavaObject>("getTracker", new object[1] { trackingCode });
					SetTrackerVal(Fields.SAMPLE_RATE, sampleFrequency.ToString());
					SetTrackerVal(Fields.APP_NAME, appName);
					SetTrackerVal(Fields.APP_ID, bundleIdentifier);
					SetTrackerVal(Fields.APP_VERSION, appVersion);
					if (anonymizeIP)
					{
						SetTrackerVal(Fields.ANONYMIZE_IP, "1");
					}
					googleAnalyticsSingleton.Call("setDryRun", dryRun);
					SetLogLevel(logLevel);
				}
			}
		}
	}

	internal void SetTrackerVal(Field fieldName, object value)
	{
		object[] args = new object[2]
		{
			fieldName.ToString(),
			value
		};
		tracker.Call(GoogleAnalyticsV3.SET, args);
	}

	public void SetSampleFrequency(float sampleFrequency)
	{
		this.sampleFrequency = sampleFrequency;
	}

	private void SetLogLevel(GoogleAnalyticsV3.DebugMode logLevel)
	{
		using (logger = googleAnalyticsSingleton.Call<AndroidJavaObject>("getLogger", new object[0]))
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.analytics.tracking.android.Logger$LogLevel"))
			{
				switch (logLevel)
				{
				case GoogleAnalyticsV3.DebugMode.ERROR:
				{
					using (AndroidJavaObject androidJavaObject4 = androidJavaClass.GetStatic<AndroidJavaObject>("ERROR"))
					{
						logger.Call("setLogLevel", androidJavaObject4);
						break;
					}
				}
				case GoogleAnalyticsV3.DebugMode.VERBOSE:
				{
					using (AndroidJavaObject androidJavaObject3 = androidJavaClass.GetStatic<AndroidJavaObject>("VERBOSE"))
					{
						logger.Call("setLogLevel", androidJavaObject3);
						break;
					}
				}
				case GoogleAnalyticsV3.DebugMode.INFO:
				{
					using (AndroidJavaObject androidJavaObject2 = androidJavaClass.GetStatic<AndroidJavaObject>("INFO"))
					{
						logger.Call("setLogLevel", androidJavaObject2);
						break;
					}
				}
				default:
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("WARNING"))
					{
						logger.Call("setLogLevel", androidJavaObject);
						break;
					}
				}
				}
			}
		}
	}

	private void SetSessionOnBuilder(AndroidJavaObject hitBuilder)
	{
		if (startSessionOnNextHit)
		{
			object[] args = new object[2]
			{
				Fields.SESSION_CONTROL.ToString(),
				"start"
			};
			hitBuilder.Call<AndroidJavaObject>("set", args);
			startSessionOnNextHit = false;
		}
		else if (endSessionOnNextHit)
		{
			object[] args2 = new object[2]
			{
				Fields.SESSION_CONTROL.ToString(),
				"end"
			};
			hitBuilder.Call<AndroidJavaObject>("set", args2);
			endSessionOnNextHit = false;
		}
	}

	private AndroidJavaObject BuildMap(string methodName)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.analytics.tracking.android.MapBuilder"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>(methodName, new object[0]))
			{
				SetSessionOnBuilder(androidJavaObject);
				return androidJavaObject.Call<AndroidJavaObject>("build", new object[0]);
			}
		}
	}

	private AndroidJavaObject BuildMap(string methodName, object[] args)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.analytics.tracking.android.MapBuilder"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>(methodName, args))
			{
				SetSessionOnBuilder(androidJavaObject);
				return androidJavaObject.Call<AndroidJavaObject>("build", new object[0]);
			}
		}
	}

	private AndroidJavaObject BuildMap(string methodName, Dictionary<AndroidJavaObject, string> parameters)
	{
		return BuildMap(methodName, null, parameters);
	}

	private AndroidJavaObject BuildMap(string methodName, object[] simpleArgs, Dictionary<AndroidJavaObject, string> parameters)
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap"))
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.analytics.tracking.android.MapBuilder"))
			{
				IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
				object[] array = new object[2];
				foreach (KeyValuePair<AndroidJavaObject, string> parameter in parameters)
				{
					using (AndroidJavaObject androidJavaObject2 = parameter.Key)
					{
						using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", parameter.Value))
						{
							array[0] = androidJavaObject2;
							array[1] = androidJavaObject3;
							AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
						}
					}
				}
				if (simpleArgs != null)
				{
					using (AndroidJavaObject androidJavaObject4 = androidJavaClass.CallStatic<AndroidJavaObject>(methodName, simpleArgs))
					{
						androidJavaObject4.Call<AndroidJavaObject>(GoogleAnalyticsV3.SET_ALL, new object[1] { androidJavaObject });
						SetSessionOnBuilder(androidJavaObject4);
						return androidJavaObject4.Call<AndroidJavaObject>("build", new object[0]);
					}
				}
				using (AndroidJavaObject androidJavaObject5 = androidJavaClass.CallStatic<AndroidJavaObject>(methodName, new object[0]))
				{
					androidJavaObject5.Call<AndroidJavaObject>(GoogleAnalyticsV3.SET_ALL, new object[1] { androidJavaObject });
					SetSessionOnBuilder(androidJavaObject5);
					return androidJavaObject5.Call<AndroidJavaObject>("build", new object[0]);
				}
			}
		}
	}

	private Dictionary<AndroidJavaObject, string> AddCustomVariablesAndCampaignParameters<T>(HitBuilder<T> builder)
	{
		Dictionary<AndroidJavaObject, string> dictionary = new Dictionary<AndroidJavaObject, string>();
		foreach (KeyValuePair<int, string> customDimension in builder.GetCustomDimensions())
		{
			AndroidJavaObject key = analyticsTrackingFields.CallStatic<AndroidJavaObject>("customDimension", new object[1] { customDimension.Key });
			dictionary.Add(key, customDimension.Value);
		}
		foreach (KeyValuePair<int, string> customMetric in builder.GetCustomMetrics())
		{
			AndroidJavaObject key = analyticsTrackingFields.CallStatic<AndroidJavaObject>("customMetric", new object[1] { customMetric.Key });
			dictionary.Add(key, customMetric.Value);
		}
		if (dictionary.Keys.Count > 0 && GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.VERBOSE))
		{
			Debug.Log("Added custom variables to hit.");
		}
		if (!string.IsNullOrEmpty(builder.GetCampaignSource()))
		{
			AndroidJavaObject key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("CAMPAIGN_SOURCE");
			dictionary.Add(key, builder.GetCampaignSource());
			key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("CAMPAIGN_MEDIUM");
			dictionary.Add(key, builder.GetCampaignMedium());
			key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("CAMPAIGN_NAME");
			dictionary.Add(key, builder.GetCampaignName());
			key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("CAMPAIGN_CONTENT");
			dictionary.Add(key, builder.GetCampaignContent());
			key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("CAMPAIGN_KEYWORD");
			dictionary.Add(key, builder.GetCampaignKeyword());
			key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("CAMPAIGN_ID");
			dictionary.Add(key, builder.GetCampaignID());
			key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("GCLID");
			dictionary.Add(key, builder.GetGclid());
			key = analyticsTrackingFields.GetStatic<AndroidJavaObject>("DCLID");
			dictionary.Add(key, builder.GetDclid());
			if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.VERBOSE))
			{
				Debug.Log("Added campaign parameters to hit.");
			}
		}
		if (dictionary.Keys.Count > 0)
		{
			return dictionary;
		}
		return null;
	}

	internal void StartSession()
	{
		startSessionOnNextHit = true;
	}

	internal void StopSession()
	{
		endSessionOnNextHit = true;
	}

	public void SetOptOut(bool optOut)
	{
		googleAnalyticsSingleton.Call("setAppOptOut", optOut);
	}

	internal void LogScreen(AppViewHitBuilder builder)
	{
		using (AndroidJavaObject androidJavaObject = analyticsTrackingFields.GetStatic<AndroidJavaObject>("SCREEN_NAME"))
		{
			object[] args = new object[2]
			{
				androidJavaObject,
				builder.GetScreenName()
			};
			tracker.Call(GoogleAnalyticsV3.SET, args);
		}
		Dictionary<AndroidJavaObject, string> dictionary = AddCustomVariablesAndCampaignParameters(builder);
		if (dictionary != null)
		{
			object obj = BuildMap(GoogleAnalyticsV3.APP_VIEW, dictionary);
			tracker.Call(GoogleAnalyticsV3.SEND, obj);
		}
		else
		{
			object[] args2 = new object[1] { BuildMap(GoogleAnalyticsV3.APP_VIEW) };
			tracker.Call(GoogleAnalyticsV3.SEND, args2);
		}
	}

	internal void LogEvent(EventHitBuilder builder)
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.Long", builder.GetEventValue()))
		{
			object[] array = new object[4]
			{
				builder.GetEventCategory(),
				builder.GetEventAction(),
				builder.GetEventLabel(),
				androidJavaObject
			};
			Dictionary<AndroidJavaObject, string> dictionary = AddCustomVariablesAndCampaignParameters(builder);
			object obj = ((dictionary == null) ? BuildMap(GoogleAnalyticsV3.EVENT_HIT, array) : BuildMap(GoogleAnalyticsV3.EVENT_HIT, array, dictionary));
			tracker.Call(GoogleAnalyticsV3.SEND, obj);
		}
	}

	internal void LogTransaction(TransactionHitBuilder builder)
	{
		AndroidJavaObject[] array = new AndroidJavaObject[3]
		{
			new AndroidJavaObject("java.lang.Double", builder.GetRevenue()),
			new AndroidJavaObject("java.lang.Double", builder.GetTax()),
			new AndroidJavaObject("java.lang.Double", builder.GetShipping())
		};
		object[] array2 = new object[6]
		{
			builder.GetTransactionID(),
			builder.GetAffiliation(),
			array[0],
			array[1],
			array[2],
			null
		};
		if (builder.GetCurrencyCode() == null)
		{
			array2[5] = GoogleAnalyticsV3.currencySymbol;
		}
		else
		{
			array2[5] = builder.GetCurrencyCode();
		}
		Dictionary<AndroidJavaObject, string> dictionary = AddCustomVariablesAndCampaignParameters(builder);
		object obj = ((dictionary == null) ? BuildMap(GoogleAnalyticsV3.TRANSACTION_HIT, array2) : BuildMap(GoogleAnalyticsV3.TRANSACTION_HIT, array2, dictionary));
		tracker.Call(GoogleAnalyticsV3.SEND, obj);
	}

	internal void LogItem(ItemHitBuilder builder)
	{
		object[] array = ((builder.GetCurrencyCode() == null) ? new object[6] : new object[7]
		{
			null,
			null,
			null,
			null,
			null,
			null,
			builder.GetCurrencyCode()
		});
		array[0] = builder.GetTransactionID();
		array[1] = builder.GetName();
		array[2] = builder.GetSKU();
		array[3] = builder.GetCategory();
		array[4] = new AndroidJavaObject("java.lang.Double", builder.GetPrice());
		array[5] = new AndroidJavaObject("java.lang.Long", builder.GetQuantity());
		Dictionary<AndroidJavaObject, string> dictionary = AddCustomVariablesAndCampaignParameters(builder);
		object obj = ((dictionary == null) ? BuildMap(GoogleAnalyticsV3.ITEM_HIT, array) : BuildMap(GoogleAnalyticsV3.ITEM_HIT, array, dictionary));
		tracker.Call(GoogleAnalyticsV3.SEND, obj);
	}

	public void LogException(ExceptionHitBuilder builder)
	{
		object[] array = new object[2]
		{
			builder.GetExceptionDescription(),
			new AndroidJavaObject("java.lang.Boolean", builder.IsFatal())
		};
		Dictionary<AndroidJavaObject, string> dictionary = AddCustomVariablesAndCampaignParameters(builder);
		object obj = ((dictionary == null) ? BuildMap(GoogleAnalyticsV3.EXCEPTION_HIT, array) : BuildMap(GoogleAnalyticsV3.EXCEPTION_HIT, array, dictionary));
		tracker.Call(GoogleAnalyticsV3.SEND, obj);
	}

	public void DispatchHits()
	{
		gaServiceManagerSingleton.Call("dispatchLocalHits");
	}

	public void LogSocial(SocialHitBuilder builder)
	{
		object[] array = new object[3]
		{
			builder.GetSocialNetwork(),
			builder.GetSocialAction(),
			builder.GetSocialTarget()
		};
		Dictionary<AndroidJavaObject, string> dictionary = AddCustomVariablesAndCampaignParameters(builder);
		object obj = ((dictionary == null) ? BuildMap(GoogleAnalyticsV3.SOCIAL_HIT, array) : BuildMap(GoogleAnalyticsV3.SOCIAL_HIT, array, dictionary));
		tracker.Call(GoogleAnalyticsV3.SEND, obj);
	}

	public void LogTiming(TimingHitBuilder builder)
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.Long", builder.GetTimingInterval()))
		{
			object[] array = new object[4]
			{
				builder.GetTimingCategory(),
				androidJavaObject,
				builder.GetTimingName(),
				builder.GetTimingLabel()
			};
			Dictionary<AndroidJavaObject, string> dictionary = AddCustomVariablesAndCampaignParameters(builder);
			object obj = ((dictionary == null) ? BuildMap(GoogleAnalyticsV3.TIMING_HIT, array) : BuildMap(GoogleAnalyticsV3.TIMING_HIT, array, dictionary));
			tracker.Call(GoogleAnalyticsV3.SEND, obj);
		}
	}

	public void ClearUserIDOverride()
	{
		SetTrackerVal(Fields.USER_ID, null);
	}

	public void SetTrackingCode(string trackingCode)
	{
		this.trackingCode = trackingCode;
	}

	public void SetAppName(string appName)
	{
		this.appName = appName;
	}

	public void SetBundleIdentifier(string bundleIdentifier)
	{
		this.bundleIdentifier = bundleIdentifier;
	}

	public void SetAppVersion(string appVersion)
	{
		this.appVersion = appVersion;
	}

	public void SetDispatchPeriod(int dispatchPeriod)
	{
		this.dispatchPeriod = dispatchPeriod;
	}

	public void SetLogLevelValue(GoogleAnalyticsV3.DebugMode logLevel)
	{
		this.logLevel = logLevel;
	}

	public void SetAnonymizeIP(bool anonymizeIP)
	{
		this.anonymizeIP = anonymizeIP;
	}

	public void SetDryRun(bool dryRun)
	{
		this.dryRun = dryRun;
	}

	public void Dispose()
	{
		googleAnalyticsSingleton.Dispose();
		tracker.Dispose();
		analyticsTrackingFields.Dispose();
	}
}
