using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPFAPP.Additional;

namespace WPFAPP
{
    /// <summary>
    /// Логика взаимодействия для EditParameterWindow.xaml
    /// </summary>
    public partial class EditParameterWindow : Window
    {
        /// <summary>
        /// Свойство изменнный параметр
        /// </summary>
        public Parameter EditedParameter { get; private set; }

        /// <summary>
        /// Инициализация экземпляра класса <see cref="EditParameterWindow"/>.
        /// </summary>
        /// <param name="parameter">Изначальное значение параметра</param>
        public EditParameterWindow(Parameter parameter)
        {
            InitializeComponent();

            // Заполнение текст бокса исходными данными
            CategoryTextBox.Text = parameter.Category;
            NameTextBox.Text = parameter.Name;
            UnitTextBox.Text = parameter.Unit;
            ClassLinkTextBox.Text = parameter.ClassLink;
            ClassTextBox.Text = parameter.Class;
            AttributeTextBox.Text = parameter.Attribute;
            ParentClassTextBox.Text = parameter.ParentClass;
            NoteTextBox.Text = parameter.Note;
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Сохранить"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик </param>
        /// <param name="e">Аргументы события</param>
        private void SaveButton_Click(object sender, RoutedEventArgs e) 
        {
            // Редактирование параметров
            EditedParameter = new Parameter
            {
                Category = CategoryTextBox.Text,
                Name = NameTextBox.Text,
                Unit = UnitTextBox.Text,
                ClassLink = ClassLinkTextBox.Text,
                Class = ClassTextBox.Text,
                Attribute = AttributeTextBox.Text,
                ParentClass = ParentClassTextBox.Text,
                Note = NoteTextBox.Text,
            };

            DialogResult = true;

            Close();
        }
    }
}
