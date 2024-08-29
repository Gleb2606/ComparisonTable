namespace WPFAPP.Additional
{
    /// <summary>
    /// Класс параметры
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Свойство идентификатор
        /// </summary>
        public double Id { get; set; }

        /// <summary>
        /// Свойство ключ подраздела
        /// </summary>
        public double SubsectionId { get; set; }

        /// <summary>
        /// Свойство категория параметра
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Свойство наименование параметра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Свойство единица измерения
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Свойство сслыка до класса
        /// </summary>
        public string ClassLink { get; set; }

        /// <summary>
        /// Свойство класс
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Свойство атрибут
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Свойство ролительский класс
        /// </summary>
        public string ParentClass { get; set; }

        /// <summary>
        /// Свойство примечание
        /// </summary>
        public string Note { get; set; }

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
