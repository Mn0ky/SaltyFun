using System;
using UnityEngine;

namespace SaltyFun
{
    class ItemSaveable
    {
        private Texture2D _itemIcon;

        public string ItemTitle { get; set; } = "";

        public string ItemFlareText { get; set; } = "";

        public string ItemPrice { get; set; } = "";

        public string ItemType { get; set; } = "";

        public string ItemStatusEffect { get; set; } = "";

        public string ItemEffectAmount { get; set; } = "";

        public string ItemIcon
        {
            set
            {
                byte[] imgData = Convert.FromBase64String(value);

                var textureImg = new Texture2D(64, 64);
                textureImg.LoadImage(imgData);

                _itemIcon = textureImg;
            }
        }

        public Texture2D ItemIconTextureImage => _itemIcon;

        //public string ItemRarityColorHex { get; set; } = "";

        public string ItemRarityName { get; set; } = "";

    }
}