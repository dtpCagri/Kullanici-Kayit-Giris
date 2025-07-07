using System.Diagnostics;
using KayitGiris.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace KayitGiris.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // VeriTabaný Baðlantýsý
        private string connectionString = "Server=.;Database=KayitGirisDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult KullaniciKaydi()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Giris(Kullanici kullanici)
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection(connectionString))
                {
                    string sorgu = "SELECT * FROM Kullanicilar WHERE TC_No = @TC AND Sifre = @Sifre";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@TC", kullanici.TC);
                    komut.Parameters.AddWithValue("@Sifre", kullanici.Sifre);


                    baglanti.Open();
                    SqlDataReader reader = komut.ExecuteReader();
                    if (reader.HasRows)
                    {
                        TempData["GirisBilgisi"] = "Giris Basarili!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["GirisBilgisi"] = "TC Kimlik No veya Sifre Hatali!";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["GirisBilgisi"] = "Hata: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]

        public IActionResult Kayit(Kullanici kullanici)
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection(connectionString))
                {
                    string sorgu = "INSERT INTO Kullanicilar (TC_No, Sifre) VALUES (@TC, @Sifre)";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@TC", kullanici.TC);
                    komut.Parameters.AddWithValue("@Sifre", kullanici.Sifre);
                    baglanti.Open();
                    int sonuc = komut.ExecuteNonQuery();
                    if (sonuc > 0)
                    {
                        TempData["GirisBilgisi"] = "Kullanici Kaydi Basarili!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["KayitBilgisi"] = "Kullanici Kaydi Basarisiz!";
                        return RedirectToAction("KullaniciKaydi");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["KayitBilgisi"] = "Hata: " + ex.Message;
                return RedirectToAction("KullaniciKaydi");
            }
        }
    }
}
