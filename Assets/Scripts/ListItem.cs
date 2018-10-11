using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ListItem : MonoBehaviour
    {
        public bool expanded = false;

        private float minimizedSize = 60;
        private float expandedSize = 200;
        private float minAnchorYMin = 0;
        private float expAnchorYMin = 0.85f;


        public GameObject _itemTitle;
        public GameObject _seriesTitle;
        public GameObject _description;
        public GameObject _id;
        public GameObject _duration;
        public GameObject _image;

        private const string YleImageUrl = "http://images.cdn.yle.fi/image/upload/w_160,h_90/";

        void Start()
        {
            _itemTitle.GetComponent<RectTransform>().anchorMin.Set(_itemTitle.GetComponent<RectTransform>().anchorMin.x, expAnchorYMin);
            _itemTitle.GetComponent<RectTransform>().offsetMin = new Vector2(_itemTitle.GetComponent<RectTransform>().offsetMin.x, -50);
            _id.SetActive(false);
            _seriesTitle.SetActive(false);
            _duration.SetActive(false);
            _image.SetActive(false);
            _description.SetActive(false);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minimizedSize);
            expanded = false;
        }

        /// <summary>
        /// Expands and contracts the UI element to show/hide extra information.
        /// </summary>
        public void ChangeSize()
        {
            if (!expanded)
            {
                _itemTitle.GetComponent<RectTransform>().anchorMin.Set(_itemTitle.GetComponent<RectTransform>().anchorMin.x, minAnchorYMin);
                _itemTitle.GetComponent<RectTransform>().offsetMin = new Vector2(_itemTitle.GetComponent<RectTransform>().offsetMin.x, -4);
                _id.SetActive(true);
                _seriesTitle.SetActive(true);
                _duration.SetActive(true);
                _image.SetActive(true);
                _description.SetActive(true);
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expandedSize);
                expanded = true;
            }

            else if (expanded)
            {
                _itemTitle.GetComponent<RectTransform>().anchorMin.Set(_itemTitle.GetComponent<RectTransform>().anchorMin.x, expAnchorYMin);
                _itemTitle.GetComponent<RectTransform>().offsetMin = new Vector2(_itemTitle.GetComponent<RectTransform>().offsetMin.x, -50);
                _id.SetActive(false);
                _seriesTitle.SetActive(false);
                _duration.SetActive(false);
                _image.SetActive(false);
                _description.SetActive(false);
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minimizedSize);
                expanded = false;
            }
        }

        /// <summary>
        /// Sets the YLE item data to the UI element.
        /// </summary>
        /// <param name="itemTitle">Title of the YLE item</param>
        /// <param name="seriesTitle">Title of the series the item is part of</param>
        /// <param name="description">Description of the YLE item</param>
        /// <param name="itemId">ID of the YLE item. Used in URL for single item requests</param>
        /// <param name="imageId">ID of the image of the YLE item. Used to translate into URL for image request</param>
        /// <param name="itemDuration">Duration of the YLE item program.</param>
        public void SetVariables(string itemTitle, string seriesTitle, string description, string itemId, string imageId, string itemDuration)
        {
            _itemTitle.GetComponent<Text>().text = itemTitle;
            _seriesTitle.GetComponent<Text>().text += " " + seriesTitle;
            _description.GetComponent<Text>().text = description;
            _id.GetComponent<Text>().text += itemId;
            _duration.GetComponent<Text>().text += FormatDuration(itemDuration);
            StartCoroutine(GetImage(imageId));
        }

        /// <summary>
        /// Formats the duration string into clearer form.
        /// </summary>
        /// <param name="dur">Duration in string form</param>
        /// <returns>Formatted duration</returns>
        private string FormatDuration(string dur)
        {
            if (dur == "PT0M0S")
                return "Unknown";
            else
            {
                TimeSpan durSpan = XmlConvert.ToTimeSpan(dur);
                string durStrHrs = string.Format("{0:0}h" , durSpan.Hours);
                string durStrMins = string.Format("{0:0}m ", durSpan.Minutes);
                string durStrSec = string.Format("{0:0}s", durSpan.Seconds);
                string durStr = durStrHrs + durStrMins + durStrSec;
                if (durSpan.Hours == 0)
                {
                    if (durSpan.Minutes == 0)
                        durStr = durStrSec;
                    else if (durSpan.Seconds == 0)
                        durStr = durStrMins;
                    else
                        durStr = durStrMins + durStrSec;
                }
                if (durSpan.Seconds == 0)
                {
                    if (durSpan.Minutes == 0)
                        durStr = durStrHrs;
                    else if (durSpan.Hours == 0)
                        durStr = durStrMins;
                    else
                        durStr = durStrHrs + durStrMins;
                }
                return durStr;
            }
        }

        /// <summary>
        /// Gets the image from URL using image ID.
        /// </summary>
        /// <param name="imgId">ID of the YLE item's image</param>
        private IEnumerator GetImage(string imgId)
        {
            string imageUrl = YleImageUrl + imgId;
            WWW www = new WWW(imageUrl);

            yield return www;

            Texture2D tex = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);

            www.LoadImageIntoTexture(tex);

            Rect rec = new Rect(0, 0, tex.width, tex.height);
            Sprite spriteToUse = Sprite.Create(tex, rec, Vector2.zero, 100);
            _image.GetComponent<Image>().sprite = spriteToUse;
        }
    }
}
