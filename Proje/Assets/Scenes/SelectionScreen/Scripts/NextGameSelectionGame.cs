using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;  // PhotonPlayer yerine Player'ı kullanıyoruz
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;  // PhotonHashtable'ı kullanıyoruz

public class NextGame : MonoBehaviourPunCallbacks
{
    private string currentPlayerName; // Mevcut oyuncunun ismi
    private string opponentName; // Rakibin ismi

    // Butona tıklandığında çalışacak olan fonksiyon
    public void goWarScene()
    {
        // RegionClickHandler'ın opponentName özelliğinin null olup olmadığını kontrol et
        if (RegionClickHandler.opponentName != null)
        {
            opponentName = RegionClickHandler.opponentName;
            Debug.Log("Opponent Name (Savunan Kişi): " + opponentName);
        }
        else
        {
            Debug.LogError("Rakip adı null! RegionClickHandler'ı kontrol edin.");
            return; // opponentName null ise fonksiyonu erken sonlandırıyoruz
        }

        // Mevcut oyuncunun ismini alıyoruz
        currentPlayerName = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerName")
            ? PhotonNetwork.LocalPlayer.CustomProperties["PlayerName"].ToString()
            : "Oyuncu Adı Yok";

        Debug.Log("Current Player Name (Saldıran Kişi): " + currentPlayerName);

        // War durumu her iki oyuncu için true olarak ayarlanıyor
        SetWarIsOnline(currentPlayerName);  // Mevcut oyuncu için
        SetWarIsOnline(opponentName);       // Rakip oyuncu için
    }

    // WarIsOnline bilgisini true olarak güncelleyen fonksiyon

private void SetWarIsOnline(string playerName)
{
    // Oyuncuyu bulmak için PhotonNetwork.PlayerList içinde döngü kullanıyoruz
    foreach (var player in PhotonNetwork.PlayerList)
    {
        // Eğer oyuncunun playerName'i eşleşiyorsa
        if (player.NickName == playerName)
        {
            // Mevcut CustomProperties'i doğrudan alıyoruz
            var customProperties = player.CustomProperties;

            // Geçici değişkenlere bilgileri koyuyoruz
            var warIsOnline = customProperties.ContainsKey("warIsOnline") ? customProperties["warIsOnline"] : false;
            var playerNameTemp = player.NickName; // Oyuncunun ismini alıyoruz (örnek)
            var otherPropertyTemp = customProperties.ContainsKey("otherProperty") ? customProperties["otherProperty"] : null; // Diğer özellik

            // Debug ile mevcut bilgileri yazdırıyoruz
            Debug.Log("Current Player Info:");
            Debug.Log("Player Name: " + playerNameTemp);
            Debug.Log("War Is Online: " + warIsOnline);
            Debug.Log("Other Property: " + otherPropertyTemp);

            // WarIsOnline bilgisini true olarak güncelliyoruz
            customProperties["warIsOnline"] = true;

            // CustomProperties'i güncelliyoruz
            player.SetCustomProperties(customProperties);

            // Debug ile güncel bilgiyi yazdırıyoruz
            Debug.Log("Updated Player Info:");
            Debug.Log("Player Name: " + playerNameTemp);
            Debug.Log("Updated War Is Online: " + customProperties["warIsOnline"]);
            Debug.Log("Other Property: " + otherPropertyTemp);

            break;  // İlgili oyuncu bulunup işlem yapıldıktan sonra döngüyü sonlandırıyoruz
        }
    }
}






    // Sürekli olarak kendi warIsOnline bilgisini sorgulayıp 7. ekrana geçiş yapmak için fonksiyon
    public void CheckPlayerStatus()
    {
        // Mevcut oyuncunun warIsOnline bilgisini alıyoruz
        bool warIsOnline = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("warIsOnline")
            ? (bool)PhotonNetwork.LocalPlayer.CustomProperties["warIsOnline"]
            : false;

        // Eğer warIsOnline true ise 7. ekrana yönlendiriyoruz
        if (warIsOnline)
        {
            GoToWarScene(); // 7. ekrana yönlendirme fonksiyonu
        }
    }

    // 7. Ekrana gitme fonksiyonu
    private void GoToWarScene()
    {
        try
        {
            // Sahne yükleniyor
            SceneManager.LoadScene(7); // 7. sahne
        }
        catch (System.Exception e)
        {
            // Eğer sahne yüklenirken bir hata olursa
            Debug.LogError("Sahne yüklenirken hata oluştu: " + e.Message);
        }
    }

    // Update fonksiyonu sürekli olarak CheckPlayerStatus fonksiyonunu çağıracak
    void Update()
    {
        // Sürekli kontrol etmek için Update fonksiyonunda çağırabilirsiniz
        CheckPlayerStatus();
    }
}
