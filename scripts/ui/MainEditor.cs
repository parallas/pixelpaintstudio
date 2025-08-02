using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot.Collections;
using Parallas;

public partial class MainEditor : Control
{
    [Export] public Toolbar Toolbar { get; set; }
    [Export] public Topbar Topbar { get; set; }
    [Export] private AspectRatioContainer _aspectRatioContainer;
    [Export] private SubViewport _unscaledViewport;
    [Export] private SubViewport _finalAccumulativeViewport;
    [Export] public ToolState DefaultToolState;
    [Export] public PackedScene PlayerCanvasScene;
    [Export] public TextureRect UnscaledBackBufferTextureRect;
    [Export] public TextureRect ScaledBackBufferTextureRect;
    [Export] public Vector2I Resolution { get; private set; } = new Vector2I(640, 360);
    public Rect2I SizeRect => new Rect2I(Vector2I.Zero, Resolution);

    public readonly System.Collections.Generic.Dictionary<int, ToolState> PlayerToolStates = new();
    public readonly System.Collections.Generic.Dictionary<int, DrawState> PlayerDrawStates = new();
    public readonly System.Collections.Generic.Dictionary<int, PlayerCanvas> PlayerCanvases = new();

    public int PrimaryPlayerId { get; set; }

    public static readonly List<BrushDefinition> MagicWandDefinitions =
    [
        // GD.Load<BrushDefinition>("res://drawing_resources/brushes/wands/brush_sourgummyworm.tres"),
        // GD.Load<BrushDefinition>("res://drawing_resources/brushes/wands/brush_christmaslights.tres"),
        // GD.Load<BrushDefinition>("res://drawing_resources/brushes/wands/brush_scribble.tres"),
        // GD.Load<BrushDefinition>("res://drawing_resources/brushes/wands/brush_toothpaste.tres"),
        // GD.Load<BrushDefinition>("res://drawing_resources/brushes/wands/brush_sparkle_orbit.tres"),
    ];
    public static readonly List<InkDefinition> AllInkDefinitions =
    [
        // Base Colors (16/16)
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_00_red.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_01_orange.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_02_yellow.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_03_lime.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_04_green.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_05_blue.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_06_indigo.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_07_violet.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_08_pink.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_09_skin_light.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_10_skin_medium.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_11_skin_tan.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_12_skin_dark.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_13_black.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_14_gray.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_15_white.tres"),

        // Gradients (7/16)
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_rainbow.tres"),
        InkDefinition.FromColors(["ff0000", "ffff00", "0000ff"], 0.75f), // Red Yellow Blue
        InkDefinition.FromColors(["ff0000", "00ff00", "0000ff"], 0.75f), // Red Green Blue
        InkDefinition.FromColors(["00aaff", "ffaa00"], 0.75f, repeatCount: 3), // PixelDough Stripes
        InkDefinition.FromColors(["1e1f21", "85c46c"], 0.85f, true), // Mimi Eyes
        InkDefinition.FromColors(["0033aa", "00aaff"], 0.1f, repeatCount: 2), // Watery
        InkDefinition.FromColors(["aa3300", "ffaa00"], 0.1f, repeatCount: 2), // Lavay
        InkDefinition.FromColors(["00aa33", "00ffaa"], 0.1f, repeatCount: 2), // Acidy
        InkDefinition.FromColors(["ff00aa", "aa00ff"], 0.1f, repeatCount: 2), // Poisony
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_metallic.tres"),
        null,
        null,
        null,
        null,
        null,
        null,

        // Pride (16/16)
        InkDefinition.FromColors(["e60001", "ff8d02", "ffed06", "008220", "004dfe", "760088"], 0.8f), // Pride
        InkDefinition.FromColors(["5acffb", "f5abb9", "ffffff", "f5abb9", "5acffb"], 0.8f), // Trans Pride
        InkDefinition.FromColors(["f5ea29", "ffffff", "9a59ce", "2d2d2d"], 0.8f), // NB Pride
        InkDefinition.FromColors(["ffd900", "ffd900", "8d01af", "ffd900", "ffd900"], 0.8f), // Intersex Pride
        InkDefinition.FromColors(["fe75a0", "f5f5f5", "bf16d5", "2c2c2c", "333ebc"], 0.8f), // Genderfluid Pride
        InkDefinition.FromColors(["b899de", "ffffff", "6b8e3b"], 0.8f), // Genderqueer Pride
        InkDefinition.FromColors(["000000", "b9b9b9", "ffffff", "b8f484", "ffffff", "b9b9b9", "000000"], 0.8f), // Agender Pride
        InkDefinition.FromColors(["c57aa1", "eba6cb", "d6c6e7", "ffffff", "d6c6e7", "9ac6e7", "6b83cf"], 0.8f), // Bigender Pride
        InkDefinition.FromColors(["7f7f7f", "c3c3c3", "fbff73", "ffffff", "fbff73", "c3c3c3", "7f7f7f"], 0.8f), // Demigender Pride
        InkDefinition.FromColors(["d62c00", "ff9956", "ffffff", "d363a4", "a40062"], 0.8f), // Lesbian Pride
        InkDefinition.FromColors(["068e6f", "27cfaa", "98e9c1", "ffffff", "7aace3", "5049cb", "3c1a77"], 0.8f), // Gay Pride
        InkDefinition.FromColors(["d70271", "d70271", "734e97", "0038a8", "0038a8"], 0.8f), // Bi Pride
        InkDefinition.FromColors(["ff218b", "ffd900", "1fb3fe"], 0.8f), // Pan Pride
        InkDefinition.FromColors(["010101", "a0a0a0", "ffffff", "9a0878"], 0.8f), // Ace Pride
        InkDefinition.FromColors(["f61db9", "05d569", "1d92f6"], 0.8f), // Polysexual Pride
        InkDefinition.FromColors(["623804", "d46400", "ffdd64", "fee6b8", "ffffff", "555555", "000000"], 0.8f), // Bear Pride

        // Holidays (10/16)
        InkDefinition.FromColors(["ffff00", "ffaa00", "eef7f8"], 0.75f), // Candy Corn
        InkDefinition.FromColors(["000000", "ffaa00"], 0.75f, repeatCount: 3), // Halloween Stripes (Black Orange)
        InkDefinition.FromColors(["aa00ff", "ffaa00"], 0.75f, repeatCount: 3), // Halloween Stripes (Purple Orange)
        InkDefinition.FromColors(["000000", "ffaa00", "aa00ff", "00aa33"], 0.75f, repeatCount: 2), // Halloween Stripes (Multicolor)
        InkDefinition.FromColors(["ff0000", "eef7f8"], 0.85f, repeatCount: 3), // Candy Cane (Red White)
        InkDefinition.FromColors(["00aa33", "eef7f8"], 0.85f, repeatCount: 3), // Candy Cane (Green White)
        InkDefinition.FromColors(["ff0000", "eef7f8", "ffaaaa", "eef7f8"], 0.85f, repeatCount: 2), // Candy Cane (Red White Pink White)
        InkDefinition.FromColors(["ff0000", "eef7f8", "00aa33", "eef7f8"], 0.85f, repeatCount: 2), // Candy Cane (Red White Green White)
        InkDefinition.FromColors(["f8aea4", "f8aea4", "f8aea4", "eef7f8", "eef7f8", "65bb9d", "eef7f8", "f74966", "f74966", "65bb9d"], 0.85f), // Candy Cane (Special)
        InkDefinition.FromColors(["0033ff", "eef7f8"], 0.85f, repeatCount: 3), // Hanukkah
        null,
        null,
        null,
        null,
        null,
        null,

        // Ponies (13/16)
        InkDefinition.FromColors(["263774", "662d8c", "ed428a", "263774"], 0.9f), // Twi
        InkDefinition.FromColors(["feb9dc", "fffaa3"], 0.9f), // Flutter
        InkDefinition.FromColors(["ed458c", "f3b7d2", "7fd1f4", "f3b7d2", "fdf8a9", "f3b7d2"], 0.9f), // Pink
        InkDefinition.FromColors(["674ca0", "edf1f4", "73e7fe", "edf1f4"], 0.9f), // Rarity
        InkDefinition.FromColors(["fff9ac", "fbba63", "ea413e", "fbba63"], 0.9f), // Apple
        InkDefinition.FromColors(["f6545b", "f78145", "f7f2b7", "74c962", "a6e3fe", "37a9e4", "76439b"], 0.9f), // RD
        InkDefinition.FromColors(["61c0cf", "79e3ad", "84a4ea", "e8b0f4"], 0.9f), // Celeste
        InkDefinition.FromColors(["75a5f9", "6469bc", "120a41", "237ce6"], 0.9f), // Moon
        InkDefinition.FromColors(["5cb0e6", "b9e0f9", "d5e9e8"], 0.9f), // Trix
        InkDefinition.FromColors(["9f7fe5", "8ec8e7", "9f7fe5", "f4eb7c"], 0.9f), // TrixHat
        InkDefinition.FromColors(["f2c8f8", "853fb4", "99ead9", "853fb4", "f2c8f8"], 0.9f), // Star
        InkDefinition.FromColors(["fddf80", "ec1641", "ecdb1c", "ec1641", "fddf80"], 0.9f), // Sunset
        InkDefinition.FromColors(["363636", "2d6b7b", "363636", "228278", "3f9c74", "62c16d", "363636"], 0.5f), // Chrysalis
        null,
        null,
        null,
    ];

