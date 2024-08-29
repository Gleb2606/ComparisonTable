using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using Npgsql;
using WPFAPP.Additional;
using Section = WPFAPP.Additional.Section;

namespace WPFAPP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Свойство коллекции разделов
        /// </summary>
        public ObservableCollection<Section> Sections { get; set; } = new ObservableCollection<Section>();

        private string connectionString = "Host=ia-im-sipr-ts.cdu.so;Port=5432;Database=table;Username=table_owner;Password=)NrN4(7,2zBRE";

        // Списки существующих идентификаторов
        List<double> existingParametersIds = new List<double>();
        List<double> existingSectionsIds = new List<double>();
        List<double> existingSubsectionsIds = new List<double>();

        // Генератор случайных чисел
        Random random = new Random();

        /// <summary>
        /// Инициализация экземпляра класса <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            LoadDataFromDatabase();
            SectionsListBox.ItemsSource = Sections;
        }

        /// <summary>
        /// Метод загрузки из БД
        /// </summary>
        private void LoadDataFromDatabase() 
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Загрузка данных из таблицы Section
                string sectionQuery = "SELECT \"Id\", \"Name\" FROM public.\"Section\"";

                using (var sectionCommand = new NpgsqlCommand(sectionQuery, connection))
                {
                    using (var reader = sectionCommand.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            Sections.Add(new Section
                            {
                                Id = reader.GetDouble(0),
                                Name = reader.GetString(1),
                            });
                        }
                    }
                }

                // Загрузка данных из таблицы SubSection
                string subsectionQuery = "SELECT \"Id\", \"SubSectionID\", \"Name\" FROM public.\"SubSection\"";

                using (var subsectionCommand = new NpgsqlCommand(subsectionQuery, connection))
                {
                    using (var reader = subsectionCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var subsection = new Subsection
                            {
                                Id = reader.GetDouble(0),
                                SectionId = reader.GetDouble(1),
                                Name = reader.GetString(2)
                            };

                            var section = Sections.FirstOrDefault(s => s.Id == subsection.SectionId);
                            section?.Subsections.Add(subsection);

                            existingSubsectionsIds.Add(subsection.Id);
                            existingSectionsIds.Add(section.Id);

                        }
                    }
                }

                // Загрузка данных из таблицы Characteristics
                string parametersQuery = "SELECT \"Id\", \"ParameterId\", \"Category\", \"Name\", \"Measurement\"," +
                                         " \"PathToClass\", \"Class\", \"Attribute\", \"Parent\", \"Type\" " +
                                         "FROM public.\"Characteristics\"";

                using (var parametersCommand = new NpgsqlCommand(parametersQuery, connection))
                {
                    using (var reader = parametersCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var parameter = new Parameter
                            {
                                Id = reader.GetDouble(0),
                                SubsectionId = reader.GetDouble(1),
                                Category = reader.GetString(2),
                                Name = reader.GetString(3),
                                Unit = reader.GetString(4),
                                ClassLink = reader.GetString(5),
                                Class = reader.GetString(6),
                                Attribute = reader.GetString(7),
                                ParentClass = reader.GetString(8),
                                Note = reader.GetString(9)
                            };

                            var subsection = Sections.SelectMany(s => s.Subsections)
                                .FirstOrDefault(ss => ss.Id == parameter.SubsectionId);

                            subsection?.Parameters.Add(parameter);

                            existingParametersIds.Add(parameter.Id);
                        }
                    }
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Сохранить"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void Save_Click(object sender, EventArgs e) 
        {
            using (var connection = new NpgsqlConnection(connectionString)) 
            {
                connection.Open();

                for (int k = Sections.Count - 1; k >= 0; k--)
                {
                    var section = Sections[k];

                    if (section.IsDeleted) 
                    {
                        string sectionDeleteQuery = "DELETE FROM public.\"Section\" WHERE \"Id\" = @Id";

                        using (var sectionCommand = new NpgsqlCommand(sectionDeleteQuery, connection))
                        {
                            sectionCommand.Parameters.AddWithValue("Id", section.Id);
                            sectionCommand.ExecuteNonQuery();
                            section.IsDeleted = false;
                            Sections.Remove(section);
                        }
                    }
                    else if (section.IsNew) 
                    {
                        string sectionInsertQuery = "INSERT INTO public.\"Section\" (\"Id\",\"Name\") VALUES (@Id, @Name)";

                        using (var sectionCommand = new NpgsqlCommand(sectionInsertQuery, connection)) 
                        {
                            sectionCommand.Parameters.AddWithValue("Id", section.Id);
                            sectionCommand.Parameters.AddWithValue("Name", section.Name);
                            sectionCommand.ExecuteNonQuery();
                            section.IsNew = false;
                        }
                    }
                    else 
                    {
                        string sectionUpdateQuery = "UPDATE public.\"Section\" SET \"Name\" = @Name WHERE \"Id\" = @Id";

                        using (var sectionCommand = new NpgsqlCommand(sectionUpdateQuery, connection))
                        {
                            sectionCommand.Parameters.AddWithValue("Id", section.Id);
                            sectionCommand.Parameters.AddWithValue("Name", section.Name);
                            sectionCommand.ExecuteNonQuery();
                            section.IsEdited = false;
                        }
                    }

                    for (int j = section.Subsections.Count - 1; j >= 0; j--)
                    {
                        var subsection = section.Subsections[j];

                        if (subsection.IsDeleted) 
                        {
                            string subsectionDeleteQuery = "DELETE FROM public.\"SubSection\" WHERE \"Id\" = @Id";
                            using (var subsectionCommand = new NpgsqlCommand(subsectionDeleteQuery, connection))
                            {
                                subsectionCommand.Parameters.AddWithValue("Id", subsection.Id);
                                subsectionCommand.ExecuteNonQuery();
                                section.IsDeleted = false;
                            }
                        }
                        else if (subsection.IsNew)
                        {
                            string subsectionInsertQuery = "INSERT INTO public.\"SubSection\" " +
                                                        "(\"Name\", \"Id\", \"SubSectionID\") VALUES (@Name, @Id, @SubSectionID)";

                            using (var subsectionCommand = new NpgsqlCommand(subsectionInsertQuery, connection))
                            {
                                subsectionCommand.Parameters.AddWithValue("Name", subsection.Name);
                                subsectionCommand.Parameters.AddWithValue("Id", subsection.Id);
                                subsectionCommand.Parameters.AddWithValue("SubSectionID", section.Id);
                                subsectionCommand.ExecuteNonQuery();
                                subsection.IsNew = false;
                            }
                        }
                        else
                        {
                            string subsectionUpdateQuery = "UPDATE public.\"SubSection\" SET \"Name\" = @Name WHERE \"Id\" = @Id";

                            using (var subsectionCommand = new NpgsqlCommand(subsectionUpdateQuery, connection))
                            {
                                subsectionCommand.Parameters.AddWithValue("Id", subsection.Id);
                                subsectionCommand.Parameters.AddWithValue("Name", subsection.Name);
                                subsectionCommand.ExecuteNonQuery();
                                subsection.IsEdited = false;
                            }
                        }

                        for (int i = subsection.Parameters.Count - 1; i >= 0; i--)
                        {
                            var parameter = subsection.Parameters[i];

                            if (parameter.IsDeleted) 
                            {
                                string parameterDeleteQuery = "DELETE FROM public.\"Characteristics\" WHERE \"Id\" = @Id";
                                using (var parameterCommand = new NpgsqlCommand(parameterDeleteQuery, connection))
                                {
                                    parameterCommand.Parameters.AddWithValue("Id", parameter.Id);
                                    parameterCommand.ExecuteNonQuery();
                                    section.IsDeleted = false;
                                    subsection.Parameters.Remove(parameter);
                                }
                            }
                            else if (parameter.IsNew)
                            {
                                string parameterInsertQuery = "INSERT INTO public.\"Characteristics\" " +
                                                       "(\"Id\", \"ParameterId\", \"Category\", \"Name\", \"Measurement\", \"PathToClass\"," +
                                                       " \"Class\", \"Attribute\", \"Parent\", \"Type\" )" +
                                                       " VALUES (@Id, @ParameterId, @Category, @Name, @Measurement, @PathToClass," +
                                                                "@Class, @Attribute, @Parent, @Type)";

                                using (var parameterCommand = new NpgsqlCommand(parameterInsertQuery, connection))
                                {
                                    parameterCommand.Parameters.AddWithValue("Id", parameter.Id);
                                    parameterCommand.Parameters.AddWithValue("ParameterId", subsection.Id);
                                    parameterCommand.Parameters.AddWithValue("Category", parameter.Category);
                                    parameterCommand.Parameters.AddWithValue("Name", parameter.Name);
                                    parameterCommand.Parameters.AddWithValue("Measurement", parameter.Unit);
                                    parameterCommand.Parameters.AddWithValue("PathToClass", parameter.ClassLink);
                                    parameterCommand.Parameters.AddWithValue("Class", parameter.Class);
                                    parameterCommand.Parameters.AddWithValue("Attribute", parameter.Attribute);
                                    parameterCommand.Parameters.AddWithValue("Parent", parameter.ParentClass);
                                    parameterCommand.Parameters.AddWithValue("Type", parameter.Note);
                                    parameterCommand.ExecuteNonQuery();
                                    parameter.IsNew = false;
                                }
                            }
                            else
                            {
                                string parameterUpdateQuery = "UPDATE public.\"Characteristics\" SET" +
                                                           " \"Category\" = @Category," +
                                                           " \"Name\" = @Name," +
                                                           " \"Measurement\" = @Measurement," +
                                                           " \"PathToClass\" = @PathToClass," +
                                                           " \"Class\" = @Class," +
                                                           " \"Attribute\" = @Attribute," +
                                                           " \"Parent\" = @Parent," +
                                                           " \"Type\" = @Type " +
                                                           "WHERE \"Id\" = @Id";

                                using (var parameterCommand = new NpgsqlCommand(parameterUpdateQuery, connection))
                                {
                                    parameterCommand.Parameters.AddWithValue("Id", parameter.Id);
                                    parameterCommand.Parameters.AddWithValue("Category", parameter.Category);
                                    parameterCommand.Parameters.AddWithValue("Name", parameter.Name);
                                    parameterCommand.Parameters.AddWithValue("Measurement", parameter.Unit);
                                    parameterCommand.Parameters.AddWithValue("PathToClass", parameter.ClassLink);
                                    parameterCommand.Parameters.AddWithValue("Class", parameter.Class);
                                    parameterCommand.Parameters.AddWithValue("Attribute", parameter.Attribute);
                                    parameterCommand.Parameters.AddWithValue("Parent", parameter.ParentClass);
                                    parameterCommand.Parameters.AddWithValue("Type", parameter.Note);
                                    parameterCommand.ExecuteNonQuery();
                                    parameter.IsEdited = false;
                                }
                            }
                        }
                    }    
                }

                connection.Close();
            }
            MessageBox.Show("Изменения сохранены в БД", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Фильтр"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void OpenFilter_Click (object sender, RoutedEventArgs e) 
        {
            var filterWindow = new FilterWindow();
            if (filterWindow.ShowDialog() == true) 
            {
                ApplyFilters(filterWindow.SectionFilter, filterWindow.SubsectionFilter,
                             filterWindow.CategoryFilter, filterWindow.NameFilter,
                             filterWindow.UnitFilter, filterWindow.LinkFilter,
                             filterWindow.ClassFilter, filterWindow.AttributeFilter,
                             filterWindow.ParentFilter, filterWindow.NoteFilter);
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Применить фильтр"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void ApplyFilters(string sectionFilter, string subsectionFilter,
                                  string categoryFilter, string nameFilter,
                                  string unitFilter, string linkFilter,
                                  string classFilter, string attributeFilter,
                                  string parentFilter, string noteFilter)
        {
            var filteredSections = Sections
                .Where(s => s.Name.ToLower().Contains(sectionFilter.ToLower())).ToList();

            SectionsListBox.ItemsSource = filteredSections;

            if (SectionsListBox.SelectedItem is Section selectedSection) 
            {
                var filterdSubsections = selectedSection.Subsections
                    .Where(ss => selectedSection.Name.ToLower().Contains(subsectionFilter.ToLower())).ToList();

                SubsectionsListBox.ItemsSource = filterdSubsections;

                if (SubsectionsListBox.SelectedItem is Subsection selectedSubsection) 
                {
                    var filteredParameters = selectedSubsection.Parameters
                        .Where(p => (string.IsNullOrEmpty(categoryFilter) || p.Category.ToLower().Contains(categoryFilter.ToLower())) &&
                                    (string.IsNullOrEmpty(nameFilter) || p.Name.ToLower().Contains(nameFilter.ToLower())) &&
                                    (string.IsNullOrEmpty(unitFilter) || p.Unit.ToLower().Contains(unitFilter.ToLower())) &&
                                    (string.IsNullOrEmpty(linkFilter) || p.ClassLink.ToLower().Contains(linkFilter.ToLower())) &&
                                    (string.IsNullOrEmpty(classFilter) || p.Class.ToLower().Contains(classFilter.ToLower())) &&
                                    (string.IsNullOrEmpty(attributeFilter) || p.Attribute.ToLower().Contains(attributeFilter.ToLower())) &&
                                    (string.IsNullOrEmpty(parentFilter) || p.ParentClass.ToLower().Contains(parentFilter.ToLower())) &&
                                    (string.IsNullOrEmpty(noteFilter) || p.Note.ToLower().Contains(noteFilter.ToLower())))
                        .ToList();
                    ParametersDataGrid.ItemsSource = filteredParameters;
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Добавить раздел"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            double parsedId = 0;
            bool isValid = false;
            string name = "";

            while (!isValid || existingSectionsIds.Contains(parsedId))
            {
                var addWindow = new AddSectionOrSubsectionWindow();

                if (addWindow.ShowDialog() != true)
                {
                    return;
                }

                if (double.TryParse(addWindow.IdTextBox.Text.Replace('.', ','), out parsedId))
                {
                    if (!existingSectionsIds.Contains(parsedId))
                    {
                        isValid = true;
                        name = addWindow.NameTextBox.Text;
                    }
                    else
                    {
                        MessageBox.Show($"ID {parsedId} уже существует", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Некорретный формат ID", "Ошибка",
                                     MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            Sections.Add(new Section 
                { Id = parsedId,
                  Name = name,
                  IsNew = true });

            SectionsListBox.Items.Refresh();
            
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Изменить раздел"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void EditSection_Click(object sender, RoutedEventArgs e) 
        {
            if (SectionsListBox.SelectedItem is Section selectedSection) 
            {
                var editWindow = new EditSectionOrSubsectionWindow(selectedSection.Id,
                                                                   selectedSection.Name);
                if (editWindow.ShowDialog() == true) 
                {
                    selectedSection.Id = double.Parse(editWindow.IdTextBox.Text.Replace('.', ','));
                    selectedSection.Name = editWindow.EditedName;
                    selectedSection.IsEdited = true;
                    SectionsListBox.ItemsSource = null;
                    SectionsListBox.ItemsSource = Sections;
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Удалить раздел"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void DeleteSection_Click (object sender, RoutedEventArgs e) 
        {
            if(SectionsListBox.SelectedItem is Section selectedSection) 
            {
                selectedSection.IsDeleted = true;
                SubsectionsListBox.ItemsSource = null;
                ParametersDataGrid.ItemsSource = null;
                SectionsListBox.Items.Refresh();
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Добавить подраздел"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void AddSubsection_Click (object sender, RoutedEventArgs e)
        {
            if (SectionsListBox.SelectedItem is Section selectedSection)
            {
                double parsedId = 0;
                bool isValid = false;
                string name = "";

                while (!isValid || existingSubsectionsIds.Contains(parsedId))
                {
                    var addWindow = new AddSectionOrSubsectionWindow();

                    if (addWindow.ShowDialog() != true)
                    {
                        return;
                    }

                    if (double.TryParse(addWindow.IdTextBox.Text.Replace('.', ','), out parsedId))
                    {
                        if (!existingSubsectionsIds.Contains(parsedId))
                        {
                            isValid = true;
                            name = addWindow.NameTextBox.Text;
                        }
                        else
                        {
                            MessageBox.Show($"ID {parsedId} уже существует", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Некорретный формат ID", "Ошибка",
                                         MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                selectedSection.Subsections.Add(new Subsection
                {
                    Id = parsedId,
                    Name = name,
                    IsNew = true
                });
                SubsectionsListBox.ItemsSource = selectedSection.Subsections;
            }
            
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Изменить подраздел"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void EditSubsection_Click (Object sender, RoutedEventArgs e) 
        {
            if (SectionsListBox.SelectedItem is Section selectedSection &&
                SubsectionsListBox.SelectedItem is Subsection selectedSubsection)
            {
                var editWindow = new EditSectionOrSubsectionWindow(selectedSubsection.Id, 
                                                                   selectedSubsection.Name);
                if (editWindow.ShowDialog() == true)
                {
                    selectedSection.Id = double.Parse(editWindow.IdTextBox.Text);
                    selectedSubsection.Name = editWindow.EditedName;
                    selectedSubsection.IsEdited = true;
                    SubsectionsListBox.ItemsSource = null;
                    SubsectionsListBox.ItemsSource = selectedSection.Subsections;
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Удалить подраздел"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void DeleteSubsection_Click (Object sender, RoutedEventArgs e) 
        {
            if (SectionsListBox.SelectedItem is Section selectedSection &&
                SubsectionsListBox.SelectedItem is Subsection selectedSubsection) 
            {
                selectedSubsection.IsDeleted = true;
                ParametersDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Добавить параметр"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void AddParameter_Click(object sender, RoutedEventArgs e)
        {
            if (SubsectionsListBox.SelectedItem is Subsection selectedSubsection)
            {
                var addWindow = new AddParameterWindow();

                double id = 0; 

                while(existingParametersIds.Contains(id)) 
                {
                    id++;
                }

                id = id + random.Next(0,99999999);

                if (addWindow.ShowDialog() == true)
                {
                    selectedSubsection.Parameters.Add(new Parameter
                    {
                        Id = id, 
                        Category = addWindow.CategoryTextBox.Text,
                        Name = addWindow.NameTextBox.Text,
                        Unit = addWindow.UnitTextBox.Text,
                        ClassLink = addWindow.ClassLinkTextBox.Text,
                        Class = addWindow.ClassTextBox.Text,
                        Attribute = addWindow.AttributeTextBox.Text,
                        ParentClass = addWindow.ParentClassTextBox.Text,
                        Note = addWindow.NoteTextBox.Text,
                        IsNew = true
                    });
                    ParametersDataGrid.ItemsSource = selectedSubsection.Parameters;
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Изменить параметр"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void EditParameter_Click (object sender, RoutedEventArgs e) 
        {
            if (SubsectionsListBox.SelectedItem is Subsection selectedSubsection &&
                ParametersDataGrid.SelectedItem is Parameter selectedParameter)
            {
                var editWindow = new EditParameterWindow(selectedParameter);
                if (editWindow.ShowDialog() == true) 
                {
                    int index = selectedSubsection.Parameters.IndexOf(selectedParameter);
                    selectedSubsection.Parameters[index] = editWindow.EditedParameter;
                    selectedSubsection.Parameters[index].IsEdited = true;
                    ParametersDataGrid.ItemsSource = null;
                    ParametersDataGrid.ItemsSource = selectedSubsection.Parameters;
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Удалить параметр"
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void DeleteParameter_Click(Object sender, RoutedEventArgs e) 
        {
            if (SubsectionsListBox.SelectedItem is Subsection selectedSubsection &&
                ParametersDataGrid.SelectedItem is Parameter selectedParameter) 
            {
                selectedParameter.IsDeleted = true;
            }
        }

        /// <summary>
        /// Обработчик события изменения в разделе
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void SectionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SectionsListBox.SelectedItem is Section selectedSection)
            {
                SubsectionsListBox.ItemsSource = selectedSection.Subsections;
                SubsectionsListBox.SelectedItem = null;
                ParametersDataGrid.ItemsSource = null;
            }
            else 
            {
                SubsectionsListBox.ItemsSource = null;
                ParametersDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// Обработчик события изменения в подразделе
        /// </summary>
        /// <param name="sender">Объект вызвавший обработчик</param>
        /// <param name="e">Аргументы события</param>
        private void SubsectionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubsectionsListBox.SelectedItem is Subsection selectedSubsection)
            {
                ParametersDataGrid.ItemsSource = selectedSubsection.Parameters;
            }
            else
            {
                ParametersDataGrid.ItemsSource = null;
            }
        }
    }
}
