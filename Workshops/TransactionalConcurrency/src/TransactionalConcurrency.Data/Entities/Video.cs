namespace TransactionalConcurrency.Data.Entities
{
    public class Video
    {
        public virtual int VideoId { get; set; }
        public virtual decimal SaleIncVat { get; set; }
        public virtual string Title { get; set; }
        public virtual byte[] RowVersion { get; set; }
    }
}