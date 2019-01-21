using System;
using System.Collections.Generic;
using RequiredObjects;
using System.IO;

//er.sushilsapkota@gmail.com

namespace ControllerClass
{
    class VisitorController {


        string visitor_file_2 = "visitors_file.csv";
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
            int visitorId = rand_int.Next(1000, 9999);
            while (get_visitor(visitorId) != null) {
                visitorId = rand_int.Next(1000, 9999);
            }
            visitor.VisitorId = visitorId;
            visitorList.Add(visitor);
            return visitor;
       }

        public List<Visitor> read_visitor_csv(string file_path)
        {
            List<Visitor> uploadedVisitorsList = new List<Visitor>();
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

                        if (this.get_visitor(visitor.VisitorId) == null)
                        {
                            uploadedVisitorsList.Add(visitor);
                            this.visitorList.Add(visitor);
                        }
                    }
                }

            }
            catch (IOException)
            {
                Console.WriteLine("File not found : " + file_path);
            }
            return uploadedVisitorsList;
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
            using (var writer = new StreamWriter(file_path, append: true))
            {
                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", visitor.VisitorId, visitor.FirstName, visitor.LastName, visitor.Contact, visitor.Occupancy, visitor.Gender, visitor.Country, visitor.Email);
                writer.WriteLine(line);
                writer.Flush();
            }
        }

    }

    class ReportController {
        
        public List<VisitorsEntry> VisitorEntryList;

        public EntryController entryController;

        public ReportController(List<VisitorsEntry> VisitorEntryList) {
            entryController = new EntryController();
            this.VisitorEntryList = VisitorEntryList;
        }

        public List<VisitorsEntry> getDailyReport(DateTime dateTime)
        {

            //Passing by value
            List<VisitorsEntry> currentVisitorsEntry = entryController.entry_merge_sort(new List<VisitorsEntry>(this.VisitorEntryList), "DATE");
            List<VisitorsEntry> foundEntries = new List<VisitorsEntry>();
            currentVisitorsEntry = entryController.entry_merge_sort(currentVisitorsEntry, "DATE");

            int min_index = 0;
            int max_index = currentVisitorsEntry.Count - 1;
            
            int mid_index = (min_index + max_index) / 2;
            if (max_index == 0)
            {
                if (currentVisitorsEntry[0].EntryDate.CompareTo(dateTime) == 0) {
                    return currentVisitorsEntry;
                }
            }
            
            while (min_index <= max_index)
            {
                mid_index = (min_index + max_index) / 2;
                DateTime selectedEntry = currentVisitorsEntry[mid_index].EntryDate;
                if (selectedEntry.CompareTo(dateTime) == 0)
                {
                    foundEntries.Add(currentVisitorsEntry[mid_index]);
                    currentVisitorsEntry.RemoveAt(mid_index);
                    max_index -= 1;
                }
                else if (dateTime.CompareTo(selectedEntry) == 1)
                {
                    min_index = mid_index + 1;
                }
                else if (dateTime.CompareTo(selectedEntry) == -1)
                {
                    max_index = mid_index - 1;
                }
            }
            return foundEntries;
        }

        public List<VisitorsEntry> getMonthlyReport(DateTime dateTime)
        {

            //Passing by value
            List<VisitorsEntry> currentVisitorsEntry = entryController.entry_merge_sort(new List<VisitorsEntry>(VisitorEntryList), "DATE");
            List<VisitorsEntry> foundEntries = new List<VisitorsEntry>();
            
            int min_index = 0;
            int max_index = currentVisitorsEntry.Count - 1;

            int mid_index = (min_index + max_index) / 2;
            if (max_index == 0)
            {
                return currentVisitorsEntry;
            }

            int count = 0;
            while (min_index <= max_index)
            {

                mid_index = (min_index + max_index) / 2;
                
                DateTime selectedEntry = currentVisitorsEntry[mid_index].EntryDate;
                Boolean isInBound = true;
                

                if (selectedEntry.CompareTo(dateTime) == 0 && isInBound)
                {
                    count += 1;
                    foundEntries.Add(currentVisitorsEntry[mid_index]);
                    currentVisitorsEntry.RemoveAt(min_index);
                    max_index -= 1;
                }
                else if (dateTime.CompareTo(selectedEntry) == 1)
                {
                    min_index = mid_index + 1;
                }
                else if (dateTime.CompareTo(selectedEntry) == -1)
                {
                    max_index = mid_index - 1;
                }
            }
            return foundEntries;
        }



    }

    class EntryController
    {
        public List<VisitorsEntry> VisitorEntryList;

        VisitorController visitorController;

        public EntryController() { }
       
        public EntryController(VisitorController visitorController, List<VisitorsEntry> VisitorEntryList) {
            this.visitorController = visitorController;
            this.VisitorEntryList = VisitorEntryList;
        }


        public VisitorsEntry getEntryById(int VisitorId)
        {

            
            VisitorEntryList = entry_merge_sort(VisitorEntryList, "ID");
            

            
            int min_index = 0;
            int max_index = VisitorEntryList.Count-1;
            int mid_index = (min_index + max_index) / 2;
            if (max_index == 1)
            {
                return VisitorEntryList[0];
            }

            while (min_index <= max_index)
            {
                mid_index = (min_index + max_index) / 2;
                if (VisitorEntryList[mid_index].VisitorId == VisitorId)
                {
                    return VisitorEntryList[mid_index];
                }
                else if (VisitorEntryList[mid_index].VisitorId > VisitorId)
                {
                    min_index = mid_index + 1;
                }
                else if (VisitorEntryList[mid_index].VisitorId < VisitorId)
                {
                    max_index = mid_index - 1;
                }
            }
            return null;
        }

        public List<VisitorsEntry> getManyEntryById(int VisitorId)
        {
            
            VisitorEntryList = entry_merge_sort(VisitorEntryList, "ID");
            List<VisitorsEntry> searchList = new List<VisitorsEntry>(VisitorEntryList);
            List<VisitorsEntry> foundList = new List<VisitorsEntry>();

            int min_index = 0;
            int max_index = searchList.Count-1;
            int mid_index = (min_index + max_index) / 2;
            if (max_index == 0)
            {
                if (searchList[0].VisitorId == VisitorId) {
                    return searchList;
                }
            }

            while (min_index <= max_index)
            {
                mid_index = (min_index + max_index) / 2;

                int ValidVisitorId = searchList[mid_index].VisitorId; ;
                

                if (ValidVisitorId == VisitorId)
                {

                    foundList.Add(searchList[mid_index]);
                    searchList.RemoveAt(mid_index);
                    max_index -= 1;
                }
                else if (ValidVisitorId < VisitorId)
                {
                    min_index = mid_index + 1;
                }
                else if (ValidVisitorId > VisitorId)
                {
                    max_index = mid_index - 1;
                }
            }
            return foundList;
        }


        public VisitorsEntry getEntryByDate(TimeSpan entryTime)
        {

            VisitorEntryList = entry_merge_sort(VisitorEntryList, "TIME");

            int min_index = 0;
            int max_index = VisitorEntryList.Count;
            int mid_index = (min_index + max_index) / 2;
            if (max_index == 1)
            {
                return null;
            }

            while (min_index <= max_index)
            {
                mid_index = (min_index + max_index) / 2;

                if (VisitorEntryList[mid_index].EntryTime.CompareTo(entryTime) == 0)
                {
                    return VisitorEntryList[mid_index];
                }
                else if (VisitorEntryList[mid_index].EntryTime.CompareTo(entryTime) == -1)
                {
                    min_index = mid_index + 1;
                }
                else if (VisitorEntryList[mid_index].EntryTime.CompareTo(entryTime) == 1)
                {
                    max_index = mid_index - 1;
                }
            }
            return null;
        }

        public List<VisitorsEntry> entry_merge_sort(List<VisitorsEntry> visitor_entry_list, String condition)
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

            entry_merge_sort(first_list, condition);
            entry_merge_sort(second_list, condition);

            entry_merge_array(first_list, second_list, visitor_entry_list, condition);
            return visitor_entry_list;
        }

        public void entry_merge_array(List<VisitorsEntry> first_list, List<VisitorsEntry> second_list, List<VisitorsEntry> visitor_entry_list, string condition)
        {
            int first_index = 0;
            int second_index = 0;
            int final_index = 0;
            
            while (first_index < first_list.Count && second_index < second_list.Count)
            {
                Boolean conditionId = first_list[first_index].VisitorId < second_list[second_index].VisitorId;
                Boolean conditionDate = (first_list[first_index].EntryDate.CompareTo(second_list[second_index].EntryDate) <= 0);
                Boolean conditionTime = (first_list[first_index].EntryTime.CompareTo(second_list[second_index].EntryTime) <= 0) && (first_list[first_index].EntryDate.CompareTo(second_list[second_index].EntryDate) <= 0);
                
                Boolean checkCondition;
                switch (condition) {
                    case "ID":
                        checkCondition = conditionId;
                        break;

                    case "DATE":
                        checkCondition = conditionDate;
                        break;

                    case "TIME":
                        checkCondition = conditionTime;
                        break;

                    default:
                        checkCondition = false;
                        break;
                }

                if (checkCondition) 
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
                        
                        VisitorsEntry visitorEntry = new VisitorsEntry(Convert.ToDateTime(values[1]), values[2].ToString(), TimeSpan.Parse(values[3]), TimeSpan.Parse(values[4]), Convert.ToDouble(values[5]));
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
                Console.WriteLine("File not found : " + file_path);
            }

        }

        

        public void initiate_entry_data(string file_path)
        {
            using (var writer = new StreamWriter(file_path))
            {
                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", "Visitor Id", "Day" ,"Entry Date", "Entry Time", "Exit Time", "Duration");
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        public void write_entry_data(VisitorsEntry visitorEntry, string file_path)
        {
            string desktop_path = file_path;
            using (var writer = new StreamWriter(desktop_path, append: true))
            {

                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", visitorEntry.VisitorId,  visitorEntry.EntryDate, visitorEntry.Day, visitorEntry.EntryTime, visitorEntry.ExitTime, visitorEntry.Duration );

                writer.WriteLine(line);
                writer.Flush();
            }
        }


    }
}