    public static readonly List<StencilData> AllStencilData =
    [
        new(1, [0b_1]), // full
        new(3, [ // dark
            0b_011,
            0b_111,
            0b_111
        ]),
        new(2, [ // thick
            0b_01,
            0b_11,
        ]),
        new(2, [ // medium
            0b_10,
            0b_01
        ]),
        new(2, [ // light
            0b_10,
            0b_00,
        ]),
        new(3, [ // thin
            0b_100,
            0b_000,
            0b_000
        ]),
        new(2, [ // stripe vert
            0b_10,
            0b_10,
        ]),
        new(2, [ // stripe horiz
            0b_11,
            0b_00,
        ]),
        new(3, [ // diagonal left
            0b_001,
            0b_010,
            0b_100,
        ]),
        new(3, [ // diagonal right
            0b_100,
            0b_010,
            0b_001,
        ]),
        new(4, [ // diamond
            0b_0010,
            0b_0101,
            0b_1000,
            0b_0101,
        ]),
        new(8, [ // checker big
            0b_11110000,
            0b_11110000,
            0b_11110000,
            0b_11110000,
            0b_00001111,
            0b_00001111,
            0b_00001111,
            0b_00001111,
        ]),
        new(16, [ // rocky / leopard print
            0b_0011100001000111,
            0b_0011000001001110,
            0b_0110000011111100,
            0b_0100000111111000,
            0b_0100011110010000,
            0b_1100111100010000,
            0b_1111111000011001,
            0b_1000110000111111,
            0b_0000100000110011,
            0b_0001100001100001,
            0b_1001100001000111,
            0b_1111111111001100,
            0b_0111000011011000,
            0b_0100000111110000,
            0b_1100001111100000,
            0b_1111111001000001
        ]),
        new(16, [ // Noisy Dark
            0b_1110111111101011,
            0b_0111110110111101,
            0b_1101101111010111,
            0b_0111111101111111,
            0b_1110101110111101,
            0b_0111110111101111,
            0b_1011101110110111,
            0b_1110111101011101,
            0b_0101110111110111,
            0b_1111011111011101,
            0b_0111110101111011,
            0b_1101011111111111,
            0b_1111101101011010,
            0b_0110111011111111,
            0b_1111111111101011,
            0b_1011010110111110
        ]),
        new(6, [ // hearts
            0b110110,
            0b111110,
            0b011100,
            0b001000,
            0b000000,
        ]),
        new(6, [ // diamonds
            0b001000,
            0b011100,
            0b111110,
            0b011100,
            0b001000,
            0b000000,
        ]),
        new(6, [ // clubs
            0b_011000,
            0b_011000,
            0b_111110,
            0b_111110,
            0b_001000,
            0b_000000,
        ]),
        new(6, [ // spades
            0b_001000,
            0b_011100,
            0b_111110,
            0b_111110,
            0b_001000,
            0b_000000,
        ]),
        new(16, [ // polka dots big
            0b_0000000000000000,
            0b_0000000000000000,
            0b_0000001111000000,
            0b_0000111111110000,
            0b_0001111111111000,
            0b_0001111111111000,
            0b_0011111111111100,
            0b_0011111111111100,
            0b_0011111111111100,
            0b_0011111111111100,
            0b_0001111111111000,
            0b_0001111111111000,
            0b_0000111111110000,
            0b_0000001111000000,
            0b_0000000000000000,
            0b_0000000000000000,
        ])
    ];

