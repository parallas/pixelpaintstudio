
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
    public Color LastEvaluatedColor = Colors.Red;
    public Vector2 LastEvaluatedScale;
    public Vector2 EvaluatedScale;
    public float DrawingTime;

    public Vector2 EvaluatedVelocity => EvaluatedPosition - LastEvaluatedPosition;
    public float EvaluatedScaleUniform => EvaluatedScale.X;
    public float LastEvaluatedScaleUniform => LastEvaluatedScale.X;

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
            EvaluatedColor = cursorColor,
            LastEvaluatedColor = cursorColor,
            EvaluatedScale = Vector2.One,
            LastEvaluatedScale = Vector2.One,

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

    public DrawState Duplicate()
    {
        return new DrawState()
        {
            State = State,
            CanvasItem = CanvasItem,
            CursorPosition = CursorPosition,
            CursorStartPosition = CursorStartPosition,
            LastCursorPosition = LastCursorPosition,
            EvaluatedPosition = EvaluatedPosition,
            LastEvaluatedPosition = LastEvaluatedPosition,
            CursorColor = CursorColor,
            EvaluatedColor = EvaluatedColor,
            LastEvaluatedColor = LastEvaluatedColor,
            EvaluatedScale = EvaluatedScale,
            LastEvaluatedScale = LastEvaluatedScale,
            DrawingTime = DrawingTime,
        };
    }
}
