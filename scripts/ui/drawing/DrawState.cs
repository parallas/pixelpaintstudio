
using Godot;

public class DrawState
{
    public States State;
    public CanvasItem CanvasItem;
    public Vector2 CursorPosition;
    public Vector2 CursorStartPosition;
    public Vector2 LastCursorPosition;
    public Vector2 EvaluatedPosition;
    public Vector2 LastEvaluatedPosition;
    public Color CursorColor = Colors.Red;
    public Color EvaluatedColor = Colors.Red;
    public Vector2 EvaluatedScale;
    public float DrawingTime;

    public enum States
    {
        Start,
        Draw,
        Finish,
    }

    public static DrawState Start(CanvasItem canvasItem, Vector2 cursorPosition, Color cursorColor)
    {
        return new DrawState
        {
            State = States.Start,
            CursorColor = cursorColor,
            CanvasItem = canvasItem,

            CursorPosition = cursorPosition,
            CursorStartPosition = cursorPosition,
            LastCursorPosition = cursorPosition,
            EvaluatedPosition = cursorPosition,
            LastEvaluatedPosition = cursorPosition,

            DrawingTime = 0f
        };
    }

    public void Process(Vector2 cursorPosition, Color cursorColor, double delta)
    {
        CursorPosition = cursorPosition;
        CursorColor = cursorColor;
        DrawingTime += (float)delta;
    }

    public void Finish(Vector2 cursorPosition, Color cursorColor, double delta)
    {
        State = States.Finish;
        CursorPosition = cursorPosition;
        CursorColor = cursorColor;
        DrawingTime += (float)delta;
    }
}
