using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNotes10.Models
{
    /// <summary>
    /// Representa una nota.
    /// </summary>
    public class Nota
    {
        /// <summary>
        /// Obtiene o establece el identificador Id.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el asunto
        /// </summary>
        [MaxLength(255)]
        public string Asunto { get; set; }

        /// <summary>
        /// Obtiene o establece el detalle
        /// </summary>
        public string Detalle { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el tamaño de la fuente.
        /// </summary>
        public int FSize { get; set; }

        /// <summary>
        /// Obtiene o establece el color de fondo del Tile.
        /// </summary>
        public string CFondo { get; set; }
    }

}
