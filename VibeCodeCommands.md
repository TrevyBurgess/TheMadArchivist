
# Vibe-code Commands

Here are some the vibe code commands I used.

# General

- Update the title to show app title and page in the form "App name - Page name"

## Controls - Breadcrumb, BreadcrumbBar

- Create a user control called Breadcrumb. It will take a folder path and a list of strings. The string will display the folder name and an arrow icon to the right
- Create a user control called BreadcrumbBar. Given a folder path, it will display each folder in the path using the Breadcrumb control for each folder. For the list of strings, include the sub folders in the folder
- Add the BreadcrumbBar to the top of HomePage.
- Cause a property change event to fire when FolderPath in HomePageViewModel gets updated
- FolderPath in BreadcrumbBar control isn't responding to a file path change in HomePage
- When a person clicks on a menu item in the BreadcrumbBar, update the file path
- Only show the folder dropdown list when a user clicks on BreadcrumbArrowIcon.
- When use clicks on BreadcrumbText, navigate to the specified folder. The exception is the last breadcrumb, since the path is the same.

## Controls - ArchiveListControl

- Add a list control in SettingsPage called ArchiveListControl. This control will display a list of file paths. This list will be remembered between sessions.
- Create a User control called ArchiveListControl. Include viewmodel. Add it to the settings panel.
- Add a button to ArchiveListControl. This will allow users to navigate the file system and populate the NewArchivePathTextBox box.
- In ArchiveListControl, add a button in each item in ArchivesListView, allowing a user to remove the item.
- Add the Documents folder as a default to ArchiveListControl
- Add a message in StatusBarText when a user adds an existing folder path to ArchiveListControl
- Add Ready message in StatusBarText when a path is successfully added to ArchiveListControl list
- When a person types in a folder path in NewArchivePathTextBox, verify that the folder. If it exists, add and show a message in StatusBarText saying "Folder Added". Show error message if operation fails.
- When the folder selector returns a real file path, add it to the file list in ArchiveListControl
- Add a plus icon to the add button on ArchiveListControl
- Add a folder browser icon to the Browse button on ArchiveListControl
- Show warning message when user deletes a folder path. If operation successful, add message 'Folder Deleted' to StatusBarText
- Sort folder paths when a user adds a new folder path to list in ArchiveListControl
- Disable AddArchiveButton when folder path is empty
- Do not show path in NewArchivePathTextBox when a folder is selected with the folder selector dialog

## Controls - FolderContentsControl

- Create a user control. It will display a list of files and folders, given a folder path place it in the appropriate location
- Update BreadcrumbBar when clicks on a file in FolderContentsControl

## Controls - FolderTreeViewControl

- Create a user control. Given a folder path, it will display a tree view of child files and folders

## Controls - Status Bar

- Add a status bar at the bottom of the app

## Toolbars

- Rename TopCommandBar to MainCommandBar
- Add a new toolbar called File. Make it dockable
- Add a Settings button to the right of the menu bar. Use the icon only. Right justify it
- Do not add a page to the navigation stack if the current page is the last page pushed onto the stack

## Library - FolderTools

- Add a method in AppTools.FolderTools.cs called MapDrive. Given a folder path, a drive letter, and a nama, it will create a mapped drive. Return error code if operation fails.
- Add a method in AppTools.FolderTools.cs called UnmapDrive. Given a drive letter, un map the drive. Return a status flag.
- Add a method in AppTools.FolderTools.cs called UpdateFolderIcon. It will take a path to an Icon file and a folder path. When called, update the folder icon with the supplied icon

## Library - FileTools

- Add a method in AppTools.FileTools.cs called SaveIcon. Given an Icon, and a file path, save the icon to file
- Add a method to AppTools.FileTools.cs called IsIdentical. Given 2 file paths, check if the files contents are the same. Optimize method for speed.

## Library - EncryptionTools

- Add a method to AppTools.EncryptionTools.cs called EncryptFile. It will encrypt a file specified by an input file path and save it to a specified location
- Add a method to AppTools.EncryptionTools.cs called DecryptFile. It will decrypt a file specified by an input file path and save it to a specified location

## Library - ImageTools

- Add a method in Tools.ImageTools.ca called ToIcon. Given the path to an image, create a windows icon. The method will return this icon

## Layout

- Add Home page
- Add a MenuBar at the top of the app
- Add a CommandBar at the top of the app, below the MenuBar
- Move the CommandBar to the left. Add a toggle to the settings to allow the user to move between left and right.

- Add back and forward Navigation buttons to the command bar, to navigate to visited pages
- Remember the last position and size of the app when it closes

## Page - Settings

- Add a page called Settings. Navigate to this page when a user clicks on Settings button
- Add a toggle to Settings. When on, this will activate dark mode
- Add System Default as an option for the Dark mode setting
- Create a viewmodel for SettingsPage
- Highlight FolderContentsDivider whenever the cursor is over it



## Page - Home Page

- Place a divider between FolderContentsPanel and HomeStackPanel. make it movable. 
- Show a left-right cursor icon when the cursor is over FolderContentsDivider

## Page - About

- Add an about page to the app. Navigate to it when a user presses the About menu command

