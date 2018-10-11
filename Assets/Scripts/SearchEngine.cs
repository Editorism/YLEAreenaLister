using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SearchEngine : MonoBehaviour
    {
        private ApiHandler apiHandler;
        public ResultList results;
        public Scrollbar slider;
        public Text inputText;
        private int offset = 0;
        private bool sliderIsUp = false;
	

        void Update ()
        {
            if (slider.value < 0.05f && sliderIsUp && offset > 0)
            {
                StartCoroutine(ExtendResultsList());
            }
            apiHandler = GetComponent<ApiHandler>();
        }

        /// <summary>
        /// Initiate search through the API Handler.
        /// </summary>
        /// <param name="keyword">Word(s) to use in search</param>
        public void SearchForPrograms(Text keyword)
        {
            apiHandler.SearchPrograms(keyword.text, offset);
            offset += 10;
        }

        /// <summary>
        /// Used to extend the results without resetting the search keyword or current results.
        /// </summary>
        private IEnumerator ExtendResultsList()
        {
            sliderIsUp = false;
            SearchForPrograms(inputText);
            yield return new WaitForSeconds(1);
            sliderIsUp = true;
        }

        /// <summary>
        /// Used to prevent more than 10 search results to be appended to the results list.
        /// </summary>
        private IEnumerator WaitForResults()
        {
            yield return new WaitForSeconds(1);
            sliderIsUp = true;
        }

        /// <summary>
        /// Used for fresh search, clearing the current results.
        /// </summary>
        /// <param name="keyword">Word(s) to use in search</param>
        public void NewSearch(Text keyword)
        {
            sliderIsUp = false;
            offset = 0;
            results.ClearResultList();
            SearchForPrograms(keyword);
            StartCoroutine(WaitForResults());
        }
    }
}
