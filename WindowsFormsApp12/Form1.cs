using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RequiredObjects;
using ControllerClass;

namespace WindowsFormsApp12
{
    public partial class Form1 : Form
    {
        List<Visitor> VisitorList { get; set; }
        List<VisitorsEntry> VisitorEntryList { get; set; }
        List<VisitorsEntry> DailyReportList { get; set; }
        EntryController entryController;
        VisitorController visitorController;

        ReportController ReportController;
        

        DateTime CurrentDate { get; set; }
        string visitor_entry_file = "visitor_entry.csv";
        string visitor_file = "visitors_file.csv";

        public Form1()
        {

            InitializeComponent();
            home_panel.Visible = true;
            insert_panel.Visible = false;
            weeklyReportPanel.Visible = false;
            dailyReportPanel.Visible = false;

            VisitorList = new List<Visitor>();

            VisitorEntryList = new List<VisitorsEntry>();

            visitorController = new VisitorController(VisitorList);

            entryController = new EntryController(visitorController, VisitorEntryList);

            DailyReportList = new List<VisitorsEntry>();


            CurrentDate = DateTime.UtcNow.Date;
            HomeDateLabel.Text = CurrentDate.ToString("dd/MM/yyyy");

            entry_error_label.Visible = false;
            visitor_id_error.Visible = false;

            loadFile();

            content_title_label.Text = "Entry Panel";
        }

        private void getDailyReport(DateTime dateTime)
        {
            //Clearing any previous data from the table
            DailyReportTable.Rows.Clear();
            ReportController = new ReportController(VisitorEntryList);
            //Retriving the key value pair with visitor id as key and visitor time duration as value
            List<VisitorsEntry> DailyReportList = ReportController.getDailyReport(dateTime);
            
            Dictionary<int, Double> visitorsEntryDict = new Dictionary<int, Double>();
            foreach (VisitorsEntry visitorsEntry in DailyReportList) {
                //Checking if the visitors entry date matches the user selected date
                // Since one visitor has enter multiple times, dictionary is used to eliminate
                // redundant data and update days value
                if (visitorsEntry.EntryDate.CompareTo(dateTime) == 0) {
                    if (visitorsEntryDict.ContainsKey(visitorsEntry.VisitorId))
                    {
                        // incrementing the visitor count of a day
                        visitorsEntryDict[visitorsEntry.VisitorId] += visitorsEntry.Duration;
                    }
                    else
                    {
                        // initializing the day key with first entry duration
                        visitorsEntryDict[visitorsEntry.VisitorId] = visitorsEntry.Duration;
                    }
                }
                
            }


            // Displaying to the data grid view
            this.addVisitorReport(visitorsEntryDict);
            totalVisitors.Text = visitorsEntryDict.Count.ToString();
        }

        // this method formats time in HH:MM:SS format for display in datagrid view
        public String formatDate(TimeSpan time)
        {
            return time.Hours + " : " + time.Minutes + " : " + time.Seconds;
        }

        // add visitors daily report 
        public void addVisitorReport(Dictionary<int, Double> entryDict)
        {

            foreach (KeyValuePair<int, Double> entry in entryDict) {
                // retriving visitor associated with the visitor key
                Visitor visitor = visitorController.getVisitor(entry.Key);
                if (visitor != null)
                {
                    this.DailyReportTable.Rows.Add(entry.Key, visitor.FirstName + " " + visitor.LastName, Convert.ToDecimal(string.Format("{0:F3}", entry.Value)).ToString() + " min");
                }
            }
            
        }


        public void loadFile()
        {

            if (File.Exists(visitor_file))
            {
                // read and insert visitor data to the list 
                visitorController.readVisitorCSV(visitor_file);
                
                // inserting loaded visitor data into the csv file
                foreach (Visitor visitor in VisitorList)
                {
                    insertToTable(visitor);
                }

                // reading and inserting entries from the entry csv file
                if (File.Exists(visitor_entry_file))
                {
                    entryController.readEntryCSV(visitor_entry_file);

                    // loading the file from the list to the data grid view
                    foreach (VisitorsEntry visitorEntry in VisitorEntryList)
                    {
                        this.addVisitorEntry(visitorEntry);

                    }
                }

            }
        }

