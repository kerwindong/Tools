using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using StringUtility.Utility;

namespace StringUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IUtility> Utilitys = new List<IUtility>();

        private IUtility Utility;

        private const string LENGTH_FORMAT = "{0} Bytes";

        private const int MAX_LENGTH = 10000000;

        public MainWindow()
        {
            InitializeComponent();

            UtilitysComboBox.SelectedIndex = 0;

            MainButton.Click += new RoutedEventHandler(MainButton_Click);

            AdvanceButton.Click += new RoutedEventHandler(AdvanceButton_Click);

            InputArea.TextChanged += new TextChangedEventHandler(InputArea_TextChanged);

            OutputArea.TextChanged += new TextChangedEventHandler(OutputArea_TextChanged);

            InputArea.MaxLength = MAX_LENGTH;

            OutputArea.MaxLength = MAX_LENGTH;

            InputLengthText.Content = string.Format(LENGTH_FORMAT, 0);

            OutLengthText.Content = string.Format(LENGTH_FORMAT, 0);

            Utilitys.Add(new CtranUtility()); 
            Utilitys.Add(new ExcelTranUtility());
            Utilitys.Add(new XmlUtility());
            Utilitys.Add(new GoogleProtoUtility());
            Utilitys.Add(new NoemaxUtility());
            Utilitys.Add(new Mapper());
            Utilitys.Add(new DesUtility());

            foreach (var utility in Utilitys)
            {
                if (Utility == null)
                {
                    Utility = utility;
                }
                var item = new ComboBoxItem();
                item.Content = utility.Name;
                UtilitysComboBox.Items.Add(item);
            }

            UtilitysComboBox.SelectionChanged += new SelectionChangedEventHandler(UtilitysComboBox_SelectionChanged);

            InitializeButton();
        }

        void UtilitysComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox != null)
            {
                ComboBoxItem selected = (ComboBoxItem)comboBox.SelectedItem;
                if (selected != null)
                {
                    Utility = Utilitys.Find(d => d.Name == (string)selected.Content);

                    InitializeButton();

                    if (Utility.HasOtherInputs)
                    {
                        OthersBoxLabel.Visibility = Visibility.Visible;
                        OthersBox.Visibility = Visibility.Visible;
                        OthersBoxLabel.Content = Utility.OtherInputsText;
                    }
                    else
                    {
                        OthersBoxLabel.Visibility = Visibility.Collapsed;
                        OthersBox.Visibility = Visibility.Collapsed;
                        OthersBoxLabel.Content = string.Empty;
                    }
                }
            }
        }

        void InputArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputLengthText.Content = string.Format(LENGTH_FORMAT, ASCIIEncoding.UTF8.GetByteCount(InputArea.Text));
        }

        void OutputArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutLengthText.Content = string.Format(LENGTH_FORMAT, ASCIIEncoding.UTF8.GetByteCount(OutputArea.Text));
        }

        void AdvanceButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utility.HasOtherInputs && !string.IsNullOrWhiteSpace(OthersBox.Text))
            {
                var args = OthersBox.Text.Split(',');

                OutputArea.Text = Utility.Advance(InputArea.Text, args);
            }
            else
            {
                OutputArea.Text = Utility.Advance(InputArea.Text);
            }
        }

        void MainButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utility.HasOtherInputs && !string.IsNullOrWhiteSpace(OthersBox.Text))
            {
                var args = OthersBox.Text.Split(',');

                OutputArea.Text = Utility.Main(InputArea.Text, args);
            }
            else
            {
                OutputArea.Text = Utility.Main(InputArea.Text);
            }
        }

        private void InitializeButton()
        {
            if (!string.IsNullOrWhiteSpace(Utility.MainName))
            {
                MainButton.Content = Utility.MainName;
                MainButton.Visibility = Visibility.Visible;
            }
            else
            {
                MainButton.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(Utility.AdvanceName))
            {
                AdvanceButton.Content = Utility.AdvanceName;
                AdvanceButton.Visibility = Visibility.Visible;
            }
            else
            {
                AdvanceButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
