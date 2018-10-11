using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class ApiHandler : MonoBehaviour
    {

        private const string YleUrl = "https://external.api.yle.fi/v1/programs/items.json";
        private const int searchLimit = 10;
        private readonly StringBuilder _keyword = new StringBuilder();
        private const string appId = "_AppIdHere_";
        private const string appKey = "_AppKeyHere_";

        public ResultList results;

        private YleItemCollection collection = new YleItemCollection();

        
        /// <summary>
        /// Handles the formatting of the request string, and sends the request using the keyword.
        /// </summary>
        /// <param name="keyword">Word(s) to use in search</param>
        /// <param name="offset">Offset used to skip a given amount of search results</param>
        public void SearchPrograms(string keyword, int offset)
        {
            _keyword.Length = 0;
            _keyword.Capacity = 0;

            _keyword.Append(YleUrl + "?app_id=" + appId + "&app_key=" + appKey);

            if (!string.IsNullOrEmpty(keyword))
            {
                _keyword.Append("&q=" + keyword);
            }

            if (offset > 0)
            {
                _keyword.Append("&offset=" + offset);
            }

            _keyword.Append("&limit=" + searchLimit);


            _keyword.Append("&availability=ondemand");


            _keyword.Append("&mediaobject=video");


            StartCoroutine(SendRequest(_keyword.ToString()));

        }

        /// <summary>
        /// Sends web request to remote URL.
        /// </summary>
        /// <param name="url">URL to send request to.</param>
        private IEnumerator SendRequest(string url)
        {
            UnityWebRequest myRequest = UnityWebRequest.Get(url);
            
            yield return myRequest.SendWebRequest();

            Debug.Log(myRequest.responseCode); //REMOVE ME

            if (myRequest.isHttpError || myRequest.isNetworkError)
            {
                Debug.Log(myRequest.error);
            }

            else
            {
                collection = JsonUtility.FromJson<YleItemCollection>(myRequest.downloadHandler.text);
                results.CreateResultList(collection.data);
            }
        }
    }

    #region YleData
    /// <summary>
    /// Object that contains a list of YLE data items.
    /// </summary>
    [Serializable]
    public class YleItemCollection
    {
        public List<YleItem> data = new List<YleItem>();
    }

    /// <summary>
    /// Container that holds all the YLE data for one found item.
    /// </summary>
    [Serializable]
    public class YleItem
    {
        public string id = "Unknown";
        public AreenaTitle title = new AreenaTitle("Unknown");
        public AreenaPartOfSeries partOfSeries = new AreenaPartOfSeries("Unknown", new AreenaTitle("None"));
        public AreenaDescription description = new AreenaDescription("Description not found.");
        public AreenaImage image = new AreenaImage("areena-etusivu-somejako.jpg", true);
        public string duration = "PT0M0S";
    }

    /// <summary>
    /// Given title of the item.
    /// </summary>
    [Serializable]
    public class AreenaTitle
    {
        public AreenaTitle(string title) { fi = title; }
        public string fi;

    }

    /// <summary>
    /// Image associated with the item.
    /// </summary>
    [Serializable]
    public class AreenaImage
    {
        public AreenaImage(string newId, bool availability) { id = newId; availability = true; img = new NonSerializedAttribute(); }
        public string id;
        public bool available = true;
        public NonSerializedAttribute img;
    }

    /// <summary>
    /// Series data of the item.
    /// </summary>
    [Serializable]
    public class AreenaPartOfSeries
    {
        public AreenaPartOfSeries(string newId, AreenaTitle newTitle) { id = newId; title = newTitle; }
        public string id;
        public AreenaTitle title;
    }

    /// <summary>
    /// Description data of the item.
    /// </summary>
    [Serializable]
    public class AreenaDescription
    {
        public AreenaDescription(string title) { fi = title; }
        public string fi;

    }
    #endregion
}