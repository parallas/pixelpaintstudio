using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Parallas;

public static class PlayerDeviceMapper
{
    public static Action<int> PlayerCreated;
    public static Action<int> PlayerRemoved;

    private static Dictionary<int, PlayerDeviceMap> IdToPlayerDevices { get; } = new Dictionary<int, PlayerDeviceMap>();
    private static Dictionary<int, int> ModifiedDeviceIdToPlayerId { get; } = new Dictionary<int, int>();

    public static void Process(double delta)
    {
        foreach (var (key, value) in IdToPlayerDevices)
        {
            value.Process(delta);

            if (key != 0) continue;
            Input.ParseInputEvent(new InputEventMouseMotion()
            {
                Device = value.DeviceIds[0],
                Position = value.MousePosition,
            });
        }
    }

    /// <summary>
    /// Registers a new player with the given device id, and returns the new player's id.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public static int RegisterNewPlayer(int deviceId)
    {
        int playerId = 0;
        while (IdToPlayerDevices.ContainsKey(playerId))
        {
            playerId++;
        }

        PlayerDeviceMap deviceMap = new PlayerDeviceMap { PlayerId = playerId, DeviceIds = [deviceId] };
        IdToPlayerDevices.Add(playerId, deviceMap);
        ModifiedDeviceIdToPlayerId.Add(deviceId, playerId);

        GD.Print($"Registered new player: {playerId} with device {deviceId}");
        PlayerCreated?.Invoke(playerId);
        return playerId;
    }

    public static void DeregisterDevice(int deviceId)
    {
        if (!TryGetPlayerDeviceMapFromDevice(deviceId, out var deviceMap)) return;
        var playerId = deviceMap.PlayerId;
        deviceMap.DeviceIds.Remove(deviceId);
        ModifiedDeviceIdToPlayerId.Remove(deviceId);
        GD.Print($"Disconnected {playerId}'s device {deviceId}");

        if (deviceMap.DeviceIds.Count > 0) return;
        IdToPlayerDevices.Remove(playerId);
        GD.Print($"Deregistered player: {playerId}");
        PlayerRemoved?.Invoke(playerId);
    }

    public static bool TryGetPlayerId(int deviceId, out int playerId)
    {
        return ModifiedDeviceIdToPlayerId.TryGetValue(deviceId, out playerId);
    }

    public static bool TryGetPlayerDeviceMap(int playerId, out PlayerDeviceMap deviceMap)
    {
        return IdToPlayerDevices.TryGetValue(playerId, out deviceMap);
    }

    public static bool TryGetPlayerDeviceMapFromDevice(int deviceId, out PlayerDeviceMap deviceMap)
    {
        deviceMap = null;
        if (!TryGetPlayerId(deviceId, out var playerId)) return false;
        return IdToPlayerDevices.TryGetValue(playerId, out deviceMap);
    }

    public static bool IsInputFromPlayer(InputEvent @event, int playerId)
    {
        if (!IdToPlayerDevices.TryGetValue(playerId, out PlayerDeviceMap map)) return false;
        return map.DeviceIds.Contains(@event.Device);
    }

    public static int GetControllerOffsetDeviceId(InputEvent @event)
    {
        var device = @event.Device;
        if (device == 0) return 0;
        if (@event is InputEventJoypadMotion or InputEventJoypadButton) device += 1;
        return device;
    }

    public static PlayerDeviceMap[] GetAllPlayerDeviceMaps()
    {
        return IdToPlayerDevices.Values.ToArray();
    }

}
