using MyNotes10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNotes10.Services.NotaService
{
    public interface INotaService
    {
        List<Nota> GetNotas();
        List<Nota> SearchNotas(string searchText);
        List<Nota> SortNotas(string SortOrder);
        Nota GetNota(int Id);
        void InsertOrUpdateNota(Nota nota);
        void DeleteNota(Nota nota);
        int CountNotas();
    }
}
