using System;
using System.Collections.Generic;
using RequiredObjects;
using System.IO;

//er.sushilsapkota@gmail.com

namespace ControllerClass
{
    class VisitorController {
        public List<Visitor> visitorList;
        public VisitorController(List<Visitor> visitorList) {
            this.visitorList = visitorList;
        }

        public List<Visitor> mergeSort(List<Visitor> visitorList)
        {
            // get total visitors in  visitors list
            int finalIndex = visitorList.Count;
            // get middle index
            int middleIndex = finalIndex / 2;

            // if the list contains only one value return the list
            if (finalIndex <= 1)
            {
                return visitorList;
            }

                
            List<Visitor> firstList = new List<Visitor>();
            List<Visitor> secondList = new List<Visitor>();

            // initialize first list from index 0 to mid index
            for (int i = 0; i < middleIndex; i++)
            {
                firstList.Add(visitorList[i]);
            }
            // initialize second list from middle index to last index
            for (int i = middleIndex; i < finalIndex; i++)
            {
                secondList.Add(visitorList[i]);
            }

            // apply recurssion
            mergeSort(firstList);
            mergeSort(secondList);

            // compares the visitors id value and sorts
            mergeArray(firstList, secondList, visitorList);
            return visitorList;
        }

        public void mergeArray(List<Visitor> firstList, List<Visitor> secondList, List<Visitor> visitorList)
        {
            int first_index = 0;
            int second_index = 0;
            int finalIndex = 0;

            // check if the list cout is greater than its index 
            while (first_index < firstList.Count && second_index < secondList.Count)
            {
                // if visitor id in first list is smaller than the second list
                if (firstList[first_index].VisitorId < secondList[second_index].VisitorId)
                {
                    // insert first list value in the merged array
                    visitorList.RemoveAt(finalIndex);
                    visitorList.Insert(finalIndex, firstList[first_index]);
                    first_index += 1;
                }
                // if second list's visitor id is smaller that first
                else
                {
                    visitorList.RemoveAt(finalIndex);
                    visitorList.Insert(finalIndex, secondList[second_index]);
                    second_index += 1;
                }
                finalIndex += 1;
            }

            // if there is still visitors in the first array list
            // add them to the merged array
            while (firstList.Count > first_index)
            {
                visitorList.RemoveAt(finalIndex);
                visitorList.Insert(finalIndex, firstList[first_index]);
                first_index += 1;
                finalIndex += 1;
            }

            // if there is still visitors in the second array list
            // add them to the merged array
            while (secondList.Count > second_index)
            {
                visitorList.RemoveAt(finalIndex);
                visitorList.Insert(finalIndex, secondList[second_index]);
                finalIndex += 1;
                second_index += 1;
            }
        }

        // inserting mer visitor to the visitors list
        public Visitor insertVisitor(Visitor visitor)
        {
            // generate random id to the newly created visitor
            Random rand_int = new Random();
            int visitorId = rand_int.Next(1000, 9999);

            // random number should be of four digits
            while (getVisitor(visitorId) != null) {
                visitorId = rand_int.Next(1000, 9999);
            }
            visitor.VisitorId = visitorId;
            visitorList.Add(visitor);
            return visitor;
       }

        // reads from the csv file
        public List<Visitor> readVisitorCSV(string filePath)
        {
            List<Visitor> uploadedVisitorsList = new List<Visitor>();
            try
            {
                
                using (var reader = new StreamReader(filePath))
                {
                    // ignoring the first item in the csv file
                    if (!reader.EndOfStream)
                    {
                        // reads each line in the csv file
                        reader.ReadLine();
                    }

                    // until the reader reaches the end
                    // initialize visitor object with the value in the list 
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        Visitor visitor = new Visitor(values[1], values[2], values[3], values[4], values[5], values[6], values[7]);
                        visitor.VisitorId = Int32.Parse(values[0]);

                        // check if the visitor with the visitor id already exists in the current program list
                        if (this.getVisitor(visitor.VisitorId) == null)
                        {
                            uploadedVisitorsList.Add(visitor);
                            this.visitorList.Add(visitor);
                        }
                    }
                }

            }
            catch (IOException)
            {
                Console.WriteLine("File not found : " + filePath);
            }
            return uploadedVisitorsList;
        }

        // uses binary search to get visitor
        public Visitor getVisitor(int visitorId)
        {
            int minIndex = 0;
            int maxIndex = this.visitorList.Count - 1;
            if (maxIndex < 0)
            {
                return null;
            }

            // sorting the array by visitors id
            visitorList = mergeSort(visitorList);

            // check if min index does not exceed the min index
            while (minIndex <= maxIndex)
            {
                // get the list value at the middle index
                int mid_index = (minIndex + maxIndex) / 2;
                if (visitorList[mid_index].VisitorId == visitorId)
                {
                    // if visitor id matches return visitor
                    return visitorList[mid_index];
                }
                // if visitor Id is greated than value in the list change minimum index to middle index
                else if (visitorId > visitorList[mid_index].VisitorId)
                {
                    minIndex = mid_index + 1;
                }
                // if visitor id is smaller than the value in the list change max index to mid index
                else if (visitorId < visitorList[mid_index].VisitorId)
                {
                    maxIndex = mid_index - 1;
                }
            }
            // if no visitor is found return null
            return null;
        }

