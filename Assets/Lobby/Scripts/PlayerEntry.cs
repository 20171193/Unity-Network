using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerReady;
    [SerializeField] Button playerReadyButton;

    private Player player;
    public Player Player { get { return player; } }

    public void SetPlayer(Player player)
    {
        this.player = player;
        playerName.text = player.NickName;

        playerReadyButton.enabled = player.IsLocal;
        playerReadyButton.image.color = player.IsLocal ? playerReadyButton.colors.normalColor : Color.gray;
    }

    public void Ready()
    {
        bool ready = player.GetReady();
        player.SetReady(!ready);
    }

    public void ChangeCustomProperty(PhotonHashTable property)
    {
        bool ready = player.GetReady();
        playerReady.text = ready ? "Ready" : "";
        playerReadyButton.image.color = ready ? Color.cyan : playerReadyButton.colors.normalColor;
    }
}
