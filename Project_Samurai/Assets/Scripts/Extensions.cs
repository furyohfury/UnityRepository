namespace Samurai
{
    public delegate void SimpleHandle();

    public delegate void ChangeColorHandle(PhaseColor color);
    public event ChangeColorHandle OnSwapColor;


    public interface IHaveColor
    {
        PhaseColor CurrentColor {get; set;}
        SpriteRenderer Sprite {get; set;}
    }
    public interface IChangeColor :IHaveColor
    {
        void ChangeColor();
        void ChangeColor(PhaseColor color);
    }
}