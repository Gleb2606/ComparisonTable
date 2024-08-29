using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFAPP
{
    /// <summary>
    /// Логика взаимодействия для AddSectionOrSubsectionWindow.xaml
    /// </summary>
    public partial class AddSectionOrSubsectionWindow : Window
    {
        /// <summary>
        /// Инициализация экземпляра класса 
        /// <see cref="AddSectionOrSubsectionWindow"/>.
        /// </summary>
        public AddSectionOrSubsectionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик, позволяющий ввести текст, только если он соответствует 
        /// регулярному выражению (ввести только числа с плавающей точкой)
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void NumericTextBox_PreviewTextInput(object sender,
                                                     TextCompositionEventArgs e) 
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
        /// Обработчик события нажатия на кнопку "Добавить"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }

}
