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

        
        DateTime CurrentDate { get; set; }
        string visitor_entry_file = "C:\\Users\\mrPra\\Desktop\\visitor_entry(" + DateTime.UtcNow.Date.Month.ToString()+"-"+DateTime.UtcNow.Date.Day.ToString() + ").csv";
        string visitor_file = "C:\\Users\\mrPra\\Desktop\\visitors_file.csv";

        public Form1()
        {
            InitializeComponent();
            home_panel.Visible = true;
            insert_panel.Visible = false;

            VisitorList = new List<Visitor>();
            VisitorEntryList = new List<VisitorsEntry>();
            visitorController = new VisitorController(VisitorList);
            entryController = new EntryController(visitorController, VisitorEntryList);
            DailyReportList = new List<VisitorsEntry>();


            CurrentDate = DateTime.UtcNow.Date;
            HomeDateLabel.Text = CurrentDate.ToString("dd/MM/yyyy");
            entry_error_label.Visible = false;

            load_file();
        }

        public void load_file() {
            //if(!file.exists){
            // file.create(path)
            //}
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
                    Console.WriteLine("Here .. .. .. ");
                    Console.WriteLine(VisitorEntryList.Count);

                    foreach (VisitorsEntry visitorEntry in VisitorEntryList)
                    {
                        this.add_visitor_entry(visitorEntry);
                        
                    }
                }

            }
        }

        public void togglePanel(int panel_id) {
            switch (panel_id)
            {
                case 1:
                    if (!home_panel.Visible) {
                        home_panel.Visible = true;
                        insert_panel.Visible = false;
                        report_panel.Visible = false;
                    }
                    break;

                case 2:
                    if (!insert_panel.Visible) {
                        insert_panel.Visible = true;
                        home_panel.Visible = false;
                        report_panel.Visible = false;
                    }
                    break;

                case 3:
                    if (!report_panel.Visible)
                    {
                        report_panel.Visible = true;
                        insert_panel.Visible = false;
                        home_panel.Visible = false;
                    }
                    break;

                default:
                    Console.WriteLine("Windows not good");
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

        private void GenerateReportBtn_Click(object sender, EventArgs e)
        {
            this.togglePanel(3);
        }

        public Visitor createVisitor() {
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
            else {
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
            if (!File.Exists(visitor_file)) {
                visitorController.initiate_visitors_data(visitor_file);
            }
            if (visitor != null)
            {
                this.insertToTable(visitor);
                visitorController.write_visitors_data(visitor, visitor_file);
                this.clear_fields();
            }
        }

        public void insertToTable(Visitor visitor) {
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
                    this.visitor_entry_table.Rows.Add(entry.VisitorId, visitor.FirstName + " " + visitor.LastName, entry.EntryTime, entry.ExitTime);
                }
                else
                {
                    this.visitor_entry_table.Rows.Add(entry.VisitorId, visitor.FirstName + " " + visitor.LastName, entry.EntryTime, "  --:--:--  ");
                }
            }
        }

        public Boolean check_entry_input()
        {
            Boolean is_valid = true;

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
                visitorController.read_visitor_csv(file_path);
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
                VisitorsEntry entry_record = new VisitorsEntry(visitor_id, CurrentDate, entry_time);

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
            }
        }

        public void refresh_visitor_entry_table() {
            visitor_entry_table.Rows.Clear();
            visitor_entry_table.Refresh();
            
            foreach (VisitorsEntry entry in VisitorEntryList) {
                this.add_visitor_entry(entry);
            }
        }

        public void update_visitor_entry_csv()
        {
            if (File.Exists(visitor_entry_file)) {
                File.Delete(@visitor_entry_file);
            }
            entryController.initiate_entry_data(visitor_entry_file);
            foreach (VisitorsEntry visitorEntry in VisitorEntryList) {
                entryController.write_entry_data(visitorEntry, visitor_entry_file);
            }
        }

        private void visitor_entry_table_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            TimeSpan givenEntryTime = TimeSpan.Parse(visitor_entry_table.Rows[e.RowIndex].Cells[2].Value.ToString());
            if (VisitorEntryList.Count == 1)
            {
                VisitorsEntry entry = VisitorEntryList[0];
                entry.ExitTime = DateTime.Now.TimeOfDay;
                VisitorEntryList.Remove(entry);
                VisitorEntryList.Add(entry);
                update_visitor_entry_csv();
                refresh_visitor_entry_table();
            }
            else if (e.RowIndex > -1 && visitor_entry_table.Rows[e.RowIndex].Cells[0] != null)
            {
                int visitor_id = Int32.Parse(visitor_entry_table.Rows[e.RowIndex].Cells[0].Value.ToString());
                VisitorsEntry visitorEntry = entryController.get_entry(givenEntryTime);
                
                if (visitorEntry != null )
                {
                    visitorEntry.ExitTime = DateTime.Now.TimeOfDay;
                    VisitorEntryList.Remove(visitorEntry);
                    VisitorEntryList.Add(visitorEntry);
                    update_visitor_entry_csv();
                }
                refresh_visitor_entry_table();
            }
        }

        private void reportDatePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = reportDatePicker.Value.Date;
            Console.WriteLine("Month" + selectedDate.Month);
            Console.WriteLine("Day" + selectedDate.Day);

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

    }
}
