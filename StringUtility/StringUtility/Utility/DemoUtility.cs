
namespace StringUtility.Utility
{
    public class DemoUtility : IUtility
    {
        public DemoUtility()
        {
            Name = "XXXXX";

            MainName = "Add";

            AdvanceName = "Remove";

            HasOtherInputs = false;

            OtherInputsText = string.Empty;
        }

        public string Main(string str, params string[] args)
        {
            return string.Empty;
        }

        public string Advance(string str, params string[] args)
        {
            return string.Empty;
        }

        public string Name { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }

        public bool HasOtherInputs { set; get; }

        public string OtherInputsText { set; get; }
    }
}
