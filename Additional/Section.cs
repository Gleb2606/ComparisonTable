using System.Collections.Generic;

namespace WPFAPP.Additional
{
    /// <summary>
    /// Класс раздел
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Свойство идентификатор
        /// </summary>
        public double Id { get; set; }

        /// <summary>
        /// Свойство наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Свойство список подразделов
        /// </summary>
        public List<Subsection> Subsections { get; set; } = new List<Subsection>();

        /// <summary>
        /// Свойство флаг (новый параметр)
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Свойство флаг (удаленный параметр)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Свойство флаг (Измененный параметр)
        /// </summary>
        public bool IsEdited { get; set; }
    }
}
