using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;

public class Login : MonoBehaviour
{

    public static event Action OnLoginSuccess;

    [SerializeField] private string PLAYER_NAME_PREFIX = "Player";

    private async void Awake()
    {
        await Login_TryCatchAsync(UnityServices.InitializeAsync());
    }

    private void Start()
    {
        LoginUI.OnLoginClicked += LoginUI_OnLoginClicked;
    }

    private void OnDisable()
    {
        LoginUI.OnLoginClicked -= LoginUI_OnLoginClicked;
    }

    private async void LoginUI_OnLoginClicked()
    {
        await AuthenticateAsync();
    }

    private async Task AuthenticateAsync()
    {
        SetupEvents();
        await SignInAnonymouslyAsync();
    }

    private async Task Login_TryCatchAsync(Task promise)
    {
        try { await promise; }
        catch (Exception e)
        { Debug.LogException(e); }
    }

    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); // get a playerID
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}"); // get an access token
            OnLoginSuccess?.Invoke();
        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
#if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone())
            {
                // When using a ParrelSync clone, switch to a different authentication profile to force the clone
                // to sign in as a different anonymous user account.
                string customArgument = ParrelSync.ClonesManager.GetArgument();
                AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
            }
#endif

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await SetPlayerNameForGuest();
            //
            Debug.Log("Sign in anonymously succeeded!");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); // get the playerID
            //Debug.Log(await AuthenticationService.Instance.GetPlayerNameAsync());
        }
        catch (AuthenticationException ex) // Authentication Error Codes
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex) // Common Error Codes
        {
            Debug.LogException(ex);
        }
    }

    // Testing function only:
    private async Task SetPlayerNameForGuest()
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(PLAYER_NAME_PREFIX);
        Debug.Log(PLAYER_NAME_PREFIX);
    }
}