        // function toggles between the views 
        public void togglePanel(int panel_id)
        {
            switch (panel_id)
            {
                case 1:
                    if (!home_panel.Visible)
                    {
                        home_panel.Visible = true;
                        insert_panel.Visible = false;
                        dailyReportPanel.Visible = false;
                        weeklyReportPanel.Visible = false;
                        content_title_label.Text = "Entry Panel";
                    }
                    break;

                case 2:
                    if (!insert_panel.Visible)
                    {
                        insert_panel.Visible = true;
                        home_panel.Visible = false;
                        dailyReportPanel.Visible = false;
                        weeklyReportPanel.Visible = false;
                        content_title_label.Text = "Visitors Panel";
                    }
                    break;

                case 3:
                    if (!dailyReportPanel.Visible)
                    {
                        dailyReportPanel.Visible = true;
                        insert_panel.Visible = false;
                        home_panel.Visible = false;
                        weeklyReportPanel.Visible = false;
                        content_title_label.Text = "Daily Report Panel";
                    }
                    break;

                case 4:
                    if (!weeklyReportPanel.Visible)
                    {

                        weeklyReportPanel.Visible = true;
                        dailyReportPanel.Visible = false;
                        insert_panel.Visible = false;
                        home_panel.Visible = false;
                        content_title_label.Text = "Weekly Report Panel";
                    }
                    break;

                default:
                    break;
            }
        }


        private void HomeBtn_Click(object sender, EventArgs e)
        {
            this.togglePanel(1);
        }

        private void insert_visitor_btn_Click(object sender, EventArgs e)
        {
            this.togglePanel(2);
        }

        private void dailyReportBtn_Click(object sender, EventArgs e)
        {
            this.togglePanel(3);
        }

