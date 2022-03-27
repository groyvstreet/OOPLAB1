using Lab1.Models.Entities;

namespace Lab1.Models.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Banks.Add(new Bank { Id = "0", Name = $"CCG", Percent = 10, BIC = "000000001" });
            context.Banks.Add(new Bank { Id = "1", Name = $"Stark", Percent = 20, BIC = "000000002" });
            context.Banks.Add(new Bank { Id = "2", Name = $"BSB", Percent = 30, BIC = "000000003" });

            context.Clients.Add(new Client
            {
                Id = Guid.NewGuid().ToString(),
                Email = "zxc@mail.ru",
                FirstName = "z",
                LastName = "x",
                Patronymic = "c",
                Password = "123456",
                PhoneNumber = "zxc",
                PassportSeries = "zxc",
                IdentificationNumber = "zxc",
                RoleName = "client",
                BankId = "0",
                Approved = true,
                CompanyId = "0"
            });

            for (int i = 0; i < 3; i++)
            {
                context.Admins.Add(new Admin
                {
                    Email = "admin@mail.ru",
                    FirstName = "admin",
                    LastName = "admin",
                    Patronymic = "admin",
                    Password = "123456",
                    PhoneNumber = "admin",
                    RoleName = "admin",
                    BankId = $"{i}"
                });

                //Managers
                context.Users.Add(new User
                {
                    Email = "manager@mail.ru",
                    FirstName = "manager",
                    LastName = "manager",
                    Patronymic = "manager",
                    Password = "123456",
                    PhoneNumber = "manager",
                    RoleName = "manager",
                    BankId = $"{i}"
                });

                //Operators
                context.Users.Add(new User
                {
                    Email = "operator@mail.ru",
                    FirstName = "operator",
                    LastName = "operator",
                    Patronymic = "operator",
                    Password = "123456",
                    PhoneNumber = "operator",
                    RoleName = "operator",
                    BankId = $"{i}"
                });
            }

            context.Companies.Add(new Company
            {
                Id = "0",
                Type = "ИП",
                LegalName = "ZXC",
                LegalAddress = "Беларусь, Минск, ул. Уличная, д.6",
                SalaryMoney = 100000,
                Percent = -10,
                BankId = "0",
                BIC = "000000001",
                PIN = "000000001"
            });

            context.Companies.Add(new Company
            {
                Id = "1",
                Type = "ИП",
                LegalName = "Эпл",
                LegalAddress = "Беларусь, Брест, ул. Яблочная, д.10",
                SalaryMoney = 10000,
                Percent = -8,
                BankId = "0",
                BIC = "000000001",
                PIN = "000000002"
            });

            context.Companies.Add(new Company
            {
                Id = "2",
                Type = "ООО",
                LegalName = "Самсунг",
                LegalAddress = "Беларусь, Гродно, ул. Самая, д.1",
                SalaryMoney = 9000,
                Percent = -5,
                BankId = "0",
                BIC = "000000001",
                PIN = "000000003"
            });

            context.Companies.Add(new Company
            {
                Id = "3",
                Type = "ООО",
                LegalName = "Хуавей",
                LegalAddress = "Беларусь, Волковыск, ул. Хорошая, д.5",
                SalaryMoney = 5000,
                Percent = 0,
                BankId = "1",
                BIC = "000000002",
                PIN = "000000004"
            });

            context.Companies.Add(new Company
            {
                Id = "4",
                Type = "ЗАО",
                LegalName = "Глиммер",
                LegalAddress = "Беларусь, Гомель, ул. Блудная, д.4",
                SalaryMoney = 8000,
                Percent = 0,
                BankId = "1",
                BIC = "000000002",
                PIN = "000000005"
            });

            context.Companies.Add(new Company
            {
                Id = "5",
                Type = "ИП",
                LegalName = "Покемон",
                LegalAddress = "Беларусь, Солигорск, ул. Соленая, д.2",
                SalaryMoney = 3000,
                Percent = 6,
                BankId = "1",
                BIC = "000000002",
                PIN = "000000006"
            });

            context.Companies.Add(new Company
            {
                Id = "6",
                Type = "ЗАО",
                LegalName = "Зорка",
                LegalAddress = "Беларусь, Могилев, ул. Яркая, д.3",
                SalaryMoney = 7000,
                Percent = 2,
                BankId = "1",
                BIC = "000000002",
                PIN = "000000007"
            });

            context.Companies.Add(new Company
            {
                Id = "7",
                Type = "ИП",
                LegalName = "Вагос",
                LegalAddress = "Беларусь, Минск, ул. Желтая, д.56",
                SalaryMoney = 5500,
                Percent = 0,
                BankId = "2",
                BIC = "000000003",
                PIN = "000000008"
            });

            context.Companies.Add(new Company
            {
                Id = "8",
                Type = "ЗАО",
                LegalName = "Андекс",
                LegalAddress = "Беларусь, Копыль, ул. Советская, д.11",
                SalaryMoney = 12000,
                Percent = 5,
                BankId = "2",
                BIC = "000000003",
                PIN = "000000009"
            });

            context.Companies.Add(new Company
            {
                Id = "9",
                Type = "ООО",
                LegalName = "Гугу",
                LegalAddress = "Беларусь, Полоцк, ул. Морозная, д.98",
                SalaryMoney = 3500,
                Percent = 0,
                BankId = "2",
                BIC = "000000003",
                PIN = "000000010"
            });

            await context.SaveChangesAsync();
        }
    }
}
