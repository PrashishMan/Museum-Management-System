# Museum-Management-System
Contains three main files: Form1.cs, class1.cs, and class2.cs
Form1.cs : 
*Methods:
	* load_file():
	Checks if the file exists for a filepath and loads the content 
from the 
	file to the arraylist	
	toggle_panel():
	Switches between different panels: homePanel, insertPanel reportpPanel 
	in response to atheir respective button click
	CreateVisitor():
	Receives input from the form, validates input and creates an object of 
	visitor.
	insert_visitor_btn_click():
	Responds to the button click for creating and inserting new visitor to 
	csv file and a list.
	insertToTable(Visitor visitor):
	Fills the datagrid view from the provided visitor
	add_visitor_entry(VisitorsEntry visitorentry):
	creates an obnject of visitors entry and inserts it to the csv file and 
	an a list
	check_entry_input():
	Checks whether the visitorid is empty or doesnot exist when creating 
	visitors entry;
	upload_csv_click():
	Responds to button click for uploading file from csv
	entry_btn_click():
	responds to button click and add new entry to visitors entry list and 
	csv
	refresh_visitor_entry_table():
	Refreshes the entry table after updating its data such as adding exit 
	time	
	update_visitor_entry_csv():
	updates the changes in csv file after data such as adding exit time is 
	added
	visitor_entry_table_CellContentClick_1():
	trigers an event to update the exit time of the visitor
	clear_fields():
	clear the input field value after creating its object


class2.cs
Classes:
VisitorController:
	mergesort():
	breaks visitors list array to two array on each iteration of recurssion 
	until each list contains only one visitors object.
  
	merge_array():
	compares the visitors id value and sorts by inserting the value with 
	lowest value in the list first
	 
		
	
Hold actions events for button clicks from UI. Contains source code for 
	displaying and updating content in a DataGridView Contains Code for 
	checking and validating user input



	
