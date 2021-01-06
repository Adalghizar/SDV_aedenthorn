﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Swim
{
    internal class SwimUtils
    {
        private static IMonitor Monitor;
        private static ModConfig Config;
        private static IModHelper Helper;
        public static Dictionary<string, string> seaMonsterSounds = new Dictionary<string, string>() {
            {"A","dialogueCharacter"},
            {"B","grunt"},
            {"C","throwDownITem"},
            {"D","stoneStep"},
            {"E","thudStep"},
            {"F","toolSwap"},
            {"G","bob"},
            {"H","dwoop"},
            {"I","ow"},
            {"J","breathin"},
            {"K","boop"},
            {"L","flute"},
            {"M","backpackIN"},
            {"N","croak"},
            {"O","flybuzzing"},
            {"P","skeletonStep"},
            {"Q","dustMeep"},
            {"R","throw"},
            {"S","shadowHit"},
            {"T","slingshot"},
            {"U","dwop"},
            {"V","fishingRodBend"},
            {"W","Cowboy_Footstep"},
            {"X","junimoMeep1"},
            {"Y","fallDown"},
            {"Z","harvest"},
        };
        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }

        public static Point GetEdgeWarpDestination(int idxPos, EdgeWarp edge)
        {
            try
            {
                int idx = 1 + idxPos - edge.FirstTile;
                int length = 1 + edge.LastTile - edge.FirstTile;
                int otherLength = 1 + edge.OtherMapLastTile - edge.OtherMapFirstTile;
                int otherIdx = (int)Math.Round((idx / (float)length) * otherLength);
                int tileIdx = edge.OtherMapFirstTile - 1 + otherIdx;
                if (edge.DestinationHorizontal == true)
                {
                    Monitor.Log($"idx {idx} length {length} otherIdx {otherIdx} tileIdx {tileIdx} warp point: {tileIdx},{edge.OtherMapIndex}");
                    return new Point(tileIdx, edge.OtherMapIndex);
                }
                else
                {
                    Monitor.Log($"warp point: {edge.OtherMapIndex},{tileIdx}");
                    return new Point(edge.OtherMapIndex, tileIdx);
                }
            }
            catch
            {

            }
            return Point.Zero;
        }

        public static void DiveTo(DiveLocation diveLocation, Farmer farmer)
        {
            DivePosition dp = diveLocation.OtherMapPos;
            if (dp == null)
            {
                Monitor.Log($"Diving to existing tile position");
                Point pos = farmer.getTileLocationPoint();
                dp = new DivePosition()
                {
                    X = pos.X,
                    Y = pos.Y
                };
            }
            if (!IsMapUnderwater(farmer.currentLocation.Name))
            {
                ModEntry.bubbles.Clear();
            }
            else
            {
            }

            Game1.playSound("pullItemFromWater");
            Game1.warpFarmer(diveLocation.OtherMapName, dp.X, dp.Y, false);
        }
        public static int MaxOxygen()
        {
            return Game1.player.MaxStamina * Math.Max(1, Config.OxygenMult);
        }

        public static bool IsMapUnderwater(string name)
        {
            return ModEntry.diveMaps.ContainsKey(name) && ModEntry.diveMaps[name].Features.Contains("Underwater");
        }


        public static void CheckIfMyButtonDown()
        {
            if (Game1.player == null || Game1.player.currentLocation == null || !Config.ReadyToSwim || Game1.player.currentLocation.waterTiles == null || !Context.IsPlayerFree || Helper.Input.IsDown(SButton.LeftShift))
            {
                ModEntry.myButtonDown = false;
                return;
            }

            List<SButton> dirButtons = new List<SButton>();
            foreach (InputButton ib in Game1.options.moveUpButton)
            {
                dirButtons.Add(ib.ToSButton());
            }
            foreach (InputButton ib in Game1.options.moveDownButton)
            {
                dirButtons.Add(ib.ToSButton());
            }
            foreach (InputButton ib in Game1.options.moveRightButton)
            {
                dirButtons.Add(ib.ToSButton());
            }
            foreach (InputButton ib in Game1.options.moveLeftButton)
            {
                dirButtons.Add(ib.ToSButton());
            }

            foreach (SButton b in dirButtons)
            {
                if (Helper.Input.IsDown(b))
                {
                    ModEntry.myButtonDown = true;
                    return;
                }
            }

            if (Helper.Input.IsDown(SButton.MouseLeft) && !(Game1.player.CurrentTool is StardewValley.Tools.Pan) && !(Game1.player.CurrentTool is StardewValley.Tools.FishingRod) && Config.EnableClickToSwim)
            {
                ModEntry.myButtonDown = true;
                return;
            }

            ModEntry.myButtonDown = false;
        }

        public static int CheckForBuriedItem(Farmer who)
        {
            int objectIndex = 330;
            if (Game1.random.NextDouble() < 0.1)
            {
                if (Game1.random.NextDouble() < 0.75)
                {
                    switch (Game1.random.Next(5))
                    {
                        case 0:
                            objectIndex = 96;
                            break;
                        case 1:
                            objectIndex = (who.hasOrWillReceiveMail("lostBookFound") ? ((Game1.netWorldState.Value.LostBooksFound < 21) ? 102 : 770) : 770);
                            break;
                        case 2:
                            objectIndex = 110;
                            break;
                        case 3:
                            objectIndex = 112;
                            break;
                        case 4:
                            objectIndex = 585;
                            break;
                    }
                }
                else if (Game1.random.NextDouble() < 0.75)
                {
                    var r = Game1.random.NextDouble();

                    if (r < 0.75)
                    {
                        objectIndex = ((Game1.random.NextDouble() < 0.5) ? 121 : 97);
                    }
                    else if (r < 0.80)
                    {
                        objectIndex = 99;
                    }
                    else
                    {
                        objectIndex = ((Game1.random.NextDouble() < 0.5) ? 122 : 336);
                    }
                }
                else
                {
                    objectIndex = ((Game1.random.NextDouble() < 0.5) ? 126 : 127);
                }
            }
            else
            {
                if (Game1.random.NextDouble() < 0.5)
                {
                    objectIndex = 330;
                }
                else
                {
                    if (Game1.random.NextDouble() < 0.25)
                    {
                        objectIndex = 749;
                    }
                    else if (Game1.random.NextDouble() < 0.5)
                    {
                        var r = Game1.random.NextDouble();
                        if (r < 0.7)
                        {
                            objectIndex = 535;
                        }
                        else if (r < 8.5)
                        {
                            objectIndex = 537;
                        }
                        else
                        {
                            objectIndex = 536;
                        }
                    }
                }
            }
            return objectIndex;
        }

        public static bool IsWearingScubaGear(Farmer farmer)
        {
            bool tank = ModEntry.scubaTankID != -1 && farmer.shirtItem != null && farmer.shirtItem.Value != null && farmer.shirtItem.Value.parentSheetIndex != null && farmer.shirtItem.Value.parentSheetIndex == ModEntry.scubaTankID;
            bool mask = ModEntry.scubaMaskID != -1 && farmer.hat != null && farmer.hat.Value != null && farmer.hat.Value.which != null && farmer.hat.Value.which == ModEntry.scubaMaskID;

            return tank && mask;
        }

        public static bool IsInWater(Farmer farmer)
        {
            var tiles = farmer.currentLocation.waterTiles;
            Point p = farmer.getTileLocationPoint();

            if (!farmer.swimming && farmer.currentLocation.map.GetLayer("Buildings").PickTile(new Location(p.X, p.Y) * Game1.tileSize, Game1.viewport.Size) != null)
                return false;

            return IsMapUnderwater(farmer.currentLocation.Name)
                ||
                (
                    tiles != null
                    && 
                    (
                        (p.X >= 0 && p.Y >= 0 && tiles.GetLength(0) > p.X && tiles.GetLength(1) > p.Y && tiles[p.X, p.Y])
                        ||
                        (
                            farmer.swimming 
                            &&
                            (p.X < 0 || p.Y < 0 || tiles.GetLength(0) <= p.X || tiles.GetLength(1) <= p.Y)
                        )
                    )
                );
        }

        public static List<Vector2> GetTilesInDirection(int count, Farmer farmer)
        {
            List<Vector2> tiles = new List<Vector2>();
            int dir = farmer.facingDirection;
            if (dir == 1)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(farmer.getTileLocation() + new Vector2(i, 0));
                }

            }

            if (dir == 2)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(farmer.getTileLocation() + new Vector2(0, i));
                }

            }

            if (dir == 3)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(farmer.getTileLocation() - new Vector2(i, 0));
                }

            }

            if (dir == 0)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(farmer.getTileLocation() - new Vector2(0, i));
                }

            }

            return tiles;

        }

        public static Vector2 GetNextTile(Farmer farmer)
        {
            int dir = farmer.facingDirection;
            if (dir == 1)
            {

                return farmer.getTileLocation() + new Vector2(1, 0);

            }

            if (dir == 2)
            {

                return farmer.getTileLocation() + new Vector2(0, 1);

            }

            if (dir == 3)
            {

                return farmer.getTileLocation() - new Vector2(1, 0);

            }

            if (dir == 0)
            {

                return farmer.getTileLocation() - new Vector2(0, 1);
            }
            return Vector2.Zero;
        }

        public static void MakeOxygenBar(int current, int max)
        {
            ModEntry.OxygenBarTexture = new Texture2D(Game1.graphics.GraphicsDevice, (int)Math.Round(Game1.viewport.Width * 0.74f), 30);
            Color[] data = new Color[ModEntry.OxygenBarTexture.Width * ModEntry.OxygenBarTexture.Height];
            ModEntry.OxygenBarTexture.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                if (i <= ModEntry.OxygenBarTexture.Width || i % ModEntry.OxygenBarTexture.Width == ModEntry.OxygenBarTexture.Width - 1)
                {
                    data[i] = new Color(0.5f, 1f, 0.5f);
                }
                else if (data.Length - i < ModEntry.OxygenBarTexture.Width || i % ModEntry.OxygenBarTexture.Width == 0)
                {
                    data[i] = new Color(0, 0.5f, 0);
                }
                else if ((i % ModEntry.OxygenBarTexture.Width) / (float)ModEntry.OxygenBarTexture.Width < (float)current / (float)max)
                {
                    data[i] = Color.Green;
                }
                else
                {
                    data[i] = Color.Black;
                }
            }
            ModEntry.OxygenBarTexture.SetData<Color>(data);
        }

        public static string doesTileHaveProperty(Map map, int xTile, int yTile, string propertyName, string layerName)
        {
            PropertyValue property = null;
            if (map != null && map.GetLayer(layerName) != null)
            {
                Tile tmp = map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size);
                if (tmp != null)
                {
                    tmp.TileIndexProperties.TryGetValue(propertyName, out property);
                }
                if (property == null && tmp != null)
                {
                    map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size).Properties.TryGetValue(propertyName, out property);
                }
            }
            if (property != null)
            {
                return property.ToString();
            }
            return null;
        }

        public static void ReadDiveMapData(DiveMapData data)
        {
            foreach (DiveMap map in data.Maps)
            {
                if (!ModEntry.diveMaps.ContainsKey(map.Name))
                {
                    ModEntry.diveMaps.Add(map.Name, map);
                    Monitor.Log($"added dive map info for {map.Name}", LogLevel.Debug);
                }
                else
                {
                    Monitor.Log($"dive map info already exists for {map.Name}", LogLevel.Warn);
                }
            }
        }
        public static async void SeaMonsterSay(string speech)
        {
            foreach (char c in speech)
            {
                string s = c.ToString().ToUpper();
                if (seaMonsterSounds.ContainsKey(s))
                {
                    Game1.playSoundPitched("junimoMeep1", (seaMonsterSounds.Keys.ToList().IndexOf(s) / 26) * 2 - 1);
                }
                await Task.Delay(100);
            }
        }

        public static bool IsWaterTile(Vector2 tilePos)
        {
            if (Game1.player.currentLocation != null && Game1.player.currentLocation.waterTiles != null && tilePos.X >= 0 && tilePos.Y >= 0 && Game1.player.currentLocation.waterTiles.GetLength(0) > tilePos.X && Game1.player.currentLocation.waterTiles.GetLength(1) > tilePos.Y)
            {
                return Game1.player.currentLocation.waterTiles[(int)tilePos.X, (int)tilePos.Y];
            }
            return false;
        }

        public static bool IsTilePassable(GameLocation location, Location tileLocation, xTile.Dimensions.Rectangle viewport)
        {
            PropertyValue passable = null;
            Microsoft.Xna.Framework.Rectangle tileLocationRect = new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
            Tile tmp = location.map.GetLayer("Back").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (tmp != null)
            {
                tmp.TileIndexProperties.TryGetValue("Passable", out passable);
            }
            Tile tile = location.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (location.largeTerrainFeatures != null)
            {
                using (List<LargeTerrainFeature>.Enumerator enumerator = location.largeTerrainFeatures.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.getBoundingBox().Intersects(tileLocationRect))
                        {
                            return false;
                        }
                    }
                }
            }
            Vector2 vLocation = new Vector2(tileLocation.X, tileLocation.Y);
            if (location.terrainFeatures.ContainsKey(vLocation) && tileLocationRect.Intersects(location.terrainFeatures[vLocation].getBoundingBox(vLocation)) && (!location.terrainFeatures[vLocation].isPassable(null) || (location.terrainFeatures[vLocation] is HoeDirt && ((HoeDirt)location.terrainFeatures[vLocation]).crop != null)))
            {
                return false;
            }
            bool result = passable == null && tile == null && tmp != null;
            return result;
        }
        public static bool DebrisIsAnItem(Debris debris)
        {
            return debris.debrisType == Debris.DebrisType.OBJECT || debris.debrisType == Debris.DebrisType.ARCHAEOLOGY || debris.debrisType == Debris.DebrisType.RESOURCE || debris.item != null;
        }
    }
}