        private void weeklyReportBtn_Click(object sender, EventArgs e)
        {
            this.togglePanel(4);
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // checks form input and create a new visitor object
        public Visitor createVisitor()
        {
            string first_name = insert_first_name_field.Text;
            string last_name = insert_last_name_field.Text;
            string contact = insert_contact_field.Text;
            string occupancy = insert_occupancy_combo.Text;
            string country = insert_country_field.Text;

            string email = insert_email_field.Text;
            string gender = "";

            if (error_message.Visible)
            {
                error_message.Visible = false;
            }

            if (insert_female_radio.Checked)
            {
                gender = "Female";
            }
            else if (insert_male_radio.Checked)
            {
                gender = "Male";
            }
            else
            {
                error_message.Visible = true;
                error_message.Text = "Please select gender";
            }

            if (string.IsNullOrEmpty(first_name))
            {
                error_message.Visible = true;
                error_message.Text = "Please enter your first name";
            }
            else if (string.IsNullOrEmpty(last_name))
            {
                error_message.Visible = true;
                error_message.Text = "Please enter your last name";
            }
            else if (string.IsNullOrEmpty(occupancy))
            {
                error_message.Visible = true;
                error_message.Text = "Please select your occupancy";
            }
            else if (string.IsNullOrEmpty(country))
            {
                error_message.Visible = true;
                error_message.Text = "Please select your country";
            }
            else if (string.IsNullOrEmpty(email))
            {
                error_message.Visible = true;
                error_message.Text = "Please emter your email address";
            }
            else
            {
                Visitor visitor = new Visitor(first_name, last_name, contact, occupancy,
                gender, country, email);
                return visitor;
            }
            // if the error has occured
            return null;
        }

        private void addVisitorBtn_Click(object sender, EventArgs e)
        {
            // creating and refencing a new visitor object
            Visitor visitor = this.createVisitor();
            if (visitor != null)
            {
                // inserting visitor to the data grid view
                visitorController.insertVisitor(visitor);
                if (!File.Exists(visitor_file))
                {
                    // if a new file is created initialize its columns header "visitorid, firstname, lastname and so on"
                    visitorController.initiateVisitorData(visitor_file);
                }

                if (visitor != null)
                {
                    // populate data grid view
                    this.insertToTable(visitor);
                    // write into csv file
                    visitorController.writeVisitorsData(visitor, visitor_file);

                    // clears input fields in the form
                    this.clearFields();
                }
            }
        }
        
        // populates the data grid view
        public void insertToTable(Visitor visitor)
        {
            this.visitors_table.Rows.Add(visitor.VisitorId, visitor.FirstName,
                visitor.Contact, visitor.Occupancy, visitor.Gender, visitor.Email);
        }

       
        public void addVisitorEntry(VisitorsEntry entry)
        {
            // refrences to the visitor from the visitors list
            Visitor visitor = visitorController.getVisitor(entry.VisitorId);
            if (visitor != null)
            {
                // check if the data is of entry 
                if (entry.ExitTime.ToString() != "00:00:00")
                {
                    this.visitor_entry_table.Rows.Add(entry.VisitorId, entry.Day,
                        visitor.FirstName + " " + visitor.LastName, formatDate(entry.EntryTime), formatDate(entry.ExitTime),
                        Convert.ToDecimal(string.Format("{0:F3}", entry.Duration)).ToString() + " min");
                }
                // if the data is of exit
                else
                {
                    this.visitor_entry_table.Rows.Add(entry.VisitorId, entry.Day, visitor.FirstName + " " + visitor.LastName, entry.EntryTime, "  --:--:--  ", "  --:--:--  ");
                }
            }
        }
        // checks visitors entry input
        public Boolean check_entry_input()
        {

            Boolean is_valid = true;
            entry_error_label.Visible = false;
            // if the user has not provided any visitor id
            if (visitor_id_field.Text == "")
            {
                entry_error_label.Visible = true;
                entry_error_label.Text = "Error: Visitor id field is empty!! ";
                is_valid = false;
                return false;
            }
            // stores all the entries related to visitor id
            List<VisitorsEntry> selectedEntryList = new List<VisitorsEntry>();
            try
            {
                selectedEntryList = entryController.getManyEntryById(Int32.Parse(visitor_id_field.Text));
            }
            catch (FormatException) {
                entry_error_label.Visible = true;
                entry_error_label.Text = "Error: Invalid Visitor id field !! ";
                is_valid = false;
                return false;
            }

            
            if (string.IsNullOrEmpty(visitor_id_field.Text))
            {
                entry_error_label.Visible = true;
                entry_error_label.Text = "Error: Must enter visitor id field!! ";
                is_valid = false;
            }
            else if (visitorController.getVisitor(Int32.Parse(visitor_id_field.Text)) == null)
            {
                entry_error_label.Visible = true;

                entry_error_label.Text = "Error: Invalid visitor id";
                is_valid = false;
            }
            else if (selectedEntryList.Count > 0)
            {
                foreach (VisitorsEntry visitorsEntry in selectedEntryList)
                {
                    if (visitorsEntry.ExitTime == TimeSpan.Parse("00:00:00"))
                    {
                        entry_error_label.Visible = true;
                        entry_error_label.Text = "Error: Visitor Has not checked out";
                        is_valid = false;
                    }
                }

            }
            return is_valid;
        }

        // upload csv of visitors information
        private void upload_csv_Click(object sender, EventArgs e)
        {
            // pops a dialog box for user to navigate through directories
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            DialogResult result = open_file_dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file_path = open_file_dialog.FileName;
                file_name_label.Text = file_path;
                // reads from the csv file, populates and returns the list of uploaded visitors
                List<Visitor> uploadedVisitorsList = visitorController.readVisitorCSV(file_path);

                // writing data into currently referenced csv file
                foreach (Visitor v in uploadedVisitorsList) {
                    visitorController.writeVisitorsData(v, visitor_file);
                }

                // using merge sort to sort by visitor id
                VisitorList = visitorController.mergeSort(VisitorList);
                visitors_table.Rows.Clear();
                visitors_table.Refresh();

                // inserting into table
                foreach (Visitor visitor in VisitorList)
                {
                    this.insertToTable(visitor);
                }
            }
        }

        private void entry_btn_Click(object sender, EventArgs e)
        {
            entry_error_label.Visible = true;
            if (this.check_entry_input())
            {
                int visitor_id = Int32.Parse(visitor_id_field.Text);
                TimeSpan entry_time = DateTime.Now.TimeOfDay;
                String day = DateTime.Now.DayOfWeek.ToString();
                if (entry_time.CompareTo(new TimeSpan(10, 00, 00)) != -1 && entry_time.CompareTo(new TimeSpan(17, 00, 00)) == -1
                    && day != "Sunday" && day != "Saturday") {
                    VisitorsEntry entry_record = new VisitorsEntry(visitor_id, day, CurrentDate, entry_time);
                    if (!File.Exists(visitor_entry_file))
                    {
                        // create table row head when creating new csv file
                        entryController.initiateEntryData(visitor_entry_file);
                    }

                    //Appends item to the list
                    VisitorEntryList.Add(entry_record);

                    //adds to the csv file 
                    //does not append updates the entire csv file
                    entryController.writeEntryData(entry_record, visitor_entry_file);

                    //Adds row to the table
                    this.addVisitorEntry(entry_record);
                }
            }
        }

