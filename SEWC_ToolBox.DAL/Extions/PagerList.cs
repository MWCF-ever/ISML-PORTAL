using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.Extions
{
    public interface IPagerList<TEntity>
    {

        int Paged { get; set; }

        int Size { get; set; }

        List<TEntity> Source { get; set; }

        int Total { get; set; }

        int PageCount { get; }

        TEntity[] ToArray();

        List<TEntity> ToList();
    }
    public class PagerList<TEntity> : IPagerList<TEntity>
    {
        public int Paged { get; set; }

        public int Size { get; set; }

        public List<TEntity> Source { get; set; }

        public int Total { get; set; }


        public int PageCount { get; private set; }

        public PagerList(List<TEntity> entities, int total, int paged, int size)
        {
            this.Total = total;
            this.Paged = paged;
            this.Size = size;
            this.PageCount = total % size == 0 ? total / size : (total / size) + 1;
            this.Source = entities ?? new List<TEntity>();
        }

        public TEntity[] ToArray()
        {
            return Source.ToArray();
        }

        public List<TEntity> ToList()
        {
            return Source.ToList();
        }
    }

    public static class PageModelExtions
    {
        public static PagerList<TEntity> ToPagedList<TEntity>(this IEnumerable<TEntity> source, int paged, int size)
        {
            var total = source.Count();
            var data = source.Skip((paged - 1) * size).Take(size);

            return new PagerList<TEntity>(data.ToList(), total, paged, size);
        }
        public static PagerList<TEntity> ToPagedList<TEntity>(this List<TEntity> source, int total, int paged, int size)
        {
            var data = source.ToList();

            return new PagerList<TEntity>(data, total, paged, size);
        }
    }
}
