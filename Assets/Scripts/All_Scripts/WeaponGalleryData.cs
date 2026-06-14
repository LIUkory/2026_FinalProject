using UnityEngine;

[System.Serializable]
public class WeaponGalleryData
{
    public string weaponName;       // 武器名稱
    [TextArea(3, 5)]
    public string description;      // 武器說明
    public string stats;            // 武器數值（例如：傷害 3x / CD 0.5s）
    public Sprite weaponIcon;       // 畫框裡顯示的武器圖片
}