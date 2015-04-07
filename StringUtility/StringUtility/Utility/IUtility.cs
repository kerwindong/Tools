
namespace StringUtility
{
    public interface IUtility
    {
        string Main(string str, params string[] args);

        string Advance(string str);

        string Name { set; get; }

        string MainName { set; get; }

        string AdvanceName { set; get; }

        bool HasOtherInputs { set; get; }

        string OtherInputsText { set; get; }
    }
}
