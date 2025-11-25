using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManResort.Pages
{
    public class AccommodationModel : PageModel
    {
        public List<Accommodation> Accommodations { get; set; }

        public void OnGet()
        {
            Accommodations = new List<Accommodation>
        {
            new Accommodation { Name = "Club Room", HourRate = 100, DayRate = 150, NightRate = 200, ImageUrl = "Images/DeluxeRoom.jpg" },
            new Accommodation { Name = "Deluxe Room", HourRate = 150, DayRate = 200, NightRate = 300, ImageUrl = "Images/ClubRoom.jpg" },
            new Accommodation { Name = "Grand Suite(Fan)", HourRate = 180, DayRate = 250, NightRate = 350, ImageUrl = "Images/GrandSuite.jpeg" },
            new Accommodation { Name = "Presidential Suite(Air Con)", HourRate = 200, DayRate = 350, NightRate = 400, ImageUrl = "Images/PresSuite.jpeg" }
        };
        }
    }
}

public class Accommodation
{
    public string Name { get; set; }
    public decimal HourRate { get; set; }
    public decimal DayRate { get; set; }
    public decimal NightRate { get; set; }
    public string ImageUrl { get; set; }
}