    public override void _Ready()
    {
        base._Ready();

        Input.SetMouseMode(Input.MouseModeEnum.Hidden);
        Godot.Engine.MaxFps = 120;

        CreatePlayerCanvas(0);

        foreach (var fileName in DirAccess.GetFilesAt("res://drawing_resources/brushes/wands/"))
        {
            var replace = fileName;
            if (fileName.GetExtension() == "import")
            {
                replace = fileName.Replace(".import", "");
            }
            MagicWandDefinitions.Add(GD.Load<BrushDefinition>("res://drawing_resources/brushes/wands/" + replace));
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        AddToGroup("main_editor");
        SetResolution(Resolution);
        PlayerDeviceMapper.PlayerCreated += AddPlayerToolState;
        PlayerDeviceMapper.PlayerRemoved += RemovePlayerToolState;

        PlayerDeviceMapper.PlayerCreated += CreatePlayerCanvas;
        PlayerDeviceMapper.PlayerRemoved += DeletePlayerCanvas;

        GetTree().GetRoot().SizeChanged += UpdateUnscaledViewportScale;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup("main_editor");
        PlayerDeviceMapper.PlayerCreated -= AddPlayerToolState;
        PlayerDeviceMapper.PlayerRemoved -= RemovePlayerToolState;

        PlayerDeviceMapper.PlayerCreated -= CreatePlayerCanvas;
        PlayerDeviceMapper.PlayerRemoved -= DeletePlayerCanvas;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var cursors = GetTree().GetNodesInGroup("player_cursors").Cast<GameCursor>();
        var gameCursors = cursors as GameCursor[] ?? cursors.ToArray();

        foreach (var (playerId, drawState) in PlayerDrawStates)
        {
            var drawingCursor = gameCursors.FirstOrDefault(cursor => cursor.PlayerId == playerId, null);
            if (drawingCursor is null) continue;
            if (!PlayerToolStates.TryGetValue(playerId, out var toolState)) continue;

            var color = toolState.InkDefinition.SampleColor(drawState.DrawingTime);
            drawState.Process(GetCanvasPosition(drawingCursor.GlobalPosition), color, delta);
            toolState.ToolDefinition.BrushDefinition?.Process(drawState, delta);

            drawState.CanvasItem?.QueueRedraw();
        }

        if (Input.IsActionJustPressed("quick_export"))
        {
            Input.SetMouseMode(Input.MouseModeEnum.Visible);
            SaveImage();
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);

        // if (@event.IsActionPressed("copy"))
        // {
        //     // ResourceSaver.Save()
        // }

        if (@event.IsActionPressed("paste"))
        {
            if (DisplayServer.ClipboardHasImage())
            {
                var clipboardImage = DisplayServer.ClipboardGetImage();
                clipboardImage.Convert(Image.Format.R8);
                var data = clipboardImage.GetData();
                int height = clipboardImage.GetHeight();
                int width = clipboardImage.GetWidth();
                string[] rowData = new string[height];
                for (int y = 0; y < height; y++)
                {
                    string rowString = "0b_";
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;
                        bool isFull = data[index] != byte.MinValue;
                        rowString += isFull ? "1" : "0";
                    }
                    rowData[y] = rowString;
                }
                GD.Print(rowData.Join(", "));
            }
        }

        var deviceId = PlayerDeviceMapper.GetControllerOffsetDeviceId(@event);
        if (!PlayerDeviceMapper.TryGetPlayerDeviceMapFromDevice(deviceId, out PlayerDeviceMap deviceMap)) return;
        int playerId = deviceMap.PlayerId;

        var gameCursor = GetTree().GetNodesInGroup("player_cursors").Cast<GameCursor>()
            .FirstOrDefault(cursor => cursor.PlayerId == playerId);
        if (gameCursor is null) return;
        if (!PlayerToolStates.TryGetValue(playerId, out ToolState toolState)) return;
        if (!PlayerCanvases.TryGetValue(playerId, out PlayerCanvas playerCanvas)) return;

        if (@event.IsActionPressed("click") && IsWithinCanvas(gameCursor.GlobalPosition))
        {
            if (PlayerDrawStates.ContainsKey(playerId)) return;

            playerCanvas.SetMaskTexture(toolState.StencilData.MaskTexture);

            var color = toolState.InkDefinition.SampleColor(0);
            var drawState = DrawState.Start(
                playerCanvas.DrawCanvas,
                GetCanvasPosition(gameCursor.GlobalPosition),
                color
            );
            PlayerDrawStates.TryAdd(playerId, drawState);

            toolState.ToolDefinition.BrushDefinition?.Start(drawState);
        }
        if (@event.IsActionReleased("click"))
        {
            if (!PlayerDrawStates.TryGetValue(playerId, out var drawState)) return;
            var color = toolState.InkDefinition.SampleColor(drawState.DrawingTime);
            drawState.Finish(
                GetCanvasPosition(gameCursor.GlobalPosition),
                color,
                0
            );
            toolState.ToolDefinition.BrushDefinition?.Finish(drawState, 0);
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
            GetTree().Quit();
    }

    public void SetResolution(Vector2I resolution)
    {
        Resolution = resolution;
        _aspectRatioContainer.Ratio = (float)Resolution.X / Resolution.Y;
        _finalAccumulativeViewport.Size = resolution;

        UpdateUnscaledViewportScale();

        foreach (var (i, playerCanvas) in PlayerCanvases)
        {
            playerCanvas.SetResolution(resolution);
        }
    }

    private void UpdateUnscaledViewportScale()
    {
        int nearestScaleInt = 3;
        // var targetSize = _aspectRatioContainer.GetGlobalRect().Size;
        // float ratio = targetSize.X / targetSize.Y;
        // if (ratio > 1f)
        //     nearestScaleInt = MathUtil.FloorToInt(targetSize.Y / Resolution.Y);
        // else
        //     nearestScaleInt = MathUtil.FloorToInt(targetSize.X / Resolution.X);
        // nearestScaleInt = Mathf.Max(nearestScaleInt, 1);
        // GD.Print($"{Resolution} fits into {targetSize} {nearestScaleInt} times");
        _unscaledViewport.Size = Resolution;
        _unscaledViewport.Size2DOverride = Resolution * nearestScaleInt;
    }

    private void CreatePlayerCanvas(int playerId)
    {
        var playerCanvasNode = PlayerCanvasScene.Instantiate();
        if (playerCanvasNode is not PlayerCanvas playerCanvas) return;
        playerCanvas.SetResolution(Resolution);
        playerCanvas.SetOutputTextureTarget(UnscaledBackBufferTextureRect);
        playerCanvas.SetMaskTexture(AllStencilData[0].MaskTexture);
        PlayerCanvases.TryAdd(playerId, playerCanvas);
        playerCanvas.DrawCanvas.Draw += () =>
        {
            if (PlayerDrawStates.TryGetValue(playerId, out var drawState))
                DoDrawState(playerId, drawState);
        };
        _finalAccumulativeViewport.AddChild(playerCanvas);
    }

    private void DeletePlayerCanvas(int playerId)
    {
        PlayerCanvases.TryGetValue(playerId, out PlayerCanvas playerCanvas);
        if (playerCanvas is null) return;
        _finalAccumulativeViewport.RemoveChild(playerCanvas);
        playerCanvas.QueueFree();
        PlayerCanvases.Remove(playerId);
    }

    private void DrawAllDrawStates()
    {
        foreach (var (playerId, drawState) in PlayerDrawStates)
        {
            DoDrawState(playerId, drawState);
        }
    }

    private void DoDrawState(int playerId)
    {
        var drawState = PlayerDrawStates[playerId];
        DoDrawState(playerId, drawState);
    }

    private void DoDrawState(int playerId, DrawState drawState)
    {
        if (!PlayerToolStates.TryGetValue(playerId, out var toolState)) return;

        toolState.ToolDefinition.BrushDefinition.Draw(drawState);

        // Set the last frame properties
        drawState.LastCursorPosition = drawState.CursorPosition;
        drawState.LastEvaluatedPosition = drawState.EvaluatedPosition;
        drawState.LastEvaluatedScale = drawState.EvaluatedScale;
        drawState.LastEvaluatedColor = drawState.EvaluatedColor;

        if (drawState.State == DrawState.States.Start) drawState.State = DrawState.States.Draw;
        if (drawState.State == DrawState.States.Finish) PlayerDrawStates.Remove(playerId);
    }

    private void AddPlayerToolState(int playerId)
    {
        var toolState = DefaultToolState.Duplicate() as ToolState;
        toolState!.SetStencil(AllStencilData[0]);
        PlayerToolStates.TryAdd(playerId, toolState);
    }

    private void RemovePlayerToolState(int playerId)
    {
        if (!PlayerToolStates.ContainsKey(playerId)) return;
        PlayerToolStates.Remove(playerId);
    }

    private async void SaveImage()
    {
        var dialog = new FileDialog();
        dialog.SetFileMode(FileDialog.FileModeEnum.SaveFile);
        dialog.SetAccess(FileDialog.AccessEnum.Filesystem);
        dialog.SetUseNativeDialog(true);
        dialog.SetFilters(["*.png ; PNG File"]);

        async void OnDialogOnFileSelected(string dir)
        {
            await SaveImage(dir);
            Input.SetMouseMode(Input.MouseModeEnum.Hidden);
            RemoveChild(dialog);
            dialog.FileSelected -= OnDialogOnFileSelected;
        }
        dialog.FileSelected += OnDialogOnFileSelected;
        AddChild(dialog);
        dialog.PopupCenteredRatio();
    }

    private async Task SaveImage(String dir)
    {
        GD.Print($"Saving image to: {dir}");
        if (!dir.EndsWith(".png")) dir += ".png";
        var viewport = new SubViewport() { RenderTargetUpdateMode = SubViewport.UpdateMode.Once };
        viewport.Size = Resolution;

        var background = new ColorRect() { Color = Colors.White };
        viewport.AddChild(background);
        background.SetAnchorsPreset(LayoutPreset.FullRect);

        var layerImageRect = new TextureRect()
        {
            Texture = UnscaledBackBufferTextureRect.Texture,
            Material = GD.Load<Material>("res://materials/brush_mix.tres")
        };
        viewport.AddChild(layerImageRect);
        layerImageRect.SetAnchorsPreset(LayoutPreset.FullRect);

        AddChild(viewport);

        await ToSignal(RenderingServer.Singleton, RenderingServerInstance.SignalName.FramePostDraw);
        var viewportTexture = viewport.GetTexture();
        using var image = viewportTexture.GetImage();
        image.SavePng(dir);

        RemoveChild(viewport);
    }

    public Vector2 GetCanvasPosition(Vector2 cursorPosition)
    {
        var posLocalToRect = cursorPosition - ScaledBackBufferTextureRect.GlobalPosition;
        var scaleFactor = this.Resolution / ScaledBackBufferTextureRect.Size;
        var canvasRelativePosition = posLocalToRect * scaleFactor;
        var pixelPosition = canvasRelativePosition.Round();
        return pixelPosition;
    }

    public bool IsWithinCanvas(Vector2 cursorPosition)
    {
        var canvasPosition = GetCanvasPosition(cursorPosition).ToVector2I();
        return SizeRect.HasPoint(canvasPosition);
    }
}
