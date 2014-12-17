using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using StringEscaper.Escaper;

namespace StringEscaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IEscaper> Escapers = new List<IEscaper>();
        private IEscaper Escaper;
        private const string LENGTH_FORMAT = "{0} Bytes";
        private const int MAX_LENGTH = 10000000;

        public MainWindow()
        {
            InitializeComponent();

            EscapersComboBox.SelectedIndex = 0;

            EscapeButton.Click += new RoutedEventHandler(EscapeButton_Click);

            UnescapeButton.Click += new RoutedEventHandler(UnescapeButton_Click);

            InputArea.TextChanged += new TextChangedEventHandler(InputArea_TextChanged);

            OutputArea.TextChanged += new TextChangedEventHandler(OutputArea_TextChanged);

            InputArea.MaxLength = MAX_LENGTH;

            OutputArea.MaxLength = MAX_LENGTH;

            InputLengthText.Content = string.Format(LENGTH_FORMAT, 0);

            OutLengthText.Content = string.Format(LENGTH_FORMAT, 0);

            Escapers.Add(new XmlEscaper());
            Escapers.Add(new GoogleProtoEscaper());
            Escapers.Add(new NoemaxEscaper());
            Escapers.Add(new Mapper());

            foreach (var escaper in Escapers)
            {
                if (Escaper == null)
                {
                    Escaper = escaper;
                }
                var item = new ComboBoxItem();
                item.Content = escaper.Name;
                EscapersComboBox.Items.Add(item);
            }

            EscapersComboBox.SelectionChanged += new SelectionChangedEventHandler(EscapersComboBox_SelectionChanged);
        }

        void EscapersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox != null)
            {
                ComboBoxItem selected = (ComboBoxItem)comboBox.SelectedItem;
                if (selected != null)
                {
                    Escaper = Escapers.Find(d => d.Name == selected.Content);

                    if (Escaper.NeedOthers)
                    {
                        OthersBoxLabel.Visibility = Visibility.Visible;
                        OthersBox.Visibility = Visibility.Visible;
                        OthersBoxLabel.Content = Escaper.OthersMemo;
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

        void UnescapeButton_Click(object sender, RoutedEventArgs e)
        {
            OutputArea.Text = Escaper.Unescape(InputArea.Text);
        }

        void EscapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Escaper.NeedOthers && !string.IsNullOrWhiteSpace(OthersBox.Text))
            {
                var args = OthersBox.Text.Split(',');

                OutputArea.Text = Escaper.Escape(InputArea.Text, args);
            }
            else
            {
                OutputArea.Text = Escaper.Escape(InputArea.Text);
            }
        }
    }
}
