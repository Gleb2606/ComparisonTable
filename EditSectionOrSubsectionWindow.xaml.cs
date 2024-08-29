using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFAPP
{
    /// <summary>
    /// Логика взаимодействия для EditSectionOrSubsectionWindow.xaml
    /// </summary>
    public partial class EditSectionOrSubsectionWindow : Window
    {
        /// <summary>
        /// Свойство измененное наименование
        /// </summary>
        public string EditedName { get; private set; }

        /// <summary>
        /// Инициализация экземпляра класса 
        /// <see cref="EditSectionOrSubsectionWindow"/>.
        /// </summary>
        public EditSectionOrSubsectionWindow(double currentId, string currentName)
        {
            InitializeComponent();
            NameTextBox.Text = currentName;
            IdTextBox.Text = currentId.ToString();
        }

        /// <summary>
        /// Обработчик, позволяющий ввести текст, только если он соответствует 
        /// регулярному выражению (ввести только числа с плавающей точкой)
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Паттерн реуглярного выражения
            string pattern = @"^(\d+([\.,]?\d*)?)?$";

            // Инициализация потока символов
            var textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Проверка на удовлетворение реуглярному выражению
            e.Handled = !Regex.IsMatch(newText, pattern);
        }

        /// <summary>
        /// Обработчик, позволяющий удалять символы
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Если были нажаты DELETE или BACKSPACE
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = false;
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Сохранить"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void SaveButton_Click(object sender, RoutedEventArgs e) 
        {
            EditedName = NameTextBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
