# How To Setup GDPR
## Steps
1. In the Top Menu go to TinySauce > TinySauce Settings > Edit Settings
2. This will open the Settings Scriptable Object ( or create it if it doesn't exist ).  
The Scriptable Object Settings will be inside the Assets > Resources > TinySauce folder.

## Required Information for GDPR ( Must Be Filled In )
- Company Name
- Privacy Policy URL
- Developer Contact Email

### Reasons for Information Requirements:
- Company Name is required to identify to the user whose game they are playing.
- A Privacy Policy is legally required to identify to the user what information is being collected.
- A Developer Contact Email is required if a Data Deletion Request is received.

3. If a User is in the EU region, the GDPR popup will be displayed before the ATT on iOS devices or upon startup on Android devices.

4. Within a Settings Screen for your application, add a button labeled as "GDPR".  When the user clicks on the button, call the function:
<code>PrivacyScreenUIManager.Instance.OpenPrivacyScreen();</code>

Example of how to add functionality to your own button:

<code>

    [SerializeField] private Button gdprButton;

    private void Start()
    {
        gdprButton.onClick.AddListener(PrivacyScreenUIManager.Instance.OpenPrivacyScreen);
    }

</code>

5. The popup screen will be the same screen that appears at startup, and will block user input until they have either changed their consent decision or just clicked Start Playing.

6. Verify that your scene that contains the TinySauce Prefab has an EventSystem in it.  The GDPR UI needs this in order to be interactable.
    If it does not have one, you can add one by right clicking in the Hierarchy going to UI then EventSystem.
