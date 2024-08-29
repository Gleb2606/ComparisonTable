using System.Windows;

namespace WPFAPP
{
    /// <summary>
    /// Логика взаимодействия для AddParameterWindow.xaml
    /// </summary>
    public partial class AddParameterWindow : Window
    {
        /// <summary>
        /// Инициализация экземпляра класса <see cref="AddParameterWindow"/>.
        /// </summary>
        public AddParameterWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Добавть"
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
