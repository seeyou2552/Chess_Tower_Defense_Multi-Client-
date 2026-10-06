using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


[System.Serializable]
public class ShopData
{
   public List<ShopItemEntry> ShopItems = new List<ShopItemEntry>();
   [System.NonSerialized] public List<ItemData> CachedItems;

   public async Task InitializeShopItems()
   {
      var handle = Addressables.LoadAssetsAsync<ItemData>("Item", null);

      await handle.Task;

      List<ItemData> items = (await handle.Task).ToList();
      // itemIndex 기준 오름차순 정렬
      items.Sort((a, b) => a.ItemIndex.CompareTo(b.ItemIndex));
      CachedItems = items;
      
      // 존재하지 않는 데이터 제거 및 해당 minion이 minionList에 있으면 제거
      if (PlayerManager.Instance != null && PlayerManager.Instance.Data != null && PlayerManager.Instance.Data.MinionList != null)
      {
         for (int i = ShopItems.Count - 1; i >= 0; i--)
         {
            ShopItemEntry entry = ShopItems[i];
            ItemData item = items.Find(it => it != null && it.ItemName == entry.ItemName);
            
            // 존재하지 않는 항목 제거
            if (item == null)
            {
               // 삭제된 항목이 MinionData였다면, PlayerManager의 minionList에서도 제거
               if (!string.IsNullOrEmpty(entry.ItemName))
               {
                  ItemData deletedItem = CachedItems?.Find(it => it != null && it.ItemName == entry.ItemName);
                  if (deletedItem is MinionItemData deletedMinionItem && deletedMinionItem.MinionData != null)
                  {
                     int minionIndex = deletedMinionItem.MinionData.MinionIndex;
                     if (PlayerManager.Instance.Data.MinionList.Contains(minionIndex))
                     {
                        PlayerManager.Instance.Data.MinionList.Remove(minionIndex);
                        SaveSystem.Save(PlayerManager.Instance.Data);
                     }
                  }
               }
               ShopItems.RemoveAt(i);
            }
            else if (item.ItemType == ShopItemType.ChessPiece)
            {
               MinionItemData minionItemData = item as MinionItemData;
               if (minionItemData != null && minionItemData.MinionData != null)
               {
                  entry.MinionIndex = minionItemData.MinionData.MinionIndex;
                  int minionIndex = minionItemData.MinionData.MinionIndex;
                  if (PlayerManager.Instance.Data.MinionList.Contains(minionIndex))
                  {
                     ShopItems.RemoveAt(i);
                  }
               }
            }
         }
      }
      else
      {
         // PlayerManager 없으면 단순히 존재하지 않는 데이터만 제거
         ShopItems.RemoveAll(entry => !items.Exists(item => item != null && item.ItemName == entry.ItemName));
      }
      
      // 새로운 항목 추가
      if (items != null && items.Count > 0)
      {
         foreach (ItemData item in items)
         {
            if (item == null)
               continue;

            if (!ShopItems.Exists(entry => entry.ItemName == item.ItemName))
            {
               // ChessPiece는 별도 항목 추가
               if(item.ItemType == ShopItemType.ChessPiece)
               {
                  MinionItemData minionItemData = item as MinionItemData;
                  if (minionItemData == null || minionItemData.MinionData == null)
                     continue;

                  int minionIndex = minionItemData.MinionData.MinionIndex;

                  // Player가 이미 보유했을 경우 count를 1로 설정
                  if(PlayerManager.Instance.Data.MinionList.Contains(minionIndex))
                     ShopItems.Add(new ShopItemEntry { 
                        ItemIndex = item.ItemIndex,
                        ItemName = item.ItemName,
                        Count = 1,
                        MinionIndex = minionIndex
                     });

                  else
                     ShopItems.Add(new ShopItemEntry {
                        ItemIndex = item.ItemIndex,
                        ItemName = item.ItemName,
                        Count = 0,
                        MinionIndex = minionIndex
                     });
               }

               // ChessPiece외 Item 추가
               else
               {
                  ShopItems.Add(new ShopItemEntry {
                     ItemIndex = item.ItemIndex,
                     ItemName = item.ItemName,
                     Count = 0
                  });
               }
            }
         }

      }

      // shopItems를 itemIndex 기준으로 정렬
      ShopItems.Sort((a, b) => a.ItemIndex.CompareTo(b.ItemIndex));

      SaveSystem.Save(this);
   }
   
