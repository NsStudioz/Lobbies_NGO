using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{

    public static RelayManager Instance;
    private const int maxPlayers = 3; // only clients are counted here, host is already considered as a connection,
                                      // therefore the host is not included as a connection here.

    void Awake()
    {
        Instance = this;
    }

    public async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);

            return default;
        }
    }

    public async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);

            return default;
        }
    }

    public async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);

            return default;
        }
    }
}
