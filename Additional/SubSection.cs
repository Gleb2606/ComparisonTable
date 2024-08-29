using System.Collections.Generic;

namespace WPFAPP.Additional
{
    /// <summary>
    /// Класс подраздел
    /// </summary>
    public class Subsection
    {
        /// <summary>
        /// Свойство идентификатор
        /// </summary>
        public double Id { get; set; }  

        /// <summary>
        /// Свойство ключ раздела
        /// </summary>
        public double SectionId { get; set; }

        /// <summary>
        /// Свойство наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Свойство список параметров
        /// </summary>
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        /// <summary>
        /// Свойство флаг (новый параметр)
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Свойство флаг (удаленный параметр)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Свойство флаг (измененный параметр)
        /// </summary>
        public bool IsEdited { get; set; }
    }
}
