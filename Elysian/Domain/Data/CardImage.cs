using Elysian.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysian.Domain.Data
{
    public class CardImage : IAuditableEntitiy
    {
        public int CardImageId { get; set; }
        public Guid StorageId { get; set; }
        public string FileName { get; set; }
        public string AltText { get; set; }
        public long FileSize { get; set; }
        public bool IsStorageDeleted { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModifiedByUserId { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int CardId { get; set; }
        public  Card Card { get; set; }

        public class Configuration : AuditableEntityConfiguration<CardImage>
        {
            public override void Configure(EntityTypeBuilder<CardImage> builder)
            {
                base.Configure(builder);

                builder.HasKey(k => k.CardImageId);
                builder.Property(e => e.StorageId).IsRequired();
                builder.HasIndex(e => e.StorageId).IsUnique().HasDatabaseName("AK_CardImage_StorageId");
                builder.Property(e => e.FileName).IsRequired();

                builder.HasOne(b => b.Card)
                    .WithMany()
                    .HasForeignKey(b => b.CardId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.ToTable("CardImage");
            }
        }
    }
}
