namespace Samurai
{
    public delegate void SimpleHandle();

    public delegate void ChangeColorHandle(PhaseColor color);
    public event ChangeColorHandle OnChangeColor;


    public interface IHaveColor
    {
        PhaseColor CurrentColor {get; set;}
    }
}