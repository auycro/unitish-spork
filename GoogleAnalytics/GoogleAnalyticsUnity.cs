using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoogleAnalyticsUnity : SingletonMonoBehaviour<GoogleAnalyticsUnity> {
	private string trackingCode;
	private string bundleIdentifier;
	private string appName;
	private string appVersion;
	private enum DebugMode {ERROR,WARNING,INFO,VERBOSE};
	private GoogleAnalyticsUnity.DebugMode logLevel;
	private bool anonymizeIP;
	private bool dryRun;
	private bool optOut;
	private int sessionTimeout;
	private string screenRes;
	private string clientId;
	private string url;
	private float timeStarted;
	private Dictionary<Field, object> trackerValues = new Dictionary<Field, object>();
	private bool startSessionOnNextHit = false;
	private bool endSessionOnNextHit = false;
	private bool trackingCodeSet = true;
	private string screen;
	private bool init = false;

	public void Initialize(){
		JSONObject env = WebServerEnvironment.App.EnvironmentJSONObject();

		appName = env["app_name"].str;
		trackingCode = env["ga_profile_id"].str;
		bundleIdentifier = env["bundle_identifier"].str;
		clientId = SystemInfo.deviceUniqueIdentifier;
	#if UNITY_WEBGL
		clientId = "webgl_"+LoginInfo.Instance.id;
	#endif
		appVersion = CurrentBundleVersion.version;

		url = "https://www.google-analytics.com/collect?v=1"
		+ AddParam (Fields.LANGUAGE, Application.systemLanguage.ToString ())
		+ AddParam (Fields.SCREEN_RESOLUTION, Screen.width + "x" + Screen.height)
		+ AddParam (Fields.APP_NAME, appName)
		+ AddParam (Fields.TRACKING_ID, trackingCode)
		+ AddParam (Fields.APP_ID, bundleIdentifier)
		+ AddParam (Fields.CLIENT_ID, clientId)
		+ AddParam (Fields.APP_VERSION, appVersion);

		init = true;
	}

	public string AddParam(Field parameterer, string value){
		return parameterer + "=" + value;
	}

	public void LogScreen(string screenName){
		SendGoogleAnalytics (url 
		 + AddParam(Fields.HIT_TYPE, "appview")
	     + AddParam(Fields.SCREEN_NAME, screenName)
	    );
	}

	public void SendGoogleAnalytics(string url){
		//Add for AvoidCaching
		string newUrl = url + "&z=" + UnityEngine.Random.Range (0, 500);
		StartCoroutine (this.HandleWWW(new WWW(newUrl)));
	}

	/*
    Make request using yield and coroutine to prevent lock up waiting on request to return.
  */
	public IEnumerator HandleWWW(WWW request)
	{
		//Debug.Log ("SendGA: "+request.url);
		while (!request.isDone)
		{
			yield return request;
			if (request.responseHeaders.ContainsKey("STATUS")) {
				if (request.responseHeaders["STATUS"].Contains("200 OK")) {
					Debug.Log("Successfully sent Google Analytics hit.");
				} else {
					Debug.LogWarning("Google Analytics hit request rejected with " +
						                 "status code " + request.responseHeaders["STATUS"]);
				}
			} else {
				Debug.LogWarning("Google Analytics hit request failed with error "
					                 + request.error);
			}
		}
	}

	void Start(){
		if (!init)
			Initialize ();
	}
}