        public void refreshVisitorEntryTable()
        {
            // clear previous entries
            visitor_entry_table.Rows.Clear();
            visitor_entry_table.Refresh();

            // and newly updated entries
            foreach (VisitorsEntry entry in VisitorEntryList)
            {
                this.addVisitorEntry(entry);
            }
        }


        public void updateVisitorEntryCsv()
        {
            // delete existing entry file
            if (File.Exists(visitor_entry_file))
            {
                File.Delete(@visitor_entry_file);
            }
            // create new entry file
            entryController.initiateEntryData(visitor_entry_file);
            // populate the entry file
            foreach (VisitorsEntry visitorEntry in VisitorEntryList)
            {
                entryController.writeEntryData(visitorEntry, visitor_entry_file);
            }
        }

        private void visitor_entry_table_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // get entry time of the selected row
            TimeSpan givenEntryTime = TimeSpan.Parse(visitor_entry_table.Rows[e.RowIndex].Cells[3].Value.ToString());
            // if there is only one visitor in the list
            if (VisitorEntryList.Count == 1)
            {
                VisitorsEntry entry = VisitorEntryList[0];
                entry.ExitTime = DateTime.Now.TimeOfDay;

                entry.Duration = entry.ExitTime.Subtract(entry.EntryTime).TotalMinutes;

                // updating the visitor list
                VisitorEntryList.Remove(entry);
                VisitorEntryList.Add(entry);

                // updating the csv file
                updateVisitorEntryCsv();
                refreshVisitorEntryTable();
            }
            // if selected row is not empty or null
            else if (e.RowIndex > -1 && visitor_entry_table.Rows[e.RowIndex].Cells[0] != null)
            {
                // user binary search to retrive visitor by entry time
                VisitorsEntry visitorEntry = entryController.getEntryByDate(givenEntryTime);
                // update the list and csv file after visitor exits
                if (visitorEntry != null)
                {
                    visitorEntry.ExitTime = DateTime.Now.TimeOfDay;

                    visitorEntry.Duration = visitorEntry.ExitTime.Subtract(visitorEntry.EntryTime).TotalMinutes;

                    VisitorEntryList.Remove(visitorEntry);
                    VisitorEntryList.Add(visitorEntry);
                    updateVisitorEntryCsv();
                }
                refreshVisitorEntryTable();
            }
        }
        
