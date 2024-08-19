using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elysian.Domain.Seedwork;

namespace Elysian.Domain.Data
{
    public class Card : IAuditableEntitiy
    {
        public int CardId { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Grade { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModifiedByUserId { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        public class Configuration : AuditableEntityConfiguration<Card>
        {
            public override void Configure(EntityTypeBuilder<Card> builder)
            {
                base.Configure(builder);

                builder.HasKey(k => k.CardId);
                builder.Property(e => e.SerialNumber).IsRequired();
                builder.HasIndex(e => e.SerialNumber)
                    .IsUnique()
                    .HasDatabaseName("AK_Card_SerialNumber")
                    .HasFilter($"[{nameof(IsDeleted)}] = 0");

                builder.Property(e => e.Name).IsRequired();
                builder.Property(e => e.Grade).IsRequired();

                builder.ToTable("Card");
            }
        }
    }
}
