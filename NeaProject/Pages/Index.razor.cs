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

        private Game? _game;
        private Npc? _talkingToNpc;
        private Dictionary<char, Sprite?>? _sprites;
        private FpsCounter? _fpsCounter;
        private Renderer? _renderer;
        private SKBitmap? _bitmap;
        private int currentCount = 0;
        private string? lastPressed;
        private ElementReference buttonRef;

        private void IncrementCount()
        {
            currentCount++;
        }


        private async Task SaveDataAsync()
        {
            if (LocalStorage == null)
            { return; }
            await LocalStorage.SetItemAsync("game", _game);
            await buttonRef.FocusAsync();
        }

        private async Task ClearDataAsync()
        {
            if (LocalStorage == null)
            { return; }
            await LocalStorage.RemoveItemAsync("game");
            await buttonRef.FocusAsync();
        }

        private void KeyDown(KeyboardEventArgs keyEvent)
        {
            if (_renderer == null || _game == null)
            { return; }
            var pressedKey = keyEvent.Key;
            lastPressed = pressedKey;
            pressedKey = pressedKey.ToLower();
            switch (pressedKey)
            {
                case "arrowup":
                case "w":
                    {
                        _game.MoveUp();
                        break;
                    }
                case "arrowdown":
                case "s":
                    {
                        _game.MoveDown();
                        break;
                    }
                case "arrowleft":
                case "a":
                    {
                        _game.MoveLeft();
                        break;
                    }
                case "arrowright":
                case "d":
                    {
                        _game.MoveRight();
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
            _renderer.MoveCamera(_game.Camera);
        }

        protected override async Task OnInitializedAsync()
        {
            if (LocalStorage == null||NavigationManager == null)
            { return; }
            var tileSheetUri = new Uri($"{NavigationManager.Uri}images/MapTiles/all_tiles.png?_={DateTime.Now}");
            
            if (await LocalStorage.ContainKeyAsync("game"))
            {
                _game = await LocalStorage.GetItemAsync<Game>("game");
            }
            else
            {
                var mapUri = new Uri($"{NavigationManager.Uri}map-data/map_0.txt?_={DateTime.Now}");
                string mapString = await DownloadAsync(mapUri);
                _game = new Game(mapString);
            }

            ImageLoader imageLoader = new(tileSheetUri);
            SKBitmap mapTileSheet = await imageLoader.GetBitmapAsync();
            _sprites = new Dictionary<char, Sprite?>()
            {
                //teleporters
                { '0', new Sprite(mapTileSheet, "Teleport0", 1)},
                { '1', new Sprite(mapTileSheet, "Teleport1", 1)},
                { '2', new Sprite(mapTileSheet, "Teleport2", 1)},
                { '3', new Sprite(mapTileSheet, "Teleport3", 1)},
                { '4', new Sprite(mapTileSheet, "Teleport4", 1)},

                //various cacti
                { 'c', new Sprite(mapTileSheet, "Cactus", 1)},
                { 'ç', new Sprite(mapTileSheet, "Cactus1", 1)},
                { 'ć', new Sprite(mapTileSheet, "Cactus2", 1)},
                { 'ĉ', new Sprite(mapTileSheet, "Cactus3", 1)},
                { 'č', new Sprite(mapTileSheet, "Cactus4", 1)},

                //grass
                { 'g', new Sprite(mapTileSheet, "Grass", 1)},
                { 'ĝ', new Sprite(mapTileSheet, "Grass1", 1)},
                { 'ğ', new Sprite(mapTileSheet, "Grass2", 1)},
                { 'ġ', new Sprite(mapTileSheet, "Grass3", 1)},
                { 'ģ', new Sprite(mapTileSheet, "Grass4", 1)},

                //npcs - described by a capital
                { 'B', new Sprite(mapTileSheet, "Bird", 2)},
                { 'F', new Sprite(mapTileSheet, "FinalBoss", 4)},
                { 'S', new Sprite(mapTileSheet, "Snake", 2) },

                //misc other
                { '.', null},
                { '□', new Sprite(mapTileSheet, "Space", 1)},
                { 'd', new Sprite(mapTileSheet, "Diamond", 1)}, //currently actually flowerbundle
                { 'm', new Sprite(mapTileSheet, "MarsRock", 1)},
                { 'p', new Sprite(mapTileSheet, "Player", 4)},
                { 'r', new Sprite(mapTileSheet, "Rock", 1)},
                { 's', new Sprite(mapTileSheet, "Sand", 1)},
                { 't', new Sprite(mapTileSheet, "Tree", 1)},
                { 'w', new Sprite(mapTileSheet, "Water", 1)}
            };

            _fpsCounter = new FpsCounter();
            _renderer = new Renderer(_game, _sprites);
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
            if (_renderer == null || _bitmap == null || _fpsCounter == null || _game == null)
            {
                return;
            }
            // There is a good article on the different ways to update pixel data here:
            //  https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits
            // Having tried them all maintaining and then setting the pixel byte array is the most performant for us.

            unsafe
            {
                fixed (uint* ptr = _renderer.UpdateFrameBuffer(_game))
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
