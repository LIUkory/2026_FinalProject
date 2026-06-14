// IWeapon.cs
using UnityEngine;

// 介面名稱習慣以大寫 I 開頭
public interface IWeapon
{
    // 規定所有武器被裝備時，都要執行這個動作
    void Equip();
    void Unequip();
    // 未來如果你們想統一「開火」按鍵，也可以加在這裡
    // void Attack(); 
}