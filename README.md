# Schema Mapping in Virtual Reality

## User Guide
For those who wish to download and use the project's application, follow this guide.

### Setup
This application requires Steam and Steam VR to be installed, as well as a functioning HTC Vive with its headset and two controllers. For setting up the HTC Vive and installing relevant software (Steam and Steam VR), see the manual that came with the Vive, or follow this official guide: https://support.steampowered.com/kb_article.php?ref=2001-UXCM-4439

### Save Files
- The schemas files should be stored in .sql format, and placed in the "Schemas" folder.
- The mapping results files should be stored in .txt format, and placed in the "Mappings" folder. Any exported mapping files will be stored here, under the iterative names "matchResult\#.txt".

### Running the Application
 - Start up Steam VR, and make sure the headset and both controllers are active. If they are not active the application may not start properly.
- Launch the application by double-clicking \texttt{SchemaMapper.exe}
- Put on the headset and grab the controllers, which should now be displaying the application in virtual reality.

#### Control Panel
The left controller holds the control panel, which has several options that can be selected by the right controller, using the laser pointer and the hair trigger.

- From the main menu, the "Files" menu can be chosen, or the "Edit" menu.
- The files menu contains selections to import schemas into the "source" schema, or the "target" schema. Importing a schema successfully will display it inside the application.
- Having one source schema and one target schema allows mappings to be made, or imported via the files menu. Similarly, any mappings that have been made can be exported as a text file into the "Mappings" folder.
- The Edit menu contains information about the current selection on the schemas, which can allow one field from the source schema and one field from the target schema at the same time.
- When two fields are selected, the option to create a link between them, or delete an existing link can be selected.

#### Right Controller
The right controller is the one with the laser pointer. Using the laser pointer as an aiming device and the controller's hair trigger for selection, the user can select items on the control panel in the other controller.

- Once tables have been created (imported) from the control panel, the laser pointer and hair trigger can be used to select individual fields from the tables, referred to as "clicking".
- Clicking a selected field or clicking on the void will cancel the selection, while holding down the trigger on one field and dragging the laser pointer to another field and letting go will connect them, if the fields are eligible.
- Clicking on a mapping beam will select it and both fields linking it.
- Gripping the controller will enable model manipulation, where the selected tables will follow the laser pointer of the controller. If multiple tables are gripped, they will follow the controller at the same time.
- Tables automatically rotate themselves to face you when gripped.

#### Tables
- Once imported, source schema tables are marked in blue, while target schema tables are marked in red. Typically, source schemas will stay on the left, while the target schemas stay on the right.
- Matching results are created as beams of various colour, connecting the fields of the tables. They describe the schema mapping, of which field values should go to which field, and the confidence of the connection, if the beam is generated automatically by COMA.
- The colours represent the following:
  - the beam is created manually, indicating by full confidence.
  - Green: the confidence value is at 0.99
  - Yellow: the confidence value is at 0.7
  - Red: the confidence value is at 0.0
  - Confidence values between the boundaries are indicated by a gradient between the colours, shifted linearly.

#### Exiting the Application
This section is identical to standard Steam VR applications.
Click the Vive controller's "system" button, located beneath the touch pad, to pause the game and view Steam VR's menu. At the lower left corner, a power button can be found. Clicking on it displays an additional menu in the centre of Steam VR, with the option to "exit SchemaMapper.exe".

## COMA guide
For those who wish to use COMA, or access COMA source code. The homepage which link to the other websites listed in this guide can all be found here: https://dbs.uni-leipzig.de/Research/coma.html

Both the compiled program and source code are available in the project repository, but they may not be up to date. The compiled program files are in the folder "coma 3.0 ce v3", while the SVN repository is in the folder "COMA sourcecode".

A copy of COMA 3.0 Community Edition can be found within the GitHub repository for this project: https://sourceforge.net/projects/coma-ce/
COMA's SourceForge SVN repository can be found here: https://github.com/nh1013/SchemaMap

There are notable oddities in its SVN repository that prevent COMA from being directly downloaded, requiring it to be cloned through SVN. This is detailed below.

### Installation
To use COMA, the external tool for generating mapping results, two software are needed:Java, and MySQL. Community/personal editions of these software are compatible with COMA.

The official COMA installation and support guide can be found here: https://dbs.uni-leipzig.de/de/Research/coma_index.html

Once Java and MySQL are installed, COMA requires access to a MySQL database:
- Create a server, which can be hosted locally
- Create a database named "coma-project"
- Grant full privileges to User "coma"

After this, if the MySQL server is running, COMA can be launched directly by opening "coma.bat", within the folder "coma 3.0 ce v3".

### Use
First, the MySQL server must be running for COMA to launch properly.

COMA can be launched directly by opening "coma.bat", within the folder "coma 3.0 ce v3".
The official COMA manual can be found here: https://dbs.uni-leipzig.de/file/Manual_1.pdf

The GUI will soon load.

A few noteworthy issues with COMA are:
- From the manual, "When COMA is launched for the first time, please click Match >Reset Workflow Variables in order to initialize the workflows"
- The "import mapping result" function appears to be malfunctioning, and all imports fail to load
- Another caution from the manual, "The program stores the schemas in MySQL tables and identifies them by a name object. For some formats (e.g. owl) the whole file path is used as identifier. COMA is restricted to maximum path lengths of 100. In case that your schema or ontology mapping cannot be loaded, the overall file path of your file may be too long and you may copy it into another (more superior) directory."

It should be noted that there is a difference between loading a file from its directory into COMA's MySQL database, and loading it from MySQL into COMA's workspace, which allows the user to generate match results, or edit matches.

### Source Code
As mentioned above, there is some oddity in cloning COMA's SVN repository. This is because SVN by default directs downloads and clone commands to the repository's "trunk" folder, but the source code is stored in the repository's "coma-project" folder.

The SVN repository can be found at: https://sourceforge.net/p/coma-ce/mysvn/HEAD/tree/
To properly clone the repository, first download SVN, then checkout the following URL: https://svn.code.sf.net/p/coma-ce/mysvn/
This should clone the repository, which is roughly 210 MB in size.

## Developer guide
For those who wish to further develop this project, follow this guide.

- As this is a Unity project, Unity is required to develop and run the application.
- Download any edition of Unity here: https://unity3d.com/
- Open up Unity, and select "Open" to open a project. Select whole repository's folder, and open it up.
- The first time may require some importing and setting up, so it will take longer.
- Open up the Main Scene in "\_Scenes" folder to see the current setup
- The C\# Scripts can be found in Assets/Scripts.
- COMA can be used directly, and is found in repository as well. For more information, see the previous appendix.
