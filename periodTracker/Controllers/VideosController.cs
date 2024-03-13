using Microsoft.AspNetCore.Mvc;
using periodTracker.Models;

namespace periodTracker.Controllers
{
    public class VideosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Videos()
        {
            return View();
        }
      

        public ActionResult Menstruation()
        {
            return File("~/Videos/Menstruation Stigma.mp4", "video/mp4");
        }
        public ActionResult Menstruation2()
        {
            return File("~/Videos/Menstruation What To Expect.mp4", "video/mp4");
        }

        public ActionResult Menstruation3()
        {
            return File("~/Videos/The Menstrual Cycle.mp4", "video/mp4");
        }

        public ActionResult Menstruation4()
        {
            return File("~/Videos/Period Symptoms and Self Care (1).mp4", "video/mp4");
        }
        public ActionResult Menstruation5()
        {
            return File("~/Videos/4 Yoga Poses to Relieve Cramps   Healthline.mp4", "video/mp4");
        }
        public ActionResult Menstruation6()
        {
            return File("~/Videos/How To Get Instant Relief From Menstrual Cramps And Mood Swings   Home Remedies with Upasana.mp4", "video/mp4");
        }
        public ActionResult Menstruation7()
        {
            return File("~/Videos/Period pain Try these remedies.mp4", "video/mp4");
        }
        public ActionResult Menstruation8()
        {
            return File("~/Videos/Yoga For Period Relief   How To Reduce Menstrual Pain   Glamrs Period Hacks.mp4", "video/mp4");
        }
        public ActionResult Menstruation9()
        {
            return File("~/Videos/Learn All You Need To Know About Period Hygiene With These Tips!.mp4", "video/mp4");
        }
       
        public ActionResult Menopause()
        {
            return File("~/Videos/Menopause Animation.mp4", "video/mp4");
        }
        public ActionResult Menopause2()
        { 
            return File("~/Videos/Menopause Explained.mp4", "video/mp4");
        }
        public ActionResult Menopause3()
        {
            return File("~/Videos/The 3 Stages of Menopause.mp4", "video/mp4");
        }
        public ActionResult Menopause4()
        {
            return File("~/Videos/What Are the Signs and Symptoms of Menopause.mp4", "video/mp4");
        }
       public ActionResult Pregnancy()
        {
            return File("~/Videos/10 signs you're pregnant.mp4", "video/mp4");
        }
        public ActionResult Pregnancy2()
        {
            return File("~/Videos/The surprising effects of pregnancy.mp4", "video/mp4");
        }
        public ActionResult Pregnancy3()
        {
            return File("~/Videos/Your Organs During Pregnancy.mp4", "video/mp4");
        }
        public ActionResult Pregnancy4()
        {
            return File("~/Videos/Healthy Habits Pregnancy.mp4", "video/mp4");
        }
    }
}