        // initializes the csv file with the columns headers
        public void initiateVisitorData(string filePath)
        {
            string desktop_path = filePath;
            using (var writer = new StreamWriter(desktop_path))
            {
                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", "Visitor ID", "FirstName", "LastName", "Contact", "Occupancy", "Gender", "Country", "Email");
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        // writes visitor data to the csv file
        public void writeVisitorsData(Visitor visitor, string filePath)
        {
            using (var writer = new StreamWriter(filePath, append: true))
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

        // generates list daily report based on the date inserted provided by the user
        // uses binary search algorithm
        public List<VisitorsEntry> getDailyReport(DateTime dateTime)
        {

            //Passing by value so the changed in the list does not effect the actual list
            List<VisitorsEntry> currentVisitorsEntry = entryController.entryMergeSort(new List<VisitorsEntry>(this.VisitorEntryList), "DATE");
            // stores the list of found entries
            List<VisitorsEntry> foundEntries = new List<VisitorsEntry>();

            // sorting the visitors entry by date 
            currentVisitorsEntry = entryController.entryMergeSort(currentVisitorsEntry, "DATE");

            int minIndex = 0;
            int maxIndex = currentVisitorsEntry.Count - 1;
            int mid_index = (minIndex + maxIndex) / 2;
            // if the list has only one value return the list of the date selectd by the user matches
            if (maxIndex == 0)
            {
                if (currentVisitorsEntry[0].EntryDate.CompareTo(dateTime) == 0) {
                    return currentVisitorsEntry;
                }
            }
            
           
            while (minIndex <= maxIndex)
            {
                mid_index = (minIndex + maxIndex) / 2;

                // get entry time of visitor in the middle index
                DateTime selectedEntry = currentVisitorsEntry[mid_index].EntryDate;

                // if the date from the visitor entry list matches the user selected date
                // insert the object to the visitor found list
                if (selectedEntry.CompareTo(dateTime) == 0)
                {
                    foundEntries.Add(currentVisitorsEntry[mid_index]);
                    currentVisitorsEntry.RemoveAt(mid_index);
                    // decreate the index to prevent array index out of bound
                    maxIndex -= 1;
                }
                else if (dateTime.CompareTo(selectedEntry) == 1)
                {
                    minIndex = mid_index + 1;
                }
                else if (dateTime.CompareTo(selectedEntry) == -1)
                {
                    maxIndex = mid_index - 1;
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

        // uses binary search to retrive visitor entry by id
        // similer to earlier process for retriving visitor
        public VisitorsEntry getEntryById(int VisitorId)
        {

            
            VisitorEntryList = entryMergeSort(VisitorEntryList, "ID");
            

            
            int minIndex = 0;
            int maxIndex = VisitorEntryList.Count-1;
            int mid_index = (minIndex + maxIndex) / 2;
            if (maxIndex == 1)
            {
                return VisitorEntryList[0];
            }

            while (minIndex <= maxIndex)
            {
                mid_index = (minIndex + maxIndex) / 2;
                if (VisitorEntryList[mid_index].VisitorId == VisitorId)
                {
                    return VisitorEntryList[mid_index];
                }
                else if (VisitorEntryList[mid_index].VisitorId > VisitorId)
                {
                    minIndex = mid_index + 1;
                }
                else if (VisitorEntryList[mid_index].VisitorId < VisitorId)
                {
                    maxIndex = mid_index - 1;
                }
            }
            return null;
        }

        // uses binary search to return a list of visitor based on visitors id
        // similer to method getDailyReport
        public List<VisitorsEntry> getManyEntryById(int VisitorId)
        {
            
            VisitorEntryList = entryMergeSort(VisitorEntryList, "ID");
            List<VisitorsEntry> searchList = new List<VisitorsEntry>(VisitorEntryList);
            List<VisitorsEntry> foundList = new List<VisitorsEntry>();

            int minIndex = 0;
            int maxIndex = searchList.Count-1;
            int mid_index = (minIndex + maxIndex) / 2;
            if (maxIndex == 0)
            {
                if (searchList[0].VisitorId == VisitorId) {
                    return searchList;
                }
            }

            while (minIndex <= maxIndex)
            {
                mid_index = (minIndex + maxIndex) / 2;

                int ValidVisitorId = searchList[mid_index].VisitorId; 
                

                if (ValidVisitorId == VisitorId)
                {

                    foundList.Add(searchList[mid_index]);
                    searchList.RemoveAt(mid_index);
                    maxIndex -= 1;
                }
                else if (ValidVisitorId < VisitorId)
                {
                    minIndex = mid_index + 1;
                }
                else if (ValidVisitorId > VisitorId)
                {
                    maxIndex = mid_index - 1;
                }
            }
            return foundList;
        }


        // uses bunary search to retrive visitor entry using entry time
        // used to updating visitors exit time
        public VisitorsEntry getEntryByDate(TimeSpan entryTime)
        {

            VisitorEntryList = entryMergeSort(VisitorEntryList, "TIME");

            int minIndex = 0;
            int maxIndex = VisitorEntryList.Count;
            int mid_index = (minIndex + maxIndex) / 2;
            if (maxIndex == 1)
            {
                return null;
            }

            while (minIndex <= maxIndex)
            {
                mid_index = (minIndex + maxIndex) / 2;

                if (VisitorEntryList[mid_index].EntryTime.CompareTo(entryTime) == 0)
                {
                    return VisitorEntryList[mid_index];
                }
                else if (VisitorEntryList[mid_index].EntryTime.CompareTo(entryTime) == -1)
                {
                    minIndex = mid_index + 1;
                }
                else if (VisitorEntryList[mid_index].EntryTime.CompareTo(entryTime) == 1)
                {
                    maxIndex = mid_index - 1;
                }
            }
            return null;
        }

        // sorting algorithm that uses merge sort based on the condition  for sort provided
        public List<VisitorsEntry> entryMergeSort(List<VisitorsEntry> visitorEntryList, String condition)
        {
            int finalIndex = visitorEntryList.Count;
            int middleIndex = finalIndex / 2;

            if (finalIndex <= 1)
            {
                return visitorEntryList;
            }

            List<VisitorsEntry> firstList = new List<VisitorsEntry>();
            List<VisitorsEntry> secondList = new List<VisitorsEntry>();

            for (int i = 0; i < middleIndex; i++)
            {
                firstList.Add(visitorEntryList[i]);
            }
            for (int i = middleIndex; i < finalIndex; i++)
            {
                secondList.Add(visitorEntryList[i]);
            }

            entryMergeSort(firstList, condition);
            entryMergeSort(secondList, condition);

            entryMergeArray(firstList, secondList, visitorEntryList, condition);
            return visitorEntryList;
        }

        public void entryMergeArray(List<VisitorsEntry> firstList, List<VisitorsEntry> secondList, List<VisitorsEntry> visitorEntryList, string condition)
        {
            int first_index = 0;
            int second_index = 0;
            int finalIndex = 0;
            
            while (first_index < firstList.Count && second_index < secondList.Count)
            {
                Boolean conditionId = firstList[first_index].VisitorId < secondList[second_index].VisitorId;
                Boolean conditionDate = (firstList[first_index].EntryDate.CompareTo(secondList[second_index].EntryDate) <= 0);
                Boolean conditionTime = (firstList[first_index].EntryTime.CompareTo(secondList[second_index].EntryTime) <= 0) && (firstList[first_index].EntryDate.CompareTo(secondList[second_index].EntryDate) <= 0);
                
                // sorts the list based on the condition : id, date and time
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
                    visitorEntryList.RemoveAt(finalIndex);
                    visitorEntryList.Insert(finalIndex, firstList[first_index]);
                    first_index += 1;
                }
                else
                {
                    visitorEntryList.RemoveAt(finalIndex);
                    visitorEntryList.Insert(finalIndex, secondList[second_index]);
                    second_index += 1;
                }
                finalIndex += 1;
            }
            while (firstList.Count > first_index)
            {
                visitorEntryList.RemoveAt(finalIndex);
                visitorEntryList.Insert(finalIndex, firstList[first_index]);
                first_index += 1;
                finalIndex += 1;
            }
            while (secondList.Count > second_index)
            {
                visitorEntryList.RemoveAt(finalIndex);
                visitorEntryList.Insert(finalIndex, secondList[second_index]);
                finalIndex += 1;
                second_index += 1;
            }
        }

        // reads visitors entry from csv file 
        // for initializing the entry while loading the file
        public void readEntryCSV(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
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

                        if (visitorController.getVisitor(visitorEntry.VisitorId) != null)
                        {
                            this.VisitorEntryList.Add(visitorEntry);
                        }
                    }
                }

            }
            catch (IOException)
            {
                Console.WriteLine("File not found : " + filePath);
            }

        }
        
        // initailizing the column headers of the entry file
        public void initiateEntryData(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", "Visitor Id", "Day" ,"Entry Date", "Entry Time", "Exit Time", "Duration");
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        // writing new entries to the entry csv file
        public void writeEntryData(VisitorsEntry visitorEntry, string filePath)
        {
            string desktop_path = filePath;
            using (var writer = new StreamWriter(desktop_path, append: true))
            {

                var line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", visitorEntry.VisitorId,  visitorEntry.EntryDate, visitorEntry.Day, visitorEntry.EntryTime, visitorEntry.ExitTime, visitorEntry.Duration );

                writer.WriteLine(line);
                writer.Flush();
            }
        }


    }
}
