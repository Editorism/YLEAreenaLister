using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ResultList : MonoBehaviour
    {

        public List<GameObject> searchResults;
        public GameObject scrollViewContent;
        public GameObject searchResult;
	
        /// <summary>
        /// Creates a list of Unity GameObjects from the serialized JSON data list.
        /// </summary>
        /// <param name="list">List of JSON data</param>
        public void CreateResultList(List<YleItem> list)
        {
            foreach (YleItem item in list)
            {            
                GameObject newItem = Instantiate(searchResult);
                searchResults.Add(newItem);
                newItem.GetComponent<RectTransform>().SetParent(scrollViewContent.GetComponent<RectTransform>(), false);
                newItem.GetComponent<ListItem>().SetVariables(item.title.fi, item.partOfSeries.title.fi,
                    item.description.fi, item.id, item.image.id, item.duration);
            }

        }

        /// <summary>
        /// Clears the current results from the list and destroys the instantiated prefabs connected to them.
        /// </summary>
        public void ClearResultList()
        {
            foreach(GameObject item in searchResults)
            {
                Destroy(item);
            }
            searchResults.Clear();
        }
    }
}