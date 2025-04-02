using System.Text.Json;
using AzServiceBusProdConsumer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzServiceBusProdConsumer
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(x => x.Id);

           builder.Property(x=>x.Data)
                .HasColumnType("jsonb")
                .IsRequired();          
                         

        }
    }
}
