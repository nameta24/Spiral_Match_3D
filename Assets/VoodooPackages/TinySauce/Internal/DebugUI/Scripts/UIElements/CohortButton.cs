using System;
using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.Tiny.Sauce.Internal
{
    public class CohortButton : MonoBehaviour
    {
        private const string TAG = "CohortButton";
        // Update button color for the current cohort and the targeted one
        // instead of currentCohort and feedback fields use always visible fields for the currentCohort and targetedCohort

        [SerializeField] private Text btnText;

        //public Text currentCohortText;
        public Action<string> displayFeedback;
        private string _cohortName;

        public string CohortName
        {
            /*get => cohortName;*/
            set
            {
                _cohortName = value;
                UpdateButtonText(_cohortName);
            }
        }

        private void UpdateButtonText(string name)
        {
            btnText.text = name;
        }

        public void OnClicked_SetCohort()
        {
            SavePlayerCohort(_cohortName);

            displayFeedback("Restart game to fully switch to " + _cohortName + " cohort");
        }

        private static void SavePlayerCohort(string cohort) // Same function as in the ABTestingManager
        {
            if (cohort == null)
            {
                PlayerPrefs.DeleteKey(cohort);
                return;
            }

            PlayerPrefs.SetString("ABCohort", cohort);
        }
    }
}
