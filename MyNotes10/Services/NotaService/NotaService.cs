using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNotes10.Models;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace MyNotes10.Services.NotaService
{
    public class NotaService : INotaService
    {
        String path = App.DbConnectionString;

        public List<Nota> GetNotas()
        {
            List<Nota> result;
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                result = conn.Table<Nota>().ToList();
            }
            return result;
        }

        public int CountNotas()
        {
            List<Nota> result;
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                result = conn.Table<Nota>().ToList();
            }
            return result.Count();
        }

        public List<Nota> SearchNotas(string busca)
        {
            List<Nota> result;
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                result = (from n in conn.Table<Nota>()
                          where (n.Asunto.Contains(busca) || n.Detalle .Contains (busca))
                          select n).ToList();
            }
            return result;
        }

        public Nota GetNota(int Id)
        {
            Nota result;
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                result = conn.Get<Nota>(Id);
                result.Fecha = result.Fecha.ToLocalTime();
            }
            return result;
        }

        public void InsertOrUpdateNota(Nota nota)
        {
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                if (nota.Id.Equals(0))
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Insert(nota);
                    });
                }
                else
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Update(nota);
                    });
                }
            }
        }

        public void DeleteNota(Nota nota)
        {
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                conn.RunInTransaction(() =>
                {
                    conn.Delete(nota);
                });
            }
        }

        public List<Nota> SortNotas(string SortOrder)
        {
            List<Nota> result;
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                if (SortOrder == "ASC")
                {
                    result = (from n in conn.Table<Nota>()
                              orderby n.Asunto
                              select n).ToList();
                }
                else
                {
                    result = (from n in conn.Table<Nota>()
                              orderby n.Asunto descending
                              select n).ToList();
                }
            }
            return result;
        }
    }
}
