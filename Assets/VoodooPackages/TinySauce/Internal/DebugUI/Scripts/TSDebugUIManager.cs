using UnityEngine;

namespace Voodoo.Tiny.Sauce.Internal
{
    public class TSDebugUIManager : MonoBehaviour
    {
        private const string TAG = "TSDebugUIManager";
        [SerializeField] private TSDebugUIBehaviour tsDebugUIPrefab;

        private bool isDebugUIOpen = false;

        private float maxDurationBetweenTap = 2.5f;
        private float countDown;

        private int countTapTL = 0;
        private int countTapTR = 0;

        private Vector3 mousePos;
        private int smallerScreenSliceNb = 6;
        private int biggerScreenSliceNb = 8;
        private int screenWidthSliceNb;
        private int screenHeightSliceNb;

        private int _screenSliceWidth;
        public int ScreenSliceWidth { get => _screenSliceWidth; }

        private int _screenSliceHeight;
        public int ScreenSliceHeight { get => _screenSliceHeight; }


        private void Start()
        {
            if (Screen.width < Screen.height)
            {
                screenWidthSliceNb = smallerScreenSliceNb;
                screenHeightSliceNb = biggerScreenSliceNb;
            }
            else
            {
                screenWidthSliceNb = biggerScreenSliceNb;
                screenHeightSliceNb = smallerScreenSliceNb;
            }

            _screenSliceWidth = Screen.width / screenWidthSliceNb;
            _screenSliceHeight = Screen.height / screenHeightSliceNb;
        }

        private void Update()
        {
            isDebugUIOpen = TSDebugUIBehaviour.Instance != null;
            mousePos = Input.mousePosition;

            if (!isDebugUIOpen)
            {
                if (countDown > 0)
                {
                    countDown -= Time.unscaledDeltaTime;

                    if (countDown <= 0) ResetCountsTap();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (countTapTL < 1) TapTopLeftElseReset();
                    else if (countTapTR < 2) TapTopRightElseReset();
                    else if (countTapTL < 4) TapTopLeftElseReset();
                    else if (countTapTR < 6) TapTopRightElseReset();
                }

                if (countTapTL == 4 && countTapTR == 6)
                {
                    OpenDebugUI();
                    ResetCountsTap();
                }
            }
        }

        private void OpenDebugUI()
        {
            Instantiate(tsDebugUIPrefab);
            TinySauce.TrackCustomEvent("DebugUI_OpeningEvent");
        }

        #region [TAP_FUNCTIONS]
        private void TapTopLeftElseReset()
        {
            if (mousePos.x <= ScreenSliceWidth && mousePos.y >= ScreenSliceHeight * (screenHeightSliceNb - 1))
                ValidTap(ref countTapTL);
            else
                ResetCountsTap();
        }
        private void TapTopRightElseReset()
        {
            if (mousePos.x >= ScreenSliceWidth * (screenWidthSliceNb - 1) && mousePos.y >= ScreenSliceHeight * (screenHeightSliceNb - 1))
                ValidTap(ref countTapTR);
            else
                ResetCountsTap();
        }

        private void ValidTap(ref int countToIncrement)
        {
            countDown = maxDurationBetweenTap;
            countToIncrement++;
        }

        private void ResetCountsTap()
        {
            //Debug.Log("RESET : TL=" + countTapTL + " // TR=" + countTapTR + " // countDown=" + countDown);
            countDown = 0;
            countTapTL = 0;
            countTapTR = 0;
        }
        #endregion
    }
}