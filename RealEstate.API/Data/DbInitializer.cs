using RealEstate.API.Models;

namespace RealEstate.API.Data;

public static class DbInitializer
{
    public static void Seed(RealEstateDbContext context)
    {
        // Check if any clients exist
        if (context.Clients.Any())
        {
            return; // DB has been seeded
        }

        var clients = new Client[]
        {
            new Client { Name = "Juan Perez", Email = "juan.perez@example.com", PhoneNumber = "555-0101" },
            new Client { Name = "Maria Garcia", Email = "maria.garcia@example.com", PhoneNumber = "555-0102" },
            new Client { Name = "Carlos Lopez", Email = "carlos.lopez@example.com", PhoneNumber = "555-0103" }
        };

        context.Clients.AddRange(clients);
        context.SaveChanges();

        var properties = new Property[]
        {
            new Property { Address = "123 Main St, Cityville", Price = 250000, Type = "Sale", Description = "Beautiful 3-bedroom house", OwnerId = clients[0].Id },
            new Property { Address = "456 Oak Ave, Townburg", Price = 1200, Type = "Rent", Description = "Cozy 2-bedroom apartment", OwnerId = clients[1].Id },
            new Property { Address = "789 Pine Ln, Villageton", Price = 350000, Type = "Sale", Description = "Spacious villa with garden", OwnerId = clients[0].Id },
            new Property { Address = "101 Maple Dr, Hamlet", Price = 1500, Type = "Rent", Description = "Modern studio in downtown", OwnerId = clients[2].Id }
        };

        context.Properties.AddRange(properties);
        context.SaveChanges();

        var transactions = new Transaction[]
        {
            new Transaction { PropertyId = properties[1].Id, ClientId = clients[2].Id, Amount = 1200, TransactionType = "Rent", Date = DateTime.UtcNow.AddDays(-10) },
            new Transaction { PropertyId = properties[0].Id, ClientId = clients[1].Id, Amount = 250000, TransactionType = "Sale", Date = DateTime.UtcNow.AddDays(-5) }
        };

        context.Transactions.AddRange(transactions);
        context.SaveChanges();
    }
}
