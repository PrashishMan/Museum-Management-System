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

            load_file();
            
            this.populateWeeklyReport();
        }

        private void getDailyReport(DateTime dateTime)
        {
            DailyReportTable.Rows.Clear();
            //DailyReportTable.Refresh();

            ReportController = new ReportController(VisitorEntryList);
            List<VisitorsEntry> DailyReportList = ReportController.getDailyReport(dateTime);
            
            Dictionary<int, Double> visitorsEntryDict = new Dictionary<int, Double>();
            foreach (VisitorsEntry visitorsEntry in DailyReportList) {
                if (visitorsEntryDict.ContainsKey(visitorsEntry.VisitorId)) {
                    visitorsEntryDict[visitorsEntry.VisitorId] += visitorsEntry.Duration;
                }
                else
                {
                    visitorsEntryDict[visitorsEntry.VisitorId] = visitorsEntry.Duration;
                }
            }

            this.add_visitor_report(visitorsEntryDict);
            totalVisitors.Text = visitorsEntryDict.Count.ToString();
        }

        public String formatDate(TimeSpan time)
        {
            return time.Hours + " : " + time.Minutes + " : " + time.Seconds;
        }

        public void add_visitor_report(Dictionary<int, Double> entryDict)
        {
            foreach (KeyValuePair<int, Double> entry in entryDict) {
                Visitor visitor = visitorController.get_visitor(entry.Key);
                if (visitor != null)
                {
                    this.DailyReportTable.Rows.Add(entry.Key, visitor.FirstName + " " + visitor.LastName, Convert.ToDecimal(string.Format("{0:F3}", entry.Value)).ToString() + " min");
                }
            }
            
        }


        public void load_file()
        {
            // file.create(path)
            if (File.Exists(visitor_file))
            {
                visitorController.read_visitor_csv(visitor_file);
                

                foreach (Visitor visitor in VisitorList)
                {
                    insertToTable(visitor);
                }

                if (File.Exists(visitor_entry_file))
                {
                    entryController.read_entry_csv(visitor_entry_file);

                    foreach (VisitorsEntry visitorEntry in VisitorEntryList)
                    {
                        this.add_visitor_entry(visitorEntry);

                    }
                }

            }
        }

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
                    }
                    break;

                case 2:
                    if (!insert_panel.Visible)
                    {
                        insert_panel.Visible = true;
                        home_panel.Visible = false;
                        dailyReportPanel.Visible = false;
                        weeklyReportPanel.Visible = false;
                    }
                    break;

                case 3:
                    if (!dailyReportPanel.Visible)
                    {
                        dailyReportPanel.Visible = true;
                        insert_panel.Visible = false;
                        home_panel.Visible = false;
                        weeklyReportPanel.Visible = false;
                    }
                    break;

                case 4:
                    if (!weeklyReportPanel.Visible)
                    {

                        weeklyReportPanel.Visible = true;
                        dailyReportPanel.Visible = false;
                        insert_panel.Visible = false;
                        home_panel.Visible = false;
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

        private void InsertVisitorBtn_Click(object sender, EventArgs e)
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
            return null;
        }

        private void insert_visitor_btn_Click(object sender, EventArgs e)
        {
            Visitor visitor = this.createVisitor();
            visitorController.insert_visitor(visitor);
            if (!File.Exists(visitor_file))
            {
                visitorController.initiate_visitors_data(visitor_file);
            }
            if (visitor != null)
            {
                this.insertToTable(visitor);
                visitorController.write_visitors_data(visitor, visitor_file);
                this.clear_fields();
            }
        }

        public void insertToTable(Visitor visitor)
        {
            this.visitors_table.Rows.Add(visitor.VisitorId, visitor.FirstName,
                visitor.Contact, visitor.Occupancy, visitor.Gender, visitor.Email);
        }

        public void add_visitor_entry(VisitorsEntry entry)
        {
            Visitor visitor = visitorController.get_visitor(entry.VisitorId);
            if (visitor != null)
            {
                if (entry.ExitTime.ToString() != "00:00:00")
                {
                    this.visitor_entry_table.Rows.Add(entry.VisitorId, entry.Day,
                        visitor.FirstName + " " + visitor.LastName, formatDate(entry.EntryTime), formatDate(entry.ExitTime),
                        Convert.ToDecimal(string.Format("{0:F3}", entry.Duration)).ToString() + " min");
                }
                else
                {
                    this.visitor_entry_table.Rows.Add(entry.VisitorId, entry.Day, visitor.FirstName + " " + visitor.LastName, entry.EntryTime, "  --:--:--  ", "  --:--:--  ");
                }
            }
        }

        public Boolean check_entry_input()
        {

            Boolean is_valid = true;
            entry_error_label.Visible = false;


            List<VisitorsEntry> selectedEntryList = entryController.getManyEntryById(Int32.Parse(visitor_id_field.Text));
            

            if (string.IsNullOrEmpty(visitor_id_field.Text))
            {
                entry_error_label.Visible = true;
                entry_error_label.Text = "Error: Must enter visitor id field!! ";
                is_valid = false;
            }
            else if (visitorController.get_visitor(Int32.Parse(visitor_id_field.Text)) == null)
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

        private void upload_csv_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            DialogResult result = open_file_dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file_path = open_file_dialog.FileName;
                file_name_label.Text = file_path;
                List<Visitor> uploadedVisitorsList = visitorController.read_visitor_csv(file_path);

                foreach (Visitor v in uploadedVisitorsList) {
                    visitorController.write_visitors_data(v, visitor_file);
                }

                VisitorList = visitorController.merge_sort(VisitorList);
                visitors_table.Rows.Clear();
                visitors_table.Refresh();

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

                VisitorsEntry entry_record = new VisitorsEntry(visitor_id, day, CurrentDate, entry_time);
                if (!File.Exists(visitor_entry_file))
                {
                    entryController.initiate_entry_data(visitor_entry_file);
                }

                //Appends item to the list
                VisitorEntryList.Add(entry_record);

                //adds to the csv file 
                //does not append updates the entire csv file
                entryController.write_entry_data(entry_record, visitor_entry_file);

                //Adds row to the table
                this.add_visitor_entry(entry_record);
                this.populateWeeklyReport();
            }
        }

        public void refresh_visitor_entry_table()
        {
            visitor_entry_table.Rows.Clear();
            visitor_entry_table.Refresh();

            foreach (VisitorsEntry entry in VisitorEntryList)
            {
                this.add_visitor_entry(entry);
            }
        }

        public void update_visitor_entry_csv()
        {
            if (File.Exists(visitor_entry_file))
            {
                File.Delete(@visitor_entry_file);
            }
            entryController.initiate_entry_data(visitor_entry_file);
            foreach (VisitorsEntry visitorEntry in VisitorEntryList)
            {
                entryController.write_entry_data(visitorEntry, visitor_entry_file);
            }
        }

        private void visitor_entry_table_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            TimeSpan givenEntryTime = TimeSpan.Parse(visitor_entry_table.Rows[e.RowIndex].Cells[3].Value.ToString());
            if (VisitorEntryList.Count == 1)
            {
                VisitorsEntry entry = VisitorEntryList[0];
                entry.ExitTime = DateTime.Now.TimeOfDay;

                entry.Duration = entry.ExitTime.Subtract(entry.EntryTime).TotalMinutes;


                VisitorEntryList.Remove(entry);
                VisitorEntryList.Add(entry);
                update_visitor_entry_csv();
                refresh_visitor_entry_table();
            }
            else if (e.RowIndex > -1 && visitor_entry_table.Rows[e.RowIndex].Cells[0] != null)
            {
                VisitorsEntry visitorEntry = entryController.getEntryByDate(givenEntryTime);

                if (visitorEntry != null)
                {
                    visitorEntry.ExitTime = DateTime.Now.TimeOfDay;

                    visitorEntry.Duration = visitorEntry.ExitTime.Subtract(visitorEntry.EntryTime).TotalMinutes;

                    VisitorEntryList.Remove(visitorEntry);
                    VisitorEntryList.Add(visitorEntry);
                    update_visitor_entry_csv();
                }
                refresh_visitor_entry_table();
            }
        }
        

        private void dailyReportchechbtn_Click(object sender, EventArgs e)
        {

            DateTime selectedDate = reportDatePicker.Value.Date;
            this.getDailyReport(selectedDate);

        }


        public void clear_fields()
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

            String monthInput = monthComboBox.Text;
            int month = DateTime.Now.Month;

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

        public void populateWeeklyReport() {
            Dictionary<string, int> visitorWeeklyCount = new Dictionary<string, int>();
            foreach (VisitorsEntry ve in VisitorEntryList)
            {
                if (ve.EntryDate.DayOfWeek.ToString() != "Saturday" && ve.EntryDate.DayOfWeek.ToString() != "Sunday") {
                    if (visitorWeeklyCount.ContainsKey(ve.EntryDate.DayOfWeek.ToString()))
                    {
                        visitorWeeklyCount[ve.EntryDate.DayOfWeek.ToString()] += 1;
                    }
                    else
                    {
                        visitorWeeklyCount[ve.EntryDate.DayOfWeek.ToString()] = 1;
                    }
                }
                
            }

            weeklyVisitorsView.Rows.Clear();
            weeklyVisitorsView.Refresh();

            KeyValuePair<String, int>[] arr = new KeyValuePair<String, int>[5];

            int arrCount = 0;
            foreach (KeyValuePair<String, int> ve in visitorWeeklyCount)
            {
                arr[arrCount] = ve;
                arrCount++;
            }
            arr = this.bubbleSort(arr);
            for (int i = 0; i < arr.Length; i++) {
                Console.WriteLine(arr[i]);
                this.weeklyVisitorsView.Rows.Add(arr[i].Key, arr[i].Value);
            }
        }

        public Dictionary<String, int> getVisitorsEntries(int visitorId) {
            String[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            Dictionary<String, int> visitsE = new Dictionary<string, int>();
            for (int i = 0; i < 5; i++) {
                visitsE[weekDays[i]] = 0;
            }
            Visitor visitor = visitorController.get_visitor(visitorId);
            if (visitor != null) {
                List<VisitorsEntry> entries = entryController.getManyEntryById(visitorId);
                foreach (VisitorsEntry ve in entries)
                {
                    if (visitsE.ContainsKey(ve.EntryDate.DayOfWeek.ToString()))
                    {
                        visitsE[ve.EntryDate.DayOfWeek.ToString()] += 1;
                    }
                    else {
                        visitsE[ve.EntryDate.DayOfWeek.ToString()] = 1;
                    }
                }
            }
            return visitsE;
            
        }

        public KeyValuePair<String, int>[] bubbleSort(KeyValuePair<String, int>[] arr) {

            for (int j = 1; j < arr.Length / 2; j++) {
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

        private void checkVisitorInput_Click(object sender, EventArgs e)
        {
            customerWeeklyChart.Series["Visits"].Points.Clear();
            Dictionary<String, int> visits = this.getVisitorsEntries(Int32.Parse(weeklyVisitorId.Text));
            foreach (KeyValuePair<String, int> ve in visits) {
                customerWeeklyChart.Series["Visits"].Points.AddXY(ve.Key, ve.Value);
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}
