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
        CheckPlayerStatus();
    }

    // WarIsOnline bilgisini true olarak güncelleyen fonksiyon

    public void SetWarIsOnline(string playerName)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            // Oyuncunun CustomProperties içindeki PlayerName'e bakıyoruz
            string playerCustomName = player.CustomProperties.ContainsKey("PlayerName") ? player.CustomProperties["PlayerName"].ToString() : null;

            Debug.Log("Comparing: " + playerCustomName + " with " + playerName);

            if (playerCustomName == playerName)
            {
                Debug.Log("Eşleşme bulundu!");

                // Sadece eşleşen oyuncunun CustomProperties bilgisini güncelliyoruz
                var customProperties = player.CustomProperties;

                // WarIsOnline bilgisini güncelliyoruz
                customProperties["warIsOnline"] = true;

                // CustomProperties'i güncelliyoruz
                player.SetCustomProperties(customProperties);

                // Güncel bilgiyi debug ile yazdıralım
                Debug.Log("Updated Player Info:");
                Debug.Log("Player Name: " + playerCustomName);
                Debug.Log("Updated War Is Online: " + customProperties["warIsOnline"]);

                break;  // İşlem tamamlandı, döngüyü sonlandırıyoruz
            }
            // else kısmı gereksiz, çünkü sadece eşleşen oyuncu için işlem yapıyoruz
        }
    }

    // Sürekli olarak kendi warIsOnline bilgisini sorgulayıp 7. ekrana geçiş yapmak için fonksiyon
    public void CheckPlayerStatus()
    {
        // Mevcut oyuncunun warIsOnline bilgisini alıyoruz
        bool warIsOnline = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("warIsOnline")
            ? (bool)PhotonNetwork.LocalPlayer.CustomProperties["warIsOnline"]
            : false;

        // Debug log ile warIsOnline değerini kontrol edelim
        Debug.Log("Current Player's warIsOnline: " + warIsOnline);

        // Eğer warIsOnline true ise 7. ekrana yönlendiriyoruz
        if (warIsOnline == true)
        {
            Debug.Log("War is online. Redirecting to war scene...");
            GoToWarScene(); // 7. ekrana yönlendirme fonksiyonu
        }
        else
        {
            Debug.Log("War is not online yet.");
        }
    }


    // 7. Ekrana gitme fonksiyonus
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

   


}