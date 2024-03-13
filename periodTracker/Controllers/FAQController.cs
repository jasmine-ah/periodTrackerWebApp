using Microsoft.AspNetCore.Mvc;
using periodTracker.Models;

namespace periodTracker.Controllers
{
    public class FAQController : Controller
    {
        /* public IActionResult Index()
         {
             return View();
         }*/
        public IActionResult Faq()
        {
            var faqItems = new List<FaqItem>
    {
        new FaqItem {Id=1, Question = "What is menustration?", Answer = "Menstruation is the shedding of the uterine lining that occurs in females of reproductive age, typically on a monthly basis. It is a sign that the body is preparing for a potential pregnancy." },
        new FaqItem {Id=2, Question = "When does menstruation start? ", Answer = "Menstruation usually starts between the ages of 11 and 14, although it can start earlier or later. The first menstrual period is called menarche." },
        new FaqItem { Id=3,Question = "How long does menstruation last?", Answer = "Menstruation can last anywhere from 3 to 7 days, with an average length of 5 days." },
        new FaqItem {Id = 4,  Question = "How often does menstruation occur?", Answer = "Menstruation typically occurs every 21 to 35 days, with an average cycle length of 28 days." },
        new FaqItem {Id = 5,  Question = "Can a person get pregnant during menstruation?", Answer = "While it is less likely, it is still possible to get pregnant during menstruation. Sperm can survive in the body for up to 5 days, so if ovulation occurs shortly after menstruation, pregnancy could result." },
        new FaqItem {Id = 6,  Question = "What is pregnancy?", Answer = "Pregnancy is the state of carrying a developing fetus in the uterus. It typically lasts for 40 weeks, divided into three trimesters." },
        new FaqItem {Id = 7,  Question = "How is pregnancy diagnosed?", Answer = "Pregnancy can be diagnosed through a variety of methods, including a home pregnancy test, a blood test, or an ultrasound." },
        new FaqItem {Id = 8,  Question = "What are the symptoms of pregnancy?", Answer = "Common symptoms of pregnancy include missed period, nausea, breast tenderness, fatigue, and frequent urination." },
        new FaqItem {Id = 9,  Question = "What is menopause?", Answer = "Menopause is the natural biological process that marks the end of a woman's reproductive years. It is defined as the absence of menstrual periods for 12 consecutive months." },
        new FaqItem {Id = 10,  Question = "What are the symptoms of menopause?", Answer = "Common symptoms of menopause include hot flashes, night sweats, vaginal dryness, mood changes, and difficulty sleeping." },
        new FaqItem {Id = 11,  Question = "Can menopause be treated?", Answer = "There are several treatments available for menopause, including hormone replacement therapy, lifestyle changes, and complementary therapies." },
        new FaqItem {Id = 12,  Question = "What is PMS? ", Answer = "PMS, or premenstrual syndrome, is a collection of symptoms that occur in the days leading up to menstruation. These symptoms can include mood swings, bloating, cramps, and breast tenderness." },
        new FaqItem {Id = 13,  Question = "Is PMS normal? ", Answer = "Yes, PMS is a normal part of the menstrual cycle for many women. However, if PMS symptoms are severe or interfere with daily life, there may be treatment options available." },
        new FaqItem {Id = 14,  Question = "What is period poverty?", Answer = "Period poverty is the lack of access to menstrual products, education, and facilities needed to manage menstruation. This can have a significant impact on the health, dignity, and education of girls and women." },
        new FaqItem {Id = 15,  Question = "What are positive aspects of menstruation?", Answer = "While menstruation can be associated with discomfort and inconvenience, it is also a sign of reproductive health and fertility. Additionally, menstruation can be a reminder of the power and resilience of the female body." },
        new FaqItem {Id = 16,  Question = "Does menstruation limit what people can do?", Answer = "While menstruation can cause discomfort and inconvenience, it does not need to limit what people can do. With proper management and care, people can continue to participate in all aspects of daily life during menstruation." },
        // Add more FAQ items as needed
    };
        
              return View(faqItems);
        }
    }
}

