using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.Tiny.Sauce.Internal
{
    public class TSDebugUIScreenABTest : TSDebugUIScreen
    {
        private const string TAG = "TSDebugUIScreenABTest";
        [SerializeField] private CohortButton cohortBtnPrefab;

        [SerializeField] private Transform vGroupTrans;
        [SerializeField] private Text currentCohortText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private float feedbackTextDisplayDuration = 10f;


        private List<string> cohorts;
        private List<CohortButton> cohortButtonList = new List<CohortButton>();

        private Coroutine currentCoroutine;


        private void Awake()
        {
            feedbackText.gameObject.SetActive(false);
        }

        private void Start()
        {
            InitCohorts();
        }

        private void InitCohorts()
        {
            currentCohortText.text = TinySauce.GetABTestCohort() == "" ? "Control" : TinySauce.GetABTestCohort();

            if (TinySauceBehaviour.ABTestManager != null)
                cohorts = new List<string>(TinySauceBehaviour.ABTestManager.GetAbTestValues());
            else
                cohorts = new List<string>();

            if (cohorts.Count == 0 || cohorts == null)
            {
                feedbackText.gameObject.SetActive(true);
                feedbackText.text = "No AB Test Cohorts";
                return;
            }
            else
            {
                cohorts.Add("");
                
                CohortButton cohortBtn;

                if (cohorts.Count > 5)
                    cohortBtnPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 150);
                if (cohorts.Count > 7)
                    cohortBtnPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
                
                for (int i = 0; i < cohorts.Count; i++)
                {
                    cohortBtn = Instantiate(cohortBtnPrefab, vGroupTrans);
                    cohortBtn.displayFeedback += DisplayFeedbackText;

                    if (i == cohorts.Count - 1)
                        cohortBtn.CohortName = "Control";
                    else
                        cohortBtn.CohortName = cohorts[i];
                    
                    cohortButtonList.Add(cohortBtn);
                }
            }
        }

        private void DisplayFeedbackText(string text)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(Coroutine_DisplayFeedbackText(text));
        }


        private IEnumerator Coroutine_DisplayFeedbackText(string text)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = text;

            yield return new WaitForSecondsRealtime(feedbackTextDisplayDuration);


            feedbackText.gameObject.SetActive(false);
        }
    }
}
