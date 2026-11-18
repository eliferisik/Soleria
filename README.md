# Soleria

Soleria, ASP.NET Core Razor Pages ile hazırlanmış bir ayakkabı mağazası demo uygulamasıdır. Ürün kataloğu, SQLite veritabanı ve ASP.NET Core Identity kullanır.

## Gereksinimler

- .NET 10 SDK
- Visual Studio 2026 veya .NET CLI

## Çalıştırma

1. `Soleria.sln` çözümünü Visual Studio'da açın veya `Soleria.Web` klasörüne geçin.
2. Visual Studio'da F5'e basın. Terminal kullanıyorsanız `dotnet run` komutunu çalıştırın.
3. İlk açılışta SQLite veritabanı `Soleria.Web/App_Data/soleria.db` yolunda oluşturulur ve başlangıç ürünleri eklenir.

Yerel adresler `Soleria.Web/Properties/launchSettings.json` içinde tanımlıdır.

## İçerik

- Kadın, erkek, kız çocuk ve erkek çocuk ayakkabı katalogları
- Ürün detayları, görsel galerisi ve beden seçimi
- Kategori ve ürün araması
- Sepet ve favoriler
- Üyelik ve giriş
- Demo ödeme bilgilendirme sayfası

Ödeme sayfası gerçek ödeme almaz ve kart bilgisi istemez.

## Görseller ve veriler

Ürün görselleri `Soleria.Web/wwwroot/images/` klasöründe bulunur ve projeyle birlikte gelir. Başlangıç kategorileri ve ürünleri `Soleria.Web/Data/DbSeeder.cs` içinde tanımlıdır.

SQLite veritabanı ve çalışma zamanı dosyaları Git'e eklenmez. İlk çalıştırmada veritabanı otomatik oluşturulur.

## Yerel ayarlar

Gizli veya kişisel yerel ayarlar gerekiyorsa `Soleria.Web/appsettings.Local.json` dosyasını kullanın. Bu dosya `.gitignore` ile Git dışında tutulur. Gizli anahtarları `appsettings.json` içine eklemeyin.