   /// <summary>
   /// 아이템의 현재 구매 횟수 추가
   /// </summary>
   public bool AddPurchasedItem(ItemData itemData)
   {
      if (!CheckpurchasedItem(itemData))
          return false;

      if (!ShopItems.Exists(entry => entry.ItemName == itemData.ItemName))
      {
         ShopItemEntry newEntry = new ShopItemEntry { ItemIndex = itemData.ItemIndex, ItemName = itemData.ItemName, Count = 1 };
         if (itemData.DateReset)
            newEntry.LastPurchaseDate = System.DateTime.Now.Date;
         ShopItems.Add(newEntry);
      }

      
      else
      {
         ShopItemEntry existingEntry = ShopItems.Find(entry => entry.ItemName == itemData.ItemName);
         
         // dateReset이 true인 경우 날짜 업데이트
         if (itemData.DateReset)
         {
            existingEntry.LastPurchaseDate = System.DateTime.Now.Date;
         }
         
         existingEntry.Count++;
      }

      SaveSystem.Save(this);
      return true;
   }

   private void ResetCountIfDateChanged(ShopItemEntry entry, ItemData itemData)
   {
      // dateReset이 true인 경우 날짜 체크 및 자동 초기화
      if (itemData.DateReset)
      {
         System.DateTime today = System.DateTime.Now.Date;
         if (entry.LastPurchaseDate != System.DateTime.MinValue.Date && entry.LastPurchaseDate < today)
         {
            // 자정이 지나면 count 초기화
            entry.Count = 0;
            entry.LastPurchaseDate = today;
            SaveSystem.Save(this);
         }
      }
   }

   public int GetPurchasedCount(ItemData itemData)
   {
      ShopItemEntry entry = ShopItems.Find(e => e.ItemName == itemData.ItemName);
      if (entry == null)
         return 0;

      ResetCountIfDateChanged(entry, itemData);

      return entry.Count;
   }

   public void BackPurchasedItem(ItemData itemData)
   {
      ShopItemEntry existingEntry = ShopItems.Find(entry => entry.ItemName == itemData.ItemName);
      if (existingEntry != null && existingEntry.Count > 0)
      {
         existingEntry.Count--;
         SaveSystem.Save(this);
      }
   }

   public bool CheckpurchasedItem(ItemData itemData)
   {
      ShopItemEntry entry = ShopItems.Find(item => item.ItemName == itemData.ItemName);
      if (entry == null)
         return itemData.MaxPurchaseCount > 0;
      
      ResetCountIfDateChanged(entry, itemData);
      
      return itemData.MaxPurchaseCount > entry.Count;
   }

   public void SortShopItemsByMinionIndex()
   {
      ShopItems.Sort((lhs, rhs) => GetMinionIndex(lhs.ItemIndex).CompareTo(GetMinionIndex(rhs.ItemIndex)));
   }

   private int GetMinionIndex(int itemIndex)
   {
      ItemData item = GetItemByIndex(itemIndex);
      if (item is MinionItemData minionItem && minionItem.MinionData != null)
      {
         return minionItem.MinionData.MinionIndex;
      }

      return int.MaxValue;
   }

   private ItemData GetItemByIndex(int itemIndex)
   {
      if (CachedItems != null)
      {
         return CachedItems.Find(item => item != null && item.ItemIndex == itemIndex);
      }
      return null;
   }

   [System.Serializable]
   public class ShopItemEntry
   {
      public int ItemIndex; // 정렬용
      public string ItemName; // 실제 데이터 확인용
      public int Count;
      public long LastPurchaseDateTicks;
      public int MinionIndex = -1; // minion index 저장 (없으면 -1)

      public System.DateTime LastPurchaseDate
      {
         get
         {
            if (LastPurchaseDateTicks > 0)
               return new System.DateTime(LastPurchaseDateTicks);
            else
               return System.DateTime.MinValue;
         }
         set
         {
            LastPurchaseDateTicks = value.Ticks;
         }
      }
   }
}
