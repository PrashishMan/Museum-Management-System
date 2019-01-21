using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequiredObjects
{
    public class Visitor
    {
        
        public int VisitorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public string Occupancy { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }

        public Visitor(string FirstName, string LastName, string Contact, string Occupancy, string Gender, string Country, string Email) {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Contact = Contact;
            this.Occupancy = Occupancy;
            this.Gender = Gender;
            this.Country = Country;
            this.Email = Email;                
        }

        public Visitor() { }

    }

    public class VisitorsEntry {
        public int VisitorId { get; set; }
        public String Day { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan EntryTime { get; set; }
        public TimeSpan ExitTime { get; set; }
        public Double Duration { get; set; }

        public VisitorsEntry() { }

        public VisitorsEntry(DateTime EntryDate, String Day, TimeSpan EntryTime, TimeSpan ExitTime, Double Duration)
        {
            this.EntryDate = EntryDate;
            this.Day = Day;
            this.EntryTime = EntryTime;
            this.ExitTime = ExitTime;
            this.Duration = Duration;

        }
        

        public VisitorsEntry(int VisitorId, String Day, DateTime EntryDate, TimeSpan EntryTime)
        {

            this.VisitorId = VisitorId;
            this.EntryDate = EntryDate;
            this.EntryTime = EntryTime;
            this.Day = Day;
        }

        public VisitorsEntry(int VisitorId, String Day, DateTime EntryDate, TimeSpan EntryTime, TimeSpan ExitTime, Double Duration) {
            this.VisitorId = VisitorId;
            this.EntryDate = EntryDate;
            this.EntryTime = EntryTime;
            this.ExitTime = ExitTime;
            this.Day = Day;
            this.Duration = Duration;
        }
    }
}