        // generates the daily report from the date selected
        private void dailyReportchechbtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Daily Report clicked");
            DateTime selectedDate = reportDatePicker.Value.Date;
            // retrives all the reports from the date and populates the data grid view 
            this.getDailyReport(selectedDate);

        }

        // clearing the input fields of visitor input
        public void clearFields()
        {
            insert_first_name_field.Text = "";
            insert_last_name_field.Text = "";
            insert_contact_field.Text = "";
            insert_occupancy_combo.Text = "";
            insert_country_field.Text = "";
            insert_email_field.Text = "";
            insert_male_radio.Checked = false;
            insert_female_radio.Checked = false;
        }



        private void monthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            String[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            // get month input from the user
            String monthInput = monthComboBox.Text;
            int month = DateTime.Now.Month;

            // find the index of the month from upper defined months list
            int month_index = Array.FindIndex(months, m => m == monthInput);
            
            //Get Monthly Entries ...
            List<VisitorsEntry> monthlyEntries = new List<VisitorsEntry>();
            foreach (VisitorsEntry v in VisitorEntryList)
            {
                if (month_index == v.EntryDate.Month - 1)
                {
                    monthlyEntries.Add(v);
                };
            }
            
            Dictionary<String, int> dict = new Dictionary<String, int>();

            int year = DateTime.Now.Year;
            
            ///Store Date and number of visitors
            Dictionary<String, int> entryDict = new Dictionary<String, int>();

            //Get the selected month days
            int days = DateTime.DaysInMonth(DateTime.Now.Year, month_index + 1);
            

            String[] monthlyVisitorsCount = new String[36];

            DateTime selectedDateTime = new DateTime(year, month_index + 1, 1);
            String weeksStart = selectedDateTime.DayOfWeek.ToString();
            
            int weekIndex = Array.FindIndex(weekDays, wk => wk == weeksStart);

            //initializing the count
            for (int i = 0; i < 35; i++) {
                monthlyVisitorsCount[i] = "-";
            }

            int[] dailyVisitorCount = new int[days+ 1];
            //Get visitors count for each day

            for (int day = 0; day < days; day++)
            {
                DateTime currentDate = new DateTime(year, month_index + 1, day+1);
                int visitorsCount = 0;
                foreach (VisitorsEntry me in monthlyEntries)
                {
                    if (currentDate == me.EntryDate)
                    {
                        visitorsCount += 1;
                    }
                }
                dailyVisitorCount[day] = visitorsCount;
            }
            int weekDayCount = 0;
            if (monthInput == "December") {
                days = days - 1;
            }
            for (int ind = 0; ind < days; ind++) {
                // Adding the count after the start week of the month
                monthlyVisitorsCount[ind + weekIndex] = dailyVisitorCount[weekDayCount].ToString();
               weekDayCount++;
            }
            
            weeklyReportTable.Rows.Clear();
            weeklyReportTable.Refresh();
            int s_no = 0;
            String[] weekColumns = new String[7];
            
            for (int ij = 0; ij <= 35; ij++) {

                weekColumns[ij % 7] = monthlyVisitorsCount[ij];
                int tableMod = (ij + 1) % 7;
                if (tableMod == 0)
                {
                    this.weeklyReportTable.Rows.Add(s_no, weekColumns[0], weekColumns[1], weekColumns[2], weekColumns[3], weekColumns[4], weekColumns[5], weekColumns[6]);
                    s_no += 1;
                }
            }
        }
        
        public Dictionary<String, double> getVisitorsEntries(int visitorId) {
            String[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            // stores entries grouped by weekdays and gets sum of entries 
            Dictionary<String, double> visitsE = new Dictionary<string, double>();
            for (int i = 0; i < 5; i++) {
                visitsE[weekDays[i]] = 0;
            }
            Visitor visitor = visitorController.getVisitor(visitorId);
            if (visitor != null) {
                List<VisitorsEntry> entries = entryController.getManyEntryById(visitorId);
                // inserting key value pair with week days as key and entry count as value
                foreach (VisitorsEntry ve in entries)
                {
                    // if the key exists increment the visitor entry duration
                    if (visitsE.ContainsKey(ve.EntryDate.DayOfWeek.ToString()))
                    {
                        visitsE[ve.EntryDate.DayOfWeek.ToString()] += ve.Duration;
                    }
                    // else initialize the count to initial duration
                    else {
                        visitsE[ve.EntryDate.DayOfWeek.ToString()] = ve.Duration;
                    }
                }
            }
            return visitsE;
            
        }

        // bubble sort algorithm
        public KeyValuePair<String, int>[] bubbleSort(KeyValuePair<String, int>[] arr) {
            // loop n number of times for each value in a list
            for (int j = 1; j < arr.Length; j++) {
                // loop to check and swap the adjasent value
                for (int i = 0; i < arr.Length - j; i++)
                {
                    if (arr[i + 1].Value < arr[i].Value)
                    {
                        KeyValuePair<String, int> tempDict = arr[i];
                        arr[i] = arr[i + 1];
                        arr[i + 1] = tempDict;
                    }
                }
            }
            return arr;

        }

        // populates the chart with till date visitors visit count based on week days
        private void checkVisitorInput_Click(object sender, EventArgs e)
        {
            visitor_id_error.Visible = false;
            
            // clear previously inserted visits
            customerWeeklyChart.Series["Visits"].Points.Clear();
            if (weeklyVisitorId.Text != "") {
                try
                {
                    if (visitorController.getVisitor(Int32.Parse(weeklyVisitorId.Text)) != null)
                    {
                        // holds weekdays as keys and visits as values
                        Dictionary<String, double> visits = this.getVisitorsEntries(Int32.Parse(weeklyVisitorId.Text));
                        foreach (KeyValuePair<String, double> ve in visits)
                        {
                            customerWeeklyChart.Series["Visits"].Points.AddXY(ve.Key, ve.Value);
                        }
                    }
                    else {
                        visitor_id_error.Visible = true;
                        visitor_id_error.Text = "Invalid visitor id";
                    }
                }
                catch (FormatException) {
                    visitor_id_error.Visible = true;
                    visitor_id_error.Text = "Invalid visitor id";
                }
            }
            else
            {
                visitor_id_error.Visible = true;
                visitor_id_error.Text = "Invalid visitor id";
            }
        }



        private void weeklyReport_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        // generates the weekly based on the selected date
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime weeklyDate = weeklyReportDatePicker.Value.Date;
            // holds week start date and week end date of the selected date
            Dictionary<String, DateTime> dateDurationDict = this.getDateDuration(weeklyDate);

            // gets the report in between start and end date and populates the data grid and displays chart
            this.populateWeeklyReport(dateDurationDict["start_date"], dateDurationDict["end_date"]);
            
        }


        // initializes the dictionary to display days from monday to friday even when there is not entries
        public Dictionary<String, int> initializeWeeklyDict() {
            String[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            Dictionary<String, int> weeklyDict = new Dictionary<string, int>();
            for (int i = 0; i < 5; i++)
            {
                weeklyDict[weekDays[i]] = 0;
            }

            return weeklyDict;

        }

        public void populateWeeklyReport(DateTime startDate, DateTime endTime)
        {
            // get initialized dict with week days as key and 0 as value
            Dictionary<string, int> visitorWeeklyCount = initializeWeeklyDict();
            foreach (VisitorsEntry ve in VisitorEntryList)
            {
                if (ve.EntryDate > startDate && ve.EntryDate < endTime) {
                    if (ve.EntryDate.DayOfWeek.ToString() != "Saturday" && ve.EntryDate.DayOfWeek.ToString() != "Sunday")
                    {
                        if (visitorWeeklyCount.ContainsKey(ve.EntryDate.DayOfWeek.ToString()))
                        {
                            // increment the visits by 1 if key exits
                            visitorWeeklyCount[ve.EntryDate.DayOfWeek.ToString()] += 1;
                        }
                        else
                        {
                            // assign 1 to value if key does not exist
                            visitorWeeklyCount[ve.EntryDate.DayOfWeek.ToString()] = 1;
                        }
                    }
                }
            }
            fillWeeklyGridView(visitorWeeklyCount);
        }
        

        // displayes the chart and data in data grid view
        public void fillWeeklyGridView(Dictionary<String, int> visitorWeeklyCount) {
            //clears previous data from chart and grid view
            weeklyReportChart.Series["visitDays"].Points.Clear();

            weeklyVisitorsView.Rows.Clear();
            weeklyVisitorsView.Refresh();

            // stores collection of key value pair for sorting
            KeyValuePair<String, int>[] arr = new KeyValuePair<String, int>[5];

            int arrCount = 0;

            // for each week days and count populate the chart
            foreach (KeyValuePair<String, int> ve in visitorWeeklyCount)
            {
                weeklyReportChart.Series["visitDays"].Points.AddXY(ve.Key, ve.Value);
                arr[arrCount] = ve;
                arrCount++;
            }
            // sort the array by visitors count
            arr = this.bubbleSort(arr);
            for (int i = 0; i < arr.Length; i++)
            {
                // populate the rows using sorted array
                this.weeklyVisitorsView.Rows.Add(arr[i].Key, arr[i].Value);
            }
        }

        // returns dictionary with start and end date of the week
        public Dictionary<String, DateTime> getDateDuration(DateTime weekDate) {
            // get int value of day of the week
            DayOfWeek dayOfWeek = weekDate.DayOfWeek;
            // get the start date
            DateTime startDateOfWeek = weekDate.AddDays(-(int)dayOfWeek);
            // get the end date
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6);

            // create the key value pair for start date and end date
            Dictionary<String, DateTime> dateTimeDict = new Dictionary<String, DateTime>();
            dateTimeDict.Add("start_date", startDateOfWeek);
            dateTimeDict.Add("end_date", endDateOfWeek);
            return dateTimeDict;
        }

        
    }
}
