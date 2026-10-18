using Buza.Models;
using Buza.Services;
using Microsoft.EntityFrameworkCore;

namespace Buza.Data;

public static class DatabaseSeeder
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        await db.Database.MigrateAsync();

        if (!await db.AdminUsers.AnyAsync())
        {
            var initialPassword = configuration["Admin:InitialPassword"];
            if (environment.IsProduction() && (string.IsNullOrWhiteSpace(initialPassword) || initialPassword.Length < 16))
            {
                throw new InvalidOperationException("Для первого запуска в Production задайте Admin:InitialPassword длиной не менее 16 символов.");
            }

            db.AdminUsers.Add(new AdminUser
            {
                Login = "admin",
                PasswordHash = passwordHasher.Hash(string.IsNullOrWhiteSpace(initialPassword) ? "admin123" : initialPassword),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.SiteSettings.AnyAsync())
        {
            db.SiteSettings.Add(CreateDefaultSettings());
        }

        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(CreateDefaultProducts());
        }

        await db.SaveChangesAsync();
    }

    private static SiteSettings CreateDefaultSettings() => new()
    {
        ProjectName = "Напиток здоровья и молодости — буза",
        LogoPath = "",
        Phone = "+7 (000) 000-00-00",
        Email = "info@buza-startup.ru",
        Address = "Республика Башкортостан, г. Уфа",
        SocialLinks = "VK: https://vk.com/\nTelegram: https://t.me/",
        FoundationText = "Проект развивается при поддержке грантовой программы и направлен на популяризацию национального башкирского ферментированного напитка.",
        FoundationUrl = "https://fasie.ru/",
        FederalProjectUrl = "https://национальныепроекты.рф/",
        MainImagePath = "/assets/buza-hero.png",
        HeroTitle = "Натуральная буза для энергии, здоровья и традиций каждый день",
        HeroDescription = "Мягкий ферментированный напиток на основе зерна с деликатным вкусом, живой культурой производства и уважением к башкирской гастрономической традиции.",
        ProductHistory = "Буза известна у тюркских народов как питательный напиток из злаков. В Башкортостане ее ценили за насыщенность, мягкий вкус и способность объединять семью за столом.",
        ProductDescription = "Современная буза сохраняет характер традиционного напитка, но производится в контролируемых условиях с вниманием к качеству сырья, чистоте процесса и стабильности вкуса.",
        ProductComposition = "Овес или другие зерновые культуры, питьевая вода, натуральная закваска, небольшое количество сахара по рецептуре. Точный состав может отличаться для каждой позиции каталога.",
        ProductBenefits = "Напиток содержит питательные вещества зерна, мягко насыщает, подходит для знакомства с национальной кухней и может стать полезной альтернативой сладким газированным напиткам.",
        TechnologyText = "Производство строится на бережной обработке зерна, подготовке сусла, ферментации и охлаждении готового продукта. Каждый этап контролируется по чистоте, времени и температуре.",
        ProductionSteps = "1. Отбор и подготовка сырья\n2. Термическая обработка зерновой основы\n3. Внесение закваски\n4. Контролируемая ферментация\n5. Охлаждение, фасовка и хранение",
        RawMaterials = "Используются качественные зерновые культуры, питьевая вода и натуральная закваска. Поставщики сырья подбираются с учетом стабильности качества и безопасности.",
        StartupDescription = "Стартап создает современную линейку бузы, объединяя национальную рецептуру, понятную упаковку и удобный формат покупки для городского потребителя.",
        ProjectGoals = "Популяризировать башкирский напиток, наладить малосерийное производство и подготовить продукт к масштабированию.",
        DevelopmentStages = "Исследование рецептуры, запуск MVP, дегустации, оформление бренда, развитие каталога, выход на партнерские точки продаж.",
        TeamInfo = "Команда проекта объединяет предпринимательский, технологический и маркетинговый опыт. Информация о составе команды может быть обновлена через административную панель.",
        DevelopmentPlans = "Расширение вкусовой линейки, развитие онлайн-заказов, партнерство с локальными магазинами и участие в культурных гастрономических событиях.",
        UpdatedAt = DateTime.UtcNow
    };

    private static IEnumerable<Product> CreateDefaultProducts()
    {
        var createdAt = DateTime.UtcNow;

        return
        [
            new Product
            {
                Name = "Буза классическая",
                ShortDescription = "Мягкий традиционный вкус на зерновой основе.",
                Description = "Классическая буза с нежной кремовой текстурой и натуральным зерновым послевкусием. Подходит для знакомства с продуктом и семейного стола.",
                Composition = "Зерновая основа, вода питьевая, натуральная закваска, сахар по рецептуре.",
                Volume = "0,5 л",
                Price = 180,
                IsActive = true,
                CreatedAt = createdAt,
                Images =
                [
                    new ProductImage { FilePath = "/assets/buza-product.png", CreatedAt = createdAt }
                ]
            },
            new Product
            {
                Name = "Буза семейная",
                ShortDescription = "Увеличенный объем для дегустаций и домашнего стола.",
                Description = "Формат для семьи, праздничного стола или знакомства гостей с башкирским напитком. Производится небольшими партиями.",
                Composition = "Зерновая основа, вода питьевая, натуральная закваска.",
                Volume = "1 л",
                Price = 320,
                IsActive = true,
                CreatedAt = createdAt,
                Images =
                [
                    new ProductImage { FilePath = "/assets/buza-hero.png", CreatedAt = createdAt }
                ]
            }
        ];
    }
}
