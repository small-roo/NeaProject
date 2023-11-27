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

        private Game _game = new();
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

        private async Task LoadDataAsync()
        {
            if (LocalStorage == null)
            { return; }
            _game = await LocalStorage.GetItemAsync<Game>("game");
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
            if (_renderer == null)
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
                await LoadDataAsync();
            }
            else
            {
                var mapUri = new Uri($"{NavigationManager.Uri}map-data/map_0.txt?_={DateTime.Now}");
                string mapString = await DownloadAsync(mapUri);
                _game.Map = new Map(mapString);
                _game.Player = new Player
                {
                    CurrentHp = 100,
                    // XPos = _map.Width / 2,
                    // YPos = _map.Height / 2,
                    XPos = 10,
                    YPos = 5,
                    Name = "Mellie",
                    SpriteRef = 'p',
                    AllowedTiles = new List<char> { 'g', 'm', 's', 'w' }
                };
                SetUpNpcs(_game.Map);
            }
            ImageLoader imageLoader = new(tileSheetUri);
            SKBitmap mapTileSheet = await imageLoader.GetBitmapAsync();
            _sprites = new Dictionary<char, Sprite?>()
            {
                { '.', null},
                { '□', new Sprite(mapTileSheet, "Space", 1)},
                { '0', new Sprite(mapTileSheet, "Teleport0", 1)},
                { '1', new Sprite(mapTileSheet, "Teleport1", 1)},
                { '2', new Sprite(mapTileSheet, "Teleport2", 1)},
                { '3', new Sprite(mapTileSheet, "Teleport3", 1)},
                { '4', new Sprite(mapTileSheet, "Teleport4", 1)},
                { 'd', new Sprite(mapTileSheet, "Diamond", 1)}, //currently unused                
                { 'g', new Sprite(mapTileSheet, "Grass", 1)},
                { 'm', new Sprite(mapTileSheet, "MarsRock", 1)},
                { 'p', new Sprite(mapTileSheet, "Player", 4)},
                { 'r', new Sprite(mapTileSheet, "Rock", 1)},
                { 's', new Sprite(mapTileSheet, "Sand", 1)},
                { 't', new Sprite(mapTileSheet, "Tree", 1)},
                { 'w', new Sprite(mapTileSheet, "Water", 1)},
                { 'B', new Sprite(mapTileSheet, "Bird", 2)}, //capital describes an NPC
                { 'F', new Sprite(mapTileSheet, "FinalBoss", 4)},
                { 'S', new Sprite(mapTileSheet, "Snake", 2) }
            };

            _fpsCounter = new FpsCounter();
            _renderer = new Renderer(_game, _sprites);
            _bitmap = new SKBitmap(_renderer.ViewportWidth, _renderer.ViewportHeight);
        }

        private void SetUpNpcs(Map map)
        {
            _game.Npcs.Clear();
            Random random = new();
            for (int birdNumber = 1; birdNumber <= 20; birdNumber++) //bird
            {
                BirdEnemy bird = new()
                {
                    Name = $"bird{birdNumber}",
                    SpriteRef = 'B',
                    XPos = random.Next(0, map.Width),
                    YPos = random.Next(0, map.Height),
                    FrameIndex = random.Next(0, 2),
                };
                while (map.GetOverlayTileChar(bird.XPos, bird.YPos) != '.' || map.GetTileChar(bird.XPos, bird.YPos) != 'g')
                {
                    bird.XPos = random.Next(0, map.Width);
                    bird.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(bird.XPos, bird.YPos, bird.SpriteRef);
                _game.Npcs.Add(bird);
            }
            FinalBoss finalBoss = new() //final boss
            { 
                Name = "Mellow", 
                SpriteRef = 'F', 
                XPos = 29, YPos = 13 
            };
            for (int snakeNumber = 1; snakeNumber <= 10; snakeNumber++) //snake
            {
                SnakeEnemy snake = new()
                {
                    Name = $"snake{snakeNumber}",
                    SpriteRef = 'S',
                    XPos = random.Next(0,map.Width),
                    YPos = random.Next(0,map.Height),
                    FrameIndex = random.Next(0,2)
                };
                while (map.GetOverlayTileChar(snake.XPos, snake.YPos) != '.' || map.GetTileChar(snake.XPos, snake.YPos) != 'm')
                { 
                    snake.XPos = random.Next(0, map.Width);
                    snake.YPos = random.Next(0, map.Height);
                }
                map.SetOverlayTileChar(snake.XPos, snake.YPos, snake.SpriteRef);
                _game.Npcs.Add(snake);
            }
            map.SetOverlayTileChar(finalBoss.XPos, finalBoss.YPos, finalBoss.SpriteRef);
            _game.Npcs.Add(finalBoss);
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
                fixed (uint* ptr = _renderer.UpdateFrameBuffer(_game.Camera))
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
