using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushNotificationManager : MonoBehaviour {
    public GameObject NotificationPopup;
    public Text TitleText;
    public Text ContentText;

    // Use this for initialization
    void Start () {
#if UNITY_ANDROID
        if (Input.location.status == LocationServiceStatus.Stopped)
            Input.location.Start(10, 0.1f);
#elif UNITY_IPHONE
        Input.location.Start ();
#endif
        try //The TaskDescription API is unavailable on Android 4.4 and below
        {
            var ActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var Activity = ActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
            var Resources = Activity.Call<AndroidJavaObject>("getResources");
            var AppInfo = Activity.Call<AndroidJavaObject>("getApplicationInfo");
            int IconResId = AppInfo.Get<int>("icon");
            var BitmapFactoryClass = new AndroidJavaClass("android.graphics.BitmapFactory");
            var Icon = BitmapFactoryClass.CallStatic<AndroidJavaObject>("decodeResource", Resources, IconResId);
            var ColorClass = new AndroidJavaClass("android.graphics.Color");
            var _Color = ColorClass.CallStatic<int>("argb", (int)255, (int)216, 152, 57);
            var TaskDescription = new AndroidJavaObject("android.app.ActivityManager$TaskDescription", Application.productName, Icon, _Color);
            Activity.Call("setTaskDescription", TaskDescription);
        }
        catch { }

        
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            Debug.Log("---Application Paused---");
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }
        else
        {
            GetIntentParams();
            Debug.Log("---Application Resumed---");
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            
            Firebase.Messaging.FirebaseMessaging.Subscribe("all");
        }
    }

    public void GetIntentParams()
    {
        string arguments = "";

        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        bool hasExtra = intent.Call<bool>("hasExtra", "arguments");

        if (hasExtra)
        {
            AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");
            arguments = extras.Call<string>("getString", "arguments");
        }

        Debug.Log("---Params : " + arguments + "---");
    }

    private void OnApplicationFocus(bool focus)
    {
        if(focus == false)
            Debug.Log("---Application Lose Focussed---");
        else
            Debug.Log("---Application Focussed---");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("---Application Exit---");
    }
    /*
    private void OnEnable()
    {
        Debug.Log("----App Enable----");
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

        Firebase.Messaging.FirebaseMessaging.Subscribe("all");
    }

    private void OnDisable()
    {
        Debug.Log("----App Disable----");
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }
    */
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
        string popup_title = "";
        string popup_content = "";
        var notification = e.Message.Notification;
        if (notification != null)
        {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
            popup_title = notification.Title;
            popup_content = notification.Body;
        }
        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);
        if (e.Message.Data.Count > 0)
        {
            Debug.Log("data:");
            foreach (System.Collections.Generic.KeyValuePair<string, string> iter in e.Message.Data)
            {
                Debug.Log("  " + iter.Key + ": " + iter.Value);
                if (iter.Key.Equals("body"))
                    popup_content = iter.Value;
                else if (iter.Key.Equals("title"))
                    popup_title = iter.Value;
            }
        }
        //TitleText.text = popup_title;
        //ContentText.text = popup_content;
        Debug.Log("---Received notification content : " + popup_content + "------");

        ShowNotification();
        if(popup_content.Contains("http"))
        {
            GameObject.Find("WebViewManager").GetComponent<WebViewManager>().LoadLink(popup_content);
        }
        //else
            //NotificationPopup.SetActive(true);
    }

    public void ShowNotification()
    {
        
    }

    public void OnClosePopup()
    {
        NotificationPopup.SetActive(false);
    }
}
