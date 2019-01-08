# Museum-Management-System
## Files:
 Contains three main files: Form1.cs, class1.cs, and class2.cs
## Form1.cs : 
### Methods:
	load_file():
####	Checks if the file exists for a filepath and loads the content 
####	from the file to the arralist	
	
	toggle_panel():
####	Switches between different panels: homePanel, insertPanel reportpPanel 
####	in response to atheir respective button click
	
	CreateVisitor():
####	Receives input from the form, validates input and creates an object of 
####	visitor.
	insert_visitor_btn_click():
####	Responds to the button click for creating and inserting new visitor to 
####	csv file and a list.
	insertToTable(Visitor visitor):
####	Fills the datagrid view from the provided visitor
	add_visitor_entry(VisitorsEntry visitorentry):
####	creates an obnject of visitors entry and inserts it to the csv file and 
####	an a list
	check_entry_input():
####	Checks whether the visitorid is empty or doesnot exist when creating 
####	visitors entry;
	upload_csv_click():
####	Responds to button click for uploading file from csv
	entry_btn_click():
####	responds to button click and add new entry to visitors entry list and 
####	csv
	refresh_visitor_entry_table():
####	Refreshes the entry table after updating its data such as adding exit 
####	time	
	update_visitor_entry_csv():
####	updates the changes in csv file after data such as adding exit time is 
####	added
	visitor_entry_table_CellContentClick_1():
####	trigers an event to update the exit time of the visitor
	clear_fields():
####	clear the input field value after creating its object


## class2.cs
### Classes:
### VisitorController:
	mergesort():
####	breaks visitors list array to two array on each iteration of recurssion 
####	until each list contains only one visitors object.
  
	merge_array():
####	compares the visitors id value and sorts by inserting the value with 
####	lowest value in the list first
	
	insert_visitor(Visitor visitor):
####	Generates a random id for the visitor and inserts visitors to the list	 
	
	read_visitor_csv(string file_path):
####	Retrives value from the csv file if it exists in a given filepath
	
	get_visitor(int visitor_id):
####	Uses binary search algorithm to retrive visitort from the list 
####	using visitorid	

	initiate_visitor_data():
####	inserts a title for the datagrid view from the csv file
	
	write_visitors_data():
####	performs serialization to write object into csv file

### EntryController:
	entry_merge_sort(list visitorlist):
####	breaks list into two list using recurssion until each list 
####	contains only one value

	entry_merge_array(list firstList, list secondlist, list finallist):
####	conpares conent in first and second list through visitors entry 
####	time and merges them in an accending order

	read_entry_csv(string filepath):
####	loads csv if the file exists on the given filepath
	
	get_entry(TimeSpan entrytime):
####	uses binary search algorithm to retrive the visitors entry based 
####	on the visitors entry time

	initiate_entry_data(string filepath):
####	initialize the arraylist from the file if it exits in the given 	
####	filepath

	write_entry_data(VisitorEntry visitorEntry):
####	ues serialization to write objects to the csv file

	
## class1.cs
### classes:
Visitor:
#### POJO class used to store visitor details

VisitorEntry:
#### POJO class to store the visitors entry details
		
	

	
