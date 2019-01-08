using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequiredObjects;
using System.IO;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace ControllerClass
{
    class VisitorController {

        public List<Visitor> visitorList;
        public VisitorController(List<Visitor> visitorList) {
            this.visitorList = visitorList;
        }

        public List<Visitor> merge_sort(List<Visitor> visitor_list)
        {
            int final_index = visitor_list.Count;
            int middle_index = final_index / 2;

            if (final_index <= 1)
            {
                return visitor_list;
            }

            List<Visitor> first_list = new List<Visitor>();
            List<Visitor> second_list = new List<Visitor>();

            for (int i = 0; i < middle_index; i++)
            {
                first_list.Add(visitor_list[i]);
            }
            for (int i = middle_index; i < final_index; i++)
            {
                second_list.Add(visitor_list[i]);
            }

            merge_sort(first_list);
            merge_sort(second_list);

            merge_array(first_list, second_list, visitor_list);
            return visitor_list;
        }

        public void merge_array(List<Visitor> first_list, List<Visitor> second_list, List<Visitor> visitor_list)
        {
            int first_index = 0;
            int second_index = 0;
            int final_index = 0;

            while (first_index < first_list.Count && second_index < second_list.Count)
            {
                if (first_list[first_index].VisitorId < second_list[second_index].VisitorId)
                {
                    visitor_list.RemoveAt(final_index);
                    visitor_list.Insert(final_index, first_list[first_index]);
                    first_index += 1;
                }
                else
                {
                    visitor_list.RemoveAt(final_index);
                    visitor_list.Insert(final_index, second_list[second_index]);
                    second_index += 1;
                }
                final_index += 1;
            }
            while (first_list.Count > first_index)
            {
                visitor_list.RemoveAt(final_index);
                visitor_list.Insert(final_index, first_list[first_index]);
                first_index += 1;
                final_index += 1;
            }
            while (second_list.Count > second_index)
            {
                visitor_list.RemoveAt(final_index);
                visitor_list.Insert(final_index, second_list[second_index]);
                final_index += 1;
                second_index += 1;
            }
        }

        public Visitor insert_visitor(Visitor visitor)
        {
            
            Random rand_int = new Random();
            visitor.VisitorId = rand_int.Next(1, 10000);
            visitorList.Add(visitor);
            return visitor;
       }

        public void read_visitor_csv(string file_path)
        {
            try
            {
                using (var reader = new StreamReader(file_path))
                {
                    if (!reader.EndOfStream)
                    {
                        reader.ReadLine();
                    }
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        Visitor visitor = new Visitor(values[1], values[2], values[3], values[4], values[5], values[6], values[7]);
                        visitor.VisitorId = Int32.Parse(values[0]);

                        if (this.get_visitor(visitor.VisitorId) == null  )
                        {
                            this.visitorList.Add(visitor);
                        }
                        
                    }
                }

            }
            catch (IOException)
            {
                Console.WriteLine("File not found");
            }

        }

        public Visitor get_visitor(int visitor_id)
        {
            int min_index = 0;
            int max_index = this.visitorList.Count - 1;
            if (max_index < 0)
            {
                return null;
            }

            visitorList = merge_sort(visitorList);

            while (min_index <= max_index)
            {
                int mid_index = (min_index + max_index) / 2;
                if (visitorList[mid_index].VisitorId == visitor_id)
                {
                    return visitorList[mid_index];
                }
                else if (visitor_id > visitorList[mid_index].VisitorId)
                {
                    min_index = mid_index + 1;
                }
                else if (visitor_id < visitorList[mid_index].VisitorId)
                {
                    max_index = mid_index - 1;
                }
            }
            return null;
        }

        public void initiate_visitors_data(string file_path)
        {
            string desktop_path = file_path;
            using (var writer = new StreamWriter(desktop_path))
            {
                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", "Visitor ID", "FirstName", "LastName", "Contact", "Occupancy", "Gender", "Country", "Email");
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        public void write_visitors_data(Visitor visitor, string file_path)
        {
            string desktop_path = file_path;
            using (var writer = new StreamWriter(desktop_path, append: true))
            {
                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", visitor.VisitorId, visitor.FirstName, visitor.LastName, visitor.Contact, visitor.Occupancy, visitor.Gender, visitor.Country, visitor.Email);
                writer.WriteLine(line);
                writer.Flush();
            }
        }

    }

    class EntryController
    {
        public List<VisitorsEntry> VisitorEntryList;

        VisitorController visitorController;
       
        public EntryController(VisitorController visitorController, List<VisitorsEntry> VisitorEntryList) {
            this.visitorController = visitorController;
            this.VisitorEntryList = VisitorEntryList;
        }

        public List<VisitorsEntry> entry_merge_sort(List<VisitorsEntry> visitor_entry_list)
        {
            int final_index = visitor_entry_list.Count;
            int middle_index = final_index / 2;

            if (final_index <= 1)
            {
                return visitor_entry_list;
            }

            List<VisitorsEntry> first_list = new List<VisitorsEntry>();
            List<VisitorsEntry> second_list = new List<VisitorsEntry>();

            for (int i = 0; i < middle_index; i++)
            {
                first_list.Add(visitor_entry_list[i]);
            }
            for (int i = middle_index; i < final_index; i++)
            {
                second_list.Add(visitor_entry_list[i]);
            }

            entry_merge_sort(first_list);
            entry_merge_sort(second_list);

            entry_merge_array(first_list, second_list, visitor_entry_list);
            return visitor_entry_list;
        }

        public void entry_merge_array(List<VisitorsEntry> first_list, List<VisitorsEntry> second_list, List<VisitorsEntry> visitor_entry_list)
        {
            int first_index = 0;
            int second_index = 0;
            int final_index = 0;

            while (first_index < first_list.Count && second_index < second_list.Count)
            {
                if (first_list[first_index].EntryTime < second_list[second_index].EntryTime)
                {
                    visitor_entry_list.RemoveAt(final_index);
                    visitor_entry_list.Insert(final_index, first_list[first_index]);
                    first_index += 1;
                }
                else
                {
                    visitor_entry_list.RemoveAt(final_index);
                    visitor_entry_list.Insert(final_index, second_list[second_index]);
                    second_index += 1;
                }
                final_index += 1;
            }
            while (first_list.Count > first_index)
            {
                visitor_entry_list.RemoveAt(final_index);
                visitor_entry_list.Insert(final_index, first_list[first_index]);
                first_index += 1;
                final_index += 1;
            }
            while (second_list.Count > second_index)
            {
                visitor_entry_list.RemoveAt(final_index);
                visitor_entry_list.Insert(final_index, second_list[second_index]);
                final_index += 1;
                second_index += 1;
            }
        }

        public void read_entry_csv(string file_path)
        {
            try
            {
                using (var reader = new StreamReader(file_path))
                {
                    if (!reader.EndOfStream)
                    {
                        reader.ReadLine();
                    }
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        VisitorsEntry visitorEntry = new VisitorsEntry(Convert.ToDateTime(values[1]), TimeSpan.Parse(values[2]), TimeSpan.Parse(values[3]));
                        visitorEntry.VisitorId = Int32.Parse(values[0]);

                        if (visitorController.get_visitor(visitorEntry.VisitorId) != null)
                        {
                            this.VisitorEntryList.Add(visitorEntry);
                        }
                    }
                }

            }
            catch (IOException)
            {
                Console.WriteLine("File not found");
            }

        }

        public VisitorsEntry get_entry(TimeSpan entryTime)
        {
            int min_index = 0;
            int max_index = VisitorEntryList.Count -1;
            int mid_index = (min_index + max_index) / 2; 
            if (max_index == 0)
            {
                return null;
            }

            while (min_index <= max_index)
            {
                mid_index = ((min_index + max_index) / 2);
                Console.WriteLine(VisitorEntryList.Count);
                Console.WriteLine(mid_index);
                if (VisitorEntryList[mid_index].EntryTime == entryTime)
                {
                    return VisitorEntryList[mid_index];
                }
                else if (entryTime > VisitorEntryList[mid_index].EntryTime)
                {
                    min_index = mid_index + 1;
                }
                else if (entryTime < VisitorEntryList[mid_index].EntryTime)
                {
                    max_index = mid_index - 1;
                }
            }
            return null;
        }

        public void initiate_entry_data(string file_path)
        {
            using (var writer = new StreamWriter(file_path))
            {
                var line = string.Format("{0}, {1}, {2}, {3}", "Visitor Id", "Entry Date", "Entry Time", "Exit Time");
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        public void write_entry_data(VisitorsEntry visitorEntry, string file_path)
        {
            string desktop_path = file_path;
            using (var writer = new StreamWriter(desktop_path, append: true))
            {
                var line = string.Format("{0}, {1}, {2}, {3}", visitorEntry.VisitorId, visitorEntry.EntryDate, visitorEntry.EntryTime, visitorEntry.ExitTime);
                writer.WriteLine(line);
                writer.Flush();
            }
        }


    }
}
