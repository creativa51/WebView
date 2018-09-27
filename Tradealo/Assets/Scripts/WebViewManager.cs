using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebViewManager : MonoBehaviour {
    private WebViewObject webView;
    public GameObject LoadingProgressBar;

    private bool loadingProcessFlag = false;

    private bool webViewOnceLoaded = false;
    // Use this for initialization
    void Start () {
        StartWebView();
    }
	
	// Update is called once per frame
	void Update () {
        if (webView != null)
        {
            //Debug.Log("---Loading flag : " + webView.isLoading().ToString());
            if (webView.isLoading() == true && loadingProcessFlag == false)
            {
                
                loadingProcessFlag = true;

                LoadingProgressBar.SetActive(true);
                //if (webViewOnceLoaded)
                {
                    StartCoroutine("ProcessLoading");
                }
            }
            else if(webView.isLoading() == false && loadingProcessFlag == true)
            {
                webViewOnceLoaded = true;
                StartCoroutine("FinishLoading");
            }
        }
	}

    IEnumerator ProcessLoading()
    {
        float loadingValue = 0.0f;

        while(loadingValue < 0.8f)
        {
            loadingValue += 0.1f;
            LoadingProgressBar.GetComponent<Slider>().value = loadingValue;
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator FinishLoading()
    {
        LoadingProgressBar.GetComponent<Slider>().value = 1;
        loadingProcessFlag = false;
        yield return new WaitForSeconds(1f);
        LoadingProgressBar.GetComponent<Slider>().value = 0;
        LoadingProgressBar.SetActive(true);
    }

    public void StartWebView()
    {
        webView = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webView.Init((msg) => {
            Debug.Log(string.Format("CallFromJS[{0}]", msg));
        }, false);
        webView.SetMargins(0, 20, 0, 0);
        webView.LoadURL("https://www.tradealo.com/");
        webView.SetVisibility(true);
        

        webView.EvaluateJS(
            "window.addEventListener('onpageshow', function(){" +
            "Unity.call('url:' + window.location.href);" +
            "}, false);");
    }

    public void onbtnPrev()
    {
        if (webView.CanGoBack())
        {
            webView.GoBack();
        }
    }

    public void onbtnNext()
    {
        if (webView.CanGoForward())
        {
            webView.GoForward();
        }
    }

    public void LoadLink(string link)
    {
        webView.LoadURL(link);
    }
}
