using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using NeaProject.Classes;
using NeaProject.Engine;
using SkiaSharp.Views.Blazor;
using SkiaSharp;
using Blazored.LocalStorage;

namespace NeaProject.Pages
{
    public partial class Index
    {
        [Inject] NavigationManager? NavigationManager { get; set; }
        [Inject] ILocalStorageService? LocalStorage { get; set; }

        // Worth noting that the renderer is about 50% slower when you use Web GL. My assumption is that you end up
        // interop-ing more than it seems, another copy of the byte array for the surface would cause about the level of
        // slowdown I saw. But 2D canvas is fine - after all, just a 2D game.

        private Map? _map;
        private Player? _player;
        private Dictionary<char, Sprite?>? _sprites;
        private FpsCounter? _fpsCounter;
        private Renderer? _renderer;
        private SKBitmap? _bitmap;
        private readonly List<Npc> _npcs = new();
        private int currentCount = 0;
        private string? lastPressed;
        private ElementReference buttonRef;

        // NPCs

        private FinalBoss? _finalBoss;

        private void IncrementCount()
        {
            currentCount++;
        }


        private async Task SaveDataAsync()
        {
            if (LocalStorage == null)
            { return; }
            await LocalStorage.SetItemAsync("player", _player);
            await buttonRef.FocusAsync();
        }

        private async Task ClearDataAsync()
        {
            if (LocalStorage == null)
            { return; }
            await LocalStorage.RemoveItemAsync("player");
            await buttonRef.FocusAsync();
        }

        // Movement
        private void MoveUp()
        {
            if (_player == null || _player.IsDead() || _player.HasWon() || _map == null || _renderer == null)
            {
                return;
            }
            _player.DirectionFacing = 'U';
            _player.FrameIndex = 3;
            _player.Move(_map, 0, -1);
            _renderer.MoveCamera();
        }
        private void MoveRight()
        {
            if (_player == null || _player.IsDead() || _player.HasWon() || _map == null || _renderer == null)
            {
                return;
            }
            _player.DirectionFacing = 'R';
            _player.FrameIndex = 1;
            _player.Move(_map, 1, 0);
            _renderer.MoveCamera();
        }
        private void MoveDown()
        {
            if (_player == null || _player.IsDead() || _player.HasWon() || _map == null || _renderer == null)
            {
                return;
            }
            _player.DirectionFacing = 'D';
            _player.FrameIndex = 0;
            _player.Move(_map, 0, 1);
            _renderer.MoveCamera();
        }
        private void MoveLeft()
        {
            if (_player == null || _player.IsDead() || _player.HasWon() || _map == null || _renderer == null)
            {
                return;
            }
            _player.DirectionFacing = 'L';
            _player.FrameIndex = 2;
            _player.Move(_map, -1, 0);
            _renderer.MoveCamera();
        }

        private void KeyDown(KeyboardEventArgs keyEvent)
        {

            var pressedKey = keyEvent.Key;
            lastPressed = pressedKey;
            pressedKey = pressedKey.ToLower();
            switch (pressedKey)
            {
                case "arrowup":
                case "w":
                    {
                        MoveUp();
                        break;
                    }
                case "arrowdown":
                case "s":
                    {
                        MoveDown();
                        break;
                    }
                case "arrowleft":
                case "a":
                    {
                        MoveLeft();
                        break;
                    }
                case "arrowright":
                case "d":
                    {
                        MoveRight();
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (LocalStorage == null||NavigationManager == null)
            { return; }
            var tileSheetUri = new Uri($"{NavigationManager.Uri}images/MapTiles/all_tiles.png?_={DateTime.Now}");
            var mapUri = new Uri($"{NavigationManager.Uri}map-data/map_0.txt?_={DateTime.Now}");
            string mapString = await DownloadAsync(mapUri);
            _map = new Map(mapString);
            if (await LocalStorage.ContainKeyAsync("player"))
            {
                _player = await LocalStorage.GetItemAsync<Player>("player");
            }
            else
            {
                _player = new Player
                {
                    CurrentHp = 100,
                    // XPos = _map.Width / 2,
                    // YPos = _map.Height / 2,
                    XPos = 5,
                    YPos = 5,
                    Name = "Mellie",
                    AllowedTiles = new List<char> { 'g', 's', 'w' }
                };
            }
            ImageLoader imageLoader = new();
            SKBitmap mapTileSheet = await ImageLoader.GetBitmapAsync(tileSheetUri);
            _sprites = new Dictionary<char, Sprite?>()
        {
            { '.', null},
            { 'd', new Sprite(mapTileSheet, "Diamond", 1)},
            { 'f', new Sprite(mapTileSheet, "FinalBoss", 4)},
            { 'g', new Sprite(mapTileSheet, "Grass", 1)},
            { 'p', new Sprite(mapTileSheet, "Player", 4)},
            { 'r', new Sprite(mapTileSheet, "Rock", 1)},
            { 's', new Sprite(mapTileSheet, "Sand", 1)},
            { 'w', new Sprite(mapTileSheet, "Water", 1)}
        };
            _finalBoss = new FinalBoss { Name = "Mellow" };

            _fpsCounter = new FpsCounter();
            _renderer = new Renderer(_map, _player, _sprites);
            _bitmap = new SKBitmap(_renderer.ViewportWidth, _renderer.ViewportHeight);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await buttonRef.FocusAsync();
            }
        }

        protected void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            if (_renderer == null || _bitmap == null || _fpsCounter == null)
            {
                return;
            }
            // There is a good article on the different ways to update pixel data here:
            //  https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits
            // Having tried them all maintaining and then setting the pixel byte array is the most performant for us.

            unsafe
            {
                fixed (uint* ptr = _renderer.UpdateFrameBuffer())
                {
                    _bitmap.SetPixels((IntPtr)ptr);
                }
            }

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(_bitmap, new SKRect(0, 0, _renderer.ViewportWidth * 1, _renderer.ViewportHeight * 1));
            var fps = _fpsCounter.GetCurrentFps();
            using var paint = new SKPaint
            {
                IsAntialias = true,
                StrokeWidth = 5f,
                StrokeCap = SKStrokeCap.Round,
                TextAlign = SKTextAlign.Center,
                TextSize = 24,
            };

            var surfaceSize = e.Info.Size;
            canvas.DrawText($"{fps:0.00}fps", surfaceSize.Width / 2, surfaceSize.Height - 10f, paint);
        }

        public async static Task<string> DownloadAsync(Uri uri)
        {
            using var client = new HttpClient();
            var content = await client.GetStringAsync(uri);
            return content;
        }


    }
}
