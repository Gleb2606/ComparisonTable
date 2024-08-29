using System.Windows;

namespace WPFAPP
{
    /// <summary>
    /// Логика взаимодействия для FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window
    {
        /// <summary>
        /// Свойство фильтр разделов
        /// </summary>
        public string SectionFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр подразделов
        /// </summary>
        public string SubsectionFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр категорий
        /// </summary>
        public string CategoryFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр наименований
        /// </summary>
        public string NameFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр единиц измерений 
        /// </summary>
        public string UnitFilter {  get; private set; }

        /// <summary>
        /// Своство фильтр путей до класса
        /// </summary>
        public string LinkFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр классов
        /// </summary>
        public string ClassFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр атрибутов
        /// </summary>
        public string AttributeFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр ролительских классов
        /// </summary>
        public string ParentFilter { get; private set; }

        /// <summary>
        /// Свойство фильтр примечаний
        /// </summary>
        public string NoteFilter { get; private set; }

        /// <summary>
        /// Инициализация экземпляра класса <see cref="FilterWindow"/>.
        /// </summary>
        public FilterWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Применить фильтр"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void ApplyFilter_Click(object sender, RoutedEventArgs e) 
        {
            SectionFilter = SectionFilterTextBox.Text;
            SubsectionFilter = SubsectionFilterTextBox.Text;
            CategoryFilter = CategoryFilterTextBox.Text;
            NameFilter = NameFilterTextBox.Text;
            UnitFilter = UnitFilterTextBox.Text;
            LinkFilter = LinkFilterTextBox.Text;
            ClassFilter = ClassFilterTextBox.Text;
            AttributeFilter = AttributeFilterTextBox.Text;
            ParentFilter = ParentFilterTextBox.Text;    
            NoteFilter = NoteFilterTextBox.Text;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Отмена"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void Cancel_Click(object sender, RoutedEventArgs e) 
        {
            DialogResult = false;
            Close();
        }
    }
}
