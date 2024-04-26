using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    public static bool GetReady(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        return customProperty.TryGetValue("Ready", out object value) ? (bool)value : false;
    }
    public static void SetReady(this Player player, bool value)
    {
        PhotonHashTable customProperty = new PhotonHashTable();
        customProperty["Ready"] = value;
        player.SetCustomProperties(customProperty);
    }
}
