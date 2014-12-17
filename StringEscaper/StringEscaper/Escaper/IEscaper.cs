
namespace StringEscaper
{
    public interface IEscaper
    {
        string Escape(string str, params string[] args);

        string Unescape(string str);

        string Name { set; get; }

        bool HasOtherInputs { set; get; }

        string OtherInputsText { set; get; }
    }
}